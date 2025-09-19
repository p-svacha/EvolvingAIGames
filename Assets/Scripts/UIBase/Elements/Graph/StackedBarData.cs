using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackedBarData
{
    public string SegmentName;
    public float Amount;
    public Color Color;

    public StackedBarData(string segmentName, float amount, Color color)
    {
        SegmentName = segmentName;
        Amount = amount;
        Color = color;
    }
}

