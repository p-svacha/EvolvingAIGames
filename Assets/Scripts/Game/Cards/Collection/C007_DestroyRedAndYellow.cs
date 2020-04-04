using System.Collections;
using System.Collections.Generic;
using System;

public class C007_DestroyRedAndYellow : Card
{

    public C007_DestroyRedAndYellow()
    {
        Id = 7;
        Name = "Anti-Rellow";
        Text = "Destroy all enemy Red and Yellow minions.";
        Cost = 1;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        List<Minion> toDestroy = Model.AllMinionsOfType(enemy, MinionType.Red, withoutSummonProtection: true);
        toDestroy.AddRange(Model.AllMinionsOfType(enemy, MinionType.Yellow, withoutSummonProtection: true));
        Model.DestroyMinions(self, toDestroy);
    }
}
