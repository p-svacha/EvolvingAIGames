using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C004_GreenBlood : Card
    {

        public C004_GreenBlood()
        {
            Id = 4;
            Name = "Green+";
            Text = "Summon a Green.";
            Cost = 2;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            match.SummonMinions(self, new List<Tuple<MinionType, PlayerBlob>>() {
            new Tuple<MinionType, PlayerBlob>(MinionType.Green, self),
        }, summonProtection: true);
        }
    }
}
