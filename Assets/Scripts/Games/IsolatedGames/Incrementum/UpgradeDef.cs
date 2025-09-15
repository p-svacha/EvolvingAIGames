using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incrementum
{
    public class UpgradeDef : Def
    {
        public Dictionary<ResourceDef, int> Cost { get; init; } = new Dictionary<ResourceDef, int>();
        public List<UpgradeDef> Requirements { get; init; } = new List<UpgradeDef>();
        public Dictionary<ResourceDef, int> ResourceGenerationModifiers { get; init; } = new Dictionary<ResourceDef, int>();
    }
}
