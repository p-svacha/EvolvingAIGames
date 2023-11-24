using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobCardGame
{
    public class M01_Red : Minion
    {

        public M01_Red(BlobCardMatch match, PlayerBlob owner, PlayerBlob enemy, int orderNum) : base(match, owner, enemy, orderNum)
        {
            Name = "Red";
            Text = "Deals 4 damage to the enemy.";
            Type = MinionType.Red;
            Color = Color.red;
        }

        public override void Action()
        {
            Match.DealDamage(this, Enemy, 4);
        }
    }
}
