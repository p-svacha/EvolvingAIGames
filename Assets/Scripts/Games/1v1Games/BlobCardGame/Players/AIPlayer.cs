using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlobCardGame
{
    public class AIPlayer : PlayerBlob
    {
        public Subject Brain;

        public AIPlayer(BlobCardMatch model, Subject brain) : base(model, brain.Name)
        {
            Brain = brain;
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
            1,                                                                                                      // Bias
            ((float)(Match.NumMinions(this, MinionType.Red)) / BlobCardMatch.MaxMinionsPerType),                    // Own Red
            ((float)(Match.NumMinions(this, MinionType.Yellow)) / BlobCardMatch.MaxMinionsPerType),                 // Own Yellow
            ((float)(Match.NumMinions(this, MinionType.Blue)) / BlobCardMatch.MaxMinionsPerType),                   // Own Blue
            ((float)(Match.NumMinions(this, MinionType.Green)) / BlobCardMatch.MaxMinionsPerType),                  // Own Green
            ((float)(Match.NumMinions(this, MinionType.Grey)) / BlobCardMatch.MaxMinionsPerType),                   // Own Grey
            ((float)(Match.NumMinions(Enemy, MinionType.Red)) / BlobCardMatch.MaxMinionsPerType),                   // Enemy Red
            ((float)(Match.NumMinions(Enemy, MinionType.Yellow)) / BlobCardMatch.MaxMinionsPerType),                // Enemy Yellow
            ((float)(Match.NumMinions(Enemy, MinionType.Blue)) / BlobCardMatch.MaxMinionsPerType),                  // Enemy Blue
            ((float)(Match.NumMinions(Enemy, MinionType.Green)) / BlobCardMatch.MaxMinionsPerType),                 // Enemy Green
            ((float)(Match.NumMinions(Enemy, MinionType.Grey)) / BlobCardMatch.MaxMinionsPerType),                  // Enemy Grey
            ((float)(Health)) / MaxHealth,                                                                  // Own Health
            ((float)(Enemy.Health)) / Enemy.MaxHealth,                                                      // Enemy Health
            Mathf.Min(1,((float)(Match.Turn)) / BlobCardMatch.FatigueDamageStartTurn),                              // Turn
            (NumCardOptions - BlobCardMatch.MinCardOptions) / (BlobCardMatch.MaxCardOptions - BlobCardMatch.MinCardOptions),        // Own Card Options
            (Enemy.NumCardOptions - BlobCardMatch.MinCardOptions) / (BlobCardMatch.MaxCardOptions - BlobCardMatch.MinCardOptions),  // Enemy Card Options
            };
            float[] outputs = Brain.Genome.FeedForward(inputs);

            // Visualize
            if (Match.IsVisual && Match.SimulationMode == MatchSimulationMode.Watch)
            {
                if (this == Match.Player1)
                    Match.UI.Player1CV.VisualizeOptions(outputs, optionIds);
                else
                    Match.UI.Player2CV.VisualizeOptions(outputs, optionIds);
            }

            // Save all outputs and option outputs in dictionary
            Dictionary<int, float> values = new Dictionary<int, float>();
            Dictionary<int, float> optionValues = new Dictionary<int, float>();
            for (int i = 0; i < outputs.Length; i++)
            {
                if (optionIds.Contains(i))
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
            foreach (Card c in options)
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
}
