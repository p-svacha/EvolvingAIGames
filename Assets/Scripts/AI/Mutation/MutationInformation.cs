using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutationInformation
{
    public int NumReenableConnectionMutations;
    public int NumNewNodeMutations;
    public int NumNewUniqueNodeMutations;
    public int NumNewConnectionsMutations;
    public int NumNewUniqueConnectionsMutations;

    public float MutationChanceScaleFactor;
    public float ConnectionReenableChancePerTopologyMutation;
    public float WeightMutationChancePerGenome;
    public float TopologyMutationChancePerGenome;
    public bool MultipleMutationsPerGenomeAllowed;

    public int NumTopologyMutatedGenomes;
    public int NumWeightMutatedGenomes;

    public int NumReplaceMutations;
    public int NumShiftMutations;
    public int NumScaleMutations;
    public int NumInvertMutations;
    public int NumSwapMutations;

    public int NumWeightMutations
    {
        get
        {
            return NumReplaceMutations + NumShiftMutations + NumScaleMutations + NumInvertMutations + NumSwapMutations;
        }
    }
    public int NumTopologyMutations
    {
        get
        {
            return NumNewNodeMutations + NumNewConnectionsMutations;
        }
    }

    public override string ToString()
    {
        return
            "Weight Mutation Chance per Genome: " + WeightMutationChancePerGenome + " (factor " + MutationChanceScaleFactor + "), " + "Topology Mutation Chance per Genome: " + TopologyMutationChancePerGenome + "(factor " + MutationChanceScaleFactor + "), " + "Multiple Mutations per Genome Allowed: " + MultipleMutationsPerGenomeAllowed + ", Reenable Connection Chance per Topology Mutation: " + ConnectionReenableChancePerTopologyMutation +
            "\nTopology Mutations: " + NumTopologyMutations + " on " + NumTopologyMutatedGenomes + " Genomes (" + NumNewConnectionsMutations + " [" + NumNewUniqueConnectionsMutations + " unique] new Connections, " + NumNewNodeMutations + " [" + NumNewUniqueNodeMutations + " unique] new Nodes, " + NumReenableConnectionMutations + " reenabled connections)" +
            "\nWeight Mutations: " + NumWeightMutations + " on " + NumWeightMutations + " Genomes (" + NumReplaceMutations + " Replaces, " + NumShiftMutations + " Shifts, " + NumScaleMutations + " Scales, " + NumInvertMutations + " Inverses, " + NumSwapMutations + " Swaps)";
    }
}
