using System.Collections;
using System.Collections.Generic;
using System;

public class C006_DirectAction : Card
{
    public C006_DirectAction()
    {
        Id = 6;
        Name = "Direct Action";
        Text = "Deal 7 damage to the enemy.";
        Cost = 1;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.DealDamage(self, enemy, 7);
    }
}
