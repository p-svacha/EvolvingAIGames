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
        public TextMeshProUGUI FoodIncomeText;
        public TextMeshProUGUI WoodText;
        public TextMeshProUGUI WoodIncomeText;
        public TextMeshProUGUI GoldText;
        public TextMeshProUGUI GoldIncomeText;
        public TextMeshProUGUI StoneText;
        public TextMeshProUGUI StoneIncomeText;
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
            // Resources
            FoodText.text = Player.Food.ToString();
            FoodIncomeText.text = ((WorkerUnit)Player.Units[UnitId.FoodWorker]).GetIncome().ToString() + " / " + Player.Units[UnitId.FoodWorker].GetCooldown().ToString();
            WoodText.text = Player.Wood.ToString();
            WoodIncomeText.text = ((WorkerUnit)Player.Units[UnitId.WoodWorker]).GetIncome().ToString() + " / " + Player.Units[UnitId.WoodWorker].GetCooldown().ToString(); ;
            GoldText.text = Player.Gold.ToString();
            GoldIncomeText.text = ((WorkerUnit)Player.Units[UnitId.GoldWorker]).GetIncome().ToString() + " / " + Player.Units[UnitId.GoldWorker].GetCooldown().ToString(); ;
            StoneText.text = Player.Stone.ToString();
            StoneIncomeText.text = ((WorkerUnit)Player.Units[UnitId.StoneWorker]).GetIncome().ToString() + " / " + Player.Units[UnitId.StoneWorker].GetCooldown().ToString(); ;

            // Health
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
