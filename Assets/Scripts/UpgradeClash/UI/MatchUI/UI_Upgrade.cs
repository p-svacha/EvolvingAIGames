using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UpgradeClash
{
    public class UI_Upgrade : MonoBehaviour
    {
        public Upgrade Upgrade { get; private set; }

        [Header("Elements")]
        public Image Background;
        public Image Icon;
        public GameObject ProgressBar;
        public TextMeshProUGUI DurationText;

        public void Init(Upgrade upgrade)
        {
            Upgrade = upgrade;
            Icon.sprite = UCResourceManager.Singleton.GetUpgradeSprite(Upgrade);
            UpdateValues();
        }

        public void UpdateValues()
        {
            DurationText.text = System.TimeSpan.FromSeconds(Upgrade.RemainingDuration).ToString(@"mm\:ss");

            float fullWidth = Background.GetComponent<RectTransform>().rect.width;
            float dynamicBarWidth = (1f - ((float)Upgrade.RemainingDuration / Upgrade.BaseDuration)) * fullWidth;
            ProgressBar.GetComponent<RectTransform>().sizeDelta = new Vector2(dynamicBarWidth, ProgressBar.GetComponent<RectTransform>().sizeDelta.y);
        }
    }
}
