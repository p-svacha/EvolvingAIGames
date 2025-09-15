using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobCardGame
{
    public class M03_Blue : Minion
    {

        public M03_Blue(BlobCardMatch match, PlayerBlob owner, PlayerBlob enemy, int orderNum) : base(match, owner, enemy, orderNum)
        {
            Name = "Blue";
            Text = "Destroy a random enemy minion.";
            Type = MinionType.Blue;
            Color = new Color(0.2f, 0.2f, 1);
        }

        public override void Action()
        {
            Match.DestabilizeMinions(this, Match.RandomMinionsFromPlayer(Enemy, 1, withoutDestabilized: true));
        }
    }
}
