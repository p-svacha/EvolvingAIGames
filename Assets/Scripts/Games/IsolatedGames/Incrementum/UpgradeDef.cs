using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Incrementum
{
    public class UpgradeDef : Def
    {
        public Dictionary<ResourceDef, int> Cost { get; init; } = new Dictionary<ResourceDef, int>();
        public List<UpgradeDef> Requirements { get; private set; }
        public List<string> RequirementDefNames { get; init; } = new List<string>();
        public Dictionary<ResourceDef, int> ResourceGenerationModifiers { get; init; } = new Dictionary<ResourceDef, int>();

        public override void ResolveReferences()
        {
            Requirements = new List<UpgradeDef>();
            foreach (var name in RequirementDefNames)
                Requirements.Add(DefDatabase<UpgradeDef>.GetNamed(name));
        }

        public override void OnLoadingDefsDone()
        {
            if (Requirements.Any(r => r == null)) throw new System.Exception($"UpgradeDef {DefName} has a requirement that is null.");
        }

        public string GetCostString()
        {
            return Cost != null && Cost.Count > 0 ? string.Join(", ", Cost.Select(c => $"{c.Value} {c.Key.Label}")) : "–";
        }

        public string GetEffectString()
        {
            return ResourceGenerationModifiers != null && ResourceGenerationModifiers.Count > 0 ? string.Join(", ", ResourceGenerationModifiers.Select(m => $"{(m.Value >= 0 ? "+" : "")}{m.Value} {m.Key.Label}/t")) : "–";
        }
    }
}
