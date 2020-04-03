using System.Collections;
using System.Collections.Generic;
using System;

public class C010_DecoyArmy : Card
{

    public C010_DecoyArmy()
    {
        Id = 10;
        Name = "Decoy Army";
        Text = "Summon a Grey for each enemy minion.";
        Cost = 0;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        List<Tuple<MinionType, Player>> summons = new List<Tuple<MinionType, Player>>();
        for (int i = 0; i < Model.NumMinions(enemy, withoutSummonProtection: true); i++)
        {
            summons.Add(new Tuple<MinionType, Player>(MinionType.Grey, self));
        }
        Model.SummonMultipleMinions(self, summons, summonProtection: true);
    }
}
