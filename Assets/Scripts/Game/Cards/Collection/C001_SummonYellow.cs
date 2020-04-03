using System.Collections;
using System.Collections.Generic;
using System;

public class C001_SummonYellow : Card
{

    public C001_SummonYellow()
    {
        Id = 1;
        Name = "Yellow Duo";
        Text = "Summon two Yellows.";
        Cost = 1;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.SummonMultipleMinions(self, new List<Tuple<MinionType, Player>>() {
            new Tuple<MinionType, Player>(MinionType.Yellow, self),
            new Tuple<MinionType, Player>(MinionType.Yellow, self),
        }, summonProtection: true);
    }
}
