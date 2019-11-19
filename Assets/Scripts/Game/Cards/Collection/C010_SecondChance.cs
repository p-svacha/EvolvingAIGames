using System.Collections;
using System.Collections.Generic;
using System;

public class C010_SecondChance : Card
{

    public C010_SecondChance(int id) : base(id)
    {
        Name = "Second Chance";
        Text = "Restore 40% of your missing health. Summon a random minion.";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        int healAmount = (int)((self.MaxHealth - self.Health) * 0.4f);
        Model.Heal(self, self, healAmount);
        Model.SummonMinion(self, Model.RandomMinionType(), self, summonProtection: true);
    }
}
