using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    /// <summary>
    /// A collection of permanent stat alterations that get active when a unique upgrade takes effect.
    /// </summary>
    public class UpgradePermanentEffect
    {
        /// <summary>
        /// Increase of gold income per tick.
        /// </summary>
        public int GoldIncomeBonus { get; set; }
    }
}
