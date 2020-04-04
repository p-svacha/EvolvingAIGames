using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class M04_Green : Minion
{

    public M04_Green(Match model, Player owner, Player enemy, int orderNum) : base(model, owner, enemy, orderNum)
    {
        Name = "Green";
        Text = "Summon a random minion.";
        Type = MinionType.Green;
        Color = Color.green;
    }

    public override void Action()
    {
        Model.SummonMinions(this, new List<Tuple<MinionType, Player>>() {
            new Tuple<MinionType, Player>(Model.RandomMinionType(), Owner),
        }, summonProtection: false);
    }
}
