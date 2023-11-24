using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace UpgradeClash
{
    public class UI_HealthBar : MonoBehaviour
    {
        private Player Player;

        [Header("Elements")]
        public TextMeshProUGUI Text;
        public GameObject BarContainer;
        public GameObject DynamicBar;

        public void Init(Player player)
        {
            Player = player;
        }

        public void UpdateValues()
        {
            Text.text = Player.CurrentHealth + " / " + Player.MaxHealth;

            float fullWidth = BarContainer.GetComponent<RectTransform>().rect.width;
            float dynamicBarWidth = ((float)Player.CurrentHealth / Player.MaxHealth) * fullWidth;
            DynamicBar.GetComponent<RectTransform>().sizeDelta = new Vector2(dynamicBarWidth, DynamicBar.GetComponent<RectTransform>().sizeDelta.y);

        }
    }
}
