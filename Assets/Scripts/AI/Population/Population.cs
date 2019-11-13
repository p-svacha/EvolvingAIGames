using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Population {

    public System.Random Random = new System.Random();

    // Genomes
    public List<Species> Species;
    public List<Subject> Subjects;

    // Mutations
    public MutateAlgorithm MutateAlgorithm;
    public TopologyMutator TopologyMutator;
    public WeightMutator WeightMutator;

    // Evolution
    public Speciator Speciator;
    public int Generation;

    // Parameters
    public float TakeOverBestRatio = 0.15f; // % of best subjects per species that are taken over to next generation
    public float TakeOverRandomRatio = 0.05f; // % of random subjects per species that are taken over to next generation
    public bool AreTakeOversImmuneToMutation = true; // If true, subjects that are taken over the next generation are immune to mutation
    // Rest will be newly generated as offsprings of good performing genomes from previous generation

    public float IgnoreRatio = 0.35f; // % of worst performing subjects within a species to ignore when chosing a random parent

    public int RankNeededToSurvive = 4; // The rank needed for a species at least every {GenerationsWithoutImprovementPenalty} generations to not get eliminated
    public int GenerationsWithoutImprovementPenalty = 15; // Number of generations without reaching species rank {RankNeededToSurvive} allowed to not get eliminated

    public float AdoptionRate = 0.4f; // % chance that an offspring will be checked which species it belongs to. otherwise it will get the species of its parents

    // Debug
    public bool showTimestamps = false;

    public Population(int size, int numInputs, int numOutputs, bool startWithConnections)
    {
        Subjects = new List<Subject>();
        Species = new List<Species>();
        Speciator = new Speciator();
        TopologyMutator = new TopologyMutator();
        WeightMutator = new WeightMutator();
        MutateAlgorithm = new MutateAlgorithm(TopologyMutator, WeightMutator);
        CreateInitialPopulation(size, numInputs, numOutputs, startWithConnections);
    }

    private void CreateInitialPopulation(int size, int numInputs, int numOutputs, bool startWithConnections)
    {
        for (int g = 0; g < size; g++)
        {
            List<Node> inputs = new List<Node>();
            List<Node> outputs = new List<Node>();
            List<Connection> initialConnections = new List<Connection>();

            // Create input nodes
            for (int i = 0; i < numInputs; i++)
                inputs.Add(new Node(i, NodeType.Input));

            // Create output nodes
            for (int i = numInputs; i < (numInputs + numOutputs); i++)
                outputs.Add(new Node(i, NodeType.Output));

            // Create initial connections
            if (startWithConnections)
            {
                int counter = 0;
                foreach (Node input in inputs)
                {
                    foreach (Node output in outputs)
                    {
                        Connection con = new Connection(counter++, input, output);
                        initialConnections.Add(con);
                        input.OutConnections.Add(con);
                        output.InConnections.Add(con);
                    }
                }
            }

            // Create genome and add it to the subject
            Genome newGenome = new Genome(CrossoverAlgorithm.GenomeId++, inputs, outputs, initialConnections);
            Subject newSubject = new Subject(newGenome);
            newSubject.Name = Generation + "-" + g;
            Subjects.Add(newSubject);

            // Randomize weights
            WeightMutator.RandomizeWeights(newGenome);
        }

        // Speciate all subjects.
        Speciator.Speciate(Subjects, Species);
        foreach (Subject s in Subjects) s.Genome.Species.Subjects.Add(s);

        TopologyMutator.NodeId = numInputs + numOutputs;
        if (startWithConnections) TopologyMutator.InnovationNumber = numInputs * numOutputs;
    }

    public void Update()
    {
        foreach (Subject s in Subjects) s.UpdateSubject();
    }

    /// <summary>
    /// Sets "Fitness, AdjustedFitness" for the genome in all subjects.
    /// Sets "OverallRank" in all subjects.
    /// Sets "MaxFitness, AverageFitness, Rank" for all species.
    /// </summary>
    public void GetFitness()
    {
        // Set Fitness, AdjustedFitness in the genome of all subjects
        foreach (Subject subject in Subjects)
            subject.CalculateFitnessValues();

        // Set Rank of all subjects
        List<Subject> orderedSubjects = Subjects.OrderByDescending(x => x.Genome.Fitness).ToList();
        for (int i = 0; i < orderedSubjects.Count; i++) orderedSubjects[i].OverallRank = (i + 1);

        // Set Fitness and MaxFitness for all species
        foreach (Species s in Species)
            s.CalculateFitnessValues();

        // Set Rank of each species
        List<Species> orderedSpecies = Species.OrderByDescending(x => x.AverageFitness).ToList();
        for (int i = 0; i < orderedSpecies.Count; i++) orderedSpecies[i].Rank = (i + 1);
    }

    /// <summary>
    /// Removes all species from the population that haven't at least reached rank x in y generations. Returns the amount of species eliminated.
    /// </summary>
    public int EliminateBadSpecies(int rankNeededToSurvive, int generationsWithoutImprovementPenalty)
    {
        List<Species> toRemove = new List<Species>();

        foreach (Species s in Species)
        {
            if (s.Rank <= rankNeededToSurvive)
            {
                s.GenerationsWithoutImprovement = 0;
            }
            else s.GenerationsWithoutImprovement++;
            if (s.GenerationsWithoutImprovement > generationsWithoutImprovementPenalty)
            {
                toRemove.Add(s);
                Debug.Log("A SPECIES HAS BEEN REMOVED FOR NOT IMPROVING");
            }
        }

        foreach (Species s in toRemove) Species.Remove(s);
        return toRemove.Count;
    }

    /// <summary>
    /// Choses the best performing genome from each species as its representative
    /// </summary>
    public void CreateSpeciesRepresentatives()
    {
        foreach (Species s in Species)
        {
            s.Representative = s.Subjects.OrderByDescending(x => x.Genome.AdjustedFitness).First().Genome;
        }
    }

    /// <summary>
    /// Takes over a given percentage of best subjects PER SPECIES (at least 1), adds them to the given list.
    /// If immuneToMutation is set to true, the subjects that are taken over will be immune to any mutations this evolution.
    /// Returns the number of total subjects that have been taken over
    /// </summary>
    /// <returns></returns>
    public int TakeOverBestSubjects(List<Subject> newSubjects, float takeOverBestRatio, bool immuneToMutation)
    {
        int numTakeOver = 0;

        foreach (Species s in Species)
        {
            int numSubjectsToTakeOver = (int)(s.Size * takeOverBestRatio);
            if (numSubjectsToTakeOver < 1) numSubjectsToTakeOver = 1;
            List<Subject> subjectsToTakeOver = s.Subjects.OrderByDescending(x => x.Genome.AdjustedFitness).Take(numSubjectsToTakeOver).ToList();
            foreach(Subject sub in subjectsToTakeOver)
            {
                Subject newSubject = new Subject(sub.Genome.Copy());
                newSubject.Genome.Species = s;
                newSubject.Name = sub.Name;
                newSubject.ImmunteToMutation = immuneToMutation;
                newSubjects.Add(newSubject);
                numTakeOver++;
            }
        }
        return numTakeOver;
    }

    /// <summary>
    /// Takes over a given percentage of best random PER SPECIES which are not already in the list, adds them to the list.
    /// If immuneToMutation is set to true, the subjects that are taken over will be immune to any mutations this evolution.
    /// Returns the number of total subjects that have been taken over
    /// </summary>
    public int TakeOverRandomSubjects(List<Subject> newSubjects, float takeOverRandomRatio, bool immuneToMutation)
    {
        int numTakeOver = 0;

        foreach (Species s in Species)
        {
            int numSubjectsToTakeOver = (int)(s.Size * takeOverRandomRatio);
            for(int i = 0; i < numSubjectsToTakeOver; i++)
            {
                Subject luckyOne = null;
                while(luckyOne == null || newSubjects.Exists(x => x.Name == luckyOne.Name)) {
                    luckyOne = s.Subjects[Random.Next(s.Subjects.Count)];
                }
                Subject newSubject = new Subject(luckyOne.Genome.Copy());
                newSubject.Genome.Species = s;
                newSubject.Name = luckyOne.Name;
                newSubject.ImmunteToMutation = immuneToMutation;
                newSubjects.Add(newSubject);
                numTakeOver++;
            }
        }
        return numTakeOver;
    }

    public void CalculateOffspringNumberPerSpecies(int numOffsprings)
    {
        float totalSpeciesFitness = Species.Sum(x => x.AverageFitness);
        float fitnessForOneOffspring = totalSpeciesFitness / numOffsprings;
        foreach (Species s in Species)
        {
            s.OffspringCalculationTempFitness = s.AverageFitness;
            s.OffspringCount = 0;
        }
        for (int i = 0; i < numOffsprings; i++)
        {
            Species offspringSpecies = Species.OrderByDescending(x => x.OffspringCalculationTempFitness).First();
            offspringSpecies.OffspringCalculationTempFitness -= fitnessForOneOffspring;
            offspringSpecies.OffspringCount++;
        }
        //foreach(Species s in Species)
        //    Debug.Log(s.AverageFitness + " >>> " + s.OffspringCount);
    }

    /// <summary>
    /// Creates offspring equal to each species' OffspringCount and adds them to the given list.
    /// Only genomes above the IgnoreRatio threshhold can be chosen as parents.
    /// There is a chance equal to AdoptionRate that a child will not automatically have the species of its parent and will be checked if it fits.
    /// </summary>
    public List<Subject> CreateOffsprings(List<Subject> newSubjects)
    {
        List<Subject> toSpeciate = new List<Subject>();
        foreach (Species s in Species)
        {
            for (int i = 0; i < s.OffspringCount; i++)
            {
                Genome newGenome = s.CreateOffspring(IgnoreRatio);
                Subject newSubject = new Subject(newGenome);
                if (Random.NextDouble() >= AdoptionRate) newSubject.Genome.Species = s;
                else toSpeciate.Add(newSubject);
                newSubjects.Add(newSubject);
            }
        }
        return toSpeciate;
    }

    public void ReplaceSubjects(List<Subject> newSubjects)
    {
        Subjects.Clear();
        foreach (Subject subject in newSubjects) Subjects.Add(subject);
    }

    public EvolutionInformation EvolveGeneration(float mutationScaleFactor)
    {
        DateTime stamp = DateTime.Now;
        List<Subject> newSubjects = new List<Subject>();
        int numPreviousSpecies = Species.Count;

        // Calculate fitness & rank of each subject and species
        GetFitness();
        float averageFitness = Subjects.Average(x => x.Genome.Fitness);
        float maxFitness = Subjects.Max(x => x.Genome.Fitness);
        if (showTimestamps) stamp = TimeStamp(stamp, "Get Fitness");

        // Eliminate species without improvement for too long
        int numEliminatedSpecies = EliminateBadSpecies(RankNeededToSurvive, GenerationsWithoutImprovementPenalty);
        if (showTimestamps) stamp = TimeStamp(stamp, "Eliminate Bad Species");

        // Take a random representative for each existing species
        CreateSpeciesRepresentatives();
        if (showTimestamps) stamp = TimeStamp(stamp, "Create Representatives");

        int numSubjectsImmuneToMutations = 0;
        // Take over best subjects of each species
        int numBestSubjects = TakeOverBestSubjects(newSubjects, TakeOverBestRatio, AreTakeOversImmuneToMutation);
        if (AreTakeOversImmuneToMutation) numSubjectsImmuneToMutations += numBestSubjects;
        if (showTimestamps) stamp = TimeStamp(stamp, "Take over Best Subjects");

        // Take over best subjects of each species
        int numRandomSubjects = TakeOverRandomSubjects(newSubjects, TakeOverRandomRatio, AreTakeOversImmuneToMutation);
        if (AreTakeOversImmuneToMutation) numSubjectsImmuneToMutations += numRandomSubjects;
        if (showTimestamps) stamp = TimeStamp(stamp, "Take over Random Subjects");

        // Evaluate which species is allowed to produce how many offsprings
        int numOffsprings = Size - numBestSubjects - numRandomSubjects;
        CalculateOffspringNumberPerSpecies(numOffsprings);
        if (showTimestamps) stamp = TimeStamp(stamp, "Calculate Offspring numbers");

        // Create offsprings with a chance to automatically have the same species as its parents
        List<Subject> toSpeciate = CreateOffsprings(newSubjects);
        int numSubjectsCheckedForAdoption = toSpeciate.Count;
        if (showTimestamps) stamp = TimeStamp(stamp, "Create Offsprings");

        // Moves subjects from the newSubjects list to the Subjects list (that it also clears)
        ReplaceSubjects(newSubjects);
        if (showTimestamps) stamp = TimeStamp(stamp, "Replace Subjects");

        // Mutate the genomes in all subjects that are not marked immuneToMutation according to chances in the mutatealgorithm
        MutationInformation mutationInfo = MutateAlgorithm.MutatePopulation(this, mutationScaleFactor);
        if (showTimestamps) stamp = TimeStamp(stamp, "Mutate Population");

        // Speciate all subjects that haven't gotten a species yet
        int numNewSpecies = Speciator.Speciate(toSpeciate, Species);
        if (showTimestamps) stamp = TimeStamp(stamp, "Speciate unspeciated Subjects");

        // Fill species with new subjects
        foreach (Species s in Species) s.Subjects.Clear();
        foreach (Subject subject in Subjects) subject.Genome.Species.Subjects.Add(subject);
        if (showTimestamps) stamp = TimeStamp(stamp, "Replace Subjects in Species");

        // Remove empty species
        List<Species> emptySpecies = new List<Species>();
        foreach (Species species in Species)
            if (species.Subjects.Count == 0) emptySpecies.Add(species);
        foreach (Species remove in emptySpecies) Species.Remove(remove);
        int numEmptySpecies = emptySpecies.Count;
        if (showTimestamps) stamp = TimeStamp(stamp, "Remove empty Species");

        Generation++;

        // Name the subjects
        for (int i = 0; i < Subjects.Count; i++)
            if (String.IsNullOrEmpty(Subjects[i].Name)) Subjects[i].Name = Generation + "-" + Subjects[i].Genome.Species.Id + "-" + i;

        if (Subjects.Count != Species.Sum(x => x.Subjects.Count)) throw new Exception("SPECIATION FAILED. The number of subjects in the species does not match the number of subjects in the population.");

        // Create the evolution information object
        EvolutionInformation info = new EvolutionInformation(Generation, AreTakeOversImmuneToMutation, mutationInfo, numBestSubjects, numRandomSubjects, numOffsprings, numSubjectsCheckedForAdoption, numSubjectsImmuneToMutations, numPreviousSpecies, numEliminatedSpecies, numEmptySpecies, numNewSpecies, Species.Count, maxFitness, averageFitness);
        Debug.Log(info.ToString());

        // Reset the species fitness values
        foreach (Species s in Species) s.CalculateFitnessValues();

        return info;
    }

    // This method is only used if a generation has more than one simulations. Call this when ending a simulation.
    public void EndCurrentSimulation()
    {
        foreach (Subject subject in Subjects)
        {
            subject.EndCurrentSimulation();
            subject.CalculateFitnessValues();
        }

        foreach (Species species in Species) species.CalculateFitnessValues();
    }

    private DateTime TimeStamp(DateTime stamp, string message)
    {
        Debug.Log(message + ": " + (int)((DateTime.Now - stamp).TotalMilliseconds) + " ms");
        return DateTime.Now;
    }

    public int Size
    {
        get
        {
            return Subjects.Count;
        }
    }
}
