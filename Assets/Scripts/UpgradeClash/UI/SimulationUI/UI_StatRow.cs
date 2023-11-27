using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UpgradeClash
{
    public class UI_StatRow : MonoBehaviour
    {
        [Header("Element")]
        public Button Button;
        public TextMeshProUGUI LabelText;
        public TextMeshProUGUI ValueText;

        public void Init(UCSimulationUI ui, SimulationStat stat)
        {
            Button.onClick.AddListener(() => ui.SetStatToDisplay(stat.Id));
            LabelText.text = stat.DisplayName;
            ValueText.text = stat.GetCurrentValueString();
        }
    }
}
