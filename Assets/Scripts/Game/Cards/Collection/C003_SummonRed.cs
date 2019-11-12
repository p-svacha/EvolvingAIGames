﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C003_SummonRed : Card
{

    public C003_SummonRed(MatchModel model) : base(model)
    {
        Name = "Red+";
        Text = "Summon a Red";
    }

    public override void Action(Player self, Player enemy)
    {
        Model.SummonMinion(self, MinionType.Red, self);
    }
}