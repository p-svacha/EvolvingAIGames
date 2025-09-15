using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C019_ClassicDuo : Card
    {

        public C019_ClassicDuo()
        {
            Id = 19;
            Name = "Classic Duo";
            Text = "Summon a Red and a Yellow.";
            Cost = 2;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            match.SummonMinions(self, new List<Tuple<MinionType, PlayerBlob>>() {
            new Tuple<MinionType, PlayerBlob>(MinionType.Red, self),
            new Tuple<MinionType, PlayerBlob>(MinionType.Yellow, self),
        }, summonProtection: true);
        }
    }
}
