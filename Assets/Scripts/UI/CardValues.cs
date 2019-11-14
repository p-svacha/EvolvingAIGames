using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// The object holding this script must be anchored to the bottom left!!
public class CardValues : UIElement {

    public void VisualizeOptions(float[] allCards, List<int> options)
    {
        Clear();

        float yStep = 1f / ((allCards.Length * 2) + 1);

        for (int i = 0; i < allCards.Length; i++)
        {
            float j = i;
            float xMargin = 0.1f;
            float xStart = xMargin;
            float xEnd = xMargin + allCards[i]*0.8f;
            float yStart = ((i*2)+1) * yStep;
            float yEnd = ((i*2)+2) * yStep;
            AddPanel(CardList.Cards[i].Name, options.Contains(i+1) ? Color.grey: Color.white, xStart, yStart, xEnd, yEnd, Container);
            AddText(CardList.Cards[i].Name, (int)(yStep * 200), Color.black, FontStyle.Normal, xMargin + (0.1f*xMargin), yStart, 0.5f, yEnd, Container, TextAnchor.MiddleLeft);
            AddText(allCards[i].ToString("0.0%"), (int)(yStep * 200), Color.black, FontStyle.Normal, 0.5f, yStart, 1f - xMargin - (0.1f*xMargin), yEnd, Container, TextAnchor.MiddleRight);
        }

    }

}
