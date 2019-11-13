using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPlayer : Player
{
    public AIPlayer(MatchModel model, string name, Subject brain) : base(model, name, brain)
    {
    }

    public override void PickCard(List<Card> options)
    {
        // Create options id list
        List<int> optionIds = new List<int>();
        foreach (Card c in options)
        {
            optionIds.Add(c.Id);
        }

        // Feed forward network
        float[] inputs = new float[]
        {
            1,
            ((float)(Model.NumMinions(this, MinionType.Red)) / Model.MaxMinions),
            ((float)(Model.NumMinions(this, MinionType.Yellow)) / Model.MaxMinions),
            ((float)(Model.NumMinions(this, MinionType.Blue)) / Model.MaxMinions),
            ((float)(Model.NumMinions(this, MinionType.Green)) / Model.MaxMinions),
            ((float)(Model.NumMinions(Enemy, MinionType.Red)) / Model.MaxMinions),
            ((float)(Model.NumMinions(Enemy, MinionType.Yellow)) / Model.MaxMinions),
            ((float)(Model.NumMinions(Enemy, MinionType.Blue)) / Model.MaxMinions),
            ((float)(Model.NumMinions(Enemy, MinionType.Green)) / Model.MaxMinions)
        };
        string s = "";
        foreach (float f in inputs) s += " " + f;
        Debug.Log("{" + s + "}");
        float[] outputs = Brain.Genome.FeedForward(inputs);

        // Visualize
        if(Model.Visual)
        {
            if(this == Model.Player1)
                Model.MatchUI.Player1CV.VisualizeOptions(outputs, optionIds);
            else
                Model.MatchUI.Player2CV.VisualizeOptions(outputs, optionIds);
        }

        // Save all outputs and option outputs in dictionary
        Dictionary<int, float> values = new Dictionary<int, float>();
        Dictionary<int, float> optionValues = new Dictionary<int, float>();
        for (int i = 0; i < outputs.Length; i++)
        {
            if(optionIds.Contains(i+1))
            {
                optionValues.Add(i + 1, outputs[i]);
            }
            values.Add(i + 1, outputs[i]);
        }

        // Choose best available option
        KeyValuePair<int, float> bestOption = optionValues.First(x => x.Value == optionValues.Max(y => y.Value));

        ChosenCard = options.First(x => x.Id == bestOption.Key);

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
