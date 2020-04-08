using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CrossoverAlgorithm {

    public static System.Random Random = new System.Random();
    public static int GenomeId = 0;

    public static Genome Crossover(Genome parent1, Genome parent2)
    {
        // Find fitter parent
        Genome fitParent, unfitParent;
        if(parent1.AdjustedFitness >= parent2.AdjustedFitness)
        {
            fitParent = parent1;
            unfitParent = parent2;
        }
        else
        {
            fitParent = parent2;
            unfitParent = parent1;
        }

        // Create nodes for child (same nodes as fitter parent)
        Dictionary<int, Node> childNodes = new Dictionary<int, Node>();
        foreach(Node n in fitParent.Nodes.Values)
            childNodes.Add(n.Id, new Node(n.Id, n.Type));

        // Create connections for child (same connections as fitter parent)
        Dictionary<int, Connection> childConnections = new Dictionary<int, Connection>();
        foreach (Connection c in fitParent.Connections.Values)
        {
            Node sourceNode = childNodes[c.FromNode.Id];
            Node targetNode = childNodes[c.ToNode.Id];
            Connection newConnection = new Connection(c.Id, sourceNode, targetNode);
            newConnection.Enabled = c.Enabled;
            childConnections.Add(c.Id, newConnection);
            sourceNode.OutConnections.Add(newConnection);
            targetNode.InConnections.Add(newConnection);
        }

        // Set weights for child connections (50/50 for matching connections, from fitparent if only existing in fitparent)
        foreach (Connection c in childConnections.Values)
        {
            Connection fitParentConnection = fitParent.Connections[c.Id];

            if (unfitParent.Connections.ContainsKey(c.Id) && fitParentConnection.Enabled && unfitParent.Connections[c.Id].Enabled) // Connection exists and is enabled in both parents
            {
                if (Random.NextDouble() <= 0.5f)
                    c.Weight = fitParentConnection.Weight;
                else
                    c.Weight = unfitParent.Connections[c.Id].Weight;

            }
            else // Connection only exists in fit parent
            {
                c.Weight = fitParentConnection.Weight;
            }
        }

        Genome newGenome = null;
        // Create Child
        try
        {
             newGenome = new Genome(GenomeId++, childNodes, childConnections, fitParent.MaxConnectionWeight, fitParent.MinConnectionWeight);
        }
        catch(Exception e)
        {
            Debug.Log("A circluar node structure has occured while making a child. " + e.Message);
        }
        return newGenome;
    }
}
