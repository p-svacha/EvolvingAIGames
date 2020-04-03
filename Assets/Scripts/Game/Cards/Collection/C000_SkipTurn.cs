using System.Collections;
using System.Collections.Generic;
using System;

public class C000_SkipTurn : Card
{

    public C000_SkipTurn()
    {
        Id = 0;
        Name = "Skip Turn";
        Text = "Do nothing. Save money.";
        Cost = 0;
        AlwaysAppears = true;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
    }
}
