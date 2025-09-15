using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C016_Rainbow : Card
    {

        public C016_Rainbow()
        {
            Id = 16;
            Name = "Rainbow";
            Text = "Summon three random minions from a different type.";
            Cost = 3;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            List<MinionType> types = match.RandomMinionTypes(3);

            match.SummonMinions(self, new List<Tuple<MinionType, PlayerBlob>>() {
            new Tuple<MinionType, PlayerBlob>(types[0], self),
            new Tuple<MinionType, PlayerBlob>(types[1], self),
            new Tuple<MinionType, PlayerBlob>(types[2], self),
        }, summonProtection: true);
        }
    }
}
