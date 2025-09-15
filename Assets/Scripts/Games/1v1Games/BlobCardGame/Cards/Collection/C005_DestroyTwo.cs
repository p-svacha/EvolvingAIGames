using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C005_DestroyTwo : Card
    {

        public C005_DestroyTwo()
        {
            Id = 5;
            Name = "Two Crosses";
            Text = "Destroy two random minions.";
            Cost = 1;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            match.DestroyMinions(self, match.RandomMinionsFromPlayer(enemy, 2, withoutSummonProtection: true));
        }
    }
}
