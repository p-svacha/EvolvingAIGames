using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionInformation
{
    public int Generation;

    public bool MutationImmunityForTakeOvers;
    public MutationInformation MutationInfo;

    public int NumBestSubjectsTakenOver;
    public int NumRandomSubjectsTakenOver;
    public int NumOffsprings;
    public int NumSubjects
    {
        get
        {
            return NumBestSubjectsTakenOver + NumRandomSubjectsTakenOver + NumOffsprings;
        }
    }
    public int NumSubjectsCheckedForAdoption;
    public int NumImmuneToMutationSubjects;

    public int NumPreviousSpecies;
    public int NumEliminatedSpecies;
    public int NumEmptySpecies;
    public int NumNewSpecies;
    public int NumSpecies;

    public float MaxFitness;
    public float AverageFitness;

    public int RankLimit;
    public int GensAllowedBelowLimit;

    public EvolutionInformation(int generation, bool mutationImmunityForTakeOvers, MutationInformation mutationInfo, int numBestSubjectsTakenOver, int numRandomSubjectsTakenOver, int numOffsprings, int numSubjectsCheckedForAdoption, int numImmuneToMutationSubjects, int numPreviousSpecies, int numEliminatedSpecies, int numEmptySpecies, int numNewSpecies, int numSpecies, float maxFitness, float averageFitness, int limit, int gensBelowLimit)
    {
        Generation = generation;
        MutationImmunityForTakeOvers = mutationImmunityForTakeOvers;
        MutationInfo = mutationInfo;
        NumBestSubjectsTakenOver = numBestSubjectsTakenOver;
        NumRandomSubjectsTakenOver = numRandomSubjectsTakenOver;
        NumOffsprings = numOffsprings;
        NumSubjectsCheckedForAdoption = numSubjectsCheckedForAdoption;
        NumImmuneToMutationSubjects = numImmuneToMutationSubjects;
        NumPreviousSpecies = numPreviousSpecies;
        NumEliminatedSpecies = numEliminatedSpecies;
        NumEmptySpecies = numEmptySpecies;
        NumNewSpecies = numNewSpecies;
        NumSpecies = numSpecies;
        MaxFitness = maxFitness;
        AverageFitness = averageFitness;
        RankLimit = limit;
        GensAllowedBelowLimit = gensBelowLimit;
    }

    public override string ToString()
    {
        return
            "Generation: " + Generation +
            "\nSubjects: " + NumSubjects + " (" + NumOffsprings + " Offsprings, " + NumBestSubjectsTakenOver + " Best, " + NumRandomSubjectsTakenOver + " Randoms)" +
            "\nSpecies: " + NumSpecies + " (" + NumPreviousSpecies + " Previous, " + NumNewSpecies + " New, " + NumEliminatedSpecies + " Eliminated, " + NumEmptySpecies + " Empty)" +
            "\nMaxFitness: " + (int)MaxFitness + ", Average Fitness: " + (int)AverageFitness +
            "\n\nSubjects checked for adoption: " + NumSubjectsCheckedForAdoption + ", Subjects immune to mutations: " + NumImmuneToMutationSubjects +
            "\nRank Limit: " + RankLimit + ", Generations below rank limit before extincion: " + GensAllowedBelowLimit +
            "\n" + MutationInfo.ToString() + "\n";
    }
}
