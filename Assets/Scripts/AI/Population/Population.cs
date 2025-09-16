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
    public bool InitialGenomesAreFullyConnected = true; // If true, the initial genomes have connections from every input node to every output node
    public int FixedSpeciesAmount = 4; // if this is positive, a fixed amount of species is used that will always have the same amount of subjects.
    public int FixedSpeciesSubjectSize; // How many subjects are in a species if fixed species amount is used.
    public bool IsUsingDynamicSpecies => FixedSpeciesAmount <= 0;

    public float TakeOverBestRatio = 0.05f; // % of best subjects per species that are taken over to next generation
    public float TakeOverRandomRatio = 0.01f; // % of random subjects per species that are taken over to next generation
    public bool AreTakeOversImmuneToMutation = true; // If true, subjects that are taken over the next generation are immune to mutation
    // Rest will be newly generated as offsprings of good performing genomes from previous generation

    public float IgnoreRatio = 0.2f; // % of worst performing subjects within a species to ignore when chosing a random parent

    public int RankNeededToSurvive = 4; // The rank needed for a species at least every {GenerationsWithoutImprovementPenalty} generations to not get eliminated
    public int GenerationsBelowRankAllowed = 5; // Number of generations without reaching species rank {RankNeededToSurvive} allowed to not get eliminated

    public float AdoptionRate = 0f; // % chance that an offspring will automatically have the same species as its parents

    /// Maximum difference (nodes and connections) allowed for a subject to be placed into a species.
    /// This only gets used when InitialGenomesAreFullyConnected is false.
    /// Should be higher when MultipleMutationsPerGenomeAllowed is set to true.
    public float SpeciesCompatiblityThreshold = 320;


    // Mutation parameters
    public float BaseTopologyMutationChancePerGenome = 0.14f; // % Chance that a genome will have at least 1 mutation in topology during evolution
    public float BaseWeightMutationChancePerGenome = 0.25f; // % Chance that a genome will have at least 1 mutation in weight during evolution

    public float StartMutationChanceFactor = 1.5f; // At the start of the simulation, them mutation chance is multiplied with this factor
    public float MutationChanceFactorReductionPerGeneration = 0.01f; // The amount the mutation scale factor gets reduced every generation
    public float MinMutationChanceFactor = 1f; // The mutation chance factor will never fall below this value

    public bool MultipleMutationsPerGenomeAllowed = true; // Sets if multiple mutations on the same genome are allowed
    public float MutationChanceReductionFactorPerMutation = 0.2f; // % Chance that the mutation chance gets reduced after each mutation on the same genome. Only relevant if multiple mutations are allowed

    public float MaxConnectionWeight = 1; // The maximum value the weight of a connection inside a genome can have
    public float MinConnectionWeight = -1; // The minimum value the weight of a connection inside a genome can have

    // Fitness calculation
    private Func<Subject, float> FitnessFunction; // The function used to get a subjects fitness value at the end of a generation when evolving the population.

    // Debug
    public bool DebugTimestamps = true; // If true, evolution steps taking longer than 5 seconds are logged to console

    public Population(int size, int numInputs, int[] hiddenLayers, int numOutputs, Func<Subject, float> fitnessFunction)
    {
        // Initialized functions
        FitnessFunction = fitnessFunction;

        // Initialize objects
        Subjects = new List<Subject>();
        Species = new List<Species>();
        Speciator = new Speciator(SpeciesCompatiblityThreshold);
        TopologyMutator = new TopologyMutator();
        WeightMutator = new WeightMutator();
        MutateAlgorithm = new MutateAlgorithm(TopologyMutator, WeightMutator, BaseWeightMutationChancePerGenome, BaseTopologyMutationChancePerGenome);

        // Initilaize parameters
        CurrentMutationChanceScaleFactor = StartMutationChanceFactor;

        // Create initial population
        CreateInitialPopulation(size, numInputs, hiddenLayers, numOutputs, InitialGenomesAreFullyConnected);
    }

    private void CreateInitialPopulation(int size, int numInputs, int[] hiddenLayers, int numOutputs, bool startWithConnections)
    {
        if (hiddenLayers.Length > 0 && !startWithConnections) throw new Exception("Can't start with hidden layers and without connections - startWithConnections must be set to true.");

        int nodeIdCounter = 0;
        int connectionIdCounter = 0;
        for (int g = 0; g < size; g++)
        {
            Dictionary<int, Node> allNodes = new Dictionary<int, Node>();
            List<Node> inputs = new List<Node>();
            List<Node> outputs = new List<Node>();
            List<List<Node>> hiddens = new List<List<Node>>();
            Dictionary<int, Connection> initialConnections = new Dictionary<int, Connection>();

            // Create input nodes
            nodeIdCounter = 0;
            for (int i = 0; i < numInputs; i++)
            {
                Node inputNode = new Node(nodeIdCounter++, NodeType.Input);
                inputs.Add(inputNode);
                allNodes.Add(inputNode.Id, inputNode);
            }

            // Create output nodes
            for (int i = numInputs; i < (numInputs + numOutputs); i++)
            {
                Node outputNode = new Node(nodeIdCounter++, NodeType.Output);
                outputs.Add(outputNode);
                allNodes.Add(outputNode.Id, outputNode);
            }

            // Create hidden nodes
            for(int nLayer = 0; nLayer < hiddenLayers.Length; nLayer++)
            {
                List<Node> depthLayer = new List<Node>();
                hiddens.Add(depthLayer);
                for(int nNode = 0; nNode < hiddenLayers[nLayer]; nNode++)
                {
                    Node hiddenNode = new Node(nodeIdCounter++, NodeType.Hidden);
                    depthLayer.Add(hiddenNode);
                    allNodes.Add(hiddenNode.Id, hiddenNode);
                }
            }

            // Create initial connections
            connectionIdCounter = 0;
            if (startWithConnections)
            {
                int hiddenLayerCounter = 0;
                List<Node> fromLayerNodes = inputs;
                List<Node> toLayerNodes;

                // Connections to hidden layers
                while(hiddenLayerCounter < hiddenLayers.Length)
                {
                    toLayerNodes = hiddens[hiddenLayerCounter];
                    foreach (Node fromNode in fromLayerNodes)
                    {
                        foreach (Node toNode in toLayerNodes)
                        {
                            int id = connectionIdCounter++;
                            Connection con = new Connection(id, fromNode, toNode);
                            initialConnections.Add(id, con);
                            fromNode.OutConnections.Add(con);
                            toNode.InConnections.Add(con);
                        }
                    }
                    fromLayerNodes = toLayerNodes;
                    hiddenLayerCounter++;
                }

                // Connections to output layer
                toLayerNodes = outputs;
                foreach (Node fromNode in fromLayerNodes)
                {
                    foreach (Node toNode in toLayerNodes)
                    {
                        int id = connectionIdCounter++;
                        Connection con = new Connection(id, fromNode, toNode);
                        initialConnections.Add(id, con);
                        fromNode.OutConnections.Add(con);
                        toNode.InConnections.Add(con);
                    }
                }
            }

            // Create genome and add it to the subject
            Genome newGenome = new Genome(CrossoverAlgorithm.GenomeId++, allNodes, initialConnections, MaxConnectionWeight, MinConnectionWeight);
            Subject newSubject = new Subject(newGenome);
            Subjects.Add(newSubject);

            // Randomize weights
            WeightMutator.RandomizeWeights(newGenome);
        }

        // Speciate all subjects.
        if(IsUsingDynamicSpecies) Speciator.SpeciateByCompatibility(Subjects, Species);
        else
        {
            FixedSpeciesSubjectSize = Size / FixedSpeciesAmount;
            for (int i = 0; i < FixedSpeciesAmount; i++) Species.Add(Speciator.CreateNewSpecies(Subjects[i].Genome, Species));
            Speciator.SpeciateRandomly(Subjects, Species);
        }

        foreach (Subject s in Subjects)
        {
            s.Genome.Species.Subjects.Add(s);
            s.Name = GetStandardizedSubjectName(s);
        }

        TopologyMutator.NodeId = nodeIdCounter;
        TopologyMutator.ConnectionId = connectionIdCounter;

        if (!startWithConnections) // Instantly evolve once if starting with no connections
        {
            EvolveGeneration();
        }
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
        {
            float fitnessValue = FitnessFunction(subject);
            subject.Genome.Fitness = fitnessValue;
            subject.Genome.AdjustedFitness = fitnessValue / subject.Genome.Species.Size;
        }

        // Set Rank of all subjects
        List<Subject> orderedSubjects = Subjects.OrderByDescending(x => x.Genome.Fitness).ToList();
        for (int i = 0; i < orderedSubjects.Count; i++) orderedSubjects[i].OverallRank = (i + 1);

        // Set Fitness and MaxFitness for all species
        foreach (Species s in Species)
        {
            s.CalculateFitnessValues();
        }

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
    /// <br/> Only genomes above the IgnoreRatio threshhold can be chosen as parents.
    /// <br/> There is a chance equal to AdoptionRate that a child will not automatically have the species of its parent and will be checked if it fits.
    /// </summary>
    public List<Subject> CreateOffsprings(List<Subject> newSubjects, float adoptionRate)
    {
        List<Subject> toSpeciate = new List<Subject>();
        foreach (Species s in Species)
        {
            for (int i = 0; i < s.OffspringCount; i++)
            {
                Genome newGenome = s.CreateOffspring(IgnoreRatio);
                Subject newSubject = new Subject(newGenome);
                if (UnityEngine.Random.value <= adoptionRate) newSubject.Genome.Species = s;
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

    /// <summary>
    /// Evolves a generation of subjects by ranking them by their fitness and then selecting, mutating and creating offspring of them within their species.
    /// </summary>
    public EvolutionInformation EvolveGeneration()
    {
        if (IsUsingDynamicSpecies) return EvolveWithDynamicSpecies();
        else return EvolveWithFixedSpeciesAmount();
    }

    /// <summary>
    /// Evolves the generation and calculates the species new, meaning bad species can be eliminiated and new ones can be formed.
    /// </summary>
    private EvolutionInformation EvolveWithDynamicSpecies()
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
        List<Subject> toSpeciate = CreateOffsprings(newSubjects, AdoptionRate);
        int numSubjectsCheckedForAdoption = toSpeciate.Count;
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Create Offsprings");

        // Moves subjects from the newSubjects list to the Subjects list (that it also clears)
        ReplaceSubjects(newSubjects);
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Replace Subjects");

        // Mutate the genomes in all subjects that are not marked immuneToMutation according to chances in the mutatealgorithm
        MutationInformation mutationInfo = MutateAlgorithm.MutatePopulation(this, CurrentMutationChanceScaleFactor, MultipleMutationsPerGenomeAllowed, MutationChanceReductionFactorPerMutation);
        if (DebugTimestamps) stamp = TimeStamp(stamp, "Mutate Population");

        // Speciate all subjects that haven't gotten a species yet
        int numNewSpecies = 0;
        if (FixedSpeciesAmount <= 0) numNewSpecies = Speciator.SpeciateByCompatibility(toSpeciate, Species);
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
        foreach (Subject s in Subjects.Where(x => string.IsNullOrEmpty(x.Name))) s.Name = GetStandardizedSubjectName(s);

        if (Subjects.Count != Species.Sum(x => x.Subjects.Count)) throw new Exception("SPECIATION FAILED. The number of subjects in the species does not match the number of subjects in the population.");

        // Create the evolution information object
        int evolutionTime = (int)((DateTime.Now - startTimeStamp).TotalMilliseconds);
        EvolutionInformation info = new EvolutionInformation(Generation, evolutionTime,
            AreTakeOversImmuneToMutation, mutationInfo, numBestSubjects, numRandomSubjects, numOffsprings,
            numSubjectsCheckedForAdoption, numSubjectsImmuneToMutations,
            numPreviousSpecies, numEliminatedSpecies, numEmptySpecies, numNewSpecies, Species.Count, SpeciesCompatiblityThreshold,
            maxFitness, averageFitness,
            RankNeededToSurvive, GenerationsBelowRankAllowed);
        Debug.Log(info.ToString());

        return info;
    }

    /// <summary>
    /// Evolves the generation without changing the amount of species and the amount of subjects in them.
    /// <br/> Still selects, mutates and creates offspring as usual.
    /// </summary>
    private EvolutionInformation EvolveWithFixedSpeciesAmount()
    {
        DateTime startTimeStamp = DateTime.Now;
        List<Subject> newSubjects = new List<Subject>();

        // Calculate fitness & rank of each subject and species
        GetFitness();
        float averageFitness = Subjects.Average(x => x.Genome.Fitness);
        float maxFitness = Subjects.Max(x => x.Genome.Fitness);

        int numSubjectsImmuneToMutations = 0;
        // Take over best subjects of each species
        int numBestSubjects = TakeOverBestSubjects(newSubjects, TakeOverBestRatio, AreTakeOversImmuneToMutation);
        if (AreTakeOversImmuneToMutation) numSubjectsImmuneToMutations += numBestSubjects;

        // Take over random lucky subjects of each species
        int numRandomSubjects = TakeOverRandomSubjects(newSubjects, TakeOverRandomRatio, AreTakeOversImmuneToMutation);
        if (AreTakeOversImmuneToMutation) numSubjectsImmuneToMutations += numRandomSubjects;

        // Set amount of offspring so that the species size stay the same
        int numOffsprings = Size - numBestSubjects - numRandomSubjects;
        foreach (Species species in Species) species.OffspringCount = species.Size;
        foreach (Subject subject in newSubjects) subject.Genome.Species.OffspringCount--;

        // Create offsprings with the same species as their parents
        List<Subject> toSpeciate = CreateOffsprings(newSubjects, adoptionRate: 1f);
        int numSubjectsCheckedForAdoption = toSpeciate.Count;

        // Moves subjects from the newSubjects list to the Subjects list (that it also clears)
        ReplaceSubjects(newSubjects);

        // Mutate the genomes in all subjects that are not marked immuneToMutation according to chances in the mutatealgorithm
        MutationInformation mutationInfo = MutateAlgorithm.MutatePopulation(this, CurrentMutationChanceScaleFactor, MultipleMutationsPerGenomeAllowed, MutationChanceReductionFactorPerMutation);

        // Fill species with new subjects
        foreach (Species s in Species) s.Subjects.Clear();
        foreach (Subject subject in Subjects) subject.Genome.Species.Subjects.Add(subject);

        // Go to next generation
        Generation++;
        if (CurrentMutationChanceScaleFactor > MinMutationChanceFactor) CurrentMutationChanceScaleFactor -= MutationChanceFactorReductionPerGeneration;

        // Name the subjects
        foreach (Subject s in Subjects.Where(x => string.IsNullOrEmpty(x.Name))) s.Name = GetStandardizedSubjectName(s);

        if (Subjects.Count != Species.Sum(x => x.Subjects.Count)) throw new Exception("SPECIATION FAILED. The number of subjects in the species does not match the number of subjects in the population.");

        // Create the evolution information object
        int evolutionTime = (int)((DateTime.Now - startTimeStamp).TotalMilliseconds);
        EvolutionInformation info = new EvolutionInformation(Generation, evolutionTime,
            AreTakeOversImmuneToMutation, mutationInfo, numBestSubjects, numRandomSubjects, numOffsprings,
            numSubjectsCheckedForAdoption, numSubjectsImmuneToMutations,
            numPreviousSpecies: 0, numEliminatedSpecies: 0, numEmptySpecies: 0, numNewSpecies: 0, Species.Count, SpeciesCompatiblityThreshold,
            maxFitness, averageFitness,
            RankNeededToSurvive, GenerationsBelowRankAllowed);
        Debug.Log(info.ToString());

        return info;
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

    private string GetStandardizedSubjectName(Subject s) => "S" + Generation + "/" + s.Genome.Species.Id + "/" + Subjects.IndexOf(s);
}
