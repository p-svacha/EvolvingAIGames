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

        double rng = Random.NextDouble();

        int numMutations = 0;

        while (rng <= ((topologyMutationChancePerGenome + weightMutationChancePerGenome) * (Math.Pow(1 - reducationFactor, numMutations))))
        {
            if(rng <= topologyMutationChancePerGenome)
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
            rng = multipleMutationsPerGenomeAllowed ? Random.NextDouble() : 1; // Allows for multiple mutations per genome
        }
    }
}
