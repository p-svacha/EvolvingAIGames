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
                Label = "Lumber Yard I",
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
                Label = "Lumber Yard II",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Stone, 120 },
                },
                RequirementDefNames = new List<string>()
                {
                    "LumberYard_I",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "LumberYard_III",
                Label = "Lumber Yard III",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Stone, 260 },
                },
                RequirementDefNames = new List<string>()
                {
                    "LumberYard_II",
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
                RequirementDefNames = new List<string>()
                {
                    "Quarry_I",
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
                    { ResourceDefOf.Wood, 260 },
                },
                RequirementDefNames = new List<string>()
                {
                    "Quarry_II",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Stone, 2 },
                }
            },

            // --------------- Workshop ---------------
            new UpgradeDef()
            {
                DefName = "Workshop",
                Label = "Workshop",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 200 },
                    { ResourceDefOf.Stone, 200 },
                },
                RequirementDefNames = new List<string>()
                {
                    "LumberYard_I",
                    "Quarry_I",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 2 },
                    { ResourceDefOf.Stone, 2 },
                }
            },

            // --------------- Farm ---------------
            new UpgradeDef()
            {
                DefName = "Farm_I",
                Label = "Farm I",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 120 },
                    { ResourceDefOf.Stone, 40 },
                },
                RequirementDefNames = new List<string>() { },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Food, 2 },
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
                    { ResourceDefOf.Food, 80 },
                },
                RequirementDefNames = new List<string>()
                {
                    "TownHall_I",
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
                    { ResourceDefOf.Food, 160 },
                },
                RequirementDefNames = new List<string>()
                {
                    "TownHall_II",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold, 4 },
                }
            },

            // --------------- Marketplace ---------------
            new UpgradeDef()
            {
                DefName = "Marketplace",
                Label = "Marketplace",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 150 },
                    { ResourceDefOf.Stone, 150 },
                },
                RequirementDefNames = new List<string>()
                {
                    "LumberYard_I",
                    "Quarry_I",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "TradeContract_I",
                Label   = "Trade Contract I",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 150 },
                    { ResourceDefOf.Stone, 150 },
                },
                RequirementDefNames = new List<string>()
                {
                    "Marketplace",
                    "TownHall_I"
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood,  -1 },
                    { ResourceDefOf.Stone, -1 },
                    { ResourceDefOf.Gold,   2 },
                }
            },

            // --------------- Guild Hall ---------------
            new UpgradeDef()
            {
                DefName = "GuildHall",
                Label   = "Guild Hall",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold, 400 },
                    { ResourceDefOf.Wood, 200 },
                    { ResourceDefOf.Stone, 200 },
                },
                RequirementDefNames = new List<string>()
                {
                    "Marketplace",
                    "TownHall_II"
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold,   2 },
                    { ResourceDefOf.Wood,   1 },
                    { ResourceDefOf.Stone,  1 },
                    { ResourceDefOf.Food,   1 },
                }
            },

        };
    }
}
