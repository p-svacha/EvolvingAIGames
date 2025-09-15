using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The SimulationModel is the god class that handles the whole learning process of a population over multiple generations.
/// <br/> During a generation multiple rounds of 1v1 matches between all subjects will be created to identify the best performing ones.
/// <br/> Between the generations subjects will evolve, mutation and be selected and new ones will be created.
/// </summary>
public abstract class VersusSimulationModel : MonoBehaviour
{
    // Simulation
    public SimulationPhase SimulationPhase;

    // Subjects
    protected int SubjectInputSize;
    protected int SubjectOutputSize;
    protected int[] SubjectHiddenSizes;

    // Population
    private Population Population;
    public int Generation => Population.Generation;

    /// <summary>
    /// Total amount of subjects.
    /// </summary>
    protected abstract int PopulationSize { get; }

    // Matches (refreshed each gen)
    public List<Match> Matches = new List<Match>();
    public Dictionary<Subject, List<Subject>> SubjectMatchOpponents;
    public Dictionary<Subject, int> SubjectMatchWins;
    public Dictionary<Subject, int> SubjectMatchLosses;

    /// <summary>
    /// How many matches each subject plays before the population gets evolution'd.
    /// </summary>
    protected abstract int MatchesPerGeneration { get; }

    public int MatchRound;
    public Match VisualMatch; // Match that is currently visually simulated

    /// <summary>
    /// With what algorithm matches are generated each round
    /// </summary>
    protected virtual MatchingTypeId MatchingType => MatchingTypeId.SwissSystem;

    // UI
    public SimulationUI SimulationUI;
    public MatchUI MatchUI;

    #region Initialization

    // Start is called before the first frame update
    void Start()
    {
        Init();

        // Initialize population with random subjects
        Population = new Population(PopulationSize, SubjectInputSize, SubjectHiddenSizes, SubjectOutputSize, FitnessFunction);

        // Initialize match stats
        SubjectMatchOpponents = new Dictionary<Subject, List<Subject>>();
        SubjectMatchWins = new Dictionary<Subject, int>();
        SubjectMatchLosses = new Dictionary<Subject, int>();
        foreach(Subject s in Population.Subjects)
        {
            SubjectMatchOpponents.Add(s, new List<Subject>());
            SubjectMatchWins.Add(s, 0);
            SubjectMatchLosses.Add(s, 0);
        }
        OnGenerationStarted();

        // Generate matches of first generation
        GenerateMatches();
        
        MatchRound = 0;

        // UI
        SimulationUI.gameObject.SetActive(true);
        SimulationUI.SpeciesScoreboard.UpdateScoreboard(Population);
        MatchUI.gameObject.SetActive(false);
    }

    public abstract void Init();

    private float FitnessFunction(Subject s)
    {
        float avgOpponentWinrate = SubjectMatchOpponents[s].Count > 0 ? (float)(SubjectMatchOpponents[s].Average(x => ((float)SubjectMatchWins[s] / SubjectMatchOpponents[s].Count()))) : 0; // Between 0 and 1
        return SubjectMatchWins[s] + avgOpponentWinrate;
    } 

    #endregion

    #region Match Generation

    /// <summary>
    /// Generates a new round of matches at random, pitting all subjects in the population against each other in 1v1's.
    /// </summary>
    private void GenerateMatches()
    {
        Matches.Clear();
        List<Subject> remainingSubjects = new List<Subject>();
        remainingSubjects.AddRange(Population.Subjects);

        switch(MatchingType)
        {
            case MatchingTypeId.Random:
                // If last round of generation, let the best two subjects play against each other and watch the game
                if (GetLastMatchSimulationMode() == MatchSimulationMode.Watch && MatchRound == MatchesPerGeneration - 1)
                {
                    List<Subject> bestSubjects = remainingSubjects.OrderByDescending(x => SubjectMatchWins[x]).Take(2).ToList();
                    Subject sub1 = bestSubjects[0];
                    remainingSubjects.Remove(sub1);
                    Subject sub2 = bestSubjects[1];
                    remainingSubjects.Remove(sub2);

                    Match match = GetMatch(sub1, sub2, MatchSimulationMode.Watch);
                    Matches.Add(match);
                }

                // Generate random matches
                while (remainingSubjects.Count > 0)
                {
                    Subject sub1 = remainingSubjects[Random.Range(0, remainingSubjects.Count)];
                    remainingSubjects.Remove(sub1);
                    Subject sub2 = remainingSubjects[Random.Range(0, remainingSubjects.Count)];
                    remainingSubjects.Remove(sub2);

                    Match match = GetMatch(sub1, sub2, MatchSimulationMode.SimulateInBackground);
                    Matches.Add(match);
                }
                break;

            case MatchingTypeId.SwissSystem:
                // Sort all subjects by wins and then by random value within the same amount of wins
                remainingSubjects = remainingSubjects.OrderByDescending(x => SubjectMatchWins[x]).ThenBy(x => Random.value).ToList();

                // Match up 1st vs 2nd, 3rd vs 4th etc. etc.
                for(int i = 0; i < remainingSubjects.Count; i += 2)
                {
                    Subject sub1 = remainingSubjects[i];
                    Subject sub2 = remainingSubjects[i + 1];
                    MatchSimulationMode simMode = 
                        (i == 0 &&GetLastMatchSimulationMode() == MatchSimulationMode.Watch && MatchRound == MatchesPerGeneration - 1) ? 
                        MatchSimulationMode.Watch : MatchSimulationMode.SimulateInBackground;

                    Match match = GetMatch(sub1, sub2, simMode);
                    Matches.Add(match);
                }
                break;
        }

        SimulationPhase = SimulationPhase.MatchesReady;
    }

