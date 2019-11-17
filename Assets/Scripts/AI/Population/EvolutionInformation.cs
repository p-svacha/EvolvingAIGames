using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionInformation
{
    // Title
    public int Generation;
    public int EvolutionTime; // in ms

    // Mutations
    public MutationInformation MutationInfo;

    // Subjects
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


    // Species
    public int NumPreviousSpecies;
    public int NumEliminatedSpecies;
    public int NumEmptySpecies;
    public int NumNewSpecies;
    public int NumSpecies;
    public float CompatibilityThreshhold;

    // Fitness
    public float MaxFitness;
    public float AverageFitness;

    // Misc
    public int NumSubjectsCheckedForAdoption;
    public int NumImmuneToMutationSubjects;
    public int RankLimit;
    public int GensAllowedBelowLimit;
    public bool MutationImmunityForTakeOvers;

    public EvolutionInformation(int generation, int evolutionTime, bool mutationImmunityForTakeOvers, MutationInformation mutationInfo, int numBestSubjectsTakenOver, int numRandomSubjectsTakenOver, int numOffsprings, int numSubjectsCheckedForAdoption, int numImmuneToMutationSubjects, int numPreviousSpecies, int numEliminatedSpecies, int numEmptySpecies, int numNewSpecies, int numSpecies, float compThreshhold, float maxFitness, float averageFitness, int limit, int gensBelowLimit)
    {
        Generation = generation;
        EvolutionTime = evolutionTime;
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
        CompatibilityThreshhold = compThreshhold;
        MaxFitness = maxFitness;
        AverageFitness = averageFitness;
        RankLimit = limit;
        GensAllowedBelowLimit = gensBelowLimit;
    }

    public override string ToString()
    {
        return
            "Generation: " + Generation + " (" + EvolutionTime + " ms)" +
            "\nSubjects: " + NumSubjects + " (" + NumOffsprings + " Offsprings, " + NumBestSubjectsTakenOver + " Best, " + NumRandomSubjectsTakenOver + " Randoms)" +
            "\nSpecies: " + NumSpecies + " (" + NumPreviousSpecies + " Previous, " + NumNewSpecies + " New, " + NumEliminatedSpecies + " Eliminated, " + NumEmptySpecies + " Empty), Compatibility Threshhold: " + CompatibilityThreshhold +
            "\nMaxFitness: " + (int)MaxFitness + ", Average Fitness: " + (int)AverageFitness +
            "\n\nSubjects checked for adoption: " + NumSubjectsCheckedForAdoption + ", Subjects immune to mutations: " + NumImmuneToMutationSubjects +
            "\nRank Limit: " + RankLimit + ", Generations below rank limit before extincion: " + GensAllowedBelowLimit +
            "\n" + MutationInfo.ToString() + "\n";
    }
}
