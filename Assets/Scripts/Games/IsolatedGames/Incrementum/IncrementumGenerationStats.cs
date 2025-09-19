using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Incrementum
{
    public class IncrementumGenerationStats : GenerationStats
    {
        /// <summary>
        /// All upgrade stats ordered by % acquired.
        /// </summary>
        public List<UpgradeStat> UpgradeStats;

        public IncrementumGenerationStats(EvolutionInformation info, List<Species> speciesData) : base(info, speciesData) { }

        protected override void OnComplete()
        {
            Dictionary<UpgradeDef, int> numAcquired = new Dictionary<UpgradeDef, int>();
            Dictionary<UpgradeDef, int> numNotAcquired = new Dictionary<UpgradeDef, int>();
            Dictionary<UpgradeDef, List<int>> acquiredTicks = new Dictionary<UpgradeDef, List<int>>();

            foreach (UpgradeDef def in DefDatabase<UpgradeDef>.AllDefs)
            {
                numAcquired[def] = 0;
                numNotAcquired[def] = 0;
                acquiredTicks[def] = new List<int>();
            }


            foreach (IncrementumTask t in TaskRanking)
            {
                foreach (var x in t.AcquiredUpgrades)
                {
                    UpgradeDef upgrade = x.Key;
                    if (x.Value)
                    {
                        numAcquired[upgrade]++;
                        int acquiredTick = t.History.First(h => h.Value == upgrade).Key;
                        acquiredTicks[upgrade].Add(acquiredTick);
                    }
                    else numNotAcquired[upgrade]++;
                }
            }

            UpgradeStats = new List<UpgradeStat>();
            foreach (UpgradeDef def in DefDatabase<UpgradeDef>.AllDefs) UpgradeStats.Add(new UpgradeStat(def, numAcquired[def], numNotAcquired[def], acquiredTicks[def]));

            UpgradeStats = UpgradeStats.OrderByDescending(x => x.PercentAcquired).ToList();
        }
    }
}
