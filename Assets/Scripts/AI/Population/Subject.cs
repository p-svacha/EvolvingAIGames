﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A subject represents a participant in one generation of the simulation process with a static genome.
/// <br/> Subjects will create a game-specific player instance for each match that handles match-specific data.
/// </summary>
public class Subject {

    public string Name;
    public int OverallRank;
    public Genome Genome;
    public bool ImmuneToMutation;

    // Match
    public List<Subject> Opponents;
    public int Wins;
    public int Losses;

    public Subject(Genome g)
    {
        Genome = g;
        Opponents = new List<Subject>();
    }


    // This method is only used if a generation has more than one simulation. Gets called when a simulation has ended.
    public virtual void EndCurrentSimulation()
    {

    }

    // This method returns the fitness of a subject at the point of the evolution of the next generation.
    public virtual float GetFitness()
    {
        float avgOpponentWinrate = Opponents.Count > 0 ? (float)(Opponents.Average(x => ((float)x.Wins / Opponents.Count()))) : 0; // Between 0 and 1
        return Wins + avgOpponentWinrate;
    }

    public void CalculateFitnessValues()
    {
        Genome.Fitness = GetFitness();
        Genome.AdjustedFitness = Genome.Fitness / Genome.Species.Size;
    }
}
