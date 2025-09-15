using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C012_RedArmy : Card
    {

        public C012_RedArmy()
        {
            Id = 12;
            Name = "Red Army";
            Text = "Summon 3 Reds.";
            Cost = 3;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            match.SummonMinions(self, new List<Tuple<MinionType, PlayerBlob>>() {
            new Tuple<MinionType, PlayerBlob>(MinionType.Red, self),
            new Tuple<MinionType, PlayerBlob>(MinionType.Red, self),
            new Tuple<MinionType, PlayerBlob>(MinionType.Red, self),
        }, summonProtection: true);
        }
    }
}
