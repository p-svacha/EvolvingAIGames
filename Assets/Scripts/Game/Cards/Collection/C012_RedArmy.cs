using System.Collections;
using System.Collections.Generic;
using System;

public class C012_RedArmy : Card
{

    public C012_RedArmy()
    {
        Id = 12;
        Name = "Red Army";
        Text = "Summon 3 Reds.";
        Cost = 3;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.SummonMultipleMinions(self, new List<Tuple<MinionType, Player>>() {
            new Tuple<MinionType, Player>(MinionType.Red, self),
            new Tuple<MinionType, Player>(MinionType.Red, self),
            new Tuple<MinionType, Player>(MinionType.Red, self),
        }, summonProtection: true);
    }
}
