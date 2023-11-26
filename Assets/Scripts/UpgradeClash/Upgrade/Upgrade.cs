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
        }

        /// <summary>
        /// Sets the remaining ticks until the effect takes place.
        /// </summary>
        public void SetRemainingDuration(int numTicks)
        {
            RemainingDuration = numTicks;
        }

        /// <summary>
        /// Activate this upgrade, triggering the activation effect "OnTakeEffect".
        /// </summary>
        public void TakeEffect()
        {
            RemainingDuration = 0;
            if (!Repeatable) IsInEffect = true;

            OnTakeEffect();
        }

        public virtual bool CanActivate() => true;
        protected virtual void OnTakeEffect() { }

        protected const int MaxDurationForInput = 60;
        protected const float InProgressStartValue = 0.2f;
        protected const float InProgressStopValue = 0.8f;

        /// <summary>
        /// Returns the neural network input value for this upgrade:
        /// <br/> 0 when generally possible to activate (disregarding resources and reqs)
        /// <br/> 0 when in progress and timer > MaxDurationForInput.
        /// <br/> InProgressStartValue - InProgressStopValue when in progress and timer <= MaxDurationForInput.
        /// <br/> 1 when in effect (only for uninque techs).
        /// <br/> Repeating techs ones will go from InProgressStopValue back to 0 and will change some other stat (i.e.life).
        /// <br/> Can be overriden by specific upgrades.
        /// </summary>
        public virtual float GetInputValue()
        {
            if (IsInEffect) return 1f;
            if (IsInProgress && RemainingDuration <= MaxDurationForInput) 
                return InProgressStartValue + (((MaxDurationForInput - RemainingDuration) / (float)MaxDurationForInput) * (InProgressStopValue - InProgressStartValue));
            return 0f;
        }

        /// <summary>
        /// Returns the neural network input value for "Train x Worker" upgrades.
        /// <br/> It's the relative amount of units compared to the max amount.
        /// <br/> If upgrade is in progress a small value is added based on progress.
        /// </summary>
        protected float GetUnitInputValue(Unit unit)
        {
            float value = (float)unit.Amount / unit.MaxAmount; // Base value is relative amount of units.
            if (IsInProgress)
            {
                float progressRatio = 1f - ((float)RemainingDuration / GetDuration()); // 0-1 how much progress the current activation has
                progressRatio = InProgressStartValue + (progressRatio * InProgressStopValue); // Clamp it to a smaller range (i.e. 0.2 - 0.8)
                progressRatio *= (1f / unit.MaxAmount); // Adjust it to the max amount of units so the progress effect is always smaller than the unit amount
                value += progressRatio;
            }
            return value;
        }

        public virtual int GetDuration() => BaseDuration;
    }
}
