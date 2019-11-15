using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TopologyMutator {

    public System.Random Random;

    public int NodeId;
    public int InnovationNumber; //connection id, historical marker to know if genomes share a connection

    public TopologyMutator()
    {
        Random = new System.Random();
    }

    public NewConnectionMutation AddConnection(Genome genome, List<NewConnectionMutation> mutations)
    {
        List<Connection> candidateConnections = FindCandidateConnections(genome);
        Connection newConnection = candidateConnections[Random.Next(candidateConnections.Count)];

        // Check if this exact mutation has already been done this evolution cycle
        int newConnectionId;
        List<NewConnectionMutation> existingMutations = mutations.Where(x => x.SourceNodeId == newConnection.In.Id && x.TargetNodeId == newConnection.Out.Id).ToList();
        if (existingMutations.Count > 0)
            newConnectionId = existingMutations[0].NewConnectionId;
        else newConnectionId = InnovationNumber++;

        newConnection.InnovationNumber = newConnectionId;
        newConnection.Weight = (float)(Random.NextDouble() * 2 - 1);

        newConnection.In.OutConnections.Add(newConnection);
        newConnection.Out.InConnections.Add(newConnection);

        genome.Connections.Add(newConnection);

        genome.CalculateDepths();

        return new NewConnectionMutation()
        {
            SourceNodeId = newConnection.In.Id,
            TargetNodeId = newConnection.Out.Id,
            NewConnectionId = newConnection.InnovationNumber
        };
    }

    public NewNodeMutation AddNode(Genome genome, List<NewNodeMutation> mutations)
    {
        List<Connection> candidateConnections = FindCandidateConnectionsForNewNode(genome);
        Connection connectionToSplit = candidateConnections[Random.Next(candidateConnections.Count)];
        connectionToSplit.Enabled = false;

        // Check if this exact mutation has already been done this evolution cycle
        int nodeId, toNewNodeId, fromNewNodeId;
        List<NewNodeMutation> existingMutations = mutations.Where(x => x.SplittedConnectionId == connectionToSplit.InnovationNumber).ToList();
        if(existingMutations.Count > 0)
        {
            NewNodeMutation existingMutation = existingMutations[0];
            nodeId = existingMutation.NewNodeId;
            toNewNodeId = existingMutation.ToNewNodeConnectionId;
            fromNewNodeId = existingMutation.FromNewNodeConnectionId;
        }
        else
        {
            nodeId = NodeId++;
            toNewNodeId = InnovationNumber++;
            fromNewNodeId = InnovationNumber++;
        }

        // Create new node
        Node newNode = new Node(nodeId, NodeType.Hidden);

        // Create new connections
        Connection toNewNode = new Connection(toNewNodeId, connectionToSplit.In, newNode);
        connectionToSplit.In.OutConnections.Add(toNewNode);
        newNode.InConnections.Add(toNewNode);
        toNewNode.Weight = 1;

        Connection fromNewNode = new Connection(fromNewNodeId, newNode, connectionToSplit.Out);
        newNode.OutConnections.Add(fromNewNode);
        connectionToSplit.Out.InConnections.Add(fromNewNode);
        fromNewNode.Weight = connectionToSplit.Weight;

        genome.HiddenNodes.Add(newNode);
        genome.Nodes.Add(newNode);
        genome.Connections.Add(toNewNode);
        genome.Connections.Add(fromNewNode);

        genome.CalculateDepths();

        return new NewNodeMutation()
        {
            SplittedConnectionId = connectionToSplit.InnovationNumber,
            NewNodeId = newNode.Id,
            ToNewNodeConnectionId = toNewNode.InnovationNumber,
            FromNewNodeConnectionId = fromNewNode.InnovationNumber
        };
    }

    public List<Connection> FindCandidateConnections(Genome g)
    {
        List<Node> sourceCandidates = g.InputNodes.Concat(g.HiddenNodes).ToList();
        List<Node> targetCandidates = g.HiddenNodes.Concat(g.OutputNodes).ToList();
        List<Connection> candidateConnections = new List<Connection>();

        foreach (Node source in sourceCandidates)
        {
            foreach (Node target in targetCandidates)
            {
                Node connectionSource = source;
                Node connectionTarget = target;

                // Swap source and target if target has a lower depth, ie comes first in the network
                if (target.Depth < source.Depth)
                {
                    connectionSource = target;
                    connectionTarget = source;
                }

                if (connectionTarget != connectionSource && !connectionSource.IsConnectedTo(connectionTarget))
                {
                    candidateConnections.Add(new Connection(-1, connectionSource, connectionTarget));
                }
            }
        }
        return candidateConnections;
    }

    public List<Connection> FindCandidateConnectionsForNewNode(Genome g)
    {
        return g.Connections.Where(x => x.Enabled).ToList();
    }

}
