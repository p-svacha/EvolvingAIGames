using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPlayer : Player
{
    public AIPlayer(Match model, Subject brain) : base(model, brain.Name, brain)
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
            1,                                                                                              // Bias
            ((float)(Model.NumMinions(this, MinionType.Red)) / Model.MaxMinionsPerType),                    // Own Red
            ((float)(Model.NumMinions(this, MinionType.Yellow)) / Model.MaxMinionsPerType),                 // Own Yellow
            ((float)(Model.NumMinions(this, MinionType.Blue)) / Model.MaxMinionsPerType),                   // Own Blue
            ((float)(Model.NumMinions(this, MinionType.Green)) / Model.MaxMinionsPerType),                  // Own Green
            ((float)(Model.NumMinions(this, MinionType.Grey)) / Model.MaxMinionsPerType),                   // Own Grey
            ((float)(Model.NumMinions(Enemy, MinionType.Red)) / Model.MaxMinionsPerType),                   // Enemy Red
            ((float)(Model.NumMinions(Enemy, MinionType.Yellow)) / Model.MaxMinionsPerType),                // Enemy Yellow
            ((float)(Model.NumMinions(Enemy, MinionType.Blue)) / Model.MaxMinionsPerType),                  // Enemy Blue
            ((float)(Model.NumMinions(Enemy, MinionType.Green)) / Model.MaxMinionsPerType),                 // Enemy Green
            ((float)(Model.NumMinions(Enemy, MinionType.Grey)) / Model.MaxMinionsPerType),                  // Enemy Grey
            ((float)(Health)) / MaxHealth,                                                                  // Own Health
            ((float)(Enemy.Health)) / Enemy.MaxHealth,                                                      // Enemy Health
            Mathf.Min(1,((float)(Model.Turn)) / Model.FatigueDamageStartTurn),                              // Turn
            (NumCardOptions - Model.MinCardOptions) / (Model.MaxCardOptions - Model.MinCardOptions),        // Own Card Options
            (Enemy.NumCardOptions - Model.MinCardOptions) / (Model.MaxCardOptions - Model.MinCardOptions),  // Enemy Card Options
        };
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
            if(optionIds.Contains(i))
            {
                optionValues.Add(i, outputs[i]);
            }
            values.Add(i, outputs[i]);
        }

        // Choose best available option (or one at random from best if there are multiple)
        List<KeyValuePair<int, float>> bestOptions = optionValues.Where(x => x.Value == optionValues.Max(y => y.Value)).ToList();
        KeyValuePair<int, float> bestOption = bestOptions[Random.Range(0, bestOptions.Count)];
        List<int> bestOptionsIds = bestOptions.Select(x => x.Key).ToList();
        BestOptions = options.Where(x => bestOptionsIds.Contains(x.Id)).ToList();
        ChosenCard = options.First(x => x.Id == bestOption.Key);

        // Update players card statistics
        foreach(Card c in options)
        {
            if (BestOptions.Contains(c)) CardsPicked[c.Id]++;
            else CardsNotPicked[c.Id]++;
        }
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
