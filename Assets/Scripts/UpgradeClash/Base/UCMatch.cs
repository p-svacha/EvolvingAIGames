using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UpgradeClash
{
    public class UCMatch : Match
    {
        // Rules
        public const int StartHealth = 100;
        public const int ResourceCap = 1000;
        public const int MaxGameLength = 3600;

        public const int StartFood = 100;
        public const int StartWood = 100;
        public const int StartGold = 100;
        public const int StartStone = 100;

        public const int WorkerCap = 10; // Max amount of workers per resource
        public const int ArmyCap = 10; // Max amount of units per type

        // Players
        public Player Player1;
        public Player Player2;

        // Match progress
        public const int SimulatedTicksPerUpdate = 10; // mutiple ticks are done per update to increase background simulation speed
        public int Ticks { get; private set; }

        // Visual
        public int TicksPerSecond;
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

            Player1.Init(this, opponent: Player2);
            Player2.Init(this, opponent: Player1);
        }

        public void Tick()
        {
            Ticks++;

            // Update players
            Player1.Tick();
            if(CheckGameOver()) return;
            Player2.Tick();
            if (CheckGameOver()) return;
        }

        /// <summary>
        /// Ends the game is a victory condition is reached.
        /// </summary>
        private bool CheckGameOver()
        {
            
            if (Player1.CurrentHealth <= 0)
            {
                if (SimulationMode == MatchSimulationMode.Play) EndGame(null);
                else EndGame(((AIPlayer)Player2).Subject);
                return true;
            }
            else if (Player2.CurrentHealth <= 0)
            {
                if (SimulationMode == MatchSimulationMode.Play) EndGame(null);
                else EndGame(((AIPlayer)Player1).Subject);
                return true;
            }
            else if(Ticks >= MaxGameLength) // Forfreit to player 2 if it would take too long
            {
                if (SimulationMode == MatchSimulationMode.Play) EndGame(null);
                else EndGame(((AIPlayer)Player2).Subject);
                return true;
            }

            return false;
        }

        public override void Update()
        {
            if (MatchPhase != MatchPhase.Running) return;

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
            Debug.Log("Game ended after " + Ticks + " ticks.   " + Winner.Name + " won.");
        }

        /// <summary>
        /// Set the visual playback speed of a match when watching.
        /// </summary>
        public void SetTicksPerSecond(int t)
        {
            TicksPerSecond = t;
        }
    }
}
