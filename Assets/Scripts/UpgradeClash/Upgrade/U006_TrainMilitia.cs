using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class U006_TrainMilitia : Upgrade
    {
        public override UpgradeId Id => UpgradeId.TrainMilitia;
        public override string Name => "Train Militia";
        public override int FoodCost => 60;
        public override int GoldCost => 20;
        public override int BaseDuration => 21;
        public override bool Repeatable => true;

        protected override void OnTakeEffect()
        {
            Self.AddMilitia();
        }

        public override bool CanActivate()
        {
            if (Self.Units[UnitId.Militia].IsAtMaxAmount) return false;
            return true;
        }

        public override float GetInputValue() => GetUnitInputValue(Self.Units[UnitId.Militia]);
    }
}
