using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

// This is the god class that controls everything
public class SimulationModel : MonoBehaviour
{
    // Simulation
    Population Population;
    public SimulationPhase SimulationPhase;
    public int MatchesPlayed;

    private int PopulationSize = 1000;
    private int MatchesPerGeneration = 20;

    // Matches
    public Match MatchModel;
    public List<Match> Matches;
    public Match BestMatch;

    // Match rules
    private int StartHealth = 30;
    private int StartCardOptions = 1;
    private int MinCardOptions = 1;
    private int MaxCardOptions = 5;
    private int MaxMinions = 30;
    private int MaxMinionsPerType = 6;
    private int FatigueDamageStartTurn = 20;

    // UI
    public SimulationUI SimulationUI;
    public MatchUI MatchUI;
    private int VisualBoardHeight = 8;

    // Visuals
    public VisualPlayer VisualPlayer;
    public VisualMinion VisualMinion;
    public Match ActiveMatch;

    // Card Statistics
    public Dictionary<int, int> CardsPicked;
    public Dictionary<int, int> CardsPickedByWinner;
    public Dictionary<int, int> CardsPickedByLoser;
    public Dictionary<int, int> CardsNotPicked;
    public Dictionary<int, float> CardPickrate;
    public Dictionary<int, float> CardWinrate;

    // Match Statistics
    public int Player1WonMatches;
    public int TotalMatches;
    public int TotalTurns;
    public int WinnerNumCardOptions;
    public int LoserNumCardOptions;

    // Start is called before the first frame update
    void Start()
    {
        // Init cards
        CardList.InitCardList();

        // Init population
        Population = new Population(PopulationSize, 16, CardList.Cards.Count, true);
        //EvolutionInformation info = Population.EvolveGeneration(7);
        //SimulationUI.EvoStats.UpdateStatistics(info);
        SimulationUI.SpeciesScoreboard.UpdateScoreboard(Population);

        // Generate matches
        Matches = new List<Match>();
        GenerateMatches();

        SimulationPhase = SimulationPhase.MatchesReady;
        MatchesPlayed = 0;

        // Init statistics
        ResetStatistics();

        // UI
        SimulationUI.MatchRules.UpdateStatistics(StartHealth, StartCardOptions, MinCardOptions, MaxCardOptions, FatigueDamageStartTurn, MaxMinions, MaxMinionsPerType);
    }

