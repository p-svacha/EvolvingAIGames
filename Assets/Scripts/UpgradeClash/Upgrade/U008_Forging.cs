using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class U008_Forging : Upgrade
    {
        public const int BonusDamage = 1;

        public override UpgradeId Id => UpgradeId.Forging;
        public override string Name => "Forging";
        public override string Description => "Give your militia +1 attack.";
        public override BuildingId Building => BuildingId.Barracks;
        public override int FoodCost => 150;
        public override int BaseDuration => 50;
        public override bool Repeatable => false;
    }
}
