using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A task refers to the thing each subject needs to do every generation in an Isolated Simulation.
/// <br/> A task can be anything, but every task is tick-based and will result in a fitness value in the end, used to advance the simulation.
/// <br/> While a task is running, the neural network of the subject performing is run through each tick.
/// </summary>
public abstract class Task
{
    /// <summary>
    /// The subject associated with this task.
    /// </summary>
    public Subject Subject { get; private set; }

    /// <summary>
    /// The current state of this task. (Ready / Running / Done)
    /// </summary>
    private TaskState State;

    /// <summary>
    /// The final fitness value of this task/subject at the end of a generation.
    /// </summary>
    private int Fitness;

    /// <summary>
    /// The current tick number of this task.
    /// </summary>
    public int TickNumber { get; private set; }

    public Task() { }
    public Task(Subject subject)
    {
        Subject = subject;
        State = TaskState.Ready;
    }

    /// <summary>
    /// Called to start the task. Once started, call Tick() each tick to advance it.
    /// </summary>
    public void Start()
    {
        if (State != TaskState.Ready) return;
        State = TaskState.Running;
        OnStart();
    }
    protected abstract void OnStart();

    /// <summary>
    /// Called to advance the task by one frame.
    /// </summary>
    public void Tick()
    {
        if (State != TaskState.Running) return;
        IncrementTick();
        OnTick();
    }
    protected abstract void OnTick();
    protected void IncrementTick() { TickNumber++; }

    /// <summary>
    /// Ends this task.
    /// </summary>
    public void End()
    {
        if (State != TaskState.Running) return;
        Fitness = CalculateFitnessValue();
        State = TaskState.Done;
        OnEnd();
    }
    protected abstract void OnEnd();
    protected abstract int CalculateFitnessValue();

    /// <summary>
    /// Returns the fitness value of a finished task.
    /// </summary>
    public int GetFitnessValue()
    {
        if (State != TaskState.Done) throw new System.Exception("Can't get fitness value of a task that is not done.");
        return Fitness;
    }
    

    public bool IsDone => State == TaskState.Done;
}
