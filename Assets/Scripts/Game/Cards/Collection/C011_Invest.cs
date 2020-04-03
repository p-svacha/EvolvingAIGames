using System.Collections;
using System.Collections.Generic;
using System;

public class C011_Invest : Card
{

    public C011_Invest()
    {
        Id = 11;
        Name = "Invest";
        Text = "Increase your future card options by 1.";
        Cost = 1;
        AlwaysAppears = true;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.AddCardOption(self, self, 1);
    }
}
