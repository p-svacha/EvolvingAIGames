using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Incrementum
{
    public class UpgradeStat
    {
        public UpgradeDef Upgrade { get; private set; }
        public int NumAcquired { get; private set; }
        public int NumNotAcquired { get; private set; }
        public float PercentAcquired { get; private set; }
        public int Acquired_EarliestTick { get; private set; }
        public int Acquired_AverageTick { get; private set; }

        public UpgradeStat(UpgradeDef upgrade, int numAcquired, int numNotAcquired, List<int> acquiredTicks)
        {
            Upgrade = upgrade;

            NumAcquired = numAcquired;
            NumNotAcquired = numNotAcquired;
            PercentAcquired = (float)numAcquired / (numAcquired + numNotAcquired);
            Acquired_EarliestTick = acquiredTicks.Count == 0 ? -1 : acquiredTicks.Min();
            Acquired_AverageTick = acquiredTicks.Count == 0 ? -1 : Mathf.RoundToInt((float)acquiredTicks.Average());
        }

        public override string ToString()
        {
            return $"{Mathf.RoundToInt(PercentAcquired*100)}% eT={Acquired_EarliestTick} μT={Acquired_AverageTick}";
        }
    }
}
