using System.Collections;
using System.Collections.Generic;
using System;

public class C006_DestroyTwo : Card
{

    public C006_DestroyTwo(int id) : base(id)
    {
        Name = "Two Crosses";
        Text = "Destroy two random minions.";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.DestroyMultipleMinions(self, Model.RandomMinionsFromPlayer(enemy, 2, withoutSummonProtection: true));
    }
}
