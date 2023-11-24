using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlobCardGame
{
    public class BlobCardSimulationModel : SimulationModel
    {
        // UI
        public new BlobCardSimulationUI SimulationUI;
        public new BlobCardMatchUI MatchUI;

        // Visuals
        public VisualPlayer VisualPlayerPrefab;
        public VisualMinion VisualMinionPrefab;

        // Card Statistics
        public Dictionary<int, int> CardsPicked;
        public Dictionary<int, int> CardsPickedByWinner;
        public Dictionary<int, int> CardsPickedByLoser;
        public Dictionary<int, int> CardsNotPicked;

        public Dictionary<int, float> CardPickedPerMatch;
        public Dictionary<int, float> CardPickrate;
        public Dictionary<int, float> CardWinrate;

        // Match Statistics
        public new List<BlobCardMatch> Matches => base.Matches.Select(x => (BlobCardMatch)x).ToList();
        public int Player1WonMatches;
        public int TotalMatches;
        public int TotalTurns;
        public int WinnerNumCardOptions;
        public int LoserNumCardOptions;

        public override void InitSimulationParameters()
        {
            CardList.InitCardList();

            PopulationSize = 600;

            SubjectInputSize = 16;
            SubjectOutputSize = CardList.Cards.Count;
            SubjectHiddenSizes = new int[0];

            MatchesPerGeneration = 12;

            // UI
            ResetStatistics();
            SimulationUI.MatchRules.UpdateStatistics();
        }

        public override Match GetMatch(Subject sub1, Subject sub2, MatchSimulationMode simulationMode)
        {
            return new BlobCardMatch(this, sub1, sub2, simulationMode);
        }

        public override void OnMatchRoundFinished()
        {
            UpdateStatistics();
        }

        public override void OnGenerationFinished()
        {
            ResetStatistics();
        }

        #region Statistics

        private void UpdateStatistics()
        {
            UpdateMatchStatistics();
            UpdateCardStatistics();
        }

        /// <summary>
        /// Updates the match statistics according to the matches in the Matches list. Also updates the UI Element.
        /// </summary>
        private void UpdateMatchStatistics()
        {
            TotalMatches += Matches.Count;

            // Player 1 Winrate
            Player1WonMatches += Matches.Where(x => x.Winner == x.Subject1).Count();
            float p1winrate = (float)Player1WonMatches / TotalMatches;

            // Game Length
            TotalTurns += Matches.Sum(x => x.Turn);
            float avgGameLength = (float)TotalTurns / TotalMatches;

            // Card Options
            WinnerNumCardOptions += Matches.Sum(x => x.WinnerBlob.NumCardOptions);
            LoserNumCardOptions += Matches.Sum(x => x.LoserBlob.NumCardOptions);
            float avgWinnerCardOptions = (float)WinnerNumCardOptions / TotalMatches;
            float avgLoserCardOptions = (float)LoserNumCardOptions / TotalMatches;

            SimulationUI.MatchStatistics.UpdateStatistics(p1winrate, avgGameLength, avgWinnerCardOptions, avgLoserCardOptions);
        }

        /// <summary>
        /// Updates the card statistics according to the matches in the Matches list. Also updates the UI Element.
        /// </summary>
        private void UpdateCardStatistics()
        {
            foreach (BlobCardMatch m in Matches)
            {
                if (m.Phase != BlobCardMatchPhase.GameEnded) throw new System.Exception("Match not finished");
                foreach (KeyValuePair<int, int> kvp in m.Player1.CardsPicked) CardsPicked[kvp.Key] += kvp.Value;
                foreach (KeyValuePair<int, int> kvp in m.Player2.CardsPicked) CardsPicked[kvp.Key] += kvp.Value;

                foreach (KeyValuePair<int, int> kvp in m.Player1.CardsNotPicked) CardsNotPicked[kvp.Key] += kvp.Value;
                foreach (KeyValuePair<int, int> kvp in m.Player2.CardsNotPicked) CardsNotPicked[kvp.Key] += kvp.Value;

                foreach (KeyValuePair<int, int> kvp in m.WinnerBlob.CardsPicked) CardsPickedByWinner[kvp.Key] += kvp.Value;
                foreach (KeyValuePair<int, int> kvp in m.LoserBlob.CardsPicked) CardsPickedByLoser[kvp.Key] += kvp.Value;
            }

            CardPickedPerMatch.Clear();
            CardPickrate.Clear();
            CardWinrate.Clear();

            for (int i = 0; i < CardList.Cards.Count; i++)
            {
                CardPickedPerMatch.Add(i, (float)CardsPicked[i] / TotalMatches / 2); // The /2 is because it is per player
                CardPickrate.Add(i, (float)CardsPicked[i] / (CardsPicked[i] + CardsNotPicked[i]));
                CardWinrate.Add(i, (float)CardsPickedByWinner[i] / (CardsPickedByWinner[i] + CardsPickedByLoser[i]));
            }

            SimulationUI.UpdateCardStatistics(CardPickedPerMatch, CardPickrate, CardWinrate);
        }

        /// <summary>
        /// Resets all match and card statistics.
        /// </summary>
        private void ResetStatistics()
        {
            CardsPicked = new Dictionary<int, int>();
            CardsPickedByWinner = new Dictionary<int, int>();
            CardsPickedByLoser = new Dictionary<int, int>();
            CardsNotPicked = new Dictionary<int, int>();
            CardPickedPerMatch = new Dictionary<int, float>();
            CardPickrate = new Dictionary<int, float>();
            CardWinrate = new Dictionary<int, float>();
            for (int i = 0; i < CardList.Cards.Count; i++)
            {
                CardsPicked.Add(i, 0);
                CardsPickedByWinner.Add(i, 0);
                CardsPickedByLoser.Add(i, 0);
                CardsNotPicked.Add(i, 0);
            }
            Player1WonMatches = 0;
            WinnerNumCardOptions = 0;
            LoserNumCardOptions = 0;
            TotalMatches = 0;
            TotalTurns = 0;
        }

        #endregion
    }
}