    private void GenerateMatches()
    {
        Matches.Clear();
        List<Subject> remainingSubjects = new List<Subject>();
        remainingSubjects.AddRange(Population.Subjects);

        // If last round of generation, let the best two subjects play against each other
        if (SimulationUI.WatchGame.isOn && MatchesPlayed == MatchesPerGeneration - 1)
        {
            List<Subject> bestSubjects = remainingSubjects.OrderByDescending(x => x.Wins).Take(2).ToList();
            Subject sub1 = bestSubjects[0];
            remainingSubjects.Remove(sub1);
            Subject sub2 = bestSubjects[1];
            remainingSubjects.Remove(sub2);

            Match match = new Match();

            Player player1 = new AIPlayer(match, sub1);
            Player player2 = new AIPlayer(match, sub2);
            match.InitGame(player1, player2, StartHealth, StartCardOptions, MinCardOptions, MaxCardOptions, MaxMinions, MaxMinionsPerType, FatigueDamageStartTurn, false);
            Matches.Add(match);

            BestMatch = match;
        }

        // Generate random matches
        while(remainingSubjects.Count > 0)
        {
            Subject sub1 = remainingSubjects[Random.Range(0, remainingSubjects.Count)];
            remainingSubjects.Remove(sub1);
            Subject sub2 = remainingSubjects[Random.Range(0, remainingSubjects.Count)];
            remainingSubjects.Remove(sub2);

            Match match = new Match();

            Player player1 = new AIPlayer(match, sub1);
            Player player2 = new AIPlayer(match, sub2);
            match.InitGame(player1, player2, StartHealth, StartCardOptions, MinCardOptions, MaxCardOptions, MaxMinions, MaxMinionsPerType, FatigueDamageStartTurn, false);
            Matches.Add(match);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (SimulationPhase)
        {
            case SimulationPhase.MatchesReady:
                //Debug.Log("Starting matchround " + Population.Generation + "." + (MatchesPlayed + 1));
                SimulationUI.TitleText.text = "Match Round " + Population.Generation + "." + (MatchesPlayed + 1);
                foreach (Match m in Matches)
                {
                    if (BestMatch != null && m == BestMatch)
                    {
                        BestMatch = null;
                        SimulationUI.gameObject.SetActive(false);
                        ActiveMatch = m;
                        m.StartMatch(true, VisualPlayer, VisualMinion, VisualBoardHeight, MatchUI);
                    }
                    else m.StartMatch();
                }
                SimulationPhase = SimulationPhase.MatchesRunning;
                break;

            case SimulationPhase.MatchesRunning:
                foreach (Match m in Matches) m.Update();
                if(Matches.TrueForAll(x => x.Phase == MatchPhase.GameEnded))
                {
                    MatchesPlayed++;
                    SimulationPhase = MatchesPlayed == MatchesPerGeneration ? SimulationPhase.GenerationFinished : SimulationPhase.MatchesFinished;
                    if(ActiveMatch != null)
                    {
                        ActiveMatch = null;
                        SimulationUI.gameObject.SetActive(true);
                    }
                }
                break;

            case SimulationPhase.MatchesFinished:
                UpdateStatistics();
                GenerateMatches();
                SimulationPhase = SimulationPhase.MatchesReady;
                break;

            case SimulationPhase.GenerationFinished:
                ResetStatistics();
                MatchesPlayed = 0;
                // Evolve and Update UI
                EvolutionInformation info = Population.EvolveGeneration();
                SimulationUI.EvoStats.UpdateStatistics(info);
                SimulationUI.SpeciesScoreboard.UpdateScoreboard(Population);

                // Generate first match round
                GenerateMatches();
                SimulationPhase = SimulationPhase.MatchesReady;
                break;
        }
    }

    private void DebugStandings()
    {

        Debug.Log("---------------- Standings after " + MatchesPlayed + " matches ----------------");
        foreach (Subject s in Population.Subjects.OrderByDescending(x => x.Wins))
        {
            Debug.Log(s.Name + ": " + s.Wins + "-" + s.Losses);
        }

    }

    private void UpdateStatistics()
    {
        UpdateMatchStatistics();
        UpdateCardStatistics();
    }

    /// <summary>
    /// Updates the match statistics according to the matches in the Matches list. Also updates the UI Element.
    /// </summary>
    private void UpdateMatchStatistics()
    {
        TotalMatches += Matches.Count;

        // Player 1 Winrate
        Player1WonMatches += Matches.Where(x => x.Winner == x.Player1).Count();
        float p1winrate = (float)Player1WonMatches / TotalMatches;

        // Game Length
        TotalTurns += Matches.Sum(x => x.Turn);
        float avgGameLength = (float)TotalTurns / TotalMatches;

        // Card Options
        WinnerNumCardOptions += Matches.Sum(x => x.Winner.NumCardOptions);
        LoserNumCardOptions += Matches.Sum(x => x.Loser.NumCardOptions);
        float avgWinnerCardOptions = (float)WinnerNumCardOptions / TotalMatches;
        float avgLoserCardOptions = (float)LoserNumCardOptions / TotalMatches;

        SimulationUI.MatchStatistics.UpdateStatistics(p1winrate, avgGameLength, avgWinnerCardOptions, avgLoserCardOptions);
    }

    /// <summary>
    /// Updates the card statistics according to the matches in the Matches list. Also updates the UI Element.
    /// </summary>
    private void UpdateCardStatistics()
    {
        foreach (Match m in Matches)
        {
            if (m.Phase != MatchPhase.GameEnded) throw new System.Exception("Match not finished");
            foreach (KeyValuePair<int, int> kvp in m.Player1.CardsPicked) CardsPicked[kvp.Key] += kvp.Value;
            foreach (KeyValuePair<int, int> kvp in m.Player2.CardsPicked) CardsPicked[kvp.Key] += kvp.Value;

            foreach (KeyValuePair<int, int> kvp in m.Player1.CardsNotPicked) CardsNotPicked[kvp.Key] += kvp.Value;
            foreach (KeyValuePair<int, int> kvp in m.Player2.CardsNotPicked) CardsNotPicked[kvp.Key] += kvp.Value;

            foreach (KeyValuePair<int, int> kvp in m.Winner.CardsPicked) CardsPickedByWinner[kvp.Key] += kvp.Value;
            foreach (KeyValuePair<int, int> kvp in m.Loser.CardsPicked) CardsPickedByLoser[kvp.Key] += kvp.Value;
        }

        CardPickrate.Clear();
        CardWinrate.Clear();

        for (int i = 0; i < CardList.Cards.Count; i++)
        {
            CardPickrate.Add(i, (float)CardsPicked[i] / (CardsPicked[i] + CardsNotPicked[i]));
            CardWinrate.Add(i, (float)CardsPickedByWinner[i] / (CardsPickedByWinner[i] + CardsPickedByLoser[i]));
        }

        SimulationUI.CardPickrates.UpdateBoard(CardPickrate, "Card Pickrates");
        SimulationUI.CardWinrates.UpdateBoard(CardWinrate, "Card Winrates");
    }

    /// <summary>
    /// Resets all match and card statistics.
    /// </summary>
    private void ResetStatistics()
    {
        CardsPicked = new Dictionary<int, int>();
        CardsPickedByWinner = new Dictionary<int, int>();
        CardsPickedByLoser = new Dictionary<int, int>();
        CardsNotPicked = new Dictionary<int, int>();
        CardPickrate = new Dictionary<int, float>();
        CardWinrate = new Dictionary<int, float>();
        for (int i = 0; i < CardList.Cards.Count; i++)
        {
            CardsPicked.Add(i, 0);
            CardsPickedByWinner.Add(i, 0);
            CardsPickedByLoser.Add(i, 0);
            CardsNotPicked.Add(i, 0);
        }
        Player1WonMatches = 0;
        WinnerNumCardOptions = 0;
        LoserNumCardOptions = 0;
        TotalMatches = 0;
        TotalTurns = 0;
    }
}
