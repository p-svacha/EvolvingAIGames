using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class U002_BuildPalisade : Upgrade
    {
        public const int DamageReduction = 1;

        public override UpgradeId Id => UpgradeId.BuildPalisade;
        public override string Name => "Build Palisade";
        public override string Description => "Decreases unit damage by " + DamageReduction + ".";
        public override BuildingId Building => BuildingId.Base;
        public override int WoodCost => 60;
        public override int BaseDuration => 70;
        public override bool Repeatable => false;
    }
}
