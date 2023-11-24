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
            PopulationSize = 100;

            SubjectInputSize = 16;
            SubjectOutputSize = 16;
            SubjectHiddenSizes = new int[] { 8, 8};

            MatchesPerGeneration = 12;
        }

        public override void OnGenerationFinished()
        {
            
        }

        public override void OnMatchRoundFinished()
        {
            
        }
    }
}
