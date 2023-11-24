using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
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

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy) { }
    }
}
