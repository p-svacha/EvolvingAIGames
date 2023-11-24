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
        public TextMeshProUGUI CostText;

        public void Init(Upgrade upgrade)
        {
            Upgrade = upgrade;
            CostText.text = upgrade.GoldCost.ToString();
        }
    }
}
