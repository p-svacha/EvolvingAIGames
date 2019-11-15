using System.Collections;
using System.Collections.Generic;
using System;

public class C009_DestroyGreen : Card
{

    public C009_DestroyGreen(int id) : base(id)
    {
        Name = "Anti Green";
        Text = "Destroy all enemy green minions.";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.DestroyMultipleMinions(self, Model.AllMinionsOfType(enemy, MinionType.Green));
    }
}
