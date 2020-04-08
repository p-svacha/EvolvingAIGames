using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Playerbar : MonoBehaviour
{
    public Text NameText;
    public Text HealthText;
    public Text CardText;
    public Text GoldText;
    public Text WoodText;

    public void UpdatePlayerbar(Match match, Player player)
    {

        // Name
        if (match.MatchType == MatchType.AI_vs_AI || match.MatchType == MatchType.Human_vs_AI && player == match.Player2)
            NameText.text = player.Name + " | " + ((AIPlayer)player).Brain.Wins + " - " + ((AIPlayer)player).Brain.Losses;
        else
            NameText.text = player.Name;

        // Health
        HealthText.text = player.Health + "/" + player.MaxHealth;

        CardText.text = player.NumCardOptions.ToString();
        GoldText.text = player.Money.ToString();
    }
}
