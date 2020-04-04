using System.Collections;
using System.Collections.Generic;
using System;

public class C013_Sacrifice : Card
{

    public C013_Sacrifice()
    {
        Id = 13;
        Name = "Sacrifice";
        Text = "Destroy a random friendly minion. Heal to full life.";
        Cost = 1;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.DestroyMinions(self, Model.RandomMinionsFromPlayer(self, 1, withoutSummonProtection: false));
        Model.Heal(self, self, self.MaxHealth);
    }
}
