using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C002_Summon2Reds : Card
{

    public C002_Summon2Reds(MatchModel model) : base(model)
    {
        Name = "Summon2Reds";
        Text = "Summon 2 Reds";
    }

    public override void Action(Player self, Player enemy)
    {
        Model.SummonMinion(self, MinionType.Red, self);
        Model.SummonMinion(self, MinionType.Red, self);
    }
}
