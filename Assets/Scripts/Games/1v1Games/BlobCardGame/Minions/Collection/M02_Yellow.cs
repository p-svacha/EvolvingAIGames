using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobCardGame
{
    public class M02_Yellow : Minion
    {

        public M02_Yellow(BlobCardMatch match, PlayerBlob owner, PlayerBlob enemy, int orderNum) : base(match, owner, enemy, orderNum)
        {
            Name = "Yellow";
            Text = "Restore 2 health.";
            Type = MinionType.Yellow;
            Color = Color.yellow;
        }

        public override void Action()
        {
            Match.Heal(this, Owner, 2);
        }
    }
}
