using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobCardGame
{
    public class HumanPlayer : PlayerBlob
    {
        public HumanPlayer(BlobCardMatch match) : base(match, "Player")
        {

        }

        public override void PickCard(List<Card> options)
        {

        }
    }
}
