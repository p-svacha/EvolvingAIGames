using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Incrementum
{
    /// <summary>
    /// A game where the AI has a certain amount of ticks to maximize the amount of gold by acquiring the right upgrades.
    /// </summary>
    public class Incrementum : IsolatedSimulationModel
    {
        // Rules
        public static int NUM_TICKS = 500; // Each game will last this many ticks.
        public static int BASE_WOOD_INCOME = 1;
        public static int BASE_STONE_INCOME = 1;
        public static int BASE_GOLD_INCOME = 0;
        public static int STARTING_WOOD_AMOUNT = 30;
        public static int STARTING_STONE_AMOUNT = 20;

        public static float UPGRADE_ACQUIRE_THRESHOLD = 0.7f; // A neural net output higher than this represents a wish to acquire the upgrade

        protected override int PopulationSize => 625;

        [Header("References")]
        public UI_Incrementum UI;

        public override void OnPrePopulationInit()
        {
            // Defs
            DefDatabaseRegistry.ClearAllDatabases();

            DefDatabase<ResourceDef>.AddDefs(ResourceDefs.Defs);
            DefDatabase<UpgradeDef>.AddDefs(UpgradeDefs.Defs);

            DefDatabaseRegistry.ResolveAllReferences();
            DefDatabaseRegistry.OnLoadingDone();

            // Network size
            IncrementumTask dummyTask = new IncrementumTask(null);
            SubjectInputSize = dummyTask.GetInputs().Length;
            SubjectHiddenSizes = new int[1] { 16 };
            SubjectOutputSize = UpgradeDefs.Defs.Count;

            // UI
            UI.Init(this);
        }

        public override void OnPostPopulationInit()
        {
        }

        protected override Task CreateTaskFor(Subject s)
        {
            return new IncrementumTask(s);
        }

        protected override void OnUpdate()
        {
            if (UI.AutoRunToggle.IsToggled)
            {
                if (SimulationPhase == IsolatedSimulationPhase.Ready) StartGeneration();
                else if (SimulationPhase == IsolatedSimulationPhase.Done) InitializeNextGeneration();
            }
        }

        public override GenerationStats CreateGenerationStats(EvolutionInformation info)
        {
            return new IncrementumGenerationStats(info);
        }

        protected override void OnGenerationInitialized(EvolutionInformation info)
        {
            UI.OnGenerationInitialized();
        }

        protected override void OnGenerationStarted()
        {
            UI.OnGenerationStarted();
        }

        protected override void OnGenerationFinished()
        {
            Debug.Log($"Gen {Generation}: {GenerationHistory.Last().BestPerformingSubject.Name} has won with a gold amount of {GenerationHistory.Last().BestPerformingTask.GetFitnessValue()}. History:\n{((IncrementumTask)GenerationHistory.Last().BestPerformingTask).GetHistoryLog()}");

            UI.OnGenerationFinished();
        }
    }
}
