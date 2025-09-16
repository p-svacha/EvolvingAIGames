using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineData
{
    public string Name;
    public List<GraphDataPoint> Points;
    public Color LineColor;
    public float Thickness;

    public LineData(string name, List<GraphDataPoint> points, Color lineColor, float thickness = 5f)
    {
        Name = name;
        Points = points ?? new List<GraphDataPoint>();
        LineColor = lineColor;
        Thickness = thickness;
    }
}

