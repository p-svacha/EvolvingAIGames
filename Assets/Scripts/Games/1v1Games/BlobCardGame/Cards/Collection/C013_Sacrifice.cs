using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C013_Sacrifice : Card
    {

        public C013_Sacrifice()
        {
            Id = 13;
            Name = "Sacrifice";
            Text = "If you have at least 1 minion, destroy a random friendly minion and heal to full life.";
            Cost = 1;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            List<Minion> toDestroy = match.RandomMinionsFromPlayer(self, 1, withoutSummonProtection: false);
            if (toDestroy.Count > 0)
            {
                match.DestroyMinions(self, toDestroy);
                match.Heal(self, self, self.MaxHealth);
            }
        }
    }
}
