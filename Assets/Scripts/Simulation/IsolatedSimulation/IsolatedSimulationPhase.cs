using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// All states the simulation can have during the simulation one generation.
/// </summary>
public enum IsolatedSimulationPhase
{
    /// <summary>
    /// The population of this generation has been generated, and all subject tasks are initialized and ready to start.
    /// </summary>
    Ready,

    /// <summary>
    /// All or some subjects are currently performing their task that will result in their fitness value. Some may be finished.
    /// </summary>
    Running,

    /// <summary>
    /// All subjects are done performing their task and all final fitness values and ranks have been assigned.
    /// </summary>
    Done,
}
