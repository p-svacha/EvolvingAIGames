using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class WoodWorker : WorkerUnit
    {
        public override UnitId Id => UnitId.WoodWorker;
        public override string Name => "Wood Worker";

        private const int BaseResourceGainAmount = 1;
        private const int BaseCooldown = 5;

        public WoodWorker(Player player) : base(player) { }

        public override int GetCooldown()
        {
            return BaseCooldown;
        }

        public override int GetIncome()
        {
            int income = Amount * BaseResourceGainAmount;
            return income;
        }

        public override int GetMaxAmount()
        {
            int maxAmount = BaseCap;
            return maxAmount;
        }

        public override void OnTick()
        {
            int resources = GetIncome();
            Self.AddWood(resources);
        }
    }
}
