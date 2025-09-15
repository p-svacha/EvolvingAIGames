using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C003_SummonBlue : Card
    {

        public C003_SummonBlue()
        {
            Id = 3;
            Name = "Blue+";
            Text = "Summon a Blue";
            Cost = 2;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            match.SummonMinions(self, new List<Tuple<MinionType, PlayerBlob>>() {
            new Tuple<MinionType, PlayerBlob>(MinionType.Blue, self),
        }, summonProtection: true);
        }
    }
}
