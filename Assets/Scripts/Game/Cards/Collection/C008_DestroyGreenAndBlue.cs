using System.Collections;
using System.Collections.Generic;
using System;

public class C008_DestroyGreenAndBlue : Card
{

    public C008_DestroyGreenAndBlue()
    {
        Id = 8;
        Name = "Anti-Glue";
        Text = "Destroy all enemy Green and Blue minions.";
        Cost = 1;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        List<Minion> toDestroy = Model.AllMinionsOfType(enemy, MinionType.Blue, withoutSummonProtection: true);
        toDestroy.AddRange(Model.AllMinionsOfType(enemy, MinionType.Green, withoutSummonProtection: true));
        Model.DestroyMultipleMinions(self, toDestroy);
    }
}
