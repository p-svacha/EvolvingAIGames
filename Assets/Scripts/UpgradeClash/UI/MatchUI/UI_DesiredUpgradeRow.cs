using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UpgradeClash
{
    public class UI_DesiredUpgradeRow : MonoBehaviour
    {
        [Header("Elements")]
        public Image Icon;
        public UI_ProgressBar ProgressBar;

        public void Init(UpgradeId id, float value)
        {
            Icon.sprite = UCResourceManager.Singleton.GetUpgradeSprite(id);
            ProgressBar.UpdateValues(value, 1f, value.ToString(".0%"));

            if (value > AIPlayer.DesiredUpgradeThreshhold) ProgressBar.SetBarColor(new Color(0f, 1f, 0f, 0.3f));
            else ProgressBar.SetBarColor(new Color(0f, 0f, 0f, 0.3f));
        }
    }
}
