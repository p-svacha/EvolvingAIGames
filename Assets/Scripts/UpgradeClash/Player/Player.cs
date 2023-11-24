using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public abstract class Player
    {
        public abstract string DisplayName { get; }

        // Attributes
        public int MaxHealth { get; private set; }
        public int CurrentHealth { get; private set; }
        public float RelativeHealth => Mathf.Min((float)CurrentHealth / UCMatch.StartHealth, 1f);
        public int Gold { get; private set; }

        // Upgrades
        public Dictionary<UpgradeId, Upgrade> Upgrades = new Dictionary<UpgradeId, Upgrade>();

        public void Init(Player opponent)
        {
            // Init health
            MaxHealth = UCMatch.StartHealth;
            CurrentHealth = UCMatch.StartHealth;

            // Create and init all upgrades
            List<Upgrade> allUpgrades = new List<Upgrade>()
            {
                new U001_GoldIncome(),
                new U002_Damage()
            };

            foreach (Upgrade upgrade in allUpgrades)
            {
                upgrade.Init(this, opponent);
                Upgrades.Add(upgrade.Id, upgrade);
            }
        }

        public void Tick()
        {
            // Update upgrades
            foreach(Upgrade upgrade in Upgrades.Values)
            {
                if(upgrade.IsInProgress)
                {
                    if(upgrade.RemainingDuration <= 1) // Upgrade takes effect
                    {

                    }
                }
            }

            // Get player inputs to activate upgrades
            List<Upgrade> inputs = GetInputs();

            foreach(Upgrade upgrade in inputs)
            {
                if (CanActivateUpgrade(upgrade)) ActivateUpgrade(upgrade);
            }
        }

        /// <summary>
        /// Returns a list of all upgrades the player wants to activate this tick, regardless if possible.
        /// <br/> All upgrades will be checked by list index as priority and activated if possible.
        /// </summary>
        public abstract List<Upgrade> GetInputs();

        #region Actions

        /// <summary>
        /// Checks and returns if an upgrade can be activated.
        /// </summary>
        public bool CanActivateUpgrade(Upgrade upgrade)
        {
            if (upgrade.IsInEffect) return false; // Upgrade is unique and already active
            if (upgrade.IsInProgress) return false; // Upgrade is currently being activated
            if (Gold < upgrade.GoldCost) return false; // Not enough gold
            return true;
        }

        public void ActivateUpgrade(Upgrade upgrade)
        {
            upgrade.SetRemainingDuration(upgrade.Duration);
            ReduceGold(upgrade.GoldCost);
        }

        public void ReduceGold(int amount)
        {
            Gold -= amount;
        }

        public void DealDamage(int amount)
        {
            CurrentHealth -= amount;
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Gold income per second.
        /// </summary>
        public int GetGoldIncome()
        {
            return UCMatch.BaseGoldIncome;
        }

        #endregion
    }
}
