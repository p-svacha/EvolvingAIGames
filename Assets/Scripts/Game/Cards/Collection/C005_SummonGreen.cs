using System.Collections;
using System.Collections.Generic;
using System;

public class C005_SummonGreen : Card
{

    public C005_SummonGreen(int id) : base(id)
    {
        Name = "Green+";
        Text = "Summon a Green";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.SummonMinion(self, MinionType.Green, self);
    }
}
