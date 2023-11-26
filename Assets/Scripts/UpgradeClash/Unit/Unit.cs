using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public abstract class Unit
    {
        public Player Self { get; private set; }
        public Player Opponent { get; private set; }
        public abstract UnitId Id { get; }
        public abstract string Name { get; }
        public int Amount { get; private set; }
        public abstract int MaxAmount { get; }
        public int LastActionTick;

        public Unit(Player player)
        {
            Self = player;
            Opponent = player.Opponent;
        }

        /// <summary>
        /// The action "OnTick" of this unit gets executed every x ticks depending on this cooldown.
        /// </summary>
        public abstract int GetCooldown();

        /// <summary>
        /// Action of this unit that gets executed.
        /// </summary>
        public abstract void OnTick();

        /// <summary>
        /// Adds 1 to this unit type.
        /// </summary>
        public void Add()
        {
            if (!IsAtMaxAmount) Amount += 1;
        }

        public bool IsAtMaxAmount => Amount >= MaxAmount;
        public int RemainingCooldown => GetCooldown() - (Self.Match.Ticks - LastActionTick);

    }
}
