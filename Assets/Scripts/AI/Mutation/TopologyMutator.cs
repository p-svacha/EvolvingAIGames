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

    /// <summary>
    /// Apply a topology mutation to a genome without any logging.
    /// </summary>
    public void MutateTopology(Genome g, float connectionReenableChancePerTopologyMutation)
    {
        if (TryReenableRandomDisabledConnection(g, probability: connectionReenableChancePerTopologyMutation)) return;

        // Apply new node or new connection mutation
        int possibleNewNodeMutations = FindCandidateConnectionsForNewNode(g).Count;
        int possibleNewConnectionMutations = FindCandidateConnections(g).Count;
        int allPossibleMutations = possibleNewNodeMutations + possibleNewConnectionMutations;
        float addNewNodeChance = (float)possibleNewNodeMutations / (float)allPossibleMutations;

        if (Random.NextDouble() <= addNewNodeChance) AddNode(g);
        else AddConnection(g);
    }


    public void MutateTopology(Genome g, MutationInformation info, List<NewNodeMutation> newNodeMutations, List<NewConnectionMutation> newConnectionMutations)
    {
        // First apply small chance to reenable an existing disabled connection
        if (TryReenableRandomDisabledConnection(g, probability: info.ConnectionReenableChancePerTopologyMutation))
        {
            info.NumReenableConnectionMutations++;
            return;
        }

        // Apply new node or new connection mutation
        int possibleNewNodeMutations = FindCandidateConnectionsForNewNode(g).Count;
        int possibleNewConnectionMutations = FindCandidateConnections(g).Count;
        int allPossibleMutations = possibleNewNodeMutations + possibleNewConnectionMutations;

        float addNewNodeChance = (float)possibleNewNodeMutations / (float)allPossibleMutations;

        double rng = Random.NextDouble();

        if (rng <= addNewNodeChance)
        {
            NewNodeMutation mutation = AddNode(g);
            if (newNodeMutations.Where(x => x.SplittedConnectionId == mutation.SplittedConnectionId).Count() == 0) newNodeMutations.Add(mutation);
            if(info != null) info.NumNewNodeMutations++;
        }
        else
        {
            NewConnectionMutation mutation = AddConnection(g);
            if (newConnectionMutations.Where(x => 
            (x.FromNodeId == mutation.FromNodeId && x.ToNodeId == mutation.ToNodeId) ||
            (x.FromNodeId == mutation.ToNodeId && x.ToNodeId == mutation.FromNodeId)
            ).Count() == 0) newConnectionMutations.Add(mutation);
            if (info != null) info.NumNewConnectionsMutations++;
        }
    }

    /// <summary>
    /// Tries to reenable a disabled connection, with a certain probability.
    /// <br/>Returns if a reenable-mutation occured.
    /// </summary>
    public bool TryReenableRandomDisabledConnection(Genome g, float probability = 0.01f)
    {
        if (Random.NextDouble() > probability) return false;

        // Find candidates
        List<Connection> disabledConnections = g.Connections.Values.Where(c => !c.Enabled).ToList();
        if (disabledConnections.Count == 0) return false;

        // Choose a candidate
        Connection connectionToEnable = disabledConnections.RandomElement();

        // Toggle the connection
        connectionToEnable.Enabled = true;

        // Recalculate depth
        g.CalculateDepths();

        return true;
    }

    /// <summary>
    /// Adds a new connection by connecting two random unconnected nodes.
    /// </summary>
    public NewConnectionMutation AddConnection(Genome genome)
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
    public NewNodeMutation AddNode(Genome genome)
    {
        var candidates = FindCandidateConnectionsForNewNode(genome);
        if (candidates.Count == 0) throw new Exception("No enabled connections to split.");
        var connectionToSplit = candidates[Random.Next(candidates.Count)];

        // IMPORTANT: derive split triple from the base-connection innovation id
        // (connectionToSplit.Id should already be a global innovation id)
        var (nodeId, toNewId, fromNewId) =
            InnovationIdTracker.GetOrCreateSplitInnovation(connectionToSplit.Id);

        // If this genome already has those connections, just disable the base and bail out
        if (genome.Connections.ContainsKey(toNewId) || genome.Connections.ContainsKey(fromNewId))
        {
            // We're re-splitting something that was already split earlier in this lineage.
            connectionToSplit.Enabled = false;       // ensure base is off
            genome.CalculateDepths();
            return new NewNodeMutation
            {
                SplittedConnectionId = connectionToSplit.Id,
                NewNodeId = genome.Nodes.ContainsKey(nodeId) ? nodeId : -1, // already exists or not
                ToNewNodeConnectionId = toNewId,
                FromNewNodeConnectionId = fromNewId
            };
        }

        connectionToSplit.Enabled = false;

        // Create / attach node + edges
        var newNode = new Node(nodeId, NodeType.Hidden);

        var toNew = new Connection(toNewId, connectionToSplit.FromNode, newNode) { Weight = 1f };
        var fromNew = new Connection(fromNewId, newNode, connectionToSplit.ToNode) { Weight = connectionToSplit.Weight };

        connectionToSplit.FromNode.OutConnections.Add(toNew);
        newNode.InConnections.Add(toNew);
        newNode.OutConnections.Add(fromNew);
        connectionToSplit.ToNode.InConnections.Add(fromNew);

        // Add to genome maps (guard node too)
        if (!genome.Nodes.ContainsKey(nodeId))
        {
            genome.HiddenNodes.Add(newNode);
            genome.Nodes.Add(nodeId, newNode);
        }

        // These are the lines that currently throw. Guard them:
        if (!genome.Connections.ContainsKey(toNewId))
            genome.Connections.Add(toNewId, toNew);
        if (!genome.Connections.ContainsKey(fromNewId))
            genome.Connections.Add(fromNewId, fromNew);

        genome.CalculateDepths();

        return new NewNodeMutation
        {
            SplittedConnectionId = connectionToSplit.Id,
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
