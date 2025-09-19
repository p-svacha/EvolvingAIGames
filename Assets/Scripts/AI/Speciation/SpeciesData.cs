using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A simple object holding the most important information about a species during a generation.
/// </summary>
public class SpeciesData
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public Color Color { get; private set; }
    public int SubjectCount { get; private set; }

    public int Rank;
    public int GenerationsBelowEliminationThreshold;
    public float MaxFitness;
    public float AverageFitness;

    public SpeciesData(int id, string name, Color color, int subjectCount, int elim)
    {
        Id = id;
        Name = name;
        Color = color;
        SubjectCount = subjectCount;
        GenerationsBelowEliminationThreshold = elim;
    }
}
