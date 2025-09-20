using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Species {

    public System.Random Random;

    public int Id;
    public string Name;
    public List<Subject> Subjects;
    public Genome Representative;
    public Color Color;

    public int GenerationsBelowEliminationThreshold;

    // Stats from last generation
    public float MaxFitness { get; set; }
    public float AverageFitness { get; set; }

    /// <summary>
    /// Rating that is used for species ranking, tracking species progress, calculation of elimination criteria, and calculation of how many offspring each species can have.
    /// <br/>Species rating is the average of top 30% performers in a species.
    /// </summary>
    public float SpeciesRating { get; set; }
    public int Rank { get; set; }

    // Offspring calculation
    public float OffspringCalculationTempFitness;
    public float OffspringCount;


    public Species(int id, string name, Genome rep, Color color)
    {
        Id = id;
        Name = name;
        Random = new System.Random();
        Representative = rep.Copy();
        Color = color;
        Subjects = new List<Subject>();
        GenerationsBelowEliminationThreshold = 0;
    }

    public void CalculateFitnessValues(float speciesRatingPortion)
    {
        AverageFitness = Subjects.Select(x => x.Genome).Sum(x => x.AdjustedFitness);
        MaxFitness = Subjects.Select(x => x.Genome).Max(x => x.Fitness);

        // Rating
        List<float> fitnesses = Subjects.Select(s => s.Genome.Fitness).OrderByDescending(f => f).ToList();
        int k = Mathf.Max(1, Mathf.CeilToInt(fitnesses.Count * speciesRatingPortion));
        float sumTop = 0f;
        for (int i = 0; i < k; i++) sumTop += fitnesses[i];
        SpeciesRating = sumTop / k;
    }

    /// <summary>
    /// Creates a new child genome in this species. The parent selection is skewed towards better performing ones.
    /// </summary>
    public Genome CreateOffspring(MutateAlgorithm mutator, float ignoreRatio, int parentSelectionPoolSize, float chanceToPickTopPerformerAsParent, float asexualReproductionChance, float connectionReenableChancePerTopologyMutation)
    {
        // Build pool of eligible parent candidates
        List<Subject> parentCandidates = Subjects.OrderByDescending(s => s.Genome.AdjustedFitness).ToList();
        int numWorstToIgnore = (int)Mathf.Ceil(parentCandidates.Count * ignoreRatio);
        parentCandidates = parentCandidates.Take(Mathf.Max(1, parentCandidates.Count - numWorstToIgnore)).ToList();

        // Check for asexual reproduction
        if (parentCandidates.Count == 1 || Random.NextDouble() < asexualReproductionChance)
        {
            // Create exact copy
            Subject singularParent = SelectRandomParent(parentCandidates, parentSelectionPoolSize, chanceToPickTopPerformerAsParent);
            Genome childGenome = singularParent.Genome.Copy();
            childGenome.Species = singularParent.Genome.Species;

            mutator.ForceMutation(childGenome, connectionReenableChancePerTopologyMutation);
            return childGenome;
        }

        // Sexual reproduction (default)
        Subject parent1 = SelectRandomParent(parentCandidates, parentSelectionPoolSize, chanceToPickTopPerformerAsParent);
        Subject parent2;
        if (parentCandidates.Count > 1)
        {
            // Build a candidate list without parent1
            List<Subject> parent2Candidates = parentCandidates.Where(s => s != parent1).ToList();
            parent2 = SelectRandomParent(parent2Candidates, parentSelectionPoolSize, chanceToPickTopPerformerAsParent);
        }
        else
        {
            parent2 = parent1; // Degenerate case; will behave like asexual crossover
        }
        
        // Perform crossover
        return CrossoverAlgorithm.Crossover(this, parent1.Genome, parent2.Genome);
    }

    /// <summary>
    /// Selects a parent from a given pool of candidates. Candidate list is expected to be sorted already by fitness.
    /// There is a % chance (chanceToPickTopPerformerAsParent) that the best performing candidate will be chosen.
    /// Else the best from a number (parentSelectionPoolSize) of fully-randomly selected one is chosen.
    /// </summary>
    private Subject SelectRandomParent(List<Subject> candidates, int parentSelectionPoolSize, float chanceToPickTopPerformerAsParent)
    {
        if (candidates == null || candidates.Count == 0) throw new System.Exception("No parent candidates.");

        // Chance to simply pick to performers.
        if (Random.NextDouble() < chanceToPickTopPerformerAsParent)
        {
            return candidates[0];
        }
        else // Pick best performing out of x randomly selected ones
        {
            int realPoolSize = Mathf.Clamp(parentSelectionPoolSize, 1, candidates.Count);
            List<Subject> randomPool = candidates.RandomElements(realPoolSize);
            return randomPool.OrderByDescending(s => s.Genome.AdjustedFitness).First();
        }
    }

    public Subject GetProportionalRandomParent(float ignoreRatio)
    {
        // Top X% by adjusted fitness
        var sorted = Subjects.OrderByDescending(s => s.Genome.AdjustedFitness).ToList();
        int count = Mathf.Max(1, Mathf.CeilToInt(sorted.Count * (1f - ignoreRatio)));
        var candidates = sorted.Take(count).ToList();

        double total = candidates.Sum(s => (double)s.Genome.AdjustedFitness);
        if (total <= 0.0)
        {
            // fallback uniform if everything is zero
            return candidates[Random.Next(candidates.Count)];
        }

        double pick = Random.NextDouble() * total;
        double cum = 0.0;
        foreach (var s in candidates)
        {
            cum += s.Genome.AdjustedFitness;
            if (pick <= cum) return s;
        }
        return candidates[candidates.Count - 1];
    }

    public int Size
    {
        get
        {
            return Subjects.Count;
        }
    }
}
