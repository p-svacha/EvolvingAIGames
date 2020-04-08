using System.Collections;
using System.Collections.Generic;
using System;

public class C016_Rainbow : Card
{

    public C016_Rainbow()
    {
        Id = 16;
        Name = "Rainbow";
        Text = "Summon three random minions from a different type.";
        Cost = 3;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        List<MinionType> types = Model.RandomMinionTypes(3);

        Model.SummonMinions(self, new List<Tuple<MinionType, Player>>() {
            new Tuple<MinionType, Player>(types[0], self),
            new Tuple<MinionType, Player>(types[1], self),
            new Tuple<MinionType, Player>(types[2], self),
        }, summonProtection: true);
    }
}
