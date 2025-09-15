using System.Collections;
using System.Collections.Generic;
using System;

namespace BlobCardGame
{
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

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            match.AddCardOption(self, self, 1);
        }
    }
}
