using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Incrementum
{
    public class ResourceDef : Def
    {
        public int BaseIncomePerTick { get; init; }
        public int ApproxAmountCap { get; init; }
        public int ApproxIncomeCap { get; init; }
    }
}
