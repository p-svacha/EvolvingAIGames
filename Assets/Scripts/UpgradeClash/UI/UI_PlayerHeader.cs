using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace UpgradeClash
{
    public class UI_PlayerHeader : MonoBehaviour
    {
        public Player Player { get; private set; }

        [Header("Elements")]
        public TextMeshProUGUI PlayerNameText;

        public TextMeshProUGUI FoodText;
        public TextMeshProUGUI FoodIncomeText;
        public UI_ProgressBar FoodProgress;

        public TextMeshProUGUI WoodText;
        public TextMeshProUGUI WoodIncomeText;
        public UI_ProgressBar WoodProgress;

        public TextMeshProUGUI GoldText;
        public TextMeshProUGUI GoldIncomeText;
        public UI_ProgressBar GoldProgress;

        public TextMeshProUGUI StoneText;
        public TextMeshProUGUI StoneIncomeText;
        public UI_ProgressBar StoneProgress;

        public UI_ProgressBar HealthBar;
        public GameObject UpgradeProgressPanel;

        public void Init(Player player)
        {
            Player = player;
            PlayerNameText.text = player.DisplayName;
            if(player is AIPlayer p) PlayerNameText.text += "  (" + p.Subject.Wins + "-" + p.Subject.Losses + ")";
        }

        public void UpdateValues()
        {
            // Resources
            FoodText.text = Player.Food.ToString();
            FoodIncomeText.text = "+ " + ((WorkerUnit)Player.Units[UnitId.FoodWorker]).GetIncome().ToString();
            FoodProgress.gameObject.SetActive(Player.Units[UnitId.FoodWorker].Amount > 0);
            FoodProgress.UpdateValues(Player.Units[UnitId.FoodWorker].RemainingCooldown, Player.Units[UnitId.FoodWorker].GetCooldown(), reverse: true);

            WoodText.text = Player.Wood.ToString();
            WoodIncomeText.text = "+ " + ((WorkerUnit)Player.Units[UnitId.WoodWorker]).GetIncome().ToString();
            WoodProgress.gameObject.SetActive(Player.Units[UnitId.WoodWorker].Amount > 0);
            WoodProgress.UpdateValues(Player.Units[UnitId.WoodWorker].RemainingCooldown, Player.Units[UnitId.WoodWorker].GetCooldown(), reverse: true);

            GoldText.text = Player.Gold.ToString();
            GoldIncomeText.text = "+ " + ((WorkerUnit)Player.Units[UnitId.GoldWorker]).GetIncome().ToString();
            GoldProgress.gameObject.SetActive(Player.Units[UnitId.GoldWorker].Amount > 0);
            GoldProgress.UpdateValues(Player.Units[UnitId.GoldWorker].RemainingCooldown, Player.Units[UnitId.GoldWorker].GetCooldown(), reverse: true);

            StoneText.text = Player.Stone.ToString();
            StoneIncomeText.text = "+ " + ((WorkerUnit)Player.Units[UnitId.StoneWorker]).GetIncome().ToString();
            StoneProgress.gameObject.SetActive(Player.Units[UnitId.StoneWorker].Amount > 0);
            StoneProgress.UpdateValues(Player.Units[UnitId.StoneWorker].RemainingCooldown, Player.Units[UnitId.StoneWorker].GetCooldown(), reverse: true);

            // Health
            HealthBar.UpdateValues(Player.CurrentHealth, Player.MaxHealth, Player.CurrentHealth.ToString() + " / " + Player.MaxHealth.ToString());

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
