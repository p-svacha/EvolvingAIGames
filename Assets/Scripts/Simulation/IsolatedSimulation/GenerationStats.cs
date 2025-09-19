using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Object containing various stats about a single generation in an Isolated Simulation.
/// </summary>
public class GenerationStats
{
    /// <summary>
    /// The evolution information leading to the population in this generation.
    /// </summary>
    public EvolutionInformation EvolutionInfo { get; private set; }
    public int GenerationNumber => EvolutionInfo == null ? 0 : EvolutionInfo.Generation;

    /// <summary>
    /// Flag if this generation is done simulating. If true, all stats are available.
    /// </summary>
    public bool IsComplete { get; private set; }

    /// <summary>
    /// How long it took for this generation to complete (in seconds).
    /// </summary>
    public float Runtime { get; private set; }

    /// <summary>
    /// Species distribution in this generation.
    /// </summary>
    public List<SpeciesData> SpeciesData { get; private set; }

    private Dictionary<Subject, Task> Tasks;
    public List<Task> TaskRanking;

    public Task BestPerformingTask { get; private set; }
    public Subject BestPerformingSubject => BestPerformingTask.Subject;

    public Task MedianTask { get; private set; }
    public Subject MedianSubject => MedianTask.Subject;

    /// <summary>
    /// Create generation stats for a generation with a population that has evolved from a previous generation.
    /// </summary>
    public GenerationStats(EvolutionInformation evoInfo, List<Species> speciesData)
    {
        EvolutionInfo = evoInfo;
        IsComplete = false;

        // Aggregate species data
        SpeciesData = new List<SpeciesData>();
        foreach (Species s in speciesData)
        {
            SpeciesData data = new SpeciesData(s.Id, s.Name, s.Color, s.Subjects.Count, s.GenerationsBelowEliminationThreshold);
            SpeciesData.Add(data);
        }
        SpeciesData = SpeciesData.OrderByDescending(x => x.SubjectCount).ToList();
    }

    /// <summary>
    /// Flags this generation as complete and calculates many additional stats based on the tasks performed.
    /// </summary>
    public void SetComplete(float runtime, Dictionary<Subject, Task> tasks)
    {
        Runtime = runtime;
        Tasks = tasks;

        // Set rank of all subjects in their tasks
        TaskRanking = Tasks.Values.OrderByDescending(t => t.GetFitnessValue()).ToList();

        BestPerformingTask = TaskRanking.First();
        MedianTask = GetPercentileTask(0.5f);

        // Complete species data
        foreach (SpeciesData data in SpeciesData)
        {
            List<Task> speciesTasks = tasks.Where(x => x.Key.Genome.Species.Id == data.Id).Select(x => x.Value).ToList();
            float avgFitness = (float)speciesTasks.Average(x => x.GetFitnessValue());
            float maxFitness = speciesTasks.Max(x => x.GetFitnessValue());

            data.AverageFitness = avgFitness;
            data.MaxFitness = maxFitness;
        }
        SpeciesData = SpeciesData.OrderByDescending(x => x.AverageFitness).ToList();
        for (int i = 0; i < SpeciesData.Count; i++) SpeciesData[i].Rank = i + 1; 


        OnComplete();

        IsComplete = true;
    }

    protected virtual void OnComplete() { }

    /// <summary>
    /// Returns the task that is ranked so that [PERCENTILE]% of all tasks are ranked higher than it and [100-PERCENTILE]% are ranked lower. 
    /// </summary>
    public Task GetPercentileTask(float percentile)
    {
        if (TaskRanking == null || TaskRanking.Count == 0)
            throw new System.Exception("No tasks available for percentile query.");

        percentile = Mathf.Clamp01(percentile);

        if (percentile <= 0f) return TaskRanking[TaskRanking.Count - 1]; // 0th = worst? (you ranked descending)
        if (percentile >= 1f) return TaskRanking[0];                     // 100th = best

        // You ranked DESC by fitness. Convert percentile to descending index.
        // For p in (0,1): idxAsc = ceil(p*N) - 1; idxDesc = (N-1) - idxAsc
        int n = TaskRanking.Count;
        int idxAsc = Mathf.CeilToInt(percentile * n) - 1;
        idxAsc = Mathf.Clamp(idxAsc, 0, n - 1);
        int idxDesc = (n - 1) - idxAsc;

        return TaskRanking[idxDesc];
    }

    public int GetPercentileFitness(float percentile) => GetPercentileTask(percentile).GetFitnessValue();

}
