using System.Collections;
using System.Collections.Generic;
using System;

public class C013_Sacrifice : Card
{

    public C013_Sacrifice()
    {
        Id = 13;
        Name = "Sacrifice";
        Text = "If you have at least 1 minion, destroy a random friendly minion and heal to full life.";
        Cost = 1;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        List<Minion> toDestroy = Model.RandomMinionsFromPlayer(self, 1, withoutSummonProtection: false);
        if (toDestroy.Count > 0)
        {
            Model.DestroyMinions(self, toDestroy);
            Model.Heal(self, self, self.MaxHealth);
        }
    }
}
