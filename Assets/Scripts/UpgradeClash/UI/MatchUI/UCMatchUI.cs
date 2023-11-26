using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UpgradeClash
{
    public class UCMatchUI : MatchUI
    {
        public UCMatch UCMatch { get; private set; }

        [Header("Elements")]
        public TextMeshProUGUI TickText;

        public UI_SpeedControl SpeedControls;

        public UI_PlayerPanel Player1Panel;
        public UI_PlayerPanel Player2Panel;

        public GenomeVisualizer Player1GV;
        public GenomeVisualizer Player2GV;

        public GameObject P1_FoodWorkerContainer;
        public GameObject P1_WoodWorkerContainer;
        public GameObject P1_GoldWorkerContainer;
        public GameObject P1_StoneWorkerContainer;

        public GameObject P2_FoodWorkerContainer;
        public GameObject P2_WoodWorkerContainer;
        public GameObject P2_GoldWorkerContainer;
        public GameObject P2_StoneWorkerContainer;

        public UI_ArmyPanel P1_ArmyPanel;
        public UI_ArmyPanel P2_ArmyPanel;

        private void Start()
        {
            SpeedControls.Init(this);  
        }

        public override void OnInit()
        {
            UCMatch = (UCMatch)Match;
            Player1Panel.Init(UCMatch.Player1);
            Player2Panel.Init(UCMatch.Player2);

            P1_ArmyPanel.Init(UCMatch.Player1);
            P2_ArmyPanel.Init(UCMatch.Player2);

            SetSpeed1();
        }

        private void Update()
        {
            TickText.text = UCMatch.Ticks.ToString();

            Player1Panel.UpdateValues();
            Player2Panel.UpdateValues();

            if (Player1GV != null && Player1GV.gameObject.activeSelf) Player1GV.VisualizeGenome(UCMatch.Subject1.Genome, true, false);
            if (Player2GV != null && Player2GV.gameObject.activeSelf) Player2GV.VisualizeGenome(UCMatch.Subject2.Genome, true, false);

            SetWorkerAmount(P1_FoodWorkerContainer, UCMatch.Player1.Units[UnitId.FoodWorker].Amount);
            SetWorkerAmount(P1_WoodWorkerContainer, UCMatch.Player1.Units[UnitId.WoodWorker].Amount);
            SetWorkerAmount(P1_GoldWorkerContainer, UCMatch.Player1.Units[UnitId.GoldWorker].Amount);
            SetWorkerAmount(P1_StoneWorkerContainer, UCMatch.Player1.Units[UnitId.StoneWorker].Amount);

            SetWorkerAmount(P2_FoodWorkerContainer, UCMatch.Player2.Units[UnitId.FoodWorker].Amount);
            SetWorkerAmount(P2_WoodWorkerContainer, UCMatch.Player2.Units[UnitId.WoodWorker].Amount);
            SetWorkerAmount(P2_GoldWorkerContainer, UCMatch.Player2.Units[UnitId.GoldWorker].Amount);
            SetWorkerAmount(P2_StoneWorkerContainer, UCMatch.Player2.Units[UnitId.StoneWorker].Amount);

            P1_ArmyPanel.UpdateValues();
            P2_ArmyPanel.UpdateValues();
        }

        private void SetWorkerAmount(GameObject container, int numWorkers)
        {
            HelperFunctions.DestroyAllChildredImmediately(container);
            for (int i = 0; i < numWorkers; i++) Instantiate(UCResourceManager.Singleton.UnitPrefab, container.transform);
        }

        public void SetSpeed0()
        {
            UCMatch.SetTicksPerSecond(0);
            SpeedControls.HighlightButton(SpeedControls.PauseButton);
        }
        public void SetSpeed1()
        {
            UCMatch.SetTicksPerSecond(1);
            SpeedControls.HighlightButton(SpeedControls.Speed1Button);
        }
        public void SetSpeed2()
        {
            UCMatch.SetTicksPerSecond(4);
            SpeedControls.HighlightButton(SpeedControls.Speed2Button);
        }
        public void SetSpeed3()
        {
            UCMatch.SetTicksPerSecond(16);
            SpeedControls.HighlightButton(SpeedControls.Speed3Button);
        }
    }
}
