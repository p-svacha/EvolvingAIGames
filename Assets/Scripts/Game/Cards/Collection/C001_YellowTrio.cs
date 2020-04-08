using System.Collections;
using System.Collections.Generic;
using System;

public class C001_YellowTrio : Card
{

    public C001_YellowTrio()
    {
        Id = 1;
        Name = "Yellow Trio";
        Text = "Summon three Yellows.";
        Cost = 2;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.SummonMinions(self, new List<Tuple<MinionType, Player>>() {
            new Tuple<MinionType, Player>(MinionType.Yellow, self),
            new Tuple<MinionType, Player>(MinionType.Yellow, self),
            new Tuple<MinionType, Player>(MinionType.Yellow, self),
        }, summonProtection: true);
    }
}
