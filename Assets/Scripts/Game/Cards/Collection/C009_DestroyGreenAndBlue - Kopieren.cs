using System.Collections;
using System.Collections.Generic;
using System;

public class C009_DestroyGreenAndBlue : Card
{

    public C009_DestroyGreenAndBlue(int id) : base(id)
    {
        Name = "Anti-Glue";
        Text = "Destroy all enemy Green and Blue minions.";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.DestroyMultipleMinions(self, Model.AllMinionsOfType(enemy, MinionType.Green, withoutSummonProtection: true));
        Model.DestroyMultipleMinions(self, Model.AllMinionsOfType(enemy, MinionType.Blue, withoutSummonProtection: true));
    }
}
