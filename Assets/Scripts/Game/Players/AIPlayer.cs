using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIPlayer : Player
{
    public AIPlayer(MatchModel model, string name) : base(model, name) { }

    public override void PickCard(List<Card> options)
    {
        ChosenCard = options[Random.Range(0, options.Count - 1)];

        // Debug
        string optionString = "";
        foreach (Card c in options) optionString += c.Name + ", ";
        optionString = optionString.TrimEnd(new char[] { ',', ' ' });
        Debug.Log(Name + " chose " + ChosenCard.Name + " from " + optionString);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
