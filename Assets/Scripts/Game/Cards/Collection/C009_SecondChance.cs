using System.Collections;
using System.Collections.Generic;
using System;

public class C009_SecondChance : Card
{

    public C009_SecondChance()
    {
        Id = 9;
        Name = "Second Chance";
        Text = "Restore 50% of your missing health. Summon a random minion.";
        Cost = 2;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        int healAmount = (int)((self.MaxHealth - self.Health) * 0.5f);
        Model.Heal(self, self, healAmount);
        Model.SummonMinion(self, Model.RandomMinionType(), self, summonProtection: true);
    }
}
