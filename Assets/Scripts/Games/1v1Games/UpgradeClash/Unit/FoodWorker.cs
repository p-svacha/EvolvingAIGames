using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class FoodWorker : WorkerUnit
    {
        public override UnitId Id => UnitId.FoodWorker;
        public override string Name => "Food Worker";

        private const int BaseResourceGainAmount = 1;
        private const int BaseCooldown = 5;

        public FoodWorker(Player player) : base(player) { }

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
            if (Self.HasBuilding(BuildingId.Mill)) maxAmount += U009_BuildMill.MaxAmountIncrease;
            return maxAmount;
        }

        public override void OnTick()
        {
            int resources = GetIncome();
            Self.AddFood(resources);
        }
    }
}
