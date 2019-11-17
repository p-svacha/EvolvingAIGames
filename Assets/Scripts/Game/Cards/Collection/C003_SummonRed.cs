using System.Collections;
using System.Collections.Generic;
using System;

public class C003_SummonRed : Card
{

    public C003_SummonRed(int id) : base(id)
    {
        Name = "Red Duo";
        Text = "Summon two Reds.";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.SummonMultipleMinions(self, new List<Tuple<MinionType, Player>>() {
            new Tuple<MinionType, Player>(MinionType.Red, self),
            new Tuple<MinionType, Player>(MinionType.Red, self),
        }, summonProtection: true);
    }
}
