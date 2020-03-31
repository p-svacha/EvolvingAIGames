using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeightMutator {

    private System.Random Random;

    public WeightMutator()
    {
        Random = new System.Random();
    }


    public bool ReplaceWeight(Genome g)
    {
        List<Connection> candidates = g.Connections.Where(x => x.Enabled).ToList();
        if (candidates.Count == 0) throw new Exception("Tried to mutate weight on a genome without any connections");
        Connection connectionToMutate = candidates[Random.Next(candidates.Count)];
        connectionToMutate.Weight = (float)(Random.NextDouble() * 2 - 1); ;
        return true;
    }

    public bool ShiftWeight(Genome g)
    {
        List<Connection> candidates = g.Connections.Where(x => x.Enabled).ToList();
        if (candidates.Count == 0) throw new Exception("Tried to mutate weight on a genome without any connections");
        Connection connectionToMutate = candidates[Random.Next(candidates.Count)];
        connectionToMutate.Weight = (float)(connectionToMutate.Weight + ((Random.NextDouble() * 2) - 1));
        if (connectionToMutate.Weight > 2) connectionToMutate.Weight = 2;
        if (connectionToMutate.Weight < -2) connectionToMutate.Weight = -2;
        return true;
    }

    public bool ScaleWeight(Genome g)
    {
        List<Connection> candidates = g.Connections.Where(x => x.Enabled).ToList();
        if (candidates.Count == 0) throw new Exception("Tried to mutate weight on a genome without any connections");
        Connection connectionToMutate = candidates[Random.Next(candidates.Count)];
        connectionToMutate.Weight = (float)(connectionToMutate.Weight * ((Random.NextDouble() * 1.5) + 0.5));
        if (connectionToMutate.Weight > 2) connectionToMutate.Weight = 2;
        if (connectionToMutate.Weight < -2) connectionToMutate.Weight = -2;
        return true;
    }

    public bool InvertWeight(Genome g)
    {
        List<Connection> candidates = g.Connections.Where(x => x.Enabled).ToList();
        if (candidates.Count == 0) throw new Exception("Tried to mutate weight on a genome without any connections");
        Connection connectionToMutate = candidates[Random.Next(candidates.Count)];
        connectionToMutate.Weight *= -1;
        return true;
    }

    public bool SwapWeights(Genome g)
    {
        List<Node> candidateNodes = g.Nodes.Where(x => x.InConnections.Where(y => y.Enabled).Count() + x.OutConnections.Where(y => y.Enabled).Count() > 1).ToList();
        if (candidateNodes.Count == 0) throw new Exception("Tried to mutate weight on a genome without any connections");
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
        return true;
    }


    public void RandomizeWeights(Genome genome)
    {
        foreach(Connection c in genome.Connections)
            c.Weight = (float)(Random.NextDouble() * 2 - 1);
    }
}