    /// <summary>
    /// Returns how the last round of matches should be presented to the player.
    /// </summary>
    public MatchSimulationMode GetLastMatchSimulationMode()
    {
        if (SimulationUI.PlayGame.isOn) return MatchSimulationMode.Play;
        if (SimulationUI.WatchGame.isOn) return MatchSimulationMode.Watch;
        else return MatchSimulationMode.SimulateInBackground;
    }

    /// <summary>
    /// Returns an instance of the specific match that should be used for the simulation.
    /// <br/> Should return the game-specific match that inherits from Match.
    /// </summary>
    public abstract Match GetMatch(Subject sub1, Subject sub2, MatchSimulationMode simulationMode);

    /// <summary>
    /// Gets called when all matches in a round are finished.
    /// </summary>
    public virtual void OnMatchRoundFinished() { }

    /// <summary>
    /// Gets called when the population of a new generation is generated and before any matches are created.
    /// </summary>
    public virtual void OnGenerationStarted() { }

    /// <summary>
    /// Gets called when all matches of the last round in a generation are finished.
    /// </summary>
    public virtual void OnGenerationFinished() { }

    #endregion

    #region Update

    // Update is called once per frame
    void Update()
    {
        switch (SimulationPhase)
        {
            case SimulationPhase.MatchesReady:
                //Debug.Log("Starting matchround " + Population.Generation + "." + (MatchesPlayed + 1));
                SimulationUI.TitleText.text = "Match Round " + Population.Generation + "." + (MatchRound + 1);
                foreach (Match match in Matches)
                {
                    if (match.IsVisual)
                    {
                        SimulationUI.gameObject.SetActive(false);
                        MatchUI.gameObject.SetActive(true);
                        MatchUI.Init(match);
                        VisualMatch = match;
                    }
                    match.StartMatchSimulation();
                }
                SimulationPhase = SimulationPhase.MatchesRunning;
                break;

            case SimulationPhase.MatchesRunning:
                foreach (Match m in Matches) m.Update();
                if(Matches.TrueForAll(x => x.MatchPhase == MatchPhase.Finished))
                {
                    MatchRound++;
                    SimulationPhase = SimulationPhase.MatchesFinished;
                    if(VisualMatch != null)
                    {
                        VisualMatch = null;
                        SimulationUI.gameObject.SetActive(true);
                        MatchUI.gameObject.SetActive(false);
                    }
                }
                break;

            case SimulationPhase.MatchesFinished:
                OnMatchRoundFinished();

                if (MatchRound >= MatchesPerGeneration)
                {
                    // Init Human vs AI game if gen is finished
                    if (GetLastMatchSimulationMode() == MatchSimulationMode.Play)
                    {
                        Matches.Clear();

                        // Create match
                        Subject bestSubject = Population.Subjects.OrderByDescending(x => SubjectMatchWins[x]).First();
                        Match match = GetMatch(null, bestSubject, MatchSimulationMode.Play);

                        // Start match
                        Matches.Add(match);
                        VisualMatch = match;
                        SimulationUI.gameObject.SetActive(false);
                        MatchUI.gameObject.SetActive(true);
                        MatchUI.Init(match);
                        match.StartMatchSimulation();
                        SimulationPhase = SimulationPhase.MatchesReady;
                    }

                    else SimulationPhase = SimulationPhase.GenerationFinished;
                }
                else
                {
                    GenerateMatches();
                }
                break;

            case SimulationPhase.GenerationFinished:
                OnGenerationFinished();

                // Reset stats
                MatchRound = 0;

                // Evolve
                EvolutionInformation info = Population.EvolveGeneration();

                // Reset match stats
                SubjectMatchOpponents.Clear();
                SubjectMatchWins.Clear();
                SubjectMatchLosses.Clear();
                foreach (Subject s in Population.Subjects)
                {
                    SubjectMatchOpponents.Add(s, new List<Subject>());
                    SubjectMatchWins.Add(s, 0);
                    SubjectMatchLosses.Add(s, 0);
                }
                OnGenerationStarted();

                // Update UI
                SimulationUI.EvoStats.UpdateStatistics(info);
                SimulationUI.SpeciesScoreboard.UpdateScoreboard(Population);

                // Generate first match round
                GenerateMatches();
                break;
        }
    }

    #endregion

    #region Debug

    public void DebugStandings()
    {

        string text = "---------------- Standings after " + MatchRound + " matches ----------------";
        List<Subject> orderedStandings = Population.Subjects.OrderByDescending(x => SubjectMatchWins[x]).ToList();
        for(int i = 0; i < orderedStandings.Count; i++)
        {
            Subject s = orderedStandings[i];
            text += ("\n" + (i+1) + ". " + s.Name + ": " + SubjectMatchWins[s] + "-" + SubjectMatchLosses[s]);
        }
        Debug.Log(text);

    }

    #endregion

    protected enum MatchingTypeId
    {
        Random,
        SwissSystem
    }
}
