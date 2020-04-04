using System.Collections;
using System.Collections.Generic;
using System;

public class C017_TacticalNuke : Card
{

    public C017_TacticalNuke()
    {
        Id = 17;
        Name = "Tactical Nuke";
        Text = "Destroy all enemy minions. Deal 5 damage to the enemy.";
        Cost = 3;
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.DestroyMinions(self, Model.AllMinionsOfPlayer(enemy, withoutSummonProtection: true));
        Model.DealDamage(self, enemy, 5);
    }
}
