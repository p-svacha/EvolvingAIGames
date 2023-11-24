using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class UCMatchUI : MatchUI
    {
        public UCMatch UCMatch { get; private set; }

        [Header("Elements")]
        public UI_PlayerPanel Player1Panel;
        public UI_PlayerPanel Player2Panel;

        public override void OnInit()
        {
            UCMatch = (UCMatch)Match;
            Player1Panel.Init(UCMatch.Player1);
            Player2Panel.Init(UCMatch.Player2);
        }
    }
}
