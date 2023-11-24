using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Speciator {

    public System.Random Random;
    public int SpeciesId = 0;

    public float CompatibilityThreshold;

    public Speciator(float compatibilityThreshhold)
    {
        CompatibilityThreshold = compatibilityThreshhold;
        Random = new System.Random();
    }

    /// <summary>
    /// Speciate all subjects in the given list to the given species. 
    /// <br/> If a subject is not compatible with any existing species, a new one will be created and added to the list.
    /// <br/> Returns the number of new species.
    /// </summary>
    public int SpeciateByCompatibility(List<Subject> subjects, List<Species> existingSpecies)
    {
        int numNewSpecies = 0;
        foreach (Subject Subject in subjects)
        {
            bool speciesFound = false;
            float lowestDistance = float.MaxValue;
            Species matchingSpecies = null;
            foreach (Species species in existingSpecies)
            {
                float compatibilityDistance = CompatibilityDistance(Subject.Genome, species.Representative);
                if (compatibilityDistance <= CompatibilityThreshold && compatibilityDistance < lowestDistance)
                {
                    lowestDistance = compatibilityDistance;
                    matchingSpecies = species;
                    speciesFound = true;
                }
            }
            if (!speciesFound)
            {
                Species newSpecies = CreateNewSpecies(Subject.Genome, existingSpecies);
                newSpecies.Color.a = 1;
                Subject.Genome.Species = newSpecies;
                existingSpecies.Add(newSpecies);
                numNewSpecies++;
            }
            else
                Subject.Genome.Species = matchingSpecies;
        }
        return numNewSpecies;
    }

    /// <summary>
    /// Assigns all subjects into one of the species in an even distribution.
    /// </summary>
    public void SpeciateRandomly(List<Subject> subjects, List<Species> existingSpecies)
    {
        int speciesIndex = 0;
        foreach(Subject subject in subjects)
        {
            subject.Genome.Species = existingSpecies[speciesIndex];
            speciesIndex++;
            if (speciesIndex >= existingSpecies.Count) speciesIndex = 0;
        }
    }

    /// <summary>
    /// Creates a new species with a unique id and color for the given representative genome.
    /// </summary>
    public Species CreateNewSpecies(Genome representativeGenome, List<Species> existingSpecies)
    {
        return new Species(SpeciesId++, representativeGenome, ColorUtils.RandomColor(existingSpecies.Select(x => x.Color).ToArray()));
    }

    public float CompatibilityDistance(Genome genome1, Genome genome2)
    {
        // Find highest gene id
        List<int> genome1GeneIds = genome1.Connections.Keys.ToList();
        List<int> genome2GeneIds = genome2.Connections.Keys.ToList();

        int numNonMatchingGenes = genome1GeneIds.Except(genome2GeneIds).Count() + genome2GeneIds.Except(genome1GeneIds).Count();

        List<int> matchingConnections = genome1GeneIds.Intersect(genome2GeneIds).ToList();
        float totalWeightDiff = 0;
        foreach(int connectionId in matchingConnections)
        {
            float g1Weight = genome1.Connections[connectionId].Weight;
            float g2Weight = genome2.Connections[connectionId].Weight;
            float weightDiff = Mathf.Abs(g1Weight - g2Weight);
            totalWeightDiff += weightDiff;
        }

        return (1f * numNonMatchingGenes) + (0.5f * totalWeightDiff);

    }
}
