using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UpgradeClash
{
    public class UCSimulationModel : SimulationModel
    {
        // Attributes
        protected override int PopulationSize => 32;
        protected override int MatchesPerGeneration => 5; // Change by 1 for doubling / halving population size
        protected override MatchingTypeId MatchingType => MatchingTypeId.SwissSystem;

        // Stats
        public Dictionary<SimulationStatId, SimulationStat> Stats;

        // UI
        private UCSimulationUI UI;

        public override Match GetMatch(Subject sub1, Subject sub2, MatchSimulationMode simulationMode)
        {
            return new UCMatch(this, sub1, sub2, simulationMode);
        }

        public override void Init()
        {
            // Neural network sizes
            AIPlayer dummy = new AIPlayer(null); // Create a dummy player to get correct amount of inputs & outputs
            dummy.Init(null, dummy);

            SubjectInputSize = dummy.GetInputs().Length;
            SubjectOutputSize = dummy.UpgradeList.Count;
            SubjectHiddenSizes = new int[] { dummy.GetInputs().Length, dummy.GetInputs().Length / 2 };

            // Stats
            Stats = new Dictionary<SimulationStatId, SimulationStat>()
            {
                { SimulationStatId.AvgGameTime, new SimulationStat(SimulationStatId.AvgGameTime, "Average Game Time") }
            };

            // UI
            UI = (UCSimulationUI)SimulationUI;
            UI.Init(this);
        }

        public override void OnGenerationStarted()
        {
            foreach (SimulationStat stat in Stats.Values) stat.AddNewValue();
        }

        public override void OnMatchRoundFinished()
        {
            // Debug
            DebugStandings();

            // Update stat values
            List<UCMatch> matches = Matches.Select(x => (UCMatch)x).ToList();
            float avgGameTime = (float)matches.Average(x => x.Ticks);
            Stats[SimulationStatId.AvgGameTime].AddCurrentGenValue(avgGameTime);

            // Update stat UI
            UI.UpdateValues();
        }
    }
}
