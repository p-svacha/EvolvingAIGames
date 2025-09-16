using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Incrementum
{
    public class UI_GenerationStats : MonoBehaviour
    {
        [Header("Prefabs")]
        public GameObject StatRowPrefab;

        [Header("Elements")]
        public TextMeshProUGUI TitleText;
        public TextMeshProUGUI RuntimeText;
        public UI_IncrementumSubjectDisplay TopPerformer;
        public UI_IncrementumSubjectDisplay MedianPerformer;

        public GameObject UpgradeStatContainer;

        public void ShowStats(IncrementumGenerationStats stats)
        {
            TitleText.text = $"Generation {stats.GenerationNumber} Stats";
            HelperFunctions.DestroyAllChildredImmediately(UpgradeStatContainer, skipElements: 1);

            if (stats.IsComplete)
            {
                RuntimeText.text = stats.Runtime.ToString("#.###") + "s";
                TopPerformer.Show((IncrementumTask)stats.BestPerformingTask);
                MedianPerformer.Show((IncrementumTask)stats.MedianTask);

                foreach (UpgradeStat upgradeStat in stats.UpgradeStats)
                {
                    GameObject statRow = GameObject.Instantiate(StatRowPrefab, UpgradeStatContainer.transform);
                    statRow.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = upgradeStat.Upgrade.Label;
                    statRow.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = upgradeStat.ToString();
                }
            }

            else
            {
                RuntimeText.text = "";
                TopPerformer.Hide();
                MedianPerformer.Hide();
            }
        }
    }
}
