using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C001_YellowTrio : Card
    {

        public C001_YellowTrio()
        {
            Id = 1;
            Name = "Yellow Trio";
            Text = "Summon three Yellows.";
            Cost = 2;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            match.SummonMinions(self, new List<Tuple<MinionType, PlayerBlob>>() {
            new Tuple<MinionType, PlayerBlob>(MinionType.Yellow, self),
            new Tuple<MinionType, PlayerBlob>(MinionType.Yellow, self),
            new Tuple<MinionType, PlayerBlob>(MinionType.Yellow, self),
        }, summonProtection: true);
        }
    }
}
