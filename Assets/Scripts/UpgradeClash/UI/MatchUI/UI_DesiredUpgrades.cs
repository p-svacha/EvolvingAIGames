using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UpgradeClash
{
    public class UI_DesiredUpgrades : MonoBehaviour
    {
        [Header("Elements")]
        public GameObject Container;

        [Header("Prefabs")]
        public UI_DesiredUpgradeRow RowPrefab;

        public void UpdateValues(AIPlayer player)
        {
            HelperFunctions.DestroyAllChildredImmediately(Container);

            // Get outputs
            float[] outputs = player.GetOutputs();

            // Sort
            Dictionary<Upgrade, float> values = new Dictionary<Upgrade, float>();
            for (int i = 0; i < player.UpgradeList.Count; i++) values.Add(player.UpgradeList[i], outputs[i]);
            values = values.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            // Filter and Show
            foreach(KeyValuePair<Upgrade, float> kvp in values)
            {
                if (kvp.Key.IsInEffect) continue; // Don't show unique ones that are already researched

                UI_DesiredUpgradeRow row = Instantiate(RowPrefab, Container.transform);
                row.Init(kvp.Key.Id, kvp.Value);
            }
        }
    }
}
