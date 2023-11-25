using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class U004_TrainGoldWorker : Upgrade
    {
        public override UpgradeId Id => UpgradeId.TrainGoldWorker;
        public override string Name => "Gold Worker";
        public override int FoodCost => 50;
        public override int BaseDuration => 25;
        public override bool Repeatable => true;

        protected override void OnTakeEffect()
        {
            Self.AddGoldWorker();
        }

        public override bool CanActivate()
        {
            if (Self.GoldWorkers >= UCMatch.WorkerCap) return false;
            if (Self.IsWorkerInProgress()) return false;
            return true;
        }
    }
}
