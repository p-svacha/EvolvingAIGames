using System.Collections;
using System.Collections.Generic;
using System;

public class C005_DestroyTwo : Card
{

    public C005_DestroyTwo()
    {
        Id = 5;
        Name = "Two Crosses";
        Text = "Destroy two random minions.";
        Cost = 1;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.DestroyMultipleMinions(self, Model.RandomMinionsFromPlayer(enemy, 2, withoutSummonProtection: true));
    }
}
