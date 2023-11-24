using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobCardGame
{
    public class BlobCardSimulationUI : SimulationUI
    {
        [Header("Elements")]
        public CardStatistics CardPicksAbsolute;
        public CardStatistics CardPickrates;
        public CardStatistics CardWinrates;

        public MatchRulesDisplay MatchRules;
        public MatchStatisticsDisplay MatchStatistics;

        [Header("Card Display")]
        public List<Sprite> CardSprites;
        public List<VisualCard> DisplayedCards;
        private float CardMarginX = 0.15f;
        private float CardMarginY = 0.1f;
        private float CardGapX = 1f; // 0.5 = The gap between two cards is the width of 0.5 cards

        public void UpdateCardStatistics(Dictionary<int, float> cardPicksPerMatch, Dictionary<int, float> cardPickrate, Dictionary<int, float> cardWinrate)
        {
            CardPicksAbsolute.UpdateBoard(cardPicksPerMatch, "Card Picks per Match per Player", false);
            CardPickrates.UpdateBoard(cardPickrate, "Card Pickrates", true);
            CardWinrates.UpdateBoard(cardWinrate, "Card Winrates", true);
        }
    }
}
