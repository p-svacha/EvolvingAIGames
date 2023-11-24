using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UpgradeClash
{
    public class UI_PlayerPanel : MonoBehaviour
    {
        public Player Player { get; private set; }

        [Header("Elements")]
        public TextMeshProUGUI PlayerNameText;
        public TextMeshProUGUI ResourceText;

        [Header("SkillTree")]
        public List<UI_Upgrade> Upgrades;

        public void Init(Player player)
        {
            Player = player;
            PlayerNameText.text = player.DisplayName;
        }
    }
}
