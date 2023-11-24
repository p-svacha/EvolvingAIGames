using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C002_SummonRed : Card
    {

        public C002_SummonRed()
        {
            Id = 2;
            Name = "Red+";
            Text = "Summon a Red.";
            Cost = 1;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            match.SummonMinions(self, new List<Tuple<MinionType, PlayerBlob>>() {
            new Tuple<MinionType, PlayerBlob>(MinionType.Red, self),
        }, summonProtection: true);
        }
    }
}
