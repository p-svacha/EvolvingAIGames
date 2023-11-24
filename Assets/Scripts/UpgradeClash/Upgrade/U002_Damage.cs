using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class U002_Damage : Upgrade
    {
        public override UpgradeId Id => UpgradeId.Damage;
        public override string Name => "Damage";
        public override int GoldCost => 20;
        public override int BaseDuration => 5;
        public override bool Repeatable => true;

        protected override void OnTakeEffect()
        {
            Opponent.DealDamage(5);
        }
    }
}
