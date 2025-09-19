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
    public int Rank { get; set; }

    // Offspring calculation
    public float OffspringCalculationTempFitness;
    public float OffspringCount;


    public Species(int id, string name, Genome rep, Color color)
    {
        Id = id;
        Name = name;
        Random = new System.Random();
        Representative = rep;
        Color = color;
        Subjects = new List<Subject>();
        GenerationsBelowEliminationThreshold = 0;
    }

    public void CalculateFitnessValues()
    {
        AverageFitness = Subjects.Select(x => x.Genome).Sum(x => x.AdjustedFitness);
        MaxFitness = Subjects.Select(x => x.Genome).Max(x => x.Fitness);
    }

    public Genome CreateOffspring(float ignoreRatio)
    {
        Subject parent1 = GetProportionalRandomParent(ignoreRatio);
        Subject parent2 = GetProportionalRandomParent(ignoreRatio);
        return CrossoverAlgorithm.Crossover(parent1.Genome, parent2.Genome);
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
