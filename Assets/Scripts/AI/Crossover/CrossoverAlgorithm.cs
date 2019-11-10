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

        // Find highest gene id and highest matching gene id
        List<int> fitParentGeneIds = fitParent.Connections.Select(x => x.InnovationNumber).ToList();
        List<int> unfitParentGeneIds = unfitParent.Connections.Select(x => x.InnovationNumber).ToList();
        int highestMatchingGeneId;
        if (fitParentGeneIds.Count == 0 || unfitParentGeneIds.Count == 0) highestMatchingGeneId = -1;
        else highestMatchingGeneId = Math.Min(fitParentGeneIds.Max(x => x), unfitParentGeneIds.Max(x => x));

        // Find matching genes
        List<int> matchingGenes = new List<int>();
        for (int i = 0; i < highestMatchingGeneId + 1; i++)
            if (fitParentGeneIds.Contains(i) && unfitParentGeneIds.Contains(i)) matchingGenes.Add(i);

        // Create nodes for child (same nodes as fitter parent)
        List<Node> childNodes = new List<Node>();
        foreach(Node n in fitParent.Nodes)
            childNodes.Add(new Node(n.Id, n.Type));

        // Create connections for child (same connections as fitter parent)
        List<Connection> childConnections = new List<Connection>();
        foreach (Connection c in fitParent.Connections)
        {
            Node sourceNode = childNodes.First(x => x.Id == c.In.Id);
            Node targetNode = childNodes.First(x => x.Id == c.Out.Id);
            Connection newConnection = new Connection(c.InnovationNumber, sourceNode, targetNode);
            childConnections.Add(newConnection);
            sourceNode.OutConnections.Add(newConnection);
            targetNode.InConnections.Add(newConnection);
        }

        // Set weights and enabled flag for child connections (50/50 for matching genes, from fitparent if only existing in fitparent)
        foreach (Connection c in childConnections)
        {
            Connection parentCon;
            if (matchingGenes.Contains(c.InnovationNumber))
            {
                if (Random.NextDouble() <= 0.5f)
                    parentCon = fitParent.Connections.First(x => x.InnovationNumber == c.InnovationNumber);
                else
                    parentCon = unfitParent.Connections.First(x => x.InnovationNumber == c.InnovationNumber);
            }
            else
                parentCon = fitParent.Connections.First(x => x.InnovationNumber == c.InnovationNumber);
            c.Weight = parentCon.Weight;
            c.Enabled = parentCon.Enabled;
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
            Debug.Log("A circluar node structure has occured while making a child. Left is the fit parent, right the unfit parent. gl hf " + e.Message);
            //GameObject.Find("GenomeVisualizer2").GetComponent<GenomeVisualizer>().VisualizeSubject(fitParent);
            //GameObject.Find("GenomeVisualizer1").GetComponent<GenomeVisualizer>().VisualizeSubject(unfitParent);
        }
        return newGenome;
    }
}
