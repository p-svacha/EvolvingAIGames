using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CardStatistics : UIElement
{

    private float RowHeight = 0.8f; // 1 means no gap between rows
    private int RowFontSize = 250;

    public void UpdateBoard(Dictionary<int, float> Pickrates, string title, bool valuesInPercent)
    {
        Clear();

        float yStep = 1f / (Pickrates.Count + 1);

        float highestValue = Pickrates.Max(x => x.Value);

        // Title
        int fontSize = 20;
        AddText(title, fontSize, Color.black, FontStyle.Bold, 0, 0, 1, yStep, Container, TextAnchor.MiddleCenter);

        var ordered = Pickrates.OrderByDescending(x => x.Value);

        for (int i = 0; i < ordered.Count(); i++)
        {
            int key = ordered.ToList()[i].Key;
            float value = ordered.ToList()[i].Value;
            float valueInPercent;
            if(valuesInPercent)
                valueInPercent = value;
            else // Highest value = 100%
                valueInPercent = value / highestValue;

            float xMargin = 0.1f;
            float xStart = xMargin;
            float xEnd = xMargin + valueInPercent * 0.8f;
            float yStart = (i + 1) * yStep;
            float yEnd = (i + 1) * yStep + yStep * RowHeight;

            // Parent Panel
            RectTransform parentPanel = AddPanel(CardList.Cards[key].Name + "_panel", Color.clear, xStart, yStart, xEnd, yEnd, Container);
            RectMask2D mask = parentPanel.gameObject.AddComponent<RectMask2D>(); // Mask so image scales within parent

            // Add image with AspectRatioFitter
            RectTransform bgImagePanel = AddPanel(CardList.Cards[key].Name + "_image", Color.white, xStart, yStart, xEnd, yEnd, parentPanel);
            Image bgImage = bgImagePanel.GetComponent<Image>();
            bgImage.sprite = CardSpriteList.CardSprites[CardList.Cards[key].Id];
            AspectRatioFitter arf = bgImage.gameObject.AddComponent<AspectRatioFitter>();
            arf.aspectMode = AspectRatioFitter.AspectMode.EnvelopeParent;

            // Add semi-transparent overlay
            RectTransform overlay = AddPanel(CardList.Cards[key].Name + "_overlay", new Color(1, 1, 1, 0.6f), xStart, yStart, xEnd, yEnd, Container);

            // Add Text
            AddText(CardList.Cards[key].Name + " [" + CardList.Cards[key].Cost + "]", (int)(yStep * RowFontSize), Color.black, FontStyle.Normal, xMargin + (0.1f * xMargin), yStart, 0.7f, yEnd, Container, TextAnchor.MiddleLeft);
            AddText(valuesInPercent ? value.ToString("0.0%") : value.ToString("0.00"), (int)(yStep * RowFontSize), Color.black, FontStyle.Normal, 0.7f, yStart, 1f - xMargin - (0.1f * xMargin), yEnd, Container, TextAnchor.MiddleRight);

            // Set background image
            
        }
    }
}
