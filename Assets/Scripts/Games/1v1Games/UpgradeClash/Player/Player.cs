using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UpgradeClash
{
    public abstract class Player
    {
        public abstract string DisplayName { get; }
        public UCMatch Match { get; private set; }
        public Player Opponent { get; private set; }

        // Attributes
        public int MaxHealth { get; private set; }
        public int CurrentHealth { get; private set; }
        
        public int Food { get; private set; }
        public int Wood { get; private set; }
        public int Gold { get; private set; }
        public int Stone { get; private set; }

        public Dictionary<UnitId, Unit> Units;


        // Neural net inputs
        public float RelativeHealth => (float)CurrentHealth / UCMatch.StartHealth;
        public float RelativeFood => (float)Food / UCMatch.ResourceCap;
        public float RelativeWood => (float)Wood / UCMatch.ResourceCap;
        public float RelativeGold => (float)Gold / UCMatch.ResourceCap;
        public float RelativeStone => (float)Stone / UCMatch.ResourceCap;

        // Upgrades (same references stored in 2 different data structures for performance).
        public Dictionary<UpgradeId, Upgrade> UpgradeDictionary = new Dictionary<UpgradeId, Upgrade>();
        public List<Upgrade> UpgradeList;

        /// <summary>
        /// Dictionary that maps which upgrade defines if a building exists.
        /// </summary>
        public Dictionary<BuildingId, Upgrade> BuildingUpgrades;

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

            // Init units
            Units = new Dictionary<UnitId, Unit>()
            {
                { UnitId.FoodWorker,  new FoodWorker(this) },
                { UnitId.WoodWorker,  new WoodWorker(this) },
                { UnitId.GoldWorker,  new GoldWorker(this) },
                { UnitId.StoneWorker,  new StoneWorker(this) },
                { UnitId.Militia,  new Militia(this) },
            };

            // Get starting units
            // AddFoodWorker();

            // Create and init all upgrades
            UpgradeList = new List<Upgrade>()
            {
                new U001_TrainFoodWorker(),
                new U003_TrainWoodWorker(),
                new U004_TrainGoldWorker(),
                //new U005_TrainStoneWorker(),
                new U006_TrainMilitia(),
                new U002_BuildPalisade(),
                new U007_BuildBarracks(),
                new U008_Forging(),
                new U009_BuildMill()
            };

            foreach (Upgrade upgrade in UpgradeList)
            {
                upgrade.Init(this, opponent);
                UpgradeDictionary.Add(upgrade.Id, upgrade);
            }

            // Init building upgrade map
            BuildingUpgrades = new Dictionary<BuildingId, Upgrade>()
            {
                { BuildingId.Barracks, UpgradeDictionary[UpgradeId.BuildBarracks] },
                { BuildingId.Palisade, UpgradeDictionary[UpgradeId.BuildPalisade] },
                { BuildingId.Mill, UpgradeDictionary[UpgradeId.BuildMill] },
            };
        }

        public void Tick()
        {
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

            // Units
            foreach(Unit unit in Units.Values)
            {
                if (unit.Amount > 0 && unit.RemainingCooldown <= 0)
                {
                    unit.LastActionTick = Match.Ticks;
                    unit.OnTick();
                }
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
            if (!upgrade.CanActivate()) return false; // Upgrade-specific requirements not met
            if (upgrade.Building != BuildingId.Base && !HasBuilding(upgrade.Building)) return false; // Upgrade building is not yet built
            foreach (Upgrade u in UpgradeList.Where(x => x.Building == upgrade.Building))
                if (u.IsInProgress) return false; // Another upgrade is in progress of the same building
            if (Food < upgrade.FoodCost) return false; // Not enough food
            if (Wood < upgrade.WoodCost) return false; // Not enough wood
            if (Gold < upgrade.GoldCost) return false; // Not enough gold
            if (Stone < upgrade.StoneCost) return false; // Not enough stone
            return true;
        }

        public void ActivateUpgrade(Upgrade upgrade)
        {
            upgrade.SetRemainingDuration(upgrade.GetDuration());
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

        public void AddFoodWorker() => Units[UnitId.FoodWorker].Add();
        public void AddWoodWorker() => Units[UnitId.WoodWorker].Add();
        public void AddGoldWorker() => Units[UnitId.GoldWorker].Add();
        public void AddStoneWorker() => Units[UnitId.StoneWorker].Add();
        public void AddMilitia() => Units[UnitId.Militia].Add();

        public void DealDamage(int amount)
        {
            CurrentHealth -= amount;
        }

        #endregion

        public int TotalUnitAmount => Units.Sum(x => x.Value.Amount);
        public int TotalResources => Food + Wood + Gold + Stone;
        public bool HasBuilding(BuildingId id) => BuildingUpgrades[id].IsInEffect;
    }
}
