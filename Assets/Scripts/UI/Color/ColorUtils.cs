using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public static class ColorUtils
{
    /// <summary>
    /// Returns a new Color that is not similar no any of the colors in similar
    /// </summary>
    public static Color RandomColor(Color[] others = null, float startTolerance = 0.5f)
    {
        System.Random random = new System.Random();
        Color toReturn = new Color(1, 1, 1, 1);
        bool tooSimilar = true;
        int counter = 0;
        float currentTolerance = startTolerance;
        while (tooSimilar && counter <= 50)
        {
            if (counter == 50) Debug.Log("couldn't find a new color.");
            counter++;
            tooSimilar = false;
            toReturn = new Color((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble(), 1);
            if (others != null)
            {
                foreach (Color other in others)
                {
                    float diff = Math.Abs(other.r - toReturn.r) + Math.Abs(other.g - toReturn.g) + Math.Abs(other.b - toReturn.b);
                    if (diff < currentTolerance) tooSimilar = true;
                }
            }
            currentTolerance += 0.05f;
        }
        return toReturn;
    }
}
