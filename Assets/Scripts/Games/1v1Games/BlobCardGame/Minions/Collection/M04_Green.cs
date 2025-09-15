using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobCardGame
{
    public class M04_Green : Minion
    {

        public M04_Green(BlobCardMatch match, PlayerBlob owner, PlayerBlob enemy, int orderNum) : base(match, owner, enemy, orderNum)
        {
            Name = "Green";
            Text = "Summon a random minion.";
            Type = MinionType.Green;
            Color = Color.green;
        }

        public override void Action()
        {
            Match.SummonMinions(this, new List<Tuple<MinionType, PlayerBlob>>() {
            new Tuple<MinionType, PlayerBlob>(Match.RandomMinionType(), Owner),
        }, summonProtection: false);
        }
    }
}
