using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace BlobCardGame
{
    // The object holding this script must be anchored to the bottom left!!
    public class CardValues : UIElement
    {

        private float RowHeight = 0.8f; // 1 means no gap between rows

        public void VisualizeOptions(float[] allCards, List<int> options)
        {
            Clear();

            float yStep = 1f / (allCards.Length + 1);

            // Create Dictionary filled with Id,Value
            Dictionary<int, float> values = new Dictionary<int, float>();

            for (int i = 0; i < allCards.Length; i++)
            {
                values.Add(i, allCards[i]);
            }

            // Title
            int fontSize = 16;
            AddText("Card Values", fontSize, Color.black, FontStyle.Bold, 0, 0, 1, yStep, Container, TextAnchor.MiddleCenter);

            var ordered = values.OrderByDescending(x => x.Value);

            for (int i = 0; i < ordered.Count(); i++)
            {
                int key = ordered.ToList()[i].Key;
                float value = ordered.ToList()[i].Value;

                float xMargin = 0.1f;
                float xStart = xMargin;
                float xEnd = xMargin + value * 0.8f;
                float yStart = (i + 1) * yStep;
                float yEnd = (i + 1) * yStep + yStep * RowHeight;
                AddPanel(CardList.Cards[key].Name, options.Contains(key) ? Color.grey : Color.white, xStart, yStart, xEnd, yEnd, Container);
                AddText(CardList.Cards[key].Name, (int)(yStep * 150), Color.black, FontStyle.Normal, xMargin + (0.1f * xMargin), yStart, 0.65f, yEnd, Container, TextAnchor.MiddleLeft);
                AddText(value.ToString("0.0%"), (int)(yStep * 150), Color.black, FontStyle.Normal, 0.65f, yStart, 1f - xMargin - (0.1f * xMargin), yEnd, Container, TextAnchor.MiddleRight);
            }

        }

    }
}
