using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class UI_StatDisplay : MonoBehaviour
    {
        [Header("Elements")]
        public GameObject Container;

        [Header("Prefabs")]
        public UI_StatRow StatRowPrefab;

        public void UpdateValues(UCSimulationUI ui, List<SimulationStat> stats)
        {
            HelperFunctions.DestroyAllChildredImmediately(Container);

            foreach(SimulationStat stat in stats)
            {
                UI_StatRow row = Instantiate(StatRowPrefab, Container.transform);
                row.Init(ui, stat);
            }
        }
    }
}
