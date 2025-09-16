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
                BaseIncomePerTick = 0,
                ApproxAmountCap = 3000,
                ApproxIncomeCap = 10,
            },

            new ResourceDef()
            {
                DefName = "Wood",
                Label = "wood",
                BaseIncomePerTick = Incrementum.BASE_WOOD_INCOME,
                ApproxAmountCap = 1000,
                ApproxIncomeCap = 8,
            },

            new ResourceDef()
            {
                DefName = "Stone",
                Label = "stone",
                BaseIncomePerTick = Incrementum.BASE_STONE_INCOME,
                ApproxAmountCap = 1000,
                ApproxIncomeCap = 8,
            },

            new ResourceDef()
            {
                DefName = "Food",
                Label = "food",
                BaseIncomePerTick = 0,
                ApproxAmountCap = 1000,
                ApproxIncomeCap = 8,
            },
        };
    }
}
