using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incrementum
{
    public static class UpgradeDefs
    {
        public static List<UpgradeDef> Defs => new List<UpgradeDef>()
        {

            // --------------- Lumberyard ---------------
            new UpgradeDef()
            {
                DefName = "LumberYard_I",
                Label = "Lumberyard I",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Stone, 40 },
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "LumberYard_II",
                Label = "Lumberyard II",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Stone, 120 },
                },
                Requirements = new List<UpgradeDef>()
                {
                    UpgradeDefOf.LumberYard_I,
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "LumberYard_III",
                Label = "Lumberyard III",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Stone, 360 },
                },
                Requirements = new List<UpgradeDef>()
                {
                    UpgradeDefOf.LumberYard_II,
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 2 },
                }
            },

            // --------------- Quarry ---------------
            new UpgradeDef()
            {
                DefName = "Quarry_I",
                Label = "Quarry I",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 40 },
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Stone, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "Quarry_II",
                Label = "Quarry II",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 120 },
                },
                Requirements = new List<UpgradeDef>()
                {
                    UpgradeDefOf.Quarry_I,
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Stone, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "Quarry_III",
                Label = "Quarry III",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 360 },
                },
                Requirements = new List<UpgradeDef>()
                {
                    UpgradeDefOf.Quarry_II,
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Stone, 2 },
                }
            },

            // --------------- Town Hall ---------------
            new UpgradeDef()
            {
                DefName = "TownHall_I",
                Label = "TownHall I",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 60 },
                    { ResourceDefOf.Stone, 60 },
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "TownHall_II",
                Label = "TownHall II",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 200 },
                    { ResourceDefOf.Stone, 200 },
                },
                Requirements = new List<UpgradeDef>()
                {
                    UpgradeDefOf.TownHall_I,
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold, 2 },
                }
            },
            new UpgradeDef()
            {
                DefName = "TownHall_III",
                Label = "TownHall III",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 600 },
                    { ResourceDefOf.Stone, 600 },
                },
                Requirements = new List<UpgradeDef>()
                {
                    UpgradeDefOf.TownHall_II,
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold, 4 },
                }
            },
        };
    }
}
