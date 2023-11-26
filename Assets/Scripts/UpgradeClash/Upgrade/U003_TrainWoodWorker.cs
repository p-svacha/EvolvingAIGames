using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class U003_TrainWoodWorker : Upgrade
    {
        public override UpgradeId Id => UpgradeId.TrainWoodWorker;
        public override string Name => "Wood Worker";
        public override int FoodCost => 50;
        public override int BaseDuration => 25;
        public override bool Repeatable => true;

        protected override void OnTakeEffect()
        {
            Self.AddWoodWorker();
        }

        public override bool CanActivate()
        {
            if (Self.Units[UnitId.WoodWorker].IsAtMaxAmount) return false;
            if (Self.IsWorkerInProgress()) return false;
            return true;
        }

        public override float GetInputValue() => GetUnitInputValue(Self.Units[UnitId.WoodWorker]);
    }
}
