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
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.DestroyMultipleMinions(self, Model.AllMinionsOfType(enemy, MinionType.Green, withoutSummonProtection: true));
        Model.DestroyMultipleMinions(self, Model.AllMinionsOfType(enemy, MinionType.Blue, withoutSummonProtection: true));
    }
}
