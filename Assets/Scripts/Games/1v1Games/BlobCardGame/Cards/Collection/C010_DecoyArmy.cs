using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C010_DecoyArmy : Card
    {

        public C010_DecoyArmy()
        {
            Id = 10;
            Name = "Decoy Army";
            Text = "Summon a Grey for each enemy minion.";
            Cost = 0;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            List<Tuple<MinionType, PlayerBlob>> summons = new List<Tuple<MinionType, PlayerBlob>>();
            for (int i = 0; i < match.NumMinions(enemy, withoutSummonProtection: true); i++)
            {
                summons.Add(new Tuple<MinionType, PlayerBlob>(MinionType.Grey, self));
            }
            match.SummonMinions(self, summons, summonProtection: true);
        }
    }
}
