using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MutationInformation
{
    public int NumNewNodeMutations;
    public int NumNewUniqueNodeMutations;
    public int NumNewConnectionsMutations;
    public int NumNewUniqueConnectionsMutations;
    public int NumTopologyMutations
    {
        get
        {
            return NumNewNodeMutations + NumNewConnectionsMutations;
        }
    }

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

    public override string ToString()
    {
        return
            "Topology Mutations: " + NumTopologyMutations + " (" + NumNewConnectionsMutations + " [" + NumNewUniqueConnectionsMutations + " unique] new Connections, " + NumNewNodeMutations + " [" + NumNewUniqueNodeMutations + " unique] new Nodes)" +
            "\nWeight Mutations: " + NumWeightMutations + " (" + NumReplaceMutations + " Replaces, " + NumShiftMutations + " Shifts, " + NumScaleMutations + " Scales, " + NumInvertMutations + " Invers, " + NumSwapMutations + " Swaps)";
    }
}
