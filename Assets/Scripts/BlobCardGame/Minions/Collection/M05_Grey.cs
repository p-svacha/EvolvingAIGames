using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobCardGame
{
    public class M05_Grey : Minion
    {

        public M05_Grey(BlobCardMatch match, PlayerBlob owner, PlayerBlob enemy, int orderNum) : base(match, owner, enemy, orderNum)
        {
            Name = "Grey";
            Text = "Do nothing.";
            Type = MinionType.Grey;
            Color = new Color(0.8f, 0.8f, 0.8f);
        }

        public override void Action()
        {
        }
    }
}
