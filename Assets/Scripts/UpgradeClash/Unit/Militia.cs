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
            return BaseAttackDamage;
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
