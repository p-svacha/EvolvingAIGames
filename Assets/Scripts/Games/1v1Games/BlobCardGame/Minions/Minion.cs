using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobCardGame
{
    public abstract class Minion : Creature
    {
        public string Text;
        public int OrderNum;
        public PlayerBlob Owner { get; private set; }
        public MinionType Type;
        public bool Destabilized;
        public bool HasSummonProtection { get; private set; }

        public Minion(BlobCardMatch match, PlayerBlob owner, PlayerBlob enemy, int orderNum) : base(match)
        {
            Owner = owner;
            Enemy = enemy;
            OrderNum = orderNum;
        }

        public void SetSummonProtection(BlobCardMatch match, bool protection)
        {
            HasSummonProtection = protection;
            if (match.IsVisual) ((VisualMinion)Visual).SetHaloEnabled(protection);
        }

        public void SetOwner(PlayerBlob player)
        {
            Owner = player;
            Enemy = player.Enemy;
        }

        public abstract void Action();
    }
}
