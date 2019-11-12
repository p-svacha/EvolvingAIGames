using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M04_Green : Minion
{

    public M04_Green(MatchModel model, Player owner, Player enemy, int orderNum) : base(model, owner, enemy, orderNum)
    {
        Name = "Green";
        Text = "Summon a random minion.";
        Type = MinionType.Green;
        Color = Color.green;
    }

    public override void Action()
    {
        Model.SummonMinion(this, Model.RandomMinionType(), Owner);
    }
}
