using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class U005_TrainStoneWorker : Upgrade
    {
        public override UpgradeId Id => UpgradeId.TrainStoneWorker;
        public override string Name => "Stone Worker";
        public override int FoodCost => 50;
        public override int BaseDuration => 25;
        public override bool Repeatable => true;

        protected override void OnTakeEffect()
        {
            Self.AddStoneWorker();
        }

        public override bool CanActivate()
        {
            if (Self.Units[UnitId.StoneWorker].IsAtMaxAmount) return false;
            if (Self.IsWorkerInProgress()) return false;
            return true;
        }

        public override float GetInputValue() => GetUnitInputValue(Self.Units[UnitId.StoneWorker]);
    }
}
