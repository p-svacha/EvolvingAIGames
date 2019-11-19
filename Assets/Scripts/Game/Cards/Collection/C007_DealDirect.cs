using System.Collections;
using System.Collections.Generic;
using System;

public class C007_DealDirect : Card
{

    public C007_DealDirect(int id) : base(id)
    {
        Name = "Direct Action";
        Text = "Deal 10 damage to the enemy.";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.Damage(self, enemy, 10);
    }
}
