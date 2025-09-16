using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace UpgradeClash
{
    public class UCSimulationUI : SimulationUI
    {
        public UCSimulationModel Simulation { get; private set; }
        public SimulationStatId DisplayedStatInGraph { get; private set; }

        [Header("Elements")]
        public UI_StatDisplay StatDisplay;
        public UI_Graph Graph;

        public void Init(UCSimulationModel sim)
        {
            Simulation = sim;
            DisplayedStatInGraph = SimulationStatId.AvgGameTime;
            UpdateValues();
        }

        public void UpdateValues()
        {
            // Stat Display
            StatDisplay.UpdateValues(this, Simulation.Stats.Values.ToList());

            // Graph
            if (DisplayedStatInGraph == SimulationStatId.None) Graph.ClearGraph();
            else
            {
                SimulationStat stat = Simulation.Stats[DisplayedStatInGraph];

                if (stat.Values.Count == 0) Graph.ClearGraph();
                else
                {
                    List<GraphDataPoint> dataPoints = stat.Values.Select((value, index) => new GraphDataPoint(index.ToString(), value, Color.white)).ToList();
                    Graph.ShowLineGraph(dataPoints, Mathf.RoundToInt(dataPoints.Max(x => x.Value)), stat.DisplayName, lineColor: Color.black, axisColor: Color.black);
                }
            }
        }

        public void SetStatToDisplay(SimulationStatId stat)
        {
            DisplayedStatInGraph = stat;
        }
    }
}
