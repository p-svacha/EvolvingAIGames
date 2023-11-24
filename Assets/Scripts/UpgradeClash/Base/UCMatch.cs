using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class UCMatch : Match
    {
        // Rules
        public const int StartHealth = 20;
        public const int BaseGoldIncome = 10;

        // Players
        public Player Player1;
        public Player Player2;

        // Match progress
        public const int SimulatedTicksPerUpdate = 10; // mutiple ticks are done per update to increase background simulation speed
        public int Ticks;

        // Visual
        public const int TicksPerSecond = 60;
        public float TimeSinceLastTick;

        public UCMatch(UCSimulationModel model, Subject subject1, Subject subject2, MatchSimulationMode simulationMode) : base(model, subject1, subject2, simulationMode)
        {
            SimulationModel = model;

            if(simulationMode == MatchSimulationMode.Play)
            {
                Player1 = new HumanPlayer();
                Player2 = new AIPlayer(subject2);
            }
            else
            {
                Player1 = new AIPlayer(subject1);
                Player2 = new AIPlayer(subject2);
            }

            Player1.Init(opponent: Player2);
            Player2.Init(opponent: Player1);
        }

        public void Tick()
        {
            Ticks++;
            Player1.Tick();
            Player2.Tick();
            CheckGameOver();
        }

        /// <summary>
        /// Ends the game is a victory condition is reached.
        /// </summary>
        private void CheckGameOver()
        {
            
            if (Player1.CurrentHealth <= 0)
            {
                if (SimulationMode == MatchSimulationMode.Play) EndGame(null);
                else EndGame(((AIPlayer)Player2).Subject);
            }
            else if (Player2.CurrentHealth <= 0)
            {
                if (SimulationMode == MatchSimulationMode.Play) EndGame(null);
                else EndGame(((AIPlayer)Player1).Subject);
            }
        }

        public override void Update()
        {
            if (IsVisual) // force 60 FPS when visual
            {
                TimeSinceLastTick += Time.deltaTime;
                if (TimeSinceLastTick >= 1f / TicksPerSecond)
                {
                    Tick();
                    TimeSinceLastTick = 0f;
                }
            }
            else // do multiple ticks at once when not visual
            {
                for (int i = 0; i < SimulatedTicksPerUpdate; i++)
                {
                    Tick();
                    if (MatchPhase == MatchPhase.Finished) break;
                }
            }
        }

        public override void OnMatchEnd()
        {
            Debug.Log("Game ended after " + Ticks + " ticks.");
        }
    }
}
