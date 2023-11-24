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
        public override int Duration => 5;
        public override bool Repeatable => true;

        protected override void OnActivate()
        {
            Opponent.DealDamage(10);
        }
    }
}
