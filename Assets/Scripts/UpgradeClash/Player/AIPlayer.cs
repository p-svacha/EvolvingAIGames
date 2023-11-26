using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UpgradeClash
{
    public class AIPlayer : Player
    {
        public Subject Subject;
        public override string DisplayName => Subject.Name;

        /// <summary>
        /// Neural network output values above this threshhold are considered upgrades that the player wants to click.
        /// </summary>
        private const float DesiredUpgradeThreshhold = 0.8f;

        public AIPlayer(Subject subject)
        {
            Subject = subject;
        }

        public override List<Upgrade> GetDesiredUpgrades()
        {
            // Get output of neural net
            float[] outputs = GetOutputs();

            // Get desired upgrades (all above a threshhold)
            Dictionary<Upgrade, float> desiredUpgrades = new Dictionary<Upgrade, float>();
            for(int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] > DesiredUpgradeThreshhold) desiredUpgrades.Add(UpgradeList[i], outputs[i]);
            }

            // Sort desired upgrades by their output value
            desiredUpgrades = desiredUpgrades.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            // Return
            return desiredUpgrades.Keys.ToList();
        }

        public float[] GetInputs()
        {
            List<float> inputs = new List<float>();
            inputs.Add(1);  // Bias

            // Self
            inputs.Add(RelativeHealth);

            inputs.Add(RelativeFood);
            inputs.Add(RelativeWood);
            inputs.Add(RelativeGold);
            inputs.Add(RelativeStone);

            foreach (Upgrade upgrade in UpgradeList) inputs.Add(upgrade.GetInputValue());

            // Opponent
            inputs.Add(Opponent.RelativeHealth);

            inputs.Add(Opponent.RelativeFood);
            inputs.Add(Opponent.RelativeWood);
            inputs.Add(Opponent.RelativeGold);
            inputs.Add(Opponent.RelativeStone);

            foreach (Upgrade upgrade in Opponent.UpgradeList) inputs.Add(upgrade.GetInputValue());

            return inputs.ToArray();
        }

        public float[] GetOutputs()
        {
            float[] inputs = GetInputs();

            // Get output of neural net
            return Subject.Genome.FeedForward(inputs);
        }
    }
}
