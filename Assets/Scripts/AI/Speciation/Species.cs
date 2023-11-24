using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Species {

    public System.Random Random;

    public int Id;
    public List<Subject> Subjects;
    public Genome Representative;
    public Color Color;

    public int GenerationsWithoutImprovement;

    // Stats from last generation
    public float MaxFitness { get; set; }
    public float AverageFitness { get; set; }
    public int Rank { get; set; }

    // Offspring calculation
    public float OffspringCalculationTempFitness;
    public float OffspringCount;


    public Species(int id, Genome rep, Color color)
    {
        Id = id;
        Random = new System.Random();
        Representative = rep;
        Color = color;
        Subjects = new List<Subject>();
        GenerationsWithoutImprovement = 0;
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
        // Remove worst ignoreRatio % of candidates
        int numCandidates = (int)(Size * (1f - ignoreRatio));
        if (numCandidates == 0) numCandidates = 1;
        List<Subject> candidates = Subjects.OrderByDescending(x => x.Genome.AdjustedFitness).Take(numCandidates).ToList();
        int totalFitness = (int)(candidates.Sum(x => x.Genome.AdjustedFitness));
        int rng = Random.Next(totalFitness);
        //int rng = Random.Next((int)(Subjects.Sum(x => x.Genome.AdjustedFitness)));
        float sum = Subjects.First().Genome.AdjustedFitness;
        int index = 0;
        while (rng > sum)
        {
            index++;
            sum += Subjects[index].Genome.AdjustedFitness;
        }
        return Subjects[index];
    }

    public int Size
    {
        get
        {
            return Subjects.Count;
        }
    }
}
