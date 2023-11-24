using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    /*
    Upgrade neural net Input (1 for own, 1 for opponent)
    --------------------------------------------
    0 when generally possible to activate (disregarding resources and reqs)
    0-1 when activated and in progress
    1 when in effect (only for uninque techs)
    repeating techs ones will go from 0.999 back to 0 and will change some other stat (i.e.life)
    */
    public abstract class Upgrade
    {
        /// <summary>
        /// Unique identifier of this upgrade.
        /// </summary>
        public abstract UpgradeId Id {get;}

        /// <summary>
        /// Display name of the upgrade.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Food cost of the upgrade.
        /// </summary>
        public virtual int FoodCost => 0;

        /// <summary>
        /// Wood cost of the upgrade.
        /// </summary>
        public virtual int WoodCost => 0;

        /// <summary>
        /// Gold cost of the upgrade.
        /// </summary>
        public virtual int GoldCost => 0;

        /// <summary>
        /// Stone cost of the upgrade.
        /// </summary>
        public virtual int StoneCost => 0;

        /// <summary>
        /// How many ticks it takes for the upgrade to take effect.
        /// </summary>
        public abstract int BaseDuration { get; }

        /// <summary>
        /// If true, the upgrade can be clicked again after the duration.
        /// </summary>
        public abstract bool Repeatable { get; }


        /// <summary>
        /// A collection of permanent stat alterations that get active when a upgrade is in effect.
        /// </summary>
        public UpgradePermanentEffect PermanentEffect { get; private set; }

        /// <summary>
        /// Flag if the upgrade is in effect.
        /// </summary>
        public bool IsInEffect { get; private set; }

        /// <summary>
        /// How many ticks are left until the upgrade takes effect.
        /// </summary>
        public int RemainingDuration { get; private set; }

        /// <summary>
        /// Flag if the upgrade is currently being activated.
        /// </summary>
        public bool IsInProgress => RemainingDuration > 0;

        /// <summary>
        /// Reference to own player.
        /// </summary>
        public Player Self;

        /// <summary>
        /// Reference to opponent player.
        /// </summary>
        public Player Opponent;

        /// <summary>
        /// Gets executed at the start of a match.
        /// </summary>
        public void Init(Player self, Player opponent)
        {
            Self = self;
            Opponent = opponent;

            PermanentEffect = GetPermanentEffect();

            if (Repeatable && PermanentEffect != null) throw new System.Exception("Repeatable upgrades can't have permanent effects.");
        }

        /// <summary>
        /// Returns the unique and constant instance of this upgrade's permanent effect.
        /// </summary>
        protected virtual UpgradePermanentEffect GetPermanentEffect() => null;

        /// <summary>
        /// Sets the remaining ticks until the effect takes place.
        /// </summary>
        public void SetRemainingDuration(int numTicks)
        {
            RemainingDuration = numTicks;
        }

        /// <summary>
        /// Activate this upgrade, triggering the activation effect (OnActivate) and for unique (non-repeatable) upgrades also the permanent effect.
        /// </summary>
        public void TakeEffect()
        {
            RemainingDuration = 0;
            if (!Repeatable) IsInEffect = true;

            OnTakeEffect();
        }

        protected virtual void OnTakeEffect() { }

        private const int MaxDurationForInput = 120;
        /// <summary>
        /// Returns the neural network input value for this upgrade:
        /// 0 when generally possible to activate (disregarding resources and reqs)
        /// 0 when in progress and timer > MaxDurationForInput.
        /// 0.001 - 0.999 when in progress and timer <= MaxDurationForInput.
        /// 1 when in effect (only for uninque techs).
        /// Repeating techs ones will go from 0.999 back to 0 and will change some other stat (i.e.life).
        /// </summary>
        public float GetInputValue()
        {
            if (IsInEffect) return 1f;
            if (RemainingDuration > 0 && RemainingDuration <= MaxDurationForInput) 
                return (MaxDurationForInput - RemainingDuration) / (float)MaxDurationForInput;
            return 0f;
        }
    }
}
