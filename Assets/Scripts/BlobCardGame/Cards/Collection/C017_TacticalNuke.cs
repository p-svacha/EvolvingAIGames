using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C017_TacticalNuke : Card
    {

        public C017_TacticalNuke()
        {
            Id = 17;
            Name = "Tactical Nuke";
            Text = "Destroy all enemy minions. Deal 5 damage to the enemy.";
            Cost = 3;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            match.DestroyMinions(self, match.AllMinionsOfPlayer(enemy, withoutSummonProtection: true));
            match.DealDamage(self, enemy, 5);
        }
    }
}
