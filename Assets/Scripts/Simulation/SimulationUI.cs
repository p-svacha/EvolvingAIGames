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

    void Start()
    {
        CanvasWidth = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x;
        CanvasHeight = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.y;
        Factor = CanvasWidth / CanvasHeight;
    }
}
