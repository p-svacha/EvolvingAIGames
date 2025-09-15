using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class U009_BuildMill : Upgrade
    {
        public const int MaxAmountIncrease = 2;

        public override UpgradeId Id => UpgradeId.BuildMill;
        public override string Name => "Build Mill";
        public override string Description => "Increase max amount of food workers by " + MaxAmountIncrease + ".";
        public override BuildingId Building => BuildingId.Base;
        public override int WoodCost => 100;
        public override int BaseDuration => 35;
        public override bool Repeatable => false;
    }
}
