using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class UI_ArmyPanel : MonoBehaviour
    {
        public Player Player { get; private set; }

        [Header("Elements")]
        public UI_ArmyUnitPanel MilitiaPanel;

        public void Init(Player player)
        {
            Player = player;
            MilitiaPanel.Init((ArmyUnit)player.Units[UnitId.Militia]);
        }

        public void UpdateValues()
        {
            MilitiaPanel.UpdateValues();
        }
    }
}
