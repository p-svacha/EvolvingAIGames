using System.Collections;
using System.Collections.Generic;
using System;

public class C008_DestroyBlue : Card
{

    public C008_DestroyBlue(int id) : base(id)
    {
        Name = "Anti Blue";
        Text = "Destroy all enemy blue minions.";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.DestroyMultipleMinions(self, Model.AllMinionsOfType(enemy, MinionType.Blue));
    }
}
