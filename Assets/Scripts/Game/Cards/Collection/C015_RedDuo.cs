using System.Collections;
using System.Collections.Generic;
using System;

public class C015_RedDuo : Card
{

    public C015_RedDuo()
    {
        Id = 15;
        Name = "Red Duo";
        Text = "Summon 2 Reds.";
        Cost = 2;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.SummonMinions(self, new List<Tuple<MinionType, Player>>() {
            new Tuple<MinionType, Player>(MinionType.Red, self),
            new Tuple<MinionType, Player>(MinionType.Red, self),
        }, summonProtection: true);
    }
}
