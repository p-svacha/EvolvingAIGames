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

    /// <summary>
    /// Adds a new connection by connecting two random unconnected nodes.
    /// </summary>
    public NewConnectionMutation AddConnection(Genome genome, List<NewConnectionMutation> _)
    {
        var candidateConnections = FindCandidateConnections(genome);
        var newConnection = candidateConnections[Random.Next(candidateConnections.Count)];

        // use global innovation
        int newId = InnovationIdTracker.GetOrCreateConnectionInnovation(newConnection.FromNode.Id, newConnection.ToNode.Id);
        newConnection.Id = newId;
        newConnection.Weight = (float)(Random.NextDouble() * 2 - 1);

        // attach...
        newConnection.FromNode.OutConnections.Add(newConnection);
        newConnection.ToNode.InConnections.Add(newConnection);
        genome.Connections.Add(newId, newConnection);
        genome.CalculateDepths();

        return new NewConnectionMutation
        {
            FromNodeId = newConnection.FromNode.Id,
            ToNodeId = newConnection.ToNode.Id,
            NewConnectionId = newId
        };
    }

    /// <summary>
    /// Adds a new node by splitting a random existing connection.
    /// </summary>
    public NewNodeMutation AddNode(Genome genome, List<NewNodeMutation> _)
    {
        var candidate = FindCandidateConnectionsForNewNode(genome);
        var split = candidate[Random.Next(candidate.Count)];
        split.Enabled = false;


        // Get the global innovation id for the connection being split,
        // then get the split triple based on that (not on the per-genome split.Id)
        int baseConnInnov = InnovationIdTracker.GetOrCreateConnectionInnovation(
            split.FromNode.Id, split.ToNode.Id);

        var (nodeId, toNewId, fromNewId) = InnovationIdTracker.GetOrCreateSplitInnovation(split.Id);

        var newNode = new Node(nodeId, NodeType.Hidden);

        var toNew = new Connection(toNewId, split.FromNode, newNode) { Weight = 1f };
        var fromNew = new Connection(fromNewId, newNode, split.ToNode) { Weight = split.Weight };

        split.FromNode.OutConnections.Add(toNew);
        newNode.InConnections.Add(toNew);
        newNode.OutConnections.Add(fromNew);
        split.ToNode.InConnections.Add(fromNew);

        genome.HiddenNodes.Add(newNode);
        genome.Nodes.Add(nodeId, newNode);
        genome.Connections.Add(toNewId, toNew);
        genome.Connections.Add(fromNewId, fromNew);

        genome.CalculateDepths();

        return new NewNodeMutation
        {
            SplittedConnectionId = split.Id,
            NewNodeId = nodeId,
            ToNewNodeConnectionId = toNewId,
            FromNewNodeConnectionId = fromNewId
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
