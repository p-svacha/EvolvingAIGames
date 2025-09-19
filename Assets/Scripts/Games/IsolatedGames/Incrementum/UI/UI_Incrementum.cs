using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

namespace Incrementum
{
    public class UI_Incrementum : MonoBehaviour
    {
        private Incrementum Simulation;
        private int DisplayedGenerationIndex;

        [Header("Controls")]
        public SpeciesScoreboard SpeciesScoreboard;
        public UI_RankingGrid RankingGrid;

        public UI_GenerationStats GenerationStats;
        public UI_Graph FitnessGraph;
        public UI_Graph SpeciesDistributionGraph;
        public UI_ToggleButton RunSingleGenerationButton;
        public UI_ToggleButton AutoRunToggle;
        public UI_ToggleButton PlayButton;
        public UI_GenerationSelector GenerationSelector;
        public UI_PlayIncrementum PlayPanel;

        // Fitness graph style
        private static readonly float[] ThinPercentiles = { 0.20f, 0.40f, 0.60f, 0.80f };
        private static readonly Color AxisColor = Color.white;
        private static readonly Color PercentileColor = new Color(0.7f, 0.7f, 0.7f);
        private static readonly Color MedianColor = new Color(0.8f, 0.2f, 0.2f);           // red
        private static readonly Color TopColor = new Color(1f, 1f, 1f);             // light green
        private const float ThinThickness = 2f;
        private const float ThickThickness = 3f;

