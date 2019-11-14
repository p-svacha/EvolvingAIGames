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
    private int MatchesPerGeneration = 10;
    private int MinGenerationToWatch = 15;

    // Matches
    public List<Match> Matches;

    // Match rules
    public Match MatchModel;
    public int StartHealth = 40;
    public int StartCardOptions = 3;

    // UI
    public MatchUI MatchUI;
    private int VisualBoardWidth = 10;
    private int VisualBoardHeight = 8;

    // Visuals
    public VisualPlayer VisualPlayer;
    public VisualMinion VisualMinion;

    // Start is called before the first frame update
    void Start()
    {
        // Init cards
        CardList.InitCardList();

        // Init population
        Population = new Population(PopulationSize, 12, CardList.Cards.Count, false);
        Population.EvolveGeneration(1);

        // Generate matches
        Matches = new List<Match>();
        GenerateMatches();

        SimulationPhase = SimulationPhase.MatchesReady;
        MatchesPlayed = 0;
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
        if (SimulationPhase == SimulationPhase.GenerationFinished) //&& Input.GetKeyDown(KeyCode.Space)
        {
            MatchesPlayed = 0;
            Population.EvolveGeneration(1);
            GenerateMatches();
            SimulationPhase = SimulationPhase.MatchesReady;
        }

            switch (SimulationPhase)
        {
            case SimulationPhase.MatchesReady:
                Debug.Log("Starting matchround " + Population.Generation + "." + (MatchesPlayed + 1));
                Match bestMatch = Matches.First(x => x.Player1.Brain.Wins + x.Player2.Brain.Wins == Matches.Max(y => y.Player1.Brain.Wins + y.Player2.Brain.Wins));
                foreach (Match m in Matches)
                {
                    if (MatchesPlayed == MatchesPerGeneration - 1 && Population.Generation >= MinGenerationToWatch && m == bestMatch)
                    {
                        m.StartMatch(true, VisualPlayer, VisualMinion, VisualBoardWidth, VisualBoardHeight, MatchUI);
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
                    //DebugStandings();
                    SimulationPhase = MatchesPlayed == MatchesPerGeneration ? SimulationPhase.GenerationFinished : SimulationPhase.MatchesFinished;
                }
                break;

            case SimulationPhase.MatchesFinished:
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
}
