using System.Collections;
using System.Collections.Generic;
using System;

public class C003_SummonBlue : Card
{

    public C003_SummonBlue()
    {
        Id = 3;
        Name = "Blue+";
        Text = "Summon a Blue";
        Cost = 1;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.SummonMinion(self, MinionType.Blue, self, summonProtection: true);
    }
}
