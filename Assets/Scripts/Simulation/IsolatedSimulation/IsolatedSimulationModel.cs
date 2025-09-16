using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The god class that handles the whole learning process of a population over multiple generations.
/// <br/> Each generation, each subject in the population performs a task according to its neural network, achieving a fitness value.
/// <br/> In this "isolated" model, subjects do not affect other at all and perform the task completely independent from each other.
/// <br/> Between the generations subjects will evolve and mutate, prioritizing the best genes. Species will rise and fall.
/// </summary>
public abstract class IsolatedSimulationModel : MonoBehaviour
{
    // Simulation
    public IsolatedSimulationPhase SimulationPhase;

    // Subjects
    protected int SubjectInputSize; // Amount of input nodes each genome has
    protected int SubjectOutputSize; // Amount of output nodes each genome has
    protected int[] SubjectHiddenSizes; // Amount of hidden nodes (per layer) each genome has

    // Population
    protected Population Population { get; private set; }
    public int Generation => Population.Generation;

    /// <summary>
    /// Total amount of subjects.
    /// </summary>
    protected abstract int PopulationSize { get; }

    /// <summary>
    /// Dictionary containing the task for each subject in the current generation.
    /// </summary>
    private Dictionary<Subject, Task> CurrentTasks;

    /// <summary>
    /// List containing all relevant information for each generation. Index of each element equals the generation number.
    /// </summary>
    public List<GenerationStats> GenerationHistory;

    private DateTime CurrentGenerationStartTime;

    #region Initialization

    // Start is called before the first frame update
    void Start()
    {
        GenerationHistory = new List<GenerationStats>();

        OnPrePopulationInit();
        InitializeFirstGeneration();
        OnPostPopulationInit();
    }

    /// <summary>
    /// Gets called once at the very start of the simulation before the initial population has been created. Use this to set Network sizes.
    /// </summary>
    public abstract void OnPrePopulationInit();

    /// <summary>
    /// Gets called once at the very start of the simulation after the initial population has been created. Use this initialize relevant stuff that requires the initial generation.
    /// </summary>
    public abstract void OnPostPopulationInit();

    /// <summary>
    /// Function to create a generation stats object of the correct subtype.
    /// </summary>
    public abstract GenerationStats CreateGenerationStats(EvolutionInformation info);

    #endregion

    #region Loop (to override)

    protected abstract Task CreateTaskFor(Subject s);

    /// <summary>
    /// Gets calles when the population of a new generation has been generated and all tasks have been created. To start performing this generations tasks call StartGeneration().
    /// <br/> Includes the evolution information of the next generations population.
    /// </summary>
    protected virtual void OnGenerationInitialized(EvolutionInformation info) { }

    /// <summary>
    /// Gets called right after all subjects of this generation have started performing their task.
    /// </summary>
    protected virtual void OnGenerationStarted() { }

    /// <summary>
    /// Gets called when all subjects of a generation are done performing and all fitness values and ranks are assigned.
    /// </summary>
    protected virtual void OnGenerationFinished() { }

    /// <summary>
    /// Gets called every frame regardless of the simulation phase.
    /// </summary>
    protected virtual void OnUpdate() { }

    #endregion

    #region Base Loop (to call)

    public void InitializeNextGeneration()
    {
        if (SimulationPhase != IsolatedSimulationPhase.Done) return;

        // Create next generation population
        Debug.Log($"Creating population for generation {Generation + 1}.");
        EvolutionInformation info = Population.EvolveGeneration();

        // Initialize generation stats
        GenerationHistory.Add(CreateGenerationStats(info));

        // Create a task for each subject in this generation's population
        CurrentTasks = new Dictionary<Subject, Task>();
        foreach (Subject s in Population.Subjects)
        {
            CurrentTasks.Add(s, CreateTaskFor(s));
        }

        SimulationPhase = IsolatedSimulationPhase.Ready;

        // Hook
        OnGenerationInitialized(info);
    }

    /// <summary>
    /// Call this to make all subjects of this generation start performing their task.
    /// </summary>
    public void StartGeneration()
    {
        if (SimulationPhase != IsolatedSimulationPhase.Ready) return;

        Debug.Log($"Starting Generation {Generation}.");

        CurrentGenerationStartTime = DateTime.Now;

        foreach (Task task in CurrentTasks.Values)
        {
            task.Start();
        }

        SimulationPhase = IsolatedSimulationPhase.Running;

        // Hook
        OnGenerationStarted();
    }

    #endregion

    #region Base Loop (private)

    private void InitializeFirstGeneration()
    {
        // Initialize population with random subjects
        Population = new Population(PopulationSize, SubjectInputSize, SubjectHiddenSizes, SubjectOutputSize, GetFitnessValueFor);

        // Initialize generation stats
        GenerationHistory.Add(CreateGenerationStats(null));

        // Create a task for each subject in this generation's population
        CurrentTasks = new Dictionary<Subject, Task>();
        foreach (Subject s in Population.Subjects)
        {
            CurrentTasks.Add(s, CreateTaskFor(s));
        }

        SimulationPhase = IsolatedSimulationPhase.Ready;

        // Hook
        OnGenerationInitialized(null);
    }

    private void Update()
    {
        if (SimulationPhase == IsolatedSimulationPhase.Running)
        {
            // Advance all tasks
            foreach (Task task in CurrentTasks.Values)
            {
                task.Tick();
            }

            // End generation if all tasks are done
            if (CurrentTasks.Values.All(t => t.IsDone))
            {
                EndGeneration();
            }
        }

        OnUpdate();
    }

    private void EndGeneration()
    {
        if (SimulationPhase != IsolatedSimulationPhase.Running) return;

        // Complete generation info, calculating all generation stats
        float runtime = (int)((DateTime.Now - CurrentGenerationStartTime).TotalSeconds);
        GenerationHistory.Last().SetComplete(runtime, CurrentTasks);

        SimulationPhase = IsolatedSimulationPhase.Done;

        // Hook
        OnGenerationFinished();
    }

    private float GetFitnessValueFor(Subject s) => CurrentTasks[s].GetFitnessValue();

    #endregion
}
