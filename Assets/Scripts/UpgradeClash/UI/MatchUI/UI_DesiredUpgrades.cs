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
            Dictionary<UpgradeId, float> values = new Dictionary<UpgradeId, float>();
            for (int i = 0; i < player.UpgradeList.Count; i++) values.Add(player.UpgradeList[i].Id, outputs[i]);
            values = values.OrderByDescending(x => x.Value).ToDictionary(x => x.Key, x => x.Value);

            // Show
            foreach(KeyValuePair<UpgradeId, float> kvp in values)
            {
                UI_DesiredUpgradeRow row = Instantiate(RowPrefab, Container.transform);
                row.Init(kvp.Key, kvp.Value);
            }
        }
    }
}
