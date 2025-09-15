using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TaskState
{
    /// <summary>
    /// Task is ready to go but hasn't started.
    /// </summary>
    Ready,

    /// <summary>
    /// Task is currently running.
    /// </summary>
    Running,

    /// <summary>
    /// Task is done and fitness value can be returned.
    /// </summary>
    Done,
}
