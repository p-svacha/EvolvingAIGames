using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public abstract class WorkerUnit : Unit
    {
        public const int BaseCap = 2; // Max amount at start of game
        public override int MaxAmount => GetMaxAmount();
        public override int Cap => UCMatch.WorkerCap;

        public WorkerUnit(Player player) : base(player) { }

        public abstract int GetIncome();
        public abstract int GetMaxAmount();
    }
}
