using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class HumanPlayer : Player
    {
        public override string DisplayName => "Human";
        public override List<Upgrade> GetDesiredUpgrades()
        {
            throw new System.NotImplementedException();
        }
    }
}
