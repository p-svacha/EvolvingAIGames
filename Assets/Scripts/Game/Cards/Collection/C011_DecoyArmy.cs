using System.Collections;
using System.Collections.Generic;
using System;

public class C011_DecoyArmy : Card
{

    public C011_DecoyArmy(int id) : base(id)
    {
        Name = "Decoy Army";
        Text = "Summon a Grey for each enemy minion.";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        List<Tuple<MinionType, Player>> summons = new List<Tuple<MinionType, Player>>();
        for (int i = 0; i < Model.NumMinions(enemy); i++)
        {
            summons.Add(new Tuple<MinionType, Player>(MinionType.Grey, self));
        }
        Model.SummonMultipleMinions(self, summons, summonProtection: true);
    }
}
