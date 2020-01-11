using System.Collections;
using System.Collections.Generic;
using System;

public class C000_DoNothing : Card
{

    public C000_DoNothing()
    {
        Id = 0;
        Name = "What";
        Text = "Do nothing";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
    }
}
