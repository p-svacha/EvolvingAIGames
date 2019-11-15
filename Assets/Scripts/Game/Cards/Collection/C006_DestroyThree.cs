using System.Collections;
using System.Collections.Generic;
using System;

public class C006_DestroyThree : Card
{

    public C006_DestroyThree(int id) : base(id)
    {
        Name = "Three Crosses";
        Text = "Destroy three random minions.";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.DestroyMultipleMinions(self, Model.RandomMinionsFromPlayer(enemy, 3));
    }
}
