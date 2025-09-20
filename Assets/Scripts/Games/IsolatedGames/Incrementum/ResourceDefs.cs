using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incrementum
{
    public static class ResourceDefs
    {
        public static List<ResourceDef> Defs => new List<ResourceDef>()
        {
            new ResourceDef()
            {
                DefName = "Gold",
                Label = "gold",
                Description = "Target resource that the goal is to maximize. Can also be used as upgrade cost or as a negative upgrade effect",
                BaseIncomePerTick = 0,
                ApproxAmountCap = 3000,
                ApproxIncomeCap = 10,
            },

            new ResourceDef()
            {
                DefName = "Wood",
                Label = "wood",
                Description = "Main resource for buildings.",
                BaseIncomePerTick = Incrementum.BASE_WOOD_INCOME,
                ApproxAmountCap = 1000,
                ApproxIncomeCap = 8,
            },

            new ResourceDef()
            {
                DefName = "Stone",
                Label = "stone",
                Description = "Used for upgrading buildings or building more powerful buildings",
                BaseIncomePerTick = Incrementum.BASE_STONE_INCOME,
                ApproxAmountCap = 1000,
                ApproxIncomeCap = 8,
            },

            new ResourceDef()
            {
                DefName = "Food",
                Label = "food",
                Description = "Used for some buildings but mostly for upgrades within buildings",
                BaseIncomePerTick = 0,
                ApproxAmountCap = 1000,
                ApproxIncomeCap = 8,
            },

            new ResourceDef()
            {
                DefName = "Iron",
                Label = "iron",
                Description = "Mined ore used to unlock advanced buildings and craft better tools.",
                BaseIncomePerTick = 0,
                ApproxAmountCap = 600,
                ApproxIncomeCap = 6,
            },
        };
    }
}
