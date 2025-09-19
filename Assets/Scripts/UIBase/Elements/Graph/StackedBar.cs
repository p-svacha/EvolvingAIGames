using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StackedBar
{
    public string Label;
    public List<StackedBarData> Segments;

    public StackedBar(string label, List<StackedBarData> segments)
    {
        Label = label;
        Segments = segments ?? new List<StackedBarData>();
    }
}
