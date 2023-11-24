using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BlobCardGame
{
    public class Playerbar : MonoBehaviour
    {
        public Text NameText;
        public Text HealthText;
        public Text CardText;
        public Text GoldText;
        public Text WoodText;

        public void UpdatePlayerbar(BlobCardMatch match, PlayerBlob player)
        {
            // Name
            if (match.SimulationMode == MatchSimulationMode.Watch || match.SimulationMode == MatchSimulationMode.Play && player == match.Player2)
                NameText.text = player.Name + " | " + ((AIPlayer)player).Brain.Wins + " - " + ((AIPlayer)player).Brain.Losses;
            else
                NameText.text = player.Name;

            // Health
            HealthText.text = player.Health + "/" + player.MaxHealth;

            CardText.text = player.NumCardOptions.ToString();
            GoldText.text = player.Money.ToString();
        }
    }
}
