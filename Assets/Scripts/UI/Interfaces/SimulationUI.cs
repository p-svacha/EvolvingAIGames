using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SimulationUI : MonoBehaviour
{
    // Dimensions
    public float CanvasWidth;
    public float CanvasHeight;
    public float Factor;

    // Elements
    public Text TitleText;
    public Toggle WatchGame;
    public Toggle PlayGame;
    public EvolutionStatistics EvoStats;
    public SpeciesScoreboard SpeciesScoreboard;
    public CardStatistics CardPickrates;
    public CardStatistics CardWinrates;
    public MatchRulesDisplay MatchRules;
    public MatchStatisticsDisplay MatchStatistics;

    // Card Display
    public List<Sprite> CardSprites;
    public List<VisualCard> DisplayedCards;
    private float CardMarginX = 0.15f;
    private float CardMarginY = 0.1f;
    private float CardGapX = 1f; // 0.5 = The gap between two cards is the width of 0.5 cards

    void Start()
    {
        CanvasWidth = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x;
        CanvasHeight = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.y;
        Factor = CanvasWidth / CanvasHeight;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