        public void Init(Incrementum simulation)
        {
            Simulation = simulation;

            PlayPanel.Init();

            RankingGrid.Init(simulation.PopulationSize);
            GenerationSelector.Init(this);
            RunSingleGenerationButton.SetOnClick(RunSingleGenerationButton_OnClick);
            PlayButton.SetOnClick(Play_OnClick);

            // Initial empty graph
            RebuildFitnessGraph();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                GenerationSelector.SetValue(DisplayedGenerationIndex + 1, notify: true);
            }
            else if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                GenerationSelector.SetValue(DisplayedGenerationIndex - 1, notify: true);
            }
        }

        private void RunSingleGenerationButton_OnClick()
        {
            if (AutoRunToggle.IsToggled) return;

            if (Simulation.SimulationPhase == IsolatedSimulationPhase.Done)
            {
                Simulation.InitializeNextGeneration();
            }
            if (Simulation.SimulationPhase == IsolatedSimulationPhase.Ready)
            {
                Simulation.StartGeneration();
            }
        }

        private void Play_OnClick()
        {
            PlayPanel.Show();
        }

        public void OnGenerationInitialized()
        {
            GenerationSelector.SetMaxGenerationIndex(Simulation.Generation, selectNewMax: Simulation.Generation == 0);
            RebuildSpeciesDistributionGraph();
        }
        public void OnGenerationStarted()
        {

        }
        public void OnGenerationFinished()
        {
            if (DisplayedGenerationIndex == Simulation.Generation || DisplayedGenerationIndex == Simulation.Generation - 1)
            {
                GenerationSelector.SetValue(Simulation.Generation, notify: true);
            }

            // Show highscore in manual play
            string aiHighscoreText = $"<b>AI Highscore: {Simulation.GenerationHistory.Last().BestPerformingTask.GetFitnessValue()}</b>\n";
            bool isFirst = true;
            foreach (var h in ((IncrementumTask)Simulation.GenerationHistory.Last().BestPerformingTask).History)
            {
                if (!isFirst) aiHighscoreText += " --> ";
                aiHighscoreText += $"{h.Value.Label} ({h.Key})";
                isFirst = false;
            }
            PlayPanel.AiHighscoreText.text = aiHighscoreText;
            

            RebuildFitnessGraph();
        }

        public void DisplayGenerationStats(int generationIndex)
        {
            DisplayedGenerationIndex = generationIndex;

            // Show info about this generation in various elements
            IncrementumGenerationStats stats = (IncrementumGenerationStats)Simulation.GenerationHistory[DisplayedGenerationIndex];
            GenerationStats.ShowStats(stats);
            SpeciesScoreboard.UpdateScoreboard(stats.SpeciesData);
            RankingGrid.ShowGeneration(stats);
        }

        #region Fitness graph

        private void RebuildFitnessGraph()
        {
            var (lines, yMax) = BuildFitnessLines();
            string title = "Fitness by Generation";
            FitnessGraph.ShowLineGraph(lines, yMax <= 0 ? 1f : yMax, title, AxisColor, showDataPoints: false);
        }

        private (List<LineData> lines, float yMax) BuildFitnessLines()
        {
            var lines = new List<LineData>();

            // Use completed generations only, ordered by generation index
            var gens = (Simulation.GenerationHistory ?? new List<GenerationStats>())
                        .Where(gs => gs != null && gs.IsComplete)
                        .ToList();

            if (gens.Count == 0) return (lines, 1f);

            // Build series containers
            var thinSeries = ThinPercentiles.ToDictionary(p => p, _ => new List<GraphDataPoint>());
            var medianSeries = new List<GraphDataPoint>();   // 50th
            var topSeries = new List<GraphDataPoint>();   // max
            float globalMax = 0f;

            for (int i = 0; i < gens.Count; i++)
            {
                var gs = gens[i];

                // X label: prefer explicit generation number when available; else use index
                string xLabel = (gs.EvolutionInfo != null ? gs.GenerationNumber : i).ToString();

                // Thin percentiles
                foreach (var p in ThinPercentiles)
                {
                    int val = gs.GetPercentileFitness(p);
                    globalMax = Mathf.Max(globalMax, val);
                    thinSeries[p].Add(new GraphDataPoint(xLabel, val, PercentileColor));
                }

                // Median
                {
                    int val = gs.GetPercentileFitness(0.5f);
                    globalMax = Mathf.Max(globalMax, val);
                    medianSeries.Add(new GraphDataPoint(xLabel, val, MedianColor));
                }

                // Top (max of generation)
                {
                    int val = gs.BestPerformingTask?.GetFitnessValue() ?? 0;
                    globalMax = Mathf.Max(globalMax, val);
                    topSeries.Add(new GraphDataPoint(xLabel, val, TopColor));
                }
            }

            // Convert to LineData (thin whites)
            foreach (var kv in thinSeries)
            {
                if (kv.Value.Count > 0)
                    lines.Add(new LineData($"{Mathf.RoundToInt(kv.Key * 100)}th", kv.Value, PercentileColor, ThinThickness));
            }
            // Median (red, thick)
            if (medianSeries.Count > 0)
                lines.Add(new LineData("Median (50th)", medianSeries, MedianColor, ThickThickness));
            // Top (light green, thick)
            if (topSeries.Count > 0)
                lines.Add(new LineData("Top", topSeries, TopColor, ThickThickness));

            // Headroom for readability
            float yMax = globalMax <= 0f ? 1f : globalMax * 1.10f;

            return (lines, yMax);
        }

        #endregion

        #region Species Distribution graph

        private void RebuildSpeciesDistributionGraph()
        {
            var (bars, yMax) = BuildSpeciesBars();
            SpeciesDistributionGraph.ShowStackedBarGraph(
                bars,
                yMax,
                "Species distribution (subjects per generation)",
                axisColor: Color.white
            );
        }

        private (List<StackedBar> bars, float yMax) BuildSpeciesBars()
        {
            var bars = new List<StackedBar>();
            var gens = Simulation.GenerationHistory ?? new List<GenerationStats>();
            if (gens.Count == 0) return (bars, 1f);

            // Constant headroom: yMax = population size (fast + stable axes)
            // If you ever change pop size mid-run, replace with per-gen sum max.
            int populationSize = Simulation != null ? Simulation.GetComponent<Incrementum>().PopulationSize : 0;
            float yMax = populationSize > 0 ? populationSize : gens.Max(g => g.SpeciesData?.Sum(sd => sd.SubjectCount) ?? 0);

            for (int gi = 0; gi < gens.Count; gi++)
            {
                var gs = gens[gi];
                var segments = new List<StackedBarData>();

                if (gs?.SpeciesData != null && gs.SpeciesData.Count > 0)
                {
                    foreach (var sd in gs.SpeciesData.OrderBy(sd => sd.Id))
                    {
                        if (sd.SubjectCount <= 0) continue;
                        segments.Add(new StackedBarData(
                            segmentName: sd.Name ?? $"Species {sd.Id}",
                            amount: sd.SubjectCount,
                            color: sd.Color
                        ));
                    }
                }

                // X label: show generation number if present, else index
                string label = (gs?.EvolutionInfo != null ? gs.GenerationNumber : gi).ToString();
                bars.Add(new StackedBar(label, segments));
            }

            if (yMax <= 0f) yMax = 1f;
            return (bars, yMax);
        }

        #endregion
    }
}
