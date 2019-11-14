using System.Collections;
using System.Collections.Generic;
using System;

public class C004_SummonBlue : Card
{

    public C004_SummonBlue(int id) : base(id)
    {
        Name = "Blue+";
        Text = "Summon a Blue";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.SummonMinion(self, MinionType.Blue, self);
    }
}
