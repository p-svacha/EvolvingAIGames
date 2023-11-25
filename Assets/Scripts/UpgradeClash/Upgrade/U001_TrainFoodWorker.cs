using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class U001_TrainFoodWorker : Upgrade
    {
        public override UpgradeId Id => UpgradeId.TrainFoodWorker;
        public override string Name => "Food Worker";
        public override int FoodCost => 50;
        public override int BaseDuration => 25;
        public override bool Repeatable => true;

        protected override void OnTakeEffect()
        {
            Self.AddFoodWorker();
        }

        public override bool CanActivate()
        {
            if (Self.FoodWorkers >= UCMatch.WorkerCap) return false;
            if (Self.IsWorkerInProgress()) return false;
            return true;
        }
    }
}