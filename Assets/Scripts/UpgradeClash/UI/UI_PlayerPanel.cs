using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace UpgradeClash
{
    public class UI_PlayerPanel : MonoBehaviour
    {
        public Player Player { get; private set; }

        [Header("Elements")]
        public TextMeshProUGUI PlayerNameText;
        public TextMeshProUGUI FoodText;
        public TextMeshProUGUI WoodText;
        public TextMeshProUGUI GoldText;
        public TextMeshProUGUI StoneText;
        public UI_HealthBar HealthBar;
        public GameObject UpgradeProgressPanel;

        public void Init(Player player)
        {
            Player = player;
            PlayerNameText.text = player.DisplayName;
            if(player is AIPlayer p) PlayerNameText.text += "  (" + p.Subject.Wins + "-" + p.Subject.Losses + ")";
            HealthBar.Init(player);
        }

        public void UpdateValues()
        {
            FoodText.text = Player.Food.ToString() + " (+" + Player.GetFoodIncome() + ")";
            WoodText.text = Player.Wood.ToString() + " (+" + Player.GetWoodIncome() + ")";
            GoldText.text = Player.Gold.ToString() + " (+" + Player.GetGoldIncome() + ")";
            StoneText.text = Player.Stone.ToString() + " (+" + Player.GetStoneIncome() + ")";
            HealthBar.UpdateValues();

            // Upgrade Progress
            HelperFunctions.DestroyAllChildredImmediately(UpgradeProgressPanel);
            foreach(Upgrade upgrade in Player.UpgradeList.Where(x => x.IsInProgress))
            {
                UI_Upgrade upgradeUi = Instantiate(UCResourceManager.Singleton.UpgradePrefab, UpgradeProgressPanel.transform);
                upgradeUi.Init(upgrade);
            }
        }
    }
}
