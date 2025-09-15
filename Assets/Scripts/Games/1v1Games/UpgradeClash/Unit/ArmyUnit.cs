using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public abstract class ArmyUnit : Unit
    {
        public override int MaxAmount => UCMatch.ArmyCap;
        public override int Cap => UCMatch.ArmyCap;

        public ArmyUnit(Player player) : base(player) { }

        public abstract int GetAttackDamage();
    }
}
