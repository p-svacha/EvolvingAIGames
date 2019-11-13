using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This is the god class that controls everything
public class SimulationModel : MonoBehaviour
{
    // Population
    Population Population;


    // Match attributes
    public MatchModel MatchModel;
    public int StartHealth = 50;
    public int StartCardOptions = 3;

    // UI
    public MatchUI MatchUI;

    // Start is called before the first frame update
    void Start()
    {
        // Init population
        Population = new Population(2, 9, 5, true);

        // Create test match
        MatchModel TestMatch = GameObject.Instantiate(MatchModel);
        Player p1, p2 = null;
        p1 = new AIPlayer(TestMatch, "AI1", Population.Subjects[0]);
        p2 = new AIPlayer(TestMatch, "AI2", Population.Subjects[1]);
        TestMatch.InitGame(p1, p2, StartHealth, StartCardOptions, true, 10, 8, MatchUI);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
