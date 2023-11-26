using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class UCSimulationModel : SimulationModel
    {
        public override Match GetMatch(Subject sub1, Subject sub2, MatchSimulationMode simulationMode)
        {
            return new UCMatch(this, sub1, sub2, simulationMode);
        }

        public override void InitSimulationParameters()
        {
            PopulationSize = 32;

            AIPlayer dummy = new AIPlayer(null); // Create a dummy player to get correct amount of inputs & outputs
            dummy.Init(null, dummy);

            SubjectInputSize = dummy.GetInputs().Length;
            SubjectOutputSize = dummy.UpgradeList.Count;
            SubjectHiddenSizes = new int[] { dummy.GetInputs().Length, dummy.GetInputs().Length / 2 };

            MatchesPerGeneration = 2;
        }

        public override void OnGenerationFinished() { }

        public override void OnMatchRoundFinished()
        {
            DebugStandings();   
        }
    }
}
