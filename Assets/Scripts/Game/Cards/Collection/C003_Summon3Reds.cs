using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C003_Summon3Reds : Card
{

    public C003_Summon3Reds(MatchModel model) : base(model)
    {
        Name = "Summon3Reds";
        Text = "Summon 3 Reds";
    }

    public override void Action(Player self, Player enemy)
    {
        Model.SummonMinion(self, MinionType.Red, self);
        Model.SummonMinion(self, MinionType.Red, self);
        Model.SummonMinion(self, MinionType.Red, self);
    }
}
