using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C001_DoNothing : Card
{

    public C001_DoNothing(MatchModel model) : base(model)
    {
        Name = "What";
        Text = "Do nothing";
    }

    public override void Action(Player self, Player enemy)
    {
    }
}
