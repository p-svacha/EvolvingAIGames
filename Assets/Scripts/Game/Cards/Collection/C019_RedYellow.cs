using System.Collections;
using System.Collections.Generic;
using System;

public class C019_ClassicDuo : Card
{

    public C019_ClassicDuo()
    {
        Id = 19;
        Name = "Classic Duo";
        Text = "Summon a Red and a Yellow.";
        Cost = 2;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.SummonMinions(self, new List<Tuple<MinionType, Player>>() {
            new Tuple<MinionType, Player>(MinionType.Red, self),
            new Tuple<MinionType, Player>(MinionType.Yellow, self),
        }, summonProtection: true);
    }
}
