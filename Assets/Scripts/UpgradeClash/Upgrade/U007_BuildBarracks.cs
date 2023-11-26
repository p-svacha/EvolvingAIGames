using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class U007_BuildBarracks : Upgrade
    {
        public override UpgradeId Id => UpgradeId.BuildBarracks;
        public override string Name => "Build Barracks";
        public override BuildingId Building => BuildingId.Base;
        public override int WoodCost => 100;
        public override int BaseDuration => 50;
        public override bool Repeatable => false;
    }
}
