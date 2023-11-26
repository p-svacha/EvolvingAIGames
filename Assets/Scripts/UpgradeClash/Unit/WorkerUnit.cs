using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public abstract class WorkerUnit : Unit
    {
        public override int MaxAmount => UCMatch.WorkerCap;

        public WorkerUnit(Player player) : base(player) { }

        public abstract int GetIncome();
    }
}
