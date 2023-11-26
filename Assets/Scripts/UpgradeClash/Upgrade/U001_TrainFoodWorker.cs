using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class U001_TrainFoodWorker : Upgrade
    {
        public override UpgradeId Id => UpgradeId.TrainFoodWorker;
        public override string Name => "Food Worker";
        public override BuildingId Building => BuildingId.Base;
        public override int FoodCost => 50;
        public override int BaseDuration => 25;
        public override bool Repeatable => true;

        protected override void OnTakeEffect()
        {
            Self.AddFoodWorker();
        }

        public override bool CanActivate()
        {
            if (Self.Units[UnitId.FoodWorker].IsAtMaxAmount) return false;
            return true;
        }

        public override float GetInputValue() => GetUnitInputValue(Self.Units[UnitId.FoodWorker]);
    }
}
