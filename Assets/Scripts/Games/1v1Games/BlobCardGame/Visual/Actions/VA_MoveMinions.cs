using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlobCardGame
{
    public class VA_MoveMinions : VisualAction
    {
        private List<Minion> Minions;
        private List<Vector3> SourcePositions;
        private List<Vector3> TargetPositions;

        /// <summary>
        /// Moves all minions given in the dictionary from their current positions to their given target position.
        /// </summary>
        public VA_MoveMinions(Dictionary<Minion, Vector3> minionPositions)
        {
            Frames = DefaultFrames * 0.5f;

            Minions = minionPositions.Select(x => x.Key).ToList();
            SourcePositions = minionPositions.Select(x => x.Key.Visual.transform.position).ToList();
            TargetPositions = minionPositions.Select(x => x.Value).ToList();
        }

        public override void Update()
        {
            base.Update();
            for (int i = 0; i < Minions.Count; i++)
            {
                Minions[i].Visual.transform.position = Vector3.Lerp(SourcePositions[i], TargetPositions[i], CurrentFrame / Frames);
            }
        }

        public override void OnDone()
        {
            for (int i = 0; i < Minions.Count; i++)
            {
                Minions[i].Visual.transform.position = TargetPositions[i];
            }
        }
    }
}
