using System.Collections;
using System.Collections.Generic;
using System;

public class C014_Steal : Card
{

    public C014_Steal()
    {
        Id = 14;
        Name = "Steal";
        Text = "Steal a random enemy minion.";
        Cost = 1;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.StealMinions(self, self, Model.RandomMinionsFromPlayer(enemy, 1, withoutSummonProtection: true), giveMinionsSummonProtection: true);
    }
}
