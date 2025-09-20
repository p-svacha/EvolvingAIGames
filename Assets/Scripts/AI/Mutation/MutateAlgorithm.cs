using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MutateAlgorithm {

    System.Random Random;

    // Topology mutations (the ratio between new node and new connection mutations is dependant on the possible mutations of each type)
    public float BaseTopologyMutationChancePerGenome;

    // Weight mutations
    public float BaseWeightMutationChancePerGenome;

    public float MaxConnectionWeight = 20;
    public float MinConnectionWeight = -20;

    private TopologyMutator TopologyMutator;
    private WeightMutator WeightMutator;

    HashSet<Genome> WeightMutatedGenomes;
    HashSet<Genome> TopologyMutatedGenomes;

    List<NewNodeMutation> NewNodeMutations;
    List<NewConnectionMutation> NewConnectionMutations;

    public MutateAlgorithm(TopologyMutator topologyMutator, WeightMutator weightMutator, float baseWeightMutChance, float baseTopMutChance)
    {
        BaseWeightMutationChancePerGenome = baseWeightMutChance;
        BaseTopologyMutationChancePerGenome = baseTopMutChance;

        if (BaseTopologyMutationChancePerGenome + BaseWeightMutationChancePerGenome > 1f) throw new Exception("Sum of weight mutation chance and topology mutation chance is greater than 1! This is not allowed.");
        Random = new System.Random();
        TopologyMutator = topologyMutator;
        WeightMutator = weightMutator;

        WeightMutatedGenomes = new HashSet<Genome>();
        TopologyMutatedGenomes = new HashSet<Genome>();
        NewNodeMutations = new List<NewNodeMutation>();
        NewConnectionMutations = new List<NewConnectionMutation>();
    }

    public MutationInformation MutatePopulation(Population pop, float mutationScaleFactor, bool multipleMutationsPerGenomeAllowed, float reductionFactor)
    {
        MutationInformation info = new MutationInformation();
        info.ConnectionReenableChancePerTopologyMutation = pop.ConnectionReenableChancePerTopologyMutation;

        WeightMutatedGenomes.Clear();
        TopologyMutatedGenomes.Clear();

        int previousUniqueNewNodeMutations = NewNodeMutations.Count;
        int previousUniqueNewConnectionMutations = NewConnectionMutations.Count;

        float topologyMutationChancePerGenome = BaseTopologyMutationChancePerGenome * mutationScaleFactor;
        float weightMutationChancePerGenome = BaseWeightMutationChancePerGenome * mutationScaleFactor;

        foreach (Genome g in pop.Subjects.Where(x => !x.ImmuneToMutation).Select(s => s.Genome))
        {
            MutateGenome(g, topologyMutationChancePerGenome, weightMutationChancePerGenome, multipleMutationsPerGenomeAllowed, reductionFactor, info);
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
    private void MutateGenome(Genome g, float topologyMutationChancePerGenome, float weightMutationChancePerGenome, bool multipleMutationsPerGenomeAllowed, float reducationFactor, MutationInformation info)
    {
        // If the genome has no connections, it can only mutate topology
        if (g.EnabledConnections.Count() == 0)
        {
            topologyMutationChancePerGenome += weightMutationChancePerGenome;
            weightMutationChancePerGenome = 0;
        }

        int numMutations = 0;

        // Apply mutations until this breaks
        while (true)
        {
            // Check if we should apply (another) mutation
            double applyMutationRng = Random.NextDouble();
            double threshold = (topologyMutationChancePerGenome + weightMutationChancePerGenome) * Math.Pow(1 - reducationFactor, numMutations);
            if (applyMutationRng > threshold) break;

            // Check what kind of mutation we should apply
            double typeRng = Random.NextDouble();
            double totalProb = topologyMutationChancePerGenome + weightMutationChancePerGenome;
            if (totalProb <= 0f) break;

            // Apply mutation
            if (typeRng <= (topologyMutationChancePerGenome / totalProb))
            {
                TopologyMutatedGenomes.Add(g);
                TopologyMutator.MutateTopology(g, info, NewNodeMutations, NewConnectionMutations);
            }
            else
            {
                WeightMutatedGenomes.Add(g);
                WeightMutator.MutateWeight(g, info);
            }
            numMutations++;

            if (!multipleMutationsPerGenomeAllowed) break;
        }
    }

    /// <summary>
    /// Applies a weight or topology mutaton to a genome.
    /// </summary>
    public void ForceMutation(Genome g, float connectionReenableChancePerTopologyMutation)
    {
        float topologyMutationChance = 0.5f;

        // If the genome has no connections, it can only mutate topology
        if (g.EnabledConnections.Count() == 0)
        {
            topologyMutationChance = 1f;
        }

        // Force a mutation
        if (Random.NextDouble() < topologyMutationChance)
        {
            TopologyMutator.MutateTopology(g, connectionReenableChancePerTopologyMutation);
        }
        else
        {
            WeightMutator.MutateWeight(g, null);
        }
    }
}
