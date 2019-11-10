using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C001_SummonRed : Card
{

    public C001_SummonRed(MatchModel model) : base(model)
    {
        Name = "Summon Red";
        Text = "Summon a Red";
    }

    public override void Action(Player self, Player enemy)
    {
        Model.SummonMinion(self, MinionType.Red, self);
    }
}
