using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C004_SummonBlue : Card
{

    public C004_SummonBlue(MatchModel model, int id) : base(model, id)
    {
        Name = "Blue+";
        Text = "Summon a Blue";
    }

    public override void Action(Player self, Player enemy)
    {
        Model.SummonMinion(self, MinionType.Blue, self);
    }
}
