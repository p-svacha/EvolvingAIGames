using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MutateAlgorithm {

    System.Random Random;

    // Topology mutations (the ratio between new node and new connection mutations is dependant on the possible mutations of each type)
    public float TopologyMutationChance = 0.14f;

    // Weight mutations
    public float WeightMutationChancePerGenome = 0.25f;
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

        if (rng <= TopologyMutationChance * mutationScaleFactor)
        {
            int possibleNewNodeMutations = TopologyMutator.FindCandidateConnectionsForNewNode(g).Count;
            int possibleNewConnectionMutations = TopologyMutator.FindCandidateConnections(g).Count;
            int allPossibleMutations = possibleNewNodeMutations + possibleNewConnectionMutations;

            float addNewNodeChance = (float)possibleNewNodeMutations / (float)allPossibleMutations;

            double rng2 = Random.NextDouble();

            if (rng2 <= addNewNodeChance)
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
                bool canApplyMutation = WeightMutator.ReplaceWeight(g);
                if(canApplyMutation) info.NumReplaceMutations++;
            }
            else if (rng <= ReplaceChance + ShiftChance)
            {
                bool canApplyMutation = WeightMutator.ShiftWeight(g);
                if (canApplyMutation) info.NumShiftMutations++;
            }
            else if (rng <= ReplaceChance + ShiftChance + ScaleChance)
            {
                bool canApplyMutation = WeightMutator.ScaleWeight(g);
                if (canApplyMutation) info.NumScaleMutations++;
            }
            else if (rng <= ReplaceChance + ShiftChance + ScaleChance + InvertChance)
            {
                bool canApplyMutation = WeightMutator.InvertWeight(g);
                if (canApplyMutation) info.NumInvertMutations++;
            }
            else if (rng <= ReplaceChance + ShiftChance + ScaleChance + InvertChance + SwapChance)
            {
                bool canApplyMutation = WeightMutator.SwapWeights(g);
                if (canApplyMutation) info.NumSwapMutations++;
            }
        }
    }

}
