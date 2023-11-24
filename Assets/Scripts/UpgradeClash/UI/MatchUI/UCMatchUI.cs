using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class UCMatchUI : MatchUI
    {
        public UCMatch UCMatch { get; private set; }

        [Header("Elements")]
        public UI_SpeedControl SpeedControls;

        public UI_PlayerPanel Player1Panel;
        public UI_PlayerPanel Player2Panel;

        public GenomeVisualizer Player1GV;
        public GenomeVisualizer Player2GV;

        private void Start()
        {
            SpeedControls.Init(this);  
        }

        public override void OnInit()
        {
            UCMatch = (UCMatch)Match;
            Player1Panel.Init(UCMatch.Player1);
            Player2Panel.Init(UCMatch.Player2);

            SetSpeed1();
        }

        private void Update()
        {
            Player1Panel.UpdateValues();
            Player2Panel.UpdateValues();

            if (Player1GV != null) Player1GV.VisualizeGenome(UCMatch.Subject1.Genome, true, false);
            if (Player2GV != null) Player2GV.VisualizeGenome(UCMatch.Subject2.Genome, true, false);
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
