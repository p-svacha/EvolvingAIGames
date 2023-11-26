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
        public Image Icon;
        public UI_ProgressBar ProgressBar;

        public void Init(Upgrade upgrade)
        {
            Upgrade = upgrade;
            Icon.sprite = UCResourceManager.Singleton.GetUpgradeSprite(Upgrade.Id);
            UpdateValues();
        }

        public void UpdateValues()
        {
            ProgressBar.UpdateValues(Upgrade.RemainingDuration, Upgrade.GetDuration(), System.TimeSpan.FromSeconds(Upgrade.RemainingDuration).ToString(@"m\:ss"), reverse: true);
        }
    }
}
