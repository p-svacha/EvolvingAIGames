using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C005_SummonGreen : Card
{

    public C005_SummonGreen(MatchModel model) : base(model)
    {
        Name = "Green+";
        Text = "Summon a Green";
    }

    public override void Action(Player self, Player enemy)
    {
        Model.SummonMinion(self, MinionType.Green, self);
    }
}
