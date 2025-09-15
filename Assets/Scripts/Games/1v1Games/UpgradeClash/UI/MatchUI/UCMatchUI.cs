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

        public UI_PlayerHeader P1_Header;
        public UI_PlayerHeader P2_Header;

        public UI_Map P1_Map;
        public UI_Map P2_Map;

        public UI_ArmyPanel P1_ArmyPanel;
        public UI_ArmyPanel P2_ArmyPanel;

        public GenomeVisualizer Player1GV;
        public GenomeVisualizer Player2GV;

        public UI_DesiredUpgrades P1_DesiredUpgrades;
        public UI_DesiredUpgrades P2_DesiredUpgrades;

        private void Start()
        {
            SpeedControls.Init(this);  
        }

        public override void OnInit()
        {
            UCMatch = (UCMatch)Match;

            P1_Header.Init(UCMatch.Player1);
            P2_Header.Init(UCMatch.Player2);

            P1_Map.Init(UCMatch.Player1);
            P2_Map.Init(UCMatch.Player2);

            P1_ArmyPanel.Init(UCMatch.Player1);
            P2_ArmyPanel.Init(UCMatch.Player2);

            SetSpeed1();
        }

        private void Update()
        {
            TickText.text = UCMatch.Ticks.ToString();

            P1_Header.UpdateValues();
            P2_Header.UpdateValues();

            P1_Map.UpdateVales();
            P2_Map.UpdateVales();

            P1_ArmyPanel.UpdateValues();
            P2_ArmyPanel.UpdateValues();

            if (UCMatch.Player1 is AIPlayer ai1) P1_DesiredUpgrades.UpdateValues(ai1);
            if(UCMatch.Player2 is AIPlayer ai2) P2_DesiredUpgrades.UpdateValues(ai2);

            // Genome Visualization
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Player1GV.gameObject.SetActive(!Player1GV.gameObject.activeSelf);
                Player2GV.gameObject.SetActive(!Player2GV.gameObject.activeSelf);
            }

            if (Player1GV != null && Player1GV.gameObject.activeSelf) Player1GV.VisualizeGenome(UCMatch.Subject1.Genome, showConnections: false);
            if (Player2GV != null && Player2GV.gameObject.activeSelf) Player2GV.VisualizeGenome(UCMatch.Subject2.Genome, showConnections: false);
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
            UCMatch.SetTicksPerSecond(6);
            SpeedControls.HighlightButton(SpeedControls.Speed2Button);
        }
        public void SetSpeed3()
        {
            UCMatch.SetTicksPerSecond(24);
            SpeedControls.HighlightButton(SpeedControls.Speed3Button);
        }
    }
}
