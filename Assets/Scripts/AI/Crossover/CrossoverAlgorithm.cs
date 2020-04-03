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

        // Find highest connection id and highest matching connection id
        List<int> fitParentConnectionIds = fitParent.Connections.Select(x => x.InnovationNumber).ToList();
        List<int> unfitParentConnectionIds = unfitParent.Connections.Select(x => x.InnovationNumber).ToList();
        int highestMatchingConnectionId;
        if (fitParentConnectionIds.Count == 0 || unfitParentConnectionIds.Count == 0) highestMatchingConnectionId = -1;
        else highestMatchingConnectionId = Math.Min(fitParentConnectionIds.Max(x => x), unfitParentConnectionIds.Max(x => x));

        // Find matching connections
        List<int> matchingConnections = new List<int>();
        for (int i = 0; i < highestMatchingConnectionId + 1; i++)
            if (fitParentConnectionIds.Contains(i) && unfitParentConnectionIds.Contains(i)) matchingConnections.Add(i);

        // Create nodes for child (same nodes as fitter parent)
        List<Node> childNodes = new List<Node>();
        foreach(Node n in fitParent.Nodes)
            childNodes.Add(new Node(n.Id, n.Type));

        // Create connections for child (same connections as fitter parent)
        List<Connection> childConnections = new List<Connection>();
        foreach (Connection c in fitParent.Connections)
        {
            Node sourceNode = childNodes.First(x => x.Id == c.From.Id);
            Node targetNode = childNodes.First(x => x.Id == c.To.Id);
            Connection newConnection = new Connection(c.InnovationNumber, sourceNode, targetNode);
            newConnection.Enabled = c.Enabled;
            childConnections.Add(newConnection);
            sourceNode.OutConnections.Add(newConnection);
            targetNode.InConnections.Add(newConnection);
        }

        // Set weights for child connections (50/50 for matching connections, from fitparent if only existing in fitparent)
        foreach (Connection c in childConnections)
        {
            Connection fitParentConnection = fitParent.Connections.First(x => x.InnovationNumber == c.InnovationNumber);

            if (matchingConnections.Contains(c.InnovationNumber) && fitParentConnection.Enabled && unfitParent.Connections.First(x => x.InnovationNumber == c.InnovationNumber).Enabled) // Connection exists and is enabled in both parents
            {
                if (Random.NextDouble() <= 0.5f)
                    c.Weight = fitParentConnection.Weight;
                else
                    c.Weight = unfitParent.Connections.First(x => x.InnovationNumber == c.InnovationNumber).Weight;

            }
            else // Connection only exists in fit parent
            {
                c.Weight = fitParentConnection.Weight;
            }
        }

        // Debug
        // Uncomment and delete rest for normal
        // return new Genome(GenomeId++, childNodes, childConnections);


        Genome newGenome = null;
        // Create Child
        try
        {
             newGenome = new Genome(GenomeId++, childNodes, childConnections);
        }
        catch(Exception e)
        {
            Debug.Log("A circluar node structure has occured while making a child. " + e.Message);
        }
        return newGenome;
    }
}
