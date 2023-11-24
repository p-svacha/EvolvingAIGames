using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobCardGame
{
    public abstract class Card
    {
        public int Id;
        public string Name;
        public string Text;
        public bool AlwaysAppears;
        public int Cost;

        public abstract void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy);
    }
}
