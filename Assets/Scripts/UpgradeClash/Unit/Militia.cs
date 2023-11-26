using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class Militia : ArmyUnit
    {
        public override UnitId Id => UnitId.Militia;
        public override string Name => "Militia";
        private const int BaseAttackDamage = 1;
        private const int BaseAttackCooldown = 10;

        public Militia(Player player) : base(player) { }

        public override int GetAttackDamage()
        {
            int damage = BaseAttackDamage; // Base damage
            if (Self.UpgradeDictionary[UpgradeId.Forging].IsInEffect) damage += U008_Forging.BonusDamage; // Forging upgrade
            if (Opponent.UpgradeDictionary[UpgradeId.BuildPalisade].IsInEffect) damage -= U002_BuildPalisade.BaseDamageReduction; // Palisade reduction
            return damage;
        }

        public override int GetCooldown()
        {
            return BaseAttackCooldown;
        }

        public override void OnTick()
        {
            int damage = Amount * GetAttackDamage();
            Opponent.DealDamage(damage);
        }
    }
}
