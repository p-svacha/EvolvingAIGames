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
    public float CurrentMutationChanceScaleFactor;

    // Evolution
    public Speciator Speciator;
    public int Generation;

    // Evolution Parameters
    public bool InitialGenomesAreFullyConnected = false; // If true, the initial genomes have connections from every input node to every output node

    public float TakeOverBestRatio = 0.13f; // % of best subjects per species that are taken over to next generation
    public float TakeOverRandomRatio = 0.04f; // % of random subjects per species that are taken over to next generation
    public bool AreTakeOversImmuneToMutation = true; // If true, subjects that are taken over the next generation are immune to mutation
    // Rest will be newly generated as offsprings of good performing genomes from previous generation

    public float IgnoreRatio = 0.2f; // % of worst performing subjects within a species to ignore when chosing a random parent

    public int RankNeededToSurvive = 4; // The rank needed for a species at least every {GenerationsWithoutImprovementPenalty} generations to not get eliminated
    public int GenerationsBelowRankAllowed = 15; // Number of generations without reaching species rank {RankNeededToSurvive} allowed to not get eliminated

    public float AdoptionRate = 1f; // % chance that an offspring will be checked which species it belongs to. otherwise it will get the species of its parents

    /// Maximum difference (nodes and connections) allowed for a subject to be placed into a species (default is 5 for no-con start and 10 for con-start)
    /// Should be higher when MultipleMutationsPerGenomeAllowed is set to true.
    public float SpeciesCompatiblityThreshhold = 30;


    // Mutation parameters
    public float BaseTopologyMutationChancePerGenome = 0.14f; // % Chance that a genome will have at least 1 mutation in topology during evolution
    public float BaseWeightMutationChancePerGenome = 0.25f; // % Chance that a genome will have at least 1 mutation in weight during evolution

    public float StartMutationChanceFactor = 2f; // At the start of the simulation, them mutation chance is multiplied with this factor
    public float MutationChanceFactorReductionPerGeneration = 0.01f; // The amount the mutation scale factor gets reduced every generation
    public float MinMutationChanceFactor = 1f; // The mutation chance factor will never fall below this value

    public bool MultipleMutationsPerGenomeAllowed = true; // Sets if multiple mutations on the same genome are allowed
    public float MutationChanceReductionFactorPerMutation = 0.2f; // % Chance that the mutation chance gets reduced after each mutation on the same genome. Only relevant if multiple mutations are allowed

    public float MaxConnectionWeight = 20; // The maximum value the weight of a connection inside a genome can have
    public float MinConnectionWeight = -20; // The minimum value the weight of a connection inside a genome can have

    // Debug
    public bool DebugTimestamps = true; // If true, evolution steps taking longer than 5 seconds are logged to console

    public Population(int size, int numInputs, int numOutputs)
    {
        // Initialize objects
        Subjects = new List<Subject>();
        Species = new List<Species>();
        Speciator = new Speciator(SpeciesCompatiblityThreshhold);
        TopologyMutator = new TopologyMutator();
        WeightMutator = new WeightMutator();
        MutateAlgorithm = new MutateAlgorithm(TopologyMutator, WeightMutator, BaseWeightMutationChancePerGenome, BaseTopologyMutationChancePerGenome);

        // Initilaize parameters
        CurrentMutationChanceScaleFactor = StartMutationChanceFactor;

        // Create initial population
        CreateInitialPopulation(size, numInputs, numOutputs, InitialGenomesAreFullyConnected);
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
            Genome newGenome = new Genome(CrossoverAlgorithm.GenomeId++, inputs, outputs, initialConnections, MaxConnectionWeight, MinConnectionWeight);
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
                newSubject.ImmuneToMutation = immuneToMutation;
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
                newSubject.ImmuneToMutation = immuneToMutation;
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

    public EvolutionInformation EvolveGeneration()
    {
        DateTime startTimeStamp = DateTime.Now;
        DateTime stamp = DateTime.Now;
        List<Subject> newSubjects = new List<Subject>();
        int numPreviousSpecies = Species.Count;

        // Calculate fitness & rank of each subject and species
        GetFitness();
        float averageFitness = Subjects.Average(x => x.Genome.Fitness);
        float maxFitness = Subjects.Max(x => x.Genome.Fitness);
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Get Fitness");

        // Eliminate species without improvement for too long
        int numEliminatedSpecies = EliminateBadSpecies(RankNeededToSurvive, GenerationsBelowRankAllowed);
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Eliminate Bad Species");

        // Take a random representative for each existing species
        CreateSpeciesRepresentatives();
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Create Representatives");

        int numSubjectsImmuneToMutations = 0;
        // Take over best subjects of each species
        int numBestSubjects = TakeOverBestSubjects(newSubjects, TakeOverBestRatio, AreTakeOversImmuneToMutation);
        if (AreTakeOversImmuneToMutation) numSubjectsImmuneToMutations += numBestSubjects;
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Take over Best Subjects");

        // Take over random lucky subjects of each species
        int numRandomSubjects = TakeOverRandomSubjects(newSubjects, TakeOverRandomRatio, AreTakeOversImmuneToMutation);
        if (AreTakeOversImmuneToMutation) numSubjectsImmuneToMutations += numRandomSubjects;
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Take over Random Subjects");

        // Evaluate which species is allowed to produce how many offsprings
        int numOffsprings = Size - numBestSubjects - numRandomSubjects;
        CalculateOffspringNumberPerSpecies(numOffsprings);
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Calculate Offspring numbers");

        // Create offsprings with a chance to automatically have the same species as its parents
        List<Subject> toSpeciate = CreateOffsprings(newSubjects);
        int numSubjectsCheckedForAdoption = toSpeciate.Count;
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Create Offsprings");

        // Moves subjects from the newSubjects list to the Subjects list (that it also clears)
        ReplaceSubjects(newSubjects);
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Replace Subjects");

        // Mutate the genomes in all subjects that are not marked immuneToMutation according to chances in the mutatealgorithm
        MutationInformation mutationInfo = MutateAlgorithm.MutatePopulation(this, CurrentMutationChanceScaleFactor, MultipleMutationsPerGenomeAllowed, MutationChanceReductionFactorPerMutation);
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Mutate Population");

        // Speciate all subjects that haven't gotten a species yet
        int numNewSpecies = Speciator.Speciate(toSpeciate, Species);
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Speciate unspeciated Subjects");

        // Fill species with new subjects
        foreach (Species s in Species) s.Subjects.Clear();
        foreach (Subject subject in Subjects) subject.Genome.Species.Subjects.Add(subject);
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Replace Subjects in Species");

        // Remove empty species
        List<Species> emptySpecies = new List<Species>();
        foreach (Species species in Species)
            if (species.Subjects.Count == 0) emptySpecies.Add(species);
        foreach (Species remove in emptySpecies) Species.Remove(remove);
        int numEmptySpecies = emptySpecies.Count;
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Remove empty Species");

        // Go to next generation
        Generation++;
        if (CurrentMutationChanceScaleFactor > MinMutationChanceFactor) CurrentMutationChanceScaleFactor -= MutationChanceFactorReductionPerGeneration;

        // Name the subjects
        for (int i = 0; i < Subjects.Count; i++)
            if (String.IsNullOrEmpty(Subjects[i].Name)) Subjects[i].Name = Generation + "-" + Subjects[i].Genome.Species.Id + "-" + i;

        if (Subjects.Count != Species.Sum(x => x.Subjects.Count)) throw new Exception("SPECIATION FAILED. The number of subjects in the species does not match the number of subjects in the population.");

        // Create the evolution information object
        int evolutionTime = (int)((DateTime.Now - startTimeStamp).TotalMilliseconds);
        EvolutionInformation info = new EvolutionInformation(Generation, evolutionTime,
            AreTakeOversImmuneToMutation, mutationInfo, numBestSubjects, numRandomSubjects, numOffsprings, 
            numSubjectsCheckedForAdoption, numSubjectsImmuneToMutations, 
            numPreviousSpecies, numEliminatedSpecies, numEmptySpecies, numNewSpecies, Species.Count, SpeciesCompatiblityThreshhold, 
            maxFitness, averageFitness, 
            RankNeededToSurvive, GenerationsBelowRankAllowed);
        Debug.Log(info.ToString());

        // Reset the species fitness values
        //foreach (Species s in Species) s.CalculateFitnessValues();

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
        int duration = (int)((DateTime.Now - stamp).TotalMilliseconds);
        if(duration > 5000) Debug.Log(message + ": " + duration + " ms");
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
