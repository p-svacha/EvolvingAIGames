using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UpgradeClash
{
    public abstract class Player
    {
        public abstract string DisplayName { get; }
        protected UCMatch Match;
        protected Player Opponent;

        // Attributes
        public int MaxHealth { get; private set; }
        public int CurrentHealth { get; private set; }
        
        public int Food { get; private set; }
        public int Wood { get; private set; }
        public int Gold { get; private set; }
        public int Stone { get; private set; }

        public int FoodWorkers { get; private set; }
        public int WoodWorkers { get; private set; }
        public int GoldWorkers { get; private set; }
        public int StoneWorkers { get; private set; }

        public int Militia { get; private set; }
        private int LastMilitiaAttackTick;



        // Neural net inputs
        public float RelativeHealth => (float)CurrentHealth / UCMatch.StartHealth;
        public float RelativeFood => (float)Food / UCMatch.ResourceCap;
        public float RelativeWood => (float)Wood / UCMatch.ResourceCap;
        public float RelativeGold => (float)Gold / UCMatch.ResourceCap;
        public float RelativeStone => (float)Stone / UCMatch.ResourceCap;

        public float RelativeFoodWorkers => (float)FoodWorkers / UCMatch.WorkerCap;
        public float RelativeWoodWorkers => (float)WoodWorkers / UCMatch.WorkerCap;
        public float RelativeGoldWorkers => (float)GoldWorkers / UCMatch.WorkerCap;
        public float RelativeStoneWorkers => (float)StoneWorkers / UCMatch.WorkerCap;

        public float RelativeMilitia => (float)Militia / UCMatch.ArmyCap;

        // Upgrades (same references stored in 2 different data structures for performance).
        public Dictionary<UpgradeId, Upgrade> UpgradeDictionary = new Dictionary<UpgradeId, Upgrade>();
        public List<Upgrade> UpgradeList;

        public void Init(UCMatch match, Player opponent)
        {
            Match = match;
            Opponent = opponent;

            // Init health
            MaxHealth = UCMatch.StartHealth;
            CurrentHealth = UCMatch.StartHealth;

            // Init resources
            Food = UCMatch.StartFood;
            Wood = UCMatch.StartWood;
            Gold = UCMatch.StartGold;
            Stone = UCMatch.StartStone;

            FoodWorkers = 0;
            WoodWorkers = 0;
            GoldWorkers = 0;
            StoneWorkers = 0;

            // Create and init all upgrades
            UpgradeList = new List<Upgrade>()
            {
                new U001_TrainFoodWorker(),
                //new U002_Damage(),
                new U003_TrainWoodWorker(),
                new U004_TrainGoldWorker(),
                new U005_TrainStoneWorker(),
                new U006_TrainMilitia(),
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

            // Militia
            if(Match.Ticks - LastMilitiaAttackTick > GetMilitiaAttackSpeed())
            {
                LastMilitiaAttackTick = Match.Ticks;
                int damage = Militia * GetMilitiaAttackDamage();
                Opponent.DealDamage(damage);
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
            if (!upgrade.CanActivate()) return false; // Requirements not met
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

        public void AddFoodWorker()
        {
            if(FoodWorkers < UCMatch.WorkerCap) FoodWorkers += 1;
        }
        public void AddWoodWorker()
        {
            if (WoodWorkers < UCMatch.WorkerCap) WoodWorkers += 1;
        }
        public void AddGoldWorker()
        {
            if (GoldWorkers < UCMatch.WorkerCap) GoldWorkers += 1;
        }
        public void AddStoneWorker()
        {
            if (StoneWorkers < UCMatch.WorkerCap) StoneWorkers += 1;
        }

        public void AddMilitia()
        {
            if (Militia < UCMatch.ArmyCap) Militia += 1;
        }

        public void DealDamage(int amount)
        {
            CurrentHealth -= amount;
        }

        #endregion

        #region Attributes

        public bool IsWorkerInProgress()
        {
            if (UpgradeDictionary[UpgradeId.TrainFoodWorker].IsInProgress) return true;
            if (UpgradeDictionary[UpgradeId.TrainWoodWorker].IsInProgress) return true;
            if (UpgradeDictionary[UpgradeId.TrainGoldWorker].IsInProgress) return true;
            if (UpgradeDictionary[UpgradeId.TrainStoneWorker].IsInProgress) return true;
            return false;
        }

        /// <summary>
        /// Food income per tick.
        /// </summary>
        public int GetFoodIncome()
        {
            int income = FoodWorkers;
            return income;
        }
        /// <summary>
        /// Wood income per tick.
        /// </summary>
        public int GetWoodIncome()
        {
            int income = WoodWorkers;
            return income;
        }
        /// <summary>
        /// Gold income per tick.
        /// </summary>
        public int GetGoldIncome()
        {
            int income = GoldWorkers;
            return income;
        }
        /// <summary>
        /// Stone income per tick.
        /// </summary>
        public int GetStoneIncome()
        {
            int income = StoneWorkers;
            return income;
        }

        private int GetMilitiaAttackSpeed()
        {
            int cooldown = UCMatch.MilitiaBaseAttackCooldown;
            return cooldown;
        }
        private int GetMilitiaAttackDamage()
        {
            int damage = UCMatch.MilitiaBaseAttackDamage;
            return damage;
        }

        #endregion
    }
}
