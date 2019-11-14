using System.Collections;
using System.Collections.Generic;
using System;

public class C002_SummonYellow : Card
{

    public C002_SummonYellow(int id) : base(id)
    {
        Name = "Yellow Duo";
        Text = "Summon two Yellows.";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.SummonMultipleMinions(self, new List<Tuple<MinionType, Player>>() {
            new Tuple<MinionType, Player>(MinionType.Yellow, self),
            new Tuple<MinionType, Player>(MinionType.Yellow, self),
        });
    }
}
