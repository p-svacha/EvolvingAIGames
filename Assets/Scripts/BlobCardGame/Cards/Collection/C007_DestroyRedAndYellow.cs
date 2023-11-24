using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C007_DestroyRedAndYellow : Card
    {

        public C007_DestroyRedAndYellow()
        {
            Id = 7;
            Name = "Anti-Rellow";
            Text = "Destroy all enemy Red and Yellow minions.";
            Cost = 1;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            List<Minion> toDestroy = match.AllMinionsOfType(enemy, MinionType.Red, withoutSummonProtection: true);
            toDestroy.AddRange(match.AllMinionsOfType(enemy, MinionType.Yellow, withoutSummonProtection: true));
            match.DestroyMinions(self, toDestroy);
        }
    }
}
