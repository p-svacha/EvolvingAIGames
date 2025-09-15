using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace BlobCardGame
{
    // Creatures are possible sources for actions, includes Players and Minions.
    public abstract class Creature
    {
        public BlobCardMatch Match;
        public string Name;
        public PlayerBlob Enemy;
        public VisualEntity Visual;
        public Color Color;


        public Creature(BlobCardMatch match)
        {
            Match = match;
        }
    }
}
