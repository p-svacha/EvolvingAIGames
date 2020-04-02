using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeightMutator {

    private System.Random Random;

    public float ReplaceChance = 0.1f;
    public float ShiftChance = 0.4f;
    public float ScaleChance = 0.3f;
    public float InvertChance = 0.1f;
    public float SwapChance = 0.1f;

    public WeightMutator()
    {
        Random = new System.Random();
        if (ReplaceChance + ShiftChance + ScaleChance + InvertChance + SwapChance < 0.99f || ReplaceChance + ShiftChance + ScaleChance + InvertChance + SwapChance > 1.01f) throw new Exception("Sum of weight mutation chances must add up to exactly 1! (and not " + (ReplaceChance + ShiftChance + ScaleChance + InvertChance + SwapChance) + ")");
    }

    public void MutateWeight(Genome g, MutationInformation info)
    {
        double rng = Random.NextDouble();

        if (rng <= ReplaceChance)
        {
            ReplaceWeight(g);
            if (info != null) info.NumReplaceMutations++;
        }
        else if (rng <= ReplaceChance + ShiftChance)
        {
            ShiftWeight(g);
            if (info != null) info.NumShiftMutations++;
        }
        else if (rng <= ReplaceChance + ShiftChance + ScaleChance)
        {
            ScaleWeight(g);
            if (info != null) info.NumScaleMutations++;
        }
        else if (rng <= ReplaceChance + ShiftChance + ScaleChance + InvertChance)
        {
            InvertWeight(g);
            if (info != null) info.NumInvertMutations++;
        }
        else if (rng <= ReplaceChance + ShiftChance + ScaleChance + InvertChance + SwapChance)
        {
            if (CanSwapWeights(g))
            {
                SwapWeights(g);
                if (info != null) info.NumSwapMutations++;
            }
            else MutateWeight(g, info); // Try Again
        }
    }

    /// <summary>
    /// Takes a random connection in the Genome and replaces it's weight to a random new value between -1 and 1.
    /// </summary>
    public void ReplaceWeight(Genome g)
    {
        List<Connection> candidates = g.Connections.Where(x => x.Enabled).ToList();
        if (candidates.Count == 0) throw new Exception("Tried to mutate weight on a genome without any connections");
        Connection connectionToMutate = candidates[Random.Next(candidates.Count)];
        connectionToMutate.Weight = (float)(Random.NextDouble() * 2 - 1); ;
    }

    /// <summary>
    /// Takes a random connection in the Genome and replaces shifts its weight by a random new value between -1 and 1.
    /// </summary>
    public void ShiftWeight(Genome g)
    {
        List<Connection> candidates = g.Connections.Where(x => x.Enabled).ToList();
        if (candidates.Count == 0) throw new Exception("Tried to mutate weight on a genome without any connections");
        Connection connectionToMutate = candidates[Random.Next(candidates.Count)];
        connectionToMutate.Weight = (float)(connectionToMutate.Weight + ((Random.NextDouble() * 2) - 1));
        KeepConnectionWeightInRange(g, connectionToMutate);
    }

    public void ScaleWeight(Genome g)
    {
        List<Connection> candidates = g.Connections.Where(x => x.Enabled).ToList();
        if (candidates.Count == 0) throw new Exception("Tried to mutate weight on a genome without any connections");
        Connection connectionToMutate = candidates[Random.Next(candidates.Count)];
        connectionToMutate.Weight = (float)(connectionToMutate.Weight * ((Random.NextDouble() * 1.5) + 0.5));
        KeepConnectionWeightInRange(g, connectionToMutate);
    }

    public void InvertWeight(Genome g)
    {
        List<Connection> candidates = g.Connections.Where(x => x.Enabled).ToList();
        if (candidates.Count == 0) throw new Exception("Tried to mutate weight on a genome without any connections");
        Connection connectionToMutate = candidates[Random.Next(candidates.Count)];
        connectionToMutate.Weight *= -1;
    }
    
    public bool CanSwapWeights(Genome g)
    {
        List<Node> candidateNodes = g.Nodes.Where(x => x.InConnections.Where(y => y.Enabled).Count() + x.OutConnections.Where(y => y.Enabled).Count() > 1).ToList();
        return candidateNodes.Count > 0;
    }

    public void SwapWeights(Genome g)
    {
        List<Node> candidateNodes = g.Nodes.Where(x => x.InConnections.Where(y => y.Enabled).Count() + x.OutConnections.Where(y => y.Enabled).Count() > 1).ToList();
        Node nodeToMutate = candidateNodes[Random.Next(candidateNodes.Count)];

        List<Connection> nodeConnections = nodeToMutate.InConnections.Where(x => x.Enabled).Concat(nodeToMutate.OutConnections.Where(x => x.Enabled)).ToList();
        List<float> weights = nodeConnections.Select(x => x.Weight).ToList();
        foreach(Connection c in nodeConnections)
        {
            float newWeight = weights[Random.Next(weights.Count)];
            int counter = 0;
            while(newWeight == c.Weight && counter < 10)
            {
                newWeight = weights[Random.Next(weights.Count)];
                counter++;
            }
            c.Weight = newWeight;
            weights.Remove(newWeight);
        }
    }

    public void RandomizeWeights(Genome genome)
    {
        foreach(Connection c in genome.Connections)
            c.Weight = (float)(Random.NextDouble() * 2 - 1);
    }

    private void KeepConnectionWeightInRange(Genome g, Connection con)
    {
        if (con.Weight > g.MaxConnectionWeight) con.Weight = g.MaxConnectionWeight;
        if (con.Weight < g.MinConnectionWeight) con.Weight = g.MinConnectionWeight;
    }
}
