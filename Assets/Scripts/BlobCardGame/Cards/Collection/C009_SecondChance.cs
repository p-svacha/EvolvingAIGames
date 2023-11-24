using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C009_SecondChance : Card
    {

        public C009_SecondChance()
        {
            Id = 9;
            Name = "Second Chance";
            Text = "Restore 50% of your missing health. Summon a random minion.";
            Cost = 2;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            int healAmount = (int)((self.MaxHealth - self.Health) * 0.5f);
            match.Heal(self, self, healAmount);
            match.SummonMinions(self, new List<Tuple<MinionType, PlayerBlob>>() {
            new Tuple<MinionType, PlayerBlob>(match.RandomMinionType(), self),
        }, summonProtection: true);
        }
    }
}
