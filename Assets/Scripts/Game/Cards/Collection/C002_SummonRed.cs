using System.Collections;
using System.Collections.Generic;
using System;

public class C002_SummonRed : Card
{

    public C002_SummonRed()
    {
        Id = 2;
        Name = "Red+";
        Text = "Summon a Red.";
        Cost = 1;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.SummonMinions(self, new List<Tuple<MinionType, Player>>() {
            new Tuple<MinionType, Player>(MinionType.Red, self),
        }, summonProtection: true);
    }
}
