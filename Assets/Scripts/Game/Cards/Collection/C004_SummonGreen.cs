using System.Collections;
using System.Collections.Generic;
using System;

public class C004_GreenBlood : Card
{

    public C004_GreenBlood()
    {
        Id = 4;
        Name = "Green Blood";
        Text = "Summon a Green. Deal 5 damage to yourself.";
    }

    public override void Action(Match Model, Player self, Player enemy)
    {
        Model.SummonMinion(self, MinionType.Green, self, summonProtection: true);
        Model.Damage(self, self, 5);
    }
}
