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
    private Population Population;
    public int Generation => Population.Generation;

    /// <summary>
    /// Total amount of subjects.
    /// </summary>
    protected abstract int PopulationSize { get; }

    /// <summary>
    /// Dictionary holding all tasks ever performed (ordered by generation, then by subject).
    /// </summary>
    private Dictionary<int,Dictionary<Subject,Task>> Tasks;

    #region Initialization

    // Start is called before the first frame update
    void Start()
    {
        Tasks = new Dictionary<int, Dictionary<Subject, Task>>();

        OnInit();

        InitializeFirstGeneration();
    }

    /// <summary>
    /// Gets called once at the very start of the simulation. Use this to set Network sizes and initialize other relevant stuff.
    /// </summary>
    public abstract void OnInit();

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

    #endregion

    #region Base Loop (to call)

    protected void InitializeNextGeneration()
    {
        if (SimulationPhase != IsolatedSimulationPhase.Done) return;

        // Create next generation population
        EvolutionInformation info = Population.EvolveGeneration();

        // Create a task for each subject in this generation's population
        Tasks.Add(Generation, new Dictionary<Subject, Task>());
        foreach (Subject s in Population.Subjects)
        {
            Tasks[Generation].Add(s, CreateTaskFor(s));
        }

        SimulationPhase = IsolatedSimulationPhase.Ready;

        // Hook
        OnGenerationInitialized(info);
    }

    /// <summary>
    /// Call this to make all subjects of this generation start performing their task.
    /// </summary>
    protected void StartGeneration()
    {
        if (SimulationPhase != IsolatedSimulationPhase.Ready) return;

        foreach (Task task in Tasks[Generation].Values)
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

        // Create a task for each subject in this generation's population
        Tasks.Add(Generation, new Dictionary<Subject, Task>());
        foreach (Subject s in Population.Subjects)
        {
            Tasks[Generation].Add(s, CreateTaskFor(s));
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
            foreach (Task task in Tasks[Generation].Values)
            {
                task.Tick();
            }

            // End generation if all tasks are done
            if (Tasks[Generation].Values.All(t => t.IsDone))
            {
                EndGeneration();
            }
        }
    }

    private void EndGeneration()
    {
        if (SimulationPhase != IsolatedSimulationPhase.Running) return;

        // Set rank of all subjects in their tasks
        List<Subject> orderedSubjects = Tasks[Generation].OrderByDescending(t => t.Value.GetFitnessValue()).Select(t => t.Key).ToList();
        for (int i = 0; i < orderedSubjects.Count; i++) Tasks[Generation][orderedSubjects[i]].SetRank(i + 1);

        SimulationPhase = IsolatedSimulationPhase.Done;

        // Hook
        OnGenerationFinished();
    }

    private float GetFitnessValueFor(Subject s) => Tasks[Generation][s].GetFitnessValue();

    #endregion
}
