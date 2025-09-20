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
                DefName = "LumberYardSmall",
                Label = "Small Lumber Yard",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 40 },
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "LumberYardMedium",
                Label = "Medium Lumber Yard",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 120 },
                },
                RequirementDefNames = new List<string>()
                {
                    "LumberYardSmall",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "LumberYardBig",
                Label = "Big Lumber Yard",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 260 },
                    { ResourceDefOf.Stone, 50 },
                },
                RequirementDefNames = new List<string>()
                {
                    "LumberYardMedium",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 2 },
                }
            },

            // --------------- Quarry ---------------
            new UpgradeDef()
            {
                DefName = "QuarrySmall",
                Label = "Small Quarry",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 100 },
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Stone, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "QuarryBig",
                Label = "Big Quarry",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 250 },
                },
                RequirementDefNames = new List<string>()
                {
                    "QuarrySmall",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Stone, 2 },
                }
            },

            // --------------- Iron (Quarry branch) ---------------
            new UpgradeDef()
            {
                DefName = "IronMineSmall",
                Label = "Small Iron Mine",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 80 },
                },
                RequirementDefNames = new List<string>()
                {
                    "QuarrySmall",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Iron, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "IronMineBig",
                Label = "Big Iron Mine",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 160 },
                    { ResourceDefOf.Stone, 80 },
                },
                RequirementDefNames = new List<string>()
                {
                    "IronMineSmall",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Iron, 2 },
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
                },
                RequirementDefNames = new List<string>()
                {
                    "LumberYardSmall",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 1 },
                    { ResourceDefOf.Stone, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "Toolsmiths",
                Label = "Toolsmiths",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold, 100 },
                    { ResourceDefOf.Iron, 40 },
                },
                RequirementDefNames = new List<string>()
                {
                    "Workshop",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood,  1 },
                    { ResourceDefOf.Stone, 1 },
                }
            },

            // --------------- Farm ---------------
            new UpgradeDef()
            {
                DefName = "Farm",
                Label = "Farm",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 120 },
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
                DefName = "TownHallSmall",
                Label = "Small Town Hall",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 80 },
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "TaxCollectors",
                Label = "Tax Collectors",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Food, 160 },
                },
                RequirementDefNames = new List<string>()
                {
                    "TownHallSmall",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold, 2 },
                    { ResourceDefOf.Food, -1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "TownHallMedium",
                Label = "Medium Town Hall",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 200 },
                    { ResourceDefOf.Stone, 60 },
                    { ResourceDefOf.Food, 60 },
                },
                RequirementDefNames = new List<string>()
                {
                    "TownHallSmall",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold, 2 },
                }
            },
            new UpgradeDef()
            {
                DefName = "TownHallBig",
                Label = "Big Town Hall",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 600 },
                    { ResourceDefOf.Stone, 400 },
                    { ResourceDefOf.Food, 160 },
                },
                RequirementDefNames = new List<string>()
                {
                    "TownHallMedium",
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
                    { ResourceDefOf.Wood, 140 },
                    { ResourceDefOf.Stone, 70 },
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold, 1 },
                }
            },
            new UpgradeDef()
            {
                DefName = "TradeContract",
                Label   = "Trade Contract",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 100 },
                    { ResourceDefOf.Stone, 100 },
                    { ResourceDefOf.Gold, 50 },
                },
                RequirementDefNames = new List<string>()
                {
                    "Marketplace",
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
                    { ResourceDefOf.Gold, 300 },
                    { ResourceDefOf.Wood, 200 },
                    { ResourceDefOf.Stone, 200 },
                },
                RequirementDefNames = new List<string>()
                {
                    "Marketplace",
                    "TownHallSmall"
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold,   2 },
                    { ResourceDefOf.Wood,   1 },
                    { ResourceDefOf.Stone,  1 },
                    { ResourceDefOf.Food,   1 },
                }
            },

            // --------------- Bank ---------------
            new UpgradeDef()
            {
                DefName = "Bank",
                Label   = "Bank",
                Cost = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Wood, 180 },
                    { ResourceDefOf.Stone, 150 },
                    { ResourceDefOf.Iron, 120 },
                },
                RequirementDefNames = new List<string>()
                {
                    "TownHallMedium",
                },
                ResourceGenerationModifiers = new Dictionary<ResourceDef, int>()
                {
                    { ResourceDefOf.Gold, 3 },
                }
            },

        };
    }
}
