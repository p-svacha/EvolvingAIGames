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
    private int WatchAfterXGenerations = 15;

    // Matches
    public List<Match> Matches;

    // Match rules
    public Match MatchModel;
    private int StartHealth = 40;
    private int StartCardOptions = 3;

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

    // Start is called before the first frame update
    void Start()
    {
        // Init cards
        CardList.InitCardList();

        // Init population
        Population = new Population(PopulationSize, 12, CardList.Cards.Count, false);
        EvolutionInformation info = Population.EvolveGeneration(3);
        SimulationUI.EvoStats.UpdateStatistics(info);
        SimulationUI.SpeciesScoreboard.UpdateScoreboard(Population);

        // Generate matches
        Matches = new List<Match>();
        GenerateMatches();

        SimulationPhase = SimulationPhase.MatchesReady;
        MatchesPlayed = 0;

        // Init Card statistics
        CardsPicked = new Dictionary<int, int>();
        CardsPickedByWinner = new Dictionary<int, int>();
        CardsPickedByLoser = new Dictionary<int, int>();
        CardsNotPicked = new Dictionary<int, int>();
        CardPickrate = new Dictionary<int, float>();
        CardWinrate = new Dictionary<int, float>();
        for (int i = 0; i < CardList.Cards.Count; i++)
        {
            CardsPicked.Add(i + 1, 0);
            CardsPickedByWinner.Add(i + 1, 0);
            CardsPickedByLoser.Add(i + 1, 0);
            CardsNotPicked.Add(i + 1, 0);
        }
    }

    private void GenerateMatches()
    {
        Matches.Clear();
        List<Subject> remainingSubjects = new List<Subject>();
        remainingSubjects.AddRange(Population.Subjects);

        while(remainingSubjects.Count > 0)
        {
            Subject sub1 = remainingSubjects[Random.Range(0, remainingSubjects.Count)];
            remainingSubjects.Remove(sub1);
            Subject sub2 = remainingSubjects[Random.Range(0, remainingSubjects.Count)];
            remainingSubjects.Remove(sub2);

            Match match = new Match();

            Player player1 = new AIPlayer(match, sub1);
            Player player2 = new AIPlayer(match, sub2);
            match.InitGame(player1, player2, StartHealth, StartCardOptions, false);
            Matches.Add(match);
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch (SimulationPhase)
        {
            case SimulationPhase.MatchesReady:
                Debug.Log("Starting matchround " + Population.Generation + "." + (MatchesPlayed + 1));
                SimulationUI.TitleText.text = "Match Round " + Population.Generation + "." + (MatchesPlayed + 1);
                Match bestMatch = Matches.First(x => x.Player1.Brain.Wins + x.Player2.Brain.Wins == Matches.Max(y => y.Player1.Brain.Wins + y.Player2.Brain.Wins));
                foreach (Match m in Matches)
                {
                    if (MatchesPlayed == MatchesPerGeneration - 1 && Population.Generation % WatchAfterXGenerations == 0 && m == bestMatch)
                    {
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
                UpdateCardStatistics();
                SimulationUI.CardPickrates.UpdateBoard(CardPickrate, "Card Pickrates");
                SimulationUI.CardWinrates.UpdateBoard(CardWinrate, "Card Winrates");
                GenerateMatches();
                SimulationPhase = SimulationPhase.MatchesReady;
                break;

            case SimulationPhase.GenerationFinished:
                // Reset values
                CardsPicked.Clear();
                CardsPickedByWinner.Clear();
                CardsPickedByLoser.Clear();
                CardsNotPicked.Clear();
                CardPickrate.Clear();
                CardWinrate.Clear();
                for (int i = 0; i < CardList.Cards.Count; i++)
                {
                    CardsPicked.Add(i + 1, 0);
                    CardsPickedByWinner.Add(i + 1, 0);
                    CardsPickedByLoser.Add(i + 1, 0);
                    CardsNotPicked.Add(i + 1, 0);
                }
                MatchesPlayed = 0;

                // Evolve and Update UI
                EvolutionInformation info = Population.EvolveGeneration(2);
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

    private void UpdateCardStatistics()
    {
        foreach(Match m in Matches)
        {
            if (m.Phase != MatchPhase.GameEnded) throw new System.Exception("Match not finished");
            foreach(KeyValuePair<int, int> kvp in m.Player1.CardsPicked) CardsPicked[kvp.Key] += kvp.Value;
            foreach(KeyValuePair<int, int> kvp in m.Player2.CardsPicked) CardsPicked[kvp.Key] += kvp.Value;

            foreach(KeyValuePair<int, int> kvp in m.Player1.CardsNotPicked) CardsNotPicked[kvp.Key] += kvp.Value;
            foreach(KeyValuePair<int, int> kvp in m.Player2.CardsNotPicked) CardsNotPicked[kvp.Key] += kvp.Value;

            foreach (KeyValuePair<int, int> kvp in m.Winner.CardsPicked) CardsPickedByWinner[kvp.Key] += kvp.Value;
            foreach (KeyValuePair<int, int> kvp in m.Loser.CardsPicked) CardsPickedByLoser[kvp.Key] += kvp.Value;
        }

        CardPickrate.Clear();
        CardWinrate.Clear();

        for(int i = 0; i < CardList.Cards.Count; i++)
        {
            CardPickrate.Add(i + 1, (float)CardsPicked[i + 1] / (CardsPicked[i + 1] + CardsNotPicked[i + 1]));
            CardWinrate.Add(i + 1, (float)CardsPickedByWinner[i + 1] / (CardsPickedByWinner[i + 1] + CardsPickedByLoser[i + 1]));
        }
    }
}
