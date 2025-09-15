using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C014_Steal : Card
    {

        public C014_Steal()
        {
            Id = 14;
            Name = "Steal";
            Text = "Steal a random enemy minion.";
            Cost = 1;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            match.StealMinions(self, self, match.RandomMinionsFromPlayer(enemy, 1, withoutSummonProtection: true), giveMinionsSummonProtection: true);
        }
    }
}
