using System.Collections;
using System.Collections.Generic;
using System;

public class C001_DoNothing : Card
{

    public C001_DoNothing(int id) : base(id)
    {
        Name = "What";
        Text = "Do nothing";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
    }
}
