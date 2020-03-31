using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MutateAlgorithm {

    System.Random Random;

    // Topology mutations (the ratio between new node and new connection mutations is dependant on the possible mutations of each type)
    public float BaseTopologyMutationChancePerGenome = 0.14f;

    // Weight mutations
    public float BaseWeightMutationChancePerGenome = 0.25f;
    public float ReplaceChance = 0.1f;
    public float ShiftChance = 0.4f;
    public float ScaleChance = 0.3f;
    public float InvertChance = 0.1f;
    public float SwapChance = 0.1f;

    private TopologyMutator TopologyMutator;
    private WeightMutator WeightMutator;

    HashSet<Genome> WeightMutatedGenomes;
    HashSet<Genome> TopologyMutatedGenomes;

    List<NewNodeMutation> NewNodeMutations;
    List<NewConnectionMutation> NewConnectionMutations;

    public MutateAlgorithm(TopologyMutator topologyMutator, WeightMutator weightMutator)
    {
        if (ReplaceChance + ShiftChance + ScaleChance + InvertChance + SwapChance < 0.99f || ReplaceChance + ShiftChance + ScaleChance + InvertChance + SwapChance > 1.01f) throw new Exception("Sum of weight mutation chances must add up to exactly 1! (and not " + (ReplaceChance + ShiftChance + ScaleChance + InvertChance + SwapChance) + ")");
        if (BaseTopologyMutationChancePerGenome + BaseWeightMutationChancePerGenome > 1f) throw new Exception("Sum of weight mutation chance and topology mutation chance is greater than 1! This is not allowed.");
        Random = new System.Random();
        TopologyMutator = topologyMutator;
        WeightMutator = weightMutator;

        WeightMutatedGenomes = new HashSet<Genome>();
        TopologyMutatedGenomes = new HashSet<Genome>();
        NewNodeMutations = new List<NewNodeMutation>();
        NewConnectionMutations = new List<NewConnectionMutation>();
    }

    public MutationInformation MutatePopulation(Population pop, float mutationScaleFactor, bool multipleMutationsPerGenomeAllowed)
    {
        MutationInformation info = new MutationInformation();

        WeightMutatedGenomes.Clear();
        TopologyMutatedGenomes.Clear();

        int previousUniqueNewNodeMutations = NewNodeMutations.Count;
        int previousUniqueNewConnectionMutations = NewConnectionMutations.Count;

        float topologyMutationChancePerGenome = BaseTopologyMutationChancePerGenome * mutationScaleFactor;
        float weightMutationChancePerGenome = BaseWeightMutationChancePerGenome * mutationScaleFactor;

        foreach (Genome g in pop.Subjects.Where(x => !x.ImmunteToMutation).Select(s => s.Genome))
        {
            MutateGenome(g, topologyMutationChancePerGenome, weightMutationChancePerGenome, multipleMutationsPerGenomeAllowed, info);
        }

        info.NumNewUniqueConnectionsMutations = NewConnectionMutations.Count - previousUniqueNewConnectionMutations;
        info.NumNewUniqueNodeMutations = NewNodeMutations.Count - previousUniqueNewNodeMutations;
        info.MultipleMutationsPerGenomeAllowed = multipleMutationsPerGenomeAllowed;
        info.WeightMutationChancePerGenome = weightMutationChancePerGenome;
        info.TopologyMutationChancePerGenome = topologyMutationChancePerGenome;
        info.MutationChanceScaleFactor = mutationScaleFactor;
        info.NumTopologyMutatedGenomes = TopologyMutatedGenomes.Count;
        info.NumWeightMutatedGenomes = WeightMutatedGenomes.Count;

        return info;
    }

    /// <summary>
    /// Runs the mutation alogorithm through a genome with the given parameters.
    /// </summary>
    private void MutateGenome(Genome g, float topologyMutationChancePerGenome, float weightMutationChancePerGenome, bool multipleMutationsPerGenomeAllowed, MutationInformation info)
    {
        // If the genome has no connections, it can only mutate topology
        if (g.Connections.Where(x => x.Enabled).Count() == 0)
        {
            topologyMutationChancePerGenome += weightMutationChancePerGenome;
            weightMutationChancePerGenome = 0;
        }

        double rng = Random.NextDouble();

        while (rng <= topologyMutationChancePerGenome + weightMutationChancePerGenome)
        {
            if(rng <= topologyMutationChancePerGenome)
            {
                MutateTopology(g, info);
            }
            else
            {
                MutateWeight(g, info);
            }
            rng = multipleMutationsPerGenomeAllowed ? Random.NextDouble() : 1; // Allows for multiple mutations per genome
        }
    }

    private void MutateTopology(Genome g, MutationInformation info)
    {
        TopologyMutatedGenomes.Add(g);

        int possibleNewNodeMutations = TopologyMutator.FindCandidateConnectionsForNewNode(g).Count;
        int possibleNewConnectionMutations = TopologyMutator.FindCandidateConnections(g).Count;
        int allPossibleMutations = possibleNewNodeMutations + possibleNewConnectionMutations;

        float addNewNodeChance = (float)possibleNewNodeMutations / (float)allPossibleMutations;

        double rng = Random.NextDouble();

        if (rng <= addNewNodeChance)
        {
            NewNodeMutation mutation = TopologyMutator.AddNode(g, NewNodeMutations);
            if (NewNodeMutations.Where(x => x.SplittedConnectionId == mutation.SplittedConnectionId).Count() == 0) NewNodeMutations.Add(mutation);
            info.NumNewNodeMutations++;
        }
        else
        {
            NewConnectionMutation mutation = TopologyMutator.AddConnection(g, NewConnectionMutations);
            if (NewConnectionMutations.Where(x => x.SourceNodeId == mutation.SourceNodeId && x.TargetNodeId == mutation.TargetNodeId).Count() == 0) NewConnectionMutations.Add(mutation);
            info.NumNewConnectionsMutations++;
        }
    }

    private void MutateWeight(Genome g, MutationInformation info)
    {
        WeightMutatedGenomes.Add(g);

        double rng = Random.NextDouble();

        if (rng <= ReplaceChance)
        {
            bool canApplyMutation = WeightMutator.ReplaceWeight(g);
            if (canApplyMutation) info.NumReplaceMutations++;
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
