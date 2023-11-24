using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
    public class C006_DirectAction : Card
    {
        public C006_DirectAction()
        {
            Id = 6;
            Name = "Direct Action";
            Text = "Deal 7 damage to the enemy.";
            Cost = 1;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            match.DealDamage(self, enemy, 7);
        }
    }
}
