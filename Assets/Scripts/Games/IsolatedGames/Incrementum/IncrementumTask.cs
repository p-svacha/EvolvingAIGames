using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Incrementum
{
    public class IncrementumTask : Task
    {
        public Dictionary<ResourceDef, int> Resources;
        public Dictionary<UpgradeDef, bool> AcquiredUpgrades;

        /// <summary>
        /// Containing data storing the time each upgrade was acquired.
        /// </summary>
        private Dictionary<int, UpgradeDef> History;

        public IncrementumTask(Subject s) : base(s)
        {
            History = new Dictionary<int, UpgradeDef>();

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

            // Check for end
            if (TickNumber == Incrementum.NUM_TICKS)
            {
                End();
                return;
            }
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
                inputs.Add(Mathf.Min(1f, Mathf.Log(1f + Resources[res]) / Mathf.Log(res.ApproxAmountCap + 1f)));
            }

            // Resource income
            foreach (ResourceDef res in DefDatabase<ResourceDef>.AllDefs)
            {
                inputs.Add(Mathf.Min(1f, (float)GetResourceIncomePerTick(res) / res.ApproxIncomeCap));
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

            if (!AcquiredUpgrades.ContainsKey(upgrade)) Debug.Log(AcquiredUpgrades.Keys.ToList().DebugList());
            if (upgrade == null) Debug.Log(AcquiredUpgrades.Keys.ToList().DebugList());
            if (upgrade.Requirements == null) Debug.Log(AcquiredUpgrades.Keys.ToList().DebugList());
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

            History.Add(TickNumber, def);
        }




        public string GetHistoryLog()
        {
            // Deterministic ordering for readability
            var resources = DefDatabase<ResourceDef>.AllDefs.OrderBy(r => r.DefName).ToList();
            var upgrades = DefDatabase<UpgradeDef>.AllDefs.OrderBy(u => u.DefName).ToList();

            int totalUpgrades = upgrades.Count;
            int acquiredCount = AcquiredUpgrades.Count(kv => kv.Value);
            float acquiredPct = totalUpgrades > 0 ? (100f * acquiredCount) / totalUpgrades : 0f;

            var sb = new StringBuilder();
            sb.AppendLine("=== INCREMENTUM RUN =====================================");
            sb.AppendLine($"Subject: {Subject?.Name ?? "(unnamed)"}");
            sb.AppendLine($"Ticks:   {TickNumber}/{Incrementum.NUM_TICKS}");

            // Final resources
            sb.Append("Final Resources: ");
            sb.AppendLine(string.Join(", ", resources.Select(r => $"{r.Label}={Resources[r]}")));

            // Current income rates
            sb.Append("Rates / tick:   ");
            sb.AppendLine(string.Join(", ", resources.Select(r => $"{r.Label}={GetResourceIncomePerTick(r)}")));

            // Fitness (your current fitness is final Gold)
            sb.AppendLine($"Fitness (Gold): {Resources[ResourceDefOf.Gold]}");

            // Acquisition summary
            sb.AppendLine($"Upgrades acquired: {acquiredCount}/{totalUpgrades} ({acquiredPct:0.0}%)");

            // Timeline
            sb.AppendLine("Purchases:");
            if (History.Count == 0)
            {
                sb.AppendLine("  (none)");
            }
            else
            {
                foreach (var kv in History.OrderBy(k => k.Key)) // k.Key = tick
                {
                    var def = kv.Value;
                    string cost = def.Cost != null && def.Cost.Count > 0
                        ? string.Join(", ", def.Cost.Select(c => $"{c.Value} {c.Key.Label}"))
                        : "–";

                    string effects = def.ResourceGenerationModifiers != null && def.ResourceGenerationModifiers.Count > 0
                        ? string.Join(", ", def.ResourceGenerationModifiers.Select(m => $"{(m.Value >= 0 ? "+" : "")}{m.Value} {m.Key.Label}/t"))
                        : "–";

                    sb.AppendLine($"  t={kv.Key,4}: {def.Label,-16} | cost: {cost,-18} | effect: {effects}");
                }
            }

            sb.AppendLine("=========================================================");

            return sb.ToString();
        }
    }
}
