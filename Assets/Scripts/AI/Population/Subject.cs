using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Subject {

    public string Name;
    public int OverallRank;
    public Genome Genome;
    public bool ImmunteToMutation;

	public Subject(Genome g)
    {
        Genome = g;
    }

    // This method handles the stuff a subject does every frame.
	public virtual void UpdateSubject()
    {

	}

    // This method is only used if a generation has more than one simulations. Call this when ending a simulation.
    public virtual void EndCurrentSimulation()
    {

    }

    // This method returns the fitness of a subject at the point of the evolution of the next generation.
    public virtual float GetFitness()
    {
        return 0;
    }

    public void CalculateFitnessValues()
    {
        Genome.Fitness = GetFitness();
        Genome.AdjustedFitness = Genome.Fitness / Genome.Species.Size;
    }
}
