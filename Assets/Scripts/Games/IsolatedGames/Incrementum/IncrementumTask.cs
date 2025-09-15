using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Incrementum
{
    public class IncrementumTask : Task
    {
        public Dictionary<ResourceDef, int> Resources;
        public Dictionary<UpgradeDef, bool> AcquiredUpgrades;

        public IncrementumTask(Subject s) : base(s)
        {
            // Init resources
            Resources = new Dictionary<ResourceDef, int>();
            foreach (ResourceDef def in DefDatabase<ResourceDef>.AllDefs) Resources.Add(def, 0);

            Resources[ResourceDefOf.Wood] += Incrementum.STARTING_WOOD_AMOUNT;
            Resources[ResourceDefOf.Stone] += Incrementum.STARTING_STONE_AMOUNT;

            // Init upgrades
            AcquiredUpgrades = new Dictionary<UpgradeDef, bool>();
            foreach (UpgradeDef def in DefDatabase<UpgradeDef>.AllDefs) AcquiredUpgrades.Add(def, false);
        }

        protected override int CalculateFitnessValue()
        {
            return Resources[ResourceDefOf.Gold];
        }

        protected override void OnEnd()
        {
            
        }

        protected override void OnStart()
        {
            
        }

        protected override void OnTick()
        {
            // Check for end
            if (TickNumber == Incrementum.NUM_TICKS)
            {
                End();
                return;
            }

            // Income phase
            foreach (ResourceDef res in DefDatabase<ResourceDef>.AllDefs)
            {
                Resources[res] += GetResourceIncomePerTick(res);
            }

            // Decision phase
            float[] inputs = GetInputs();
            float[] outputs = Subject.Genome.FeedForward(inputs);
            UpgradeDef upgradeToAcquire = GetUpgradeToAcquire(outputs);

            // Purchase phase
            if (upgradeToAcquire != null) AcquireUpgrade(upgradeToAcquire);
        }

        /// <summary>
        /// Aggregates all inputs for the current tick.
        /// </summary>
        /// <returns></returns>
        public float[] GetInputs()
        {
            List<float> inputs = new List<float>();

            // Resource amounts
            foreach (ResourceDef res in DefDatabase<ResourceDef>.AllDefs)
            {
                inputs.Add(Mathf.Log(1f + Resources[res]) / Mathf.Log(res.ApproxAmountCap + 1f));
            }

            // Resource income
            foreach (ResourceDef res in DefDatabase<ResourceDef>.AllDefs)
            {
                inputs.Add((float)GetResourceIncomePerTick(res) / res.ApproxIncomeCap);
            }

            // Time
            inputs.Add((float)TickNumber / Incrementum.NUM_TICKS);

            // Acquired upgrades
            foreach (bool b in AcquiredUpgrades.Values) inputs.Add(b ? 1f : 0f);

            // Upgrade affordability
            foreach (UpgradeDef def in DefDatabase<UpgradeDef>.AllDefs) inputs.Add(CanAcquireUpgrade(def) ? 1f : 0f);

            // Bias
            inputs.Add(1f);

            return inputs.ToArray();
        }

        /// <summary>
        /// Returns the upgrade that will be acquired this tick.
        /// Performs all necessary checks.
        /// May return null.
        /// </summary>
        private UpgradeDef GetUpgradeToAcquire(float[] outputs)
        {
            List<UpgradeDef> upgrades = DefDatabase<UpgradeDef>.AllDefs;
            UpgradeDef toBuy = null;
            float toBuyValue = 0f;
            for (int i = 0; i < outputs.Length; i++)
            {
                if (outputs[i] > Incrementum.UPGRADE_ACQUIRE_THRESHOLD)
                {
                    UpgradeDef upgrade = upgrades[i];
                    if (CanAcquireUpgrade(upgrade))
                    {
                        if (toBuy == null || outputs[i] > toBuyValue)
                        {
                            toBuy = upgrade;
                            toBuyValue = outputs[i];
                        }
                    }
                }
            }
            return toBuy;
        }

        private int GetResourceIncomePerTick(ResourceDef def)
        {
            int value = def.BaseIncomePerTick;

            foreach (UpgradeDef upgrade in AcquiredUpgrades.Where(x => x.Value).Select(x => x.Key))
            {
                if(upgrade.ResourceGenerationModifiers.ContainsKey(def))
                {
                    value += upgrade.ResourceGenerationModifiers[def];
                }
            }

            return value;
        }

        /// <summary>
        /// Checks and returns if all requirements would be met to acquire an upgrade in the current state.
        /// </summary>
        private bool CanAcquireUpgrade(UpgradeDef upgrade)
        {
            if (AcquiredUpgrades[upgrade]) return false;
            if (upgrade.Cost.Any(x => Resources[x.Key] < x.Value)) return false;
            if (upgrade.Requirements.Any(x => !AcquiredUpgrades[x])) return false;

            return true;
        }

        /// <summary>
        /// Acquires an upgrade instantly and pays the resource cost.
        /// </summary>
        private void AcquireUpgrade(UpgradeDef def)
        {
            AcquiredUpgrades[def] = true;
            foreach(var r in def.Cost)
            {
                Resources[r.Key] -= r.Value;
            }
        }
    }
}
