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
            1,
            ((float)(Model.NumMinions(this, MinionType.Red)) / Model.MaxMinionsPerType),
            ((float)(Model.NumMinions(this, MinionType.Yellow)) / Model.MaxMinionsPerType),
            ((float)(Model.NumMinions(this, MinionType.Blue)) / Model.MaxMinionsPerType),
            ((float)(Model.NumMinions(this, MinionType.Green)) / Model.MaxMinionsPerType),
            ((float)(Model.NumMinions(Enemy, MinionType.Red)) / Model.MaxMinionsPerType),
            ((float)(Model.NumMinions(Enemy, MinionType.Yellow)) / Model.MaxMinionsPerType),
            ((float)(Model.NumMinions(Enemy, MinionType.Blue)) / Model.MaxMinionsPerType),
            ((float)(Model.NumMinions(Enemy, MinionType.Green)) / Model.MaxMinionsPerType),
            ((float)(Health))/MaxHealth,
            ((float)(Enemy.Health))/Enemy.MaxHealth,
            ((float)(Model.Turn))/50,
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
            if(optionIds.Contains(i+1))
            {
                optionValues.Add(i + 1, outputs[i]);
            }
            values.Add(i + 1, outputs[i]);
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
