using System.Collections;
using System.Collections.Generic;
using System;
using System.Linq;

namespace BlobCardGame
{
    public class C018_Shotgun : Card
    {

        public C018_Shotgun()
        {
            Id = 18;
            Name = "Shotgun";
            Text = "Destroy one enemy minion of each type.";
            Cost = 1;
        }

        public override void Action(BlobCardMatch match, PlayerBlob self, PlayerBlob enemy)
        {
            List<Minion> toDestroy = new List<Minion>();
            foreach (MinionType type in Enum.GetValues(typeof(MinionType)))
            {
                toDestroy.AddRange(match.AllMinionsOfType(enemy, type, withoutSummonProtection: true).Take(1).ToList());
            }
            match.DestroyMinions(self, toDestroy);
        }
    }
}
