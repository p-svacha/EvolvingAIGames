using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C015_RedDuo : Card
    {

        public C015_RedDuo()
        {
            Id = 15;
            Name = "Red Duo";
            Text = "Summon 2 Reds.";
            Cost = 2;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            match.SummonMinions(self, new List<Tuple<MinionType, PlayerBlob>>() {
            new Tuple<MinionType, PlayerBlob>(MinionType.Red, self),
            new Tuple<MinionType, PlayerBlob>(MinionType.Red, self),
        }, summonProtection: true);
        }
    }
}
