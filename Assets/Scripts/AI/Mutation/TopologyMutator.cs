using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TopologyMutator {

    public System.Random Random;

    public int NodeId;
    public int ConnectionId; //connection id, historical marker to know if genomes share a connection

    public TopologyMutator()
    {
        Random = new System.Random();
    }

    public void MutateTopology(Genome g, MutationInformation info, List<NewNodeMutation> newNodeMutations, List<NewConnectionMutation> newConnectionMutations)
    {
        int possibleNewNodeMutations = FindCandidateConnectionsForNewNode(g).Count;
        int possibleNewConnectionMutations = FindCandidateConnections(g).Count;
        int allPossibleMutations = possibleNewNodeMutations + possibleNewConnectionMutations;

        float addNewNodeChance = (float)possibleNewNodeMutations / (float)allPossibleMutations;

        double rng = Random.NextDouble();

        if (rng <= addNewNodeChance)
        {
            NewNodeMutation mutation = AddNode(g, newNodeMutations);
            if (newNodeMutations.Where(x => x.SplittedConnectionId == mutation.SplittedConnectionId).Count() == 0) newNodeMutations.Add(mutation);
            if(info != null) info.NumNewNodeMutations++;
        }
        else
        {
            NewConnectionMutation mutation = AddConnection(g, newConnectionMutations);
            if (newConnectionMutations.Where(x => 
            (x.FromNodeId == mutation.FromNodeId && x.ToNodeId == mutation.ToNodeId) ||
            (x.FromNodeId == mutation.ToNodeId && x.ToNodeId == mutation.FromNodeId)
            ).Count() == 0) newConnectionMutations.Add(mutation);
            if (info != null) info.NumNewConnectionsMutations++;
        }
    }

    public NewConnectionMutation AddConnection(Genome genome, List<NewConnectionMutation> mutations)
    {
        List<Connection> candidateConnections = FindCandidateConnections(genome);
        Connection newConnection = candidateConnections[Random.Next(candidateConnections.Count)];

        // Check if this exact mutation has already been done
        int newConnectionId;
        List<NewConnectionMutation> existingMutations = mutations.Where(x => 
            (x.FromNodeId == newConnection.FromNode.Id && x.ToNodeId == newConnection.ToNode.Id) || 
            (x.FromNodeId == newConnection.ToNode.Id && x.ToNodeId == newConnection.FromNode.Id)
            ).ToList();
        if (existingMutations.Count > 0)
            newConnectionId = existingMutations[0].NewConnectionId;
        else newConnectionId = ConnectionId++;

        newConnection.Id = newConnectionId;
        newConnection.Weight = (float)(Random.NextDouble() * 2 - 1);

        newConnection.FromNode.OutConnections.Add(newConnection);
        newConnection.ToNode.InConnections.Add(newConnection);

        genome.Connections.Add(newConnectionId, newConnection);

        genome.CalculateDepths();

        return new NewConnectionMutation()
        {
            FromNodeId = newConnection.FromNode.Id,
            ToNodeId = newConnection.ToNode.Id,
            NewConnectionId = newConnection.Id
        };
    }

    public NewNodeMutation AddNode(Genome genome, List<NewNodeMutation> mutations)
    {
        List<Connection> candidateConnections = FindCandidateConnectionsForNewNode(genome);
        Connection connectionToSplit = candidateConnections[Random.Next(candidateConnections.Count)];
        connectionToSplit.Enabled = false;

        // Check if this exact mutation has already been done this evolution cycle
        int nodeId, toNewNodeId, fromNewNodeId;
        List<NewNodeMutation> existingMutations = mutations.Where(x => x.SplittedConnectionId == connectionToSplit.Id).ToList();
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
            toNewNodeId = ConnectionId++;
            fromNewNodeId = ConnectionId++;
        }

        // Create new node
        Node newNode = new Node(nodeId, NodeType.Hidden);

        // Create new connections
        Connection toNewNode = new Connection(toNewNodeId, connectionToSplit.FromNode, newNode);
        connectionToSplit.FromNode.OutConnections.Add(toNewNode);
        newNode.InConnections.Add(toNewNode);
        toNewNode.Weight = 1;

        Connection fromNewNode = new Connection(fromNewNodeId, newNode, connectionToSplit.ToNode);
        newNode.OutConnections.Add(fromNewNode);
        connectionToSplit.ToNode.InConnections.Add(fromNewNode);
        fromNewNode.Weight = connectionToSplit.Weight;

        genome.HiddenNodes.Add(newNode);
        genome.Nodes.Add(nodeId, newNode);
        genome.Connections.Add(toNewNodeId, toNewNode);
        genome.Connections.Add(fromNewNodeId, fromNewNode);

        genome.CalculateDepths();

        return new NewNodeMutation()
        {
            SplittedConnectionId = connectionToSplit.Id,
            NewNodeId = newNode.Id,
            ToNewNodeConnectionId = toNewNode.Id,
            FromNewNodeConnectionId = fromNewNode.Id
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
        return g.EnabledConnections;
    }

}
