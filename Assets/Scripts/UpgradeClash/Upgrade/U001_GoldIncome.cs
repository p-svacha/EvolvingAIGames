using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class U001_GoldIncome : Upgrade
    {
        public override UpgradeId Id => UpgradeId.GoldIncome;
        public override string Name => "Gold Income";
        public override int GoldCost => 100;
        public override int Duration => 10;
        public override bool Repeatable => false;

        protected override UpgradePermanentEffect GetPermanentEffect()
        {
            return new UpgradePermanentEffect()
            {
                GoldIncomeBonus = 10
            };
        }
    }
}
