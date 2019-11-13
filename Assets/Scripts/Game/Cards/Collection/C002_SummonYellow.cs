using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C002_SummonYellow : Card
{

    public C002_SummonYellow(MatchModel model, int id) : base(model, id)
    {
        Name = "Yellow+";
        Text = "Summon a Yellow";
    }

    public override void Action(Player self, Player enemy)
    {
        Model.SummonMinion(self, MinionType.Yellow, self);
    }
}
