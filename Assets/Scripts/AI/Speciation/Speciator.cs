using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Speciator {

    public System.Random Random;
    public int SpeciesId = 0;

    public float CompatibilityThreshold;

    private MarkovChainWordGenerator NameGenerator;

    public Speciator(float compatibilityThreshhold)
    {
        CompatibilityThreshold = compatibilityThreshhold;
        Random = new System.Random();

        NameGeneratorInputDataReader.Init();
        NameGenerator = new MarkovChainWordGenerator();
    }

    /// <summary>
    /// Speciate all subjects in the given list to the given species. 
    /// <br/> If a subject is not compatible with any existing species, a new one will be created and added to the list.
    /// <br/> Returns the number of new species.
    /// </summary>
    public int SpeciateByCompatibility(List<Subject> subjects, List<Species> existingSpecies)
    {
        float highestCompatibilityDistance = float.MinValue; // Just for debugging - the highest distance that any genome was from any existing species.

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
                if (compatibilityDistance > highestCompatibilityDistance) highestCompatibilityDistance = compatibilityDistance;
            }
            // Debug.Log($"Lowest distance to another species: {lowestDistance}");
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

        Debug.Log($"The biggest distance any genome was from an existing species was {highestCompatibilityDistance}");

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
        return new Species(SpeciesId++, GetNewSpeciesName(), representativeGenome, ColorUtils.RandomColor(existingSpecies.Select(x => x.Color).ToArray()));
    }

    private string GetNewSpeciesName()
    {
        return NameGenerator.GenerateWord(wordType: "Species", nGramLength: 4, maxLength: 10).CapitalizeEachWord();
    }

    public float CompatibilityDistance(Genome g1, Genome g2)
    {
        const float c1 = 1.0f;   // topology weight
        const float c3 = 0.4f;   // weight diff

        var ids1 = new HashSet<int>(g1.Connections.Keys);
        var ids2 = new HashSet<int>(g2.Connections.Keys);

        int matches = 0;
        float wdiff = 0f;
        foreach (var id in ids1) if (ids2.Contains(id)) { matches++; wdiff += Mathf.Abs(g1.Connections[id].Weight - g2.Connections[id].Weight); }

        int diff = ids1.Except(ids2).Count() + ids2.Except(ids1).Count();

        // Key change: cap N so large nets don’t wash out topology
        int N = Mathf.Max(ids1.Count, ids2.Count);
        int Ncap = 20;                      // NEAT-style cap
        float norm = Mathf.Max(1, Mathf.Min(N, Ncap));

        float wBar = matches > 0 ? wdiff / matches : 0f;
        return (c1 * diff) / norm + c3 * wBar;
    }
}
