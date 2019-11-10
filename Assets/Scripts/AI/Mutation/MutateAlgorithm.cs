using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MutateAlgorithm {

    System.Random Random;

    // Topology mutations
    public float AddNodeChance = 0.04f;
    public float AddConnectionChance = 0.12f;

    // Weight mutations
    public float WeightMutationChancePerGenome = 0.35f;
    public float ReplaceChance = 0.1f;
    public float ShiftChance = 0.4f;
    public float ScaleChance = 0.3f;
    public float InvertChance = 0.1f;
    public float SwapChance = 0.1f;

    private TopologyMutator TopologyMutator;
    private WeightMutator WeightMutator;

    List<NewNodeMutation> NewNodeMutations;
    List<NewConnectionMutation> NewConnectionMutations;

    public MutateAlgorithm(TopologyMutator top, WeightMutator wei)
    {
        if (ReplaceChance + ShiftChance + ScaleChance + InvertChance + SwapChance < 0.99f || ReplaceChance + ShiftChance + ScaleChance + InvertChance + SwapChance > 1.01f) throw new Exception("Sum of weight mutation chances must add up to exactly 1! (and not " + (ReplaceChance + ShiftChance + ScaleChance + InvertChance + SwapChance) + ")");
        Random = new System.Random();
        TopologyMutator = top;
        WeightMutator = wei;

        NewNodeMutations = new List<NewNodeMutation>();
        NewConnectionMutations = new List<NewConnectionMutation>();
    }

    public MutationInformation MutatePopulation(Population pop, float mutationScaleFactor)
    {
        MutationInformation info = new MutationInformation();

        int previousUniqueNewNodeMutations = NewNodeMutations.Count;
        int previousUniqueNewConnectionMutations = NewConnectionMutations.Count;

        foreach (Genome g in pop.Subjects.Where(x => !x.ImmunteToMutation).Select(s => s.Genome))
        {
            MutateTopology(g, NewNodeMutations, NewConnectionMutations, mutationScaleFactor, info);
            MutateWeight(g, mutationScaleFactor, info);
        }

        info.NumNewUniqueConnectionsMutations = NewConnectionMutations.Count - previousUniqueNewConnectionMutations;
        info.NumNewUniqueNodeMutations = NewNodeMutations.Count - previousUniqueNewNodeMutations;

        return info;
    }

    public void MutateTopology(Genome g, List<NewNodeMutation> newNodeMutations, List<NewConnectionMutation> newConnectionMutations, float mutationScaleFactor, MutationInformation info)
    {
        double rng = Random.NextDouble();
        if (rng <= AddNodeChance * mutationScaleFactor)
        {
            if (!TopologyMutator.CanAddNode(g))
            {
                NewConnectionMutation mutation = TopologyMutator.AddConnection(g, newConnectionMutations);
                if (newConnectionMutations.Where(x => x.SourceNodeId == mutation.SourceNodeId && x.TargetNodeId == mutation.TargetNodeId).Count() == 0) newConnectionMutations.Add(mutation);
                info.NumNewConnectionsMutations++;
            }
            else
            {
                NewNodeMutation mutation = TopologyMutator.AddNode(g, newNodeMutations);
                if (newNodeMutations.Where(x => x.SplittedConnectionId == mutation.SplittedConnectionId).Count() == 0) newNodeMutations.Add(mutation);
                info.NumNewNodeMutations++;
            }
        }
        else if (rng <= ((AddNodeChance * mutationScaleFactor) + (AddConnectionChance * mutationScaleFactor)))
        {
            if (!TopologyMutator.CanAddConnection(g))
            {
                NewNodeMutation mutation = TopologyMutator.AddNode(g, newNodeMutations);
                if (newNodeMutations.Where(x => x.SplittedConnectionId == mutation.SplittedConnectionId).Count() == 0) newNodeMutations.Add(mutation);
                info.NumNewNodeMutations++;
            }
            else
            {
                NewConnectionMutation mutation = TopologyMutator.AddConnection(g, newConnectionMutations);
                if (newConnectionMutations.Where(x => x.SourceNodeId == mutation.SourceNodeId && x.TargetNodeId == mutation.TargetNodeId).Count() == 0) newConnectionMutations.Add(mutation);
                info.NumNewConnectionsMutations++;
            }
        }
    }

    public void MutateWeight(Genome g, float mutationScaleFactor, MutationInformation info)
    {
        if (Random.NextDouble() <= WeightMutationChancePerGenome * mutationScaleFactor)
        {
            if (g.Connections.Where(x => x.Enabled).Count() == 0) return;
            double rng = Random.NextDouble();

            if (rng <= ReplaceChance)
            {
                WeightMutator.ReplaceWeight(g);
                info.NumReplaceMutations++;
            }
            else if (rng <= ReplaceChance + ShiftChance)
            {
                WeightMutator.ShiftWeight(g);
                info.NumShiftMutations++;
            }
            else if (rng <= ReplaceChance + ShiftChance + ScaleChance)
            {
                WeightMutator.ScaleWeight(g);
                info.NumScaleMutations++;
            }
            else if (rng <= ReplaceChance + ShiftChance + ScaleChance + InvertChance)
            {
                WeightMutator.InvertWeight(g);
                info.NumInvertMutations++;
            }
            else if (rng <= ReplaceChance + ShiftChance + ScaleChance + InvertChance + SwapChance)
            {
                WeightMutator.SwapWeights(g);
                info.NumSwapMutations++;
            }
        }
    }

}
