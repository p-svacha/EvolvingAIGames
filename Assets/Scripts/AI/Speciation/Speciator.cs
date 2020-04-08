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
    /// Speciate all subjects in the given list to the given species. If a subject does not fit to a species, a new one will be created and added to the list.
    /// Returns the number of new species.
    /// </summary>
    public int Speciate(List<Subject> Subjects, List<Species> Species)
    {
        int numNewSpecies = 0;
        foreach (Subject Subject in Subjects)
        {
            bool speciesFound = false;
            float lowestDistance = float.MaxValue;
            Species matchingSpecies = null;
            foreach (Species species in Species)
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
                Species newSpecies = new Species(SpeciesId++, Subject.Genome, ColorUtils.RandomColor(Species.Select(x => x.Color).ToArray()), 0);
                newSpecies.Color.a = 1;
                Subject.Genome.Species = newSpecies;
                Species.Add(newSpecies);
                numNewSpecies++;
            }
            else
                Subject.Genome.Species = matchingSpecies;
        }
        return numNewSpecies;
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
