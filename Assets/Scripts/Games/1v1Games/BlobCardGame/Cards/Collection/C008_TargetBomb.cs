using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace BlobCardGame
{
    public class C008_TargetBomb : Card
    {

        public C008_TargetBomb()
        {
            Id = 8;
            Name = "Target Bomb";
            Text = "Destroy all enemy minions of the type that your opponent has most of.";
            Cost = 1;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            Dictionary<MinionType, int> typesOrderedByCount = match.Minions
                .Where(x => x.Owner == enemy && !x.HasSummonProtection)
                .GroupBy(x => x.Type)
                .OrderByDescending(grp => grp.Count())
                .ToDictionary(x => x.Key, x => x.Count());

            if (typesOrderedByCount.Count > 0)
            {
                List<Minion> toDestroy = match.AllMinionsOfType(enemy, typesOrderedByCount.First().Key, withoutSummonProtection: true);
                match.DestroyMinions(self, toDestroy);
            }
        }
    }
}
