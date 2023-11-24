using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UpgradeClash
{
    public abstract class Player
    {
        public abstract string DisplayName { get; }
        protected Player Opponent;

        // Attributes
        public int MaxHealth { get; private set; }
        public int CurrentHealth { get; private set; }
        public float RelativeHealth => Mathf.Min((float)CurrentHealth / UCMatch.StartHealth, 1f); // used as neural net input
        public int Food { get; private set; }
        public int Wood { get; private set; }
        public int Gold { get; private set; }
        public int Stone { get; private set; }
        public float RelativeFood => (float)Food / UCMatch.ResourceCap; // used as neural net input
        public float RelativeWood => (float)Wood / UCMatch.ResourceCap; // used as neural net input
        public float RelativeGold => (float)Gold / UCMatch.ResourceCap; // used as neural net input
        public float RelativeStone => (float)Stone / UCMatch.ResourceCap; // used as neural net input

        // Upgrades (same references stored in 2 different data structures for performance).
        public Dictionary<UpgradeId, Upgrade> UpgradeDictionary = new Dictionary<UpgradeId, Upgrade>();
        public List<Upgrade> UpgradeList;

        public void Init(Player opponent)
        {
            Opponent = opponent;

            // Init health
            MaxHealth = UCMatch.StartHealth;
            CurrentHealth = UCMatch.StartHealth;
            Food = UCMatch.StartFood;
            Wood = UCMatch.StartWood;
            Gold = UCMatch.StartGold;
            Stone = UCMatch.StartStone;

            // Create and init all upgrades
            UpgradeList = new List<Upgrade>()
            {
                new U001_GoldIncome(),
                new U002_Damage()
            };

            foreach (Upgrade upgrade in UpgradeList)
            {
                upgrade.Init(this, opponent);
                UpgradeDictionary.Add(upgrade.Id, upgrade);
            }
        }

        public void Tick()
        {
            // Resource income
            AddFood(GetFoodIncome());
            AddWood(GetWoodIncome());
            AddGold(GetGoldIncome());
            AddStone(GetStoneIncome());

            // Update upgrade countdowns
            foreach (Upgrade upgrade in UpgradeList)
            {
                if(upgrade.IsInProgress)
                {
                    if (upgrade.RemainingDuration <= 1) upgrade.TakeEffect();
                    else upgrade.SetRemainingDuration(upgrade.RemainingDuration - 1);
                }
            }

            // Get player inputs to activate upgrades
            List<Upgrade> inputs = GetDesiredUpgrades();

            foreach(Upgrade upgrade in inputs)
            {
                if (CanActivateUpgrade(upgrade)) ActivateUpgrade(upgrade);
            }
        }

        /// <summary>
        /// Returns a list of all upgrades the player wants to activate this tick, regardless if possible.
        /// <br/> All upgrades will be checked by list index as priority and activated if possible.
        /// </summary>
        public abstract List<Upgrade> GetDesiredUpgrades();

        #region Actions

        /// <summary>
        /// Checks and returns if an upgrade can be activated.
        /// </summary>
        public bool CanActivateUpgrade(Upgrade upgrade)
        {
            if (upgrade.IsInEffect) return false; // Upgrade is unique and already active
            if (upgrade.IsInProgress) return false; // Upgrade is currently being activated
            if (Food < upgrade.FoodCost) return false; // Not enough food
            if (Wood < upgrade.WoodCost) return false; // Not enough wood
            if (Gold < upgrade.GoldCost) return false; // Not enough gold
            if (Stone < upgrade.StoneCost) return false; // Not enough stone
            return true;
        }

        public void ActivateUpgrade(Upgrade upgrade)
        {
            upgrade.SetRemainingDuration(upgrade.BaseDuration);
            ReduceFood(upgrade.FoodCost);
            ReduceWood(upgrade.WoodCost);
            ReduceGold(upgrade.GoldCost);
            ReduceStone(upgrade.StoneCost);
        }

        public void AddFood(int amount)
        {
            Food += amount;
            if (Food > UCMatch.ResourceCap) Food = UCMatch.ResourceCap;
        }
        public void ReduceFood(int amount)
        {
            if (amount > Food) throw new System.Exception("Can't remove food because it would reduce to negative value.");
            Food -= amount;
        }
        public void AddWood(int amount)
        {
            Wood += amount;
            if (Wood > UCMatch.ResourceCap) Wood = UCMatch.ResourceCap;
        }
        public void ReduceWood(int amount)
        {
            if (amount > Wood) throw new System.Exception("Can't remove wood because it would reduce to negative value.");
            Wood -= amount;
        }
        public void AddGold(int amount)
        {
            Gold += amount;
            if (Gold > UCMatch.ResourceCap) Gold = UCMatch.ResourceCap;
        }
        public void ReduceGold(int amount)
        {
            if (amount > Gold) throw new System.Exception("Can't remove gold because it would reduce to negative value.");
            Gold -= amount;
        }
        public void AddStone(int amount)
        {
            Stone += amount;
            if (Stone > UCMatch.ResourceCap) Stone = UCMatch.ResourceCap;
        }
        public void ReduceStone(int amount)
        {
            if (amount > Stone) throw new System.Exception("Can't remove stone because it would reduce to negative value.");
            Stone -= amount;
        }

        public void DealDamage(int amount)
        {
            CurrentHealth -= amount;
        }

        #endregion

        #region Attributes

        /// <summary>
        /// Food income per tick.
        /// </summary>
        public int GetFoodIncome()
        {
            int income = UCMatch.BaseFoodIncome;
            return income;
        }
        /// <summary>
        /// Wood income per tick.
        /// </summary>
        public int GetWoodIncome()
        {
            int income = UCMatch.BaseWoodIncome;
            return income;
        }
        /// <summary>
        /// Gold income per tick.
        /// </summary>
        public int GetGoldIncome()
        {
            int income = UCMatch.BaseGoldIncome;
            foreach (Upgrade upgrade in UpgradeList.Where(x => x.IsInEffect && x.PermanentEffect != null))
                income += upgrade.PermanentEffect.GoldIncomeBonus;
            return income;
        }
        /// <summary>
        /// Stone income per tick.
        /// </summary>
        public int GetStoneIncome()
        {
            int income = UCMatch.BaseStoneIncome;
            return income;
        }

        #endregion
    }
}
