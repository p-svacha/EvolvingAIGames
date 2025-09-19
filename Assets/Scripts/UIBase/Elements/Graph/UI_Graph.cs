using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Used for displaying bar or line graphs.
/// The prefab with this script has to be placed in a container of the same size with anchors [0,1],[0,1].
/// </summary>
public class UI_Graph : MonoBehaviour
{
    private bool IsInitialized = false;

    /// <summary>
    /// Absolute pixel width of the graph container.
    /// </summary>
    public float ContainerWidth;
    /// <summary>
    /// Absolute pixel width of the graph container.
    /// </summary>
    public float ContainerHeight;

    public float ContainerCenterX;
    public float ContainerCenterY;

    public RectTransform GraphContainer;
    public Sprite CircleSprite;

    // Current graph attributes
    private List<GraphDataPoint> DataPoints;
    private float XStep;
    private float BarSpacing;
    private float BarWidth;
    private int FontSize;
    private float YMax;
    private float YStep;
    private float YMarginTop;
    private float AxisWidth;
    private Color AxisColor;
    private Color AxisStepColor;

    // Animation
    private List<GameObject> Bars = new List<GameObject>();
    private List<TextMeshProUGUI> BarLabels = new List<TextMeshProUGUI>();
    private float MaxValue;
    private float MaxBarHeight;
    private GraphAnimationType AnimationType;
    private float AnimationTime;
    private float AnimationDelay;
    private float AnimationSpeedModifier = 1f;
    private System.Action AnimationCallback;

    private List<GraphDataPoint> SourceDataPoints;
    private List<GraphDataPoint> TargetDataPoints;
    private float SourceYMax;
    private float TargetYMax;

    private void Awake()
    {
        
    }

    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        GraphContainer = GetComponent<RectTransform>();

        ContainerWidth = GraphContainer.rect.width;
        ContainerHeight = GraphContainer.rect.height;

        ContainerCenterX = ContainerWidth / 2f;
        ContainerCenterY = ContainerHeight / 2f;

        IsInitialized = true;
    }

    void Update()
    {
        if(AnimationType != GraphAnimationType.None)
        {
            if (AnimationDelay >= AnimationTime)
            {
                if(AnimationType == GraphAnimationType.Update)
                {
                    YMax = TargetYMax;
                    DataPoints = TargetDataPoints;
                }

                ShowBarGraph(DataPoints, YMax, YStep, BarSpacing, AxisColor, AxisStepColor);
                AnimationType = GraphAnimationType.None;
                if (AnimationCallback != null) AnimationCallback();
            }
            else
            {
                float r = AnimationDelay / AnimationTime;

                switch (AnimationType)
                {
                    case GraphAnimationType.Init:
                        float curValue = MaxValue * r;
                        float curHeight = MaxBarHeight * r;

                        for (int i = 0; i < DataPoints.Count; i++)
                        {
                            float barX = (i + 1) * XStep;
                            float barValue, barHeight;
                            if (DataPoints[i].Value < curValue)
                            {
                                barValue = DataPoints[i].Value;
                                barHeight = (barValue / YMax) * (ContainerHeight - YMarginTop);
                            }
                            else
                            {
                                barValue = curValue;
                                barHeight = curHeight;
                            }
                            Vector2 pos = new Vector2(barX, barHeight / 2);
                            Vector2 size = new Vector2(BarWidth, barHeight);
                            RectTransform rect = Bars[i].GetComponent<RectTransform>();
                            rect.anchoredPosition = pos;
                            rect.sizeDelta = size;

                            float barLabelY = barHeight + FontSize;
                            BarLabels[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(barX, barLabelY);
                            BarLabels[i].text = barValue.ToString("0.0") + "%";
                        }
                        break;

                    case GraphAnimationType.Update:
                        List<GraphDataPoint> tmpDataPoints = new List<GraphDataPoint>();
                        float tmpYMax = 0;
                        for(int i = 0; i < TargetDataPoints.Count; i++)
                        {
                            float value = SourceDataPoints[i].Value + (TargetDataPoints[i].Value - SourceDataPoints[i].Value) * r;
                            GraphDataPoint tmpDataPoint = new GraphDataPoint(TargetDataPoints[i].Label, value, TargetDataPoints[i].Color, TargetDataPoints[i].Icons, TargetDataPoints[i].IconTooltipTitles, TargetDataPoints[i].IconTooltipTexts);
                            tmpDataPoints.Add(tmpDataPoint);

                            tmpYMax = SourceYMax + (TargetYMax - SourceYMax) * r;
                        }
                        ShowBarGraph(tmpDataPoints, tmpYMax, YStep, BarSpacing, AxisColor, AxisStepColor, stopAnimation: false);
                        break;
                }

                AnimationDelay += Time.deltaTime * AnimationSpeedModifier;
            }
        }
    }

    #region Bar Graph

    /// <summary>
    /// Instantly destroys the whole graph
    /// </summary>
    public void ClearGraph(bool stopAnimation = true)
    {
        if (!IsInitialized) Initialize();

        foreach (Transform t in GraphContainer) Destroy(t.gameObject);
        Bars.Clear();
        BarLabels.Clear();
        if(stopAnimation) AnimationType = GraphAnimationType.None;
    }

    /// <summary>
    /// Instantly displays a bar graph with the given attributes
    /// </summary>
    public void ShowBarGraph(List<GraphDataPoint> dataPoints, float yMax, float yStep, float barSpacing, Color axisColor, Color axisStepColor, bool stopAnimation = true, bool zeroed = false)
    {
        ClearGraph(stopAnimation);
        DataPoints = dataPoints;

        XStep = ContainerWidth / (dataPoints.Count + 1);
        BarSpacing = barSpacing;
        BarWidth = XStep * (1 - barSpacing);
        YMax = yMax;
        YStep = yStep;
        AxisWidth = Mathf.Min(ContainerWidth, ContainerHeight) * 0.01f;
        FontSize = (int)(ContainerHeight * 0.07f);
        YMarginTop = ContainerHeight * 0.05f;
        AxisColor = axisColor;
        AxisStepColor = axisStepColor;

        DrawBarGraphAxis();

        // Bars and bar labels
        for (int i = 0; i < dataPoints.Count; i++)
        {
            float xPos = (i + 1) * XStep;
            float height = (dataPoints[i].Value / yMax) * (ContainerHeight - YMarginTop);
            if (zeroed) height = 0;
            Bars.Add(CreateBar(xPos, BarWidth, height, dataPoints[i].Color)); // Bars
            BarLabels.Add(DrawText(zeroed ? "" : dataPoints[i].Value.ToString("0.0") + "%", new Vector2(xPos, height + FontSize), new Vector2(BarWidth, FontSize), dataPoints[i].Color, FontSize)); // Bar value labels
        }
    }
    
    /// <summary>
    /// Displays an empty bar graph and initializes an animation that can either be instantly started with startAnimation = true or by calling StartInitAnimation()
    /// </summary>
    public void InitAnimatedBarGraph(List<GraphDataPoint> dataPoints, float yMax, float yStep, float barSpacing, Color axisColor, Color axisStepColor, Font font, float animationTime, bool startAnimation)
    {
        ShowBarGraph(dataPoints, yMax, yStep, barSpacing, axisColor, axisStepColor, font, zeroed: true);
        MaxValue = dataPoints.Max(x => x.Value);
        MaxBarHeight = (MaxValue / yMax) * (ContainerHeight - YMarginTop);
        AnimationTime = animationTime;
        AnimationDelay = 0f;
        if (startAnimation) StartAnimation();
    }

    /// <summary>
    /// Starts the animation that has been previously initialized with InitAnimatedGraph(). Callback gets executed when the animation is done.
    /// </summary>
    public void StartAnimation(System.Action callback = null)
    {
        AnimationType = GraphAnimationType.Init;
        AnimationCallback = callback;
    }

    /// <summary>
    /// Updates the values of an already initialized graph with an animation
    /// </summary>
    public void UpdateAnimatedBarGraph(List<GraphDataPoint> dataPoints, float yMax, float animationTime)
    {
        SourceDataPoints = DataPoints;
        TargetDataPoints = dataPoints;
        SourceYMax = YMax;
        TargetYMax = yMax;
        AnimationTime = animationTime;
        AnimationDelay = 0f;
        AnimationType = GraphAnimationType.Update;
    }

    /// <summary>
    /// Modifies the speed of the animation.
    /// </summary>
    public void SetAnimationSpeedModifier(float speed)
    {
        AnimationSpeedModifier = speed;
    }

    private GameObject CreateBar(float x, float width, float height, Color c)
    {
        Vector2 position = new Vector2(x, height / 2);
        Vector2 dimensions = new Vector2(width, height);
        return DrawRectangle(position, dimensions, c);
    }

    private void DrawBarGraphAxis()
    {
        // Axis origin
        DrawRectangle(new Vector2(-AxisWidth / 2, -AxisWidth / 2), new Vector2(AxisWidth, AxisWidth), AxisColor);

        // X-axis
        DrawRectangle(new Vector2(ContainerWidth / 2, -AxisWidth / 2), new Vector2(ContainerWidth, AxisWidth), AxisColor);
        for (int i = 0; i < DataPoints.Count; i++)
        {
            float xPos = (i + 1) * XStep + 1;
            DrawText(DataPoints[i].Label, new Vector2(xPos, -FontSize), new Vector2(BarWidth, FontSize), DataPoints[i].Color, FontSize); // X-axis labels
            for (int j = 0; j < DataPoints[i].Icons.Count; j++) // X-axis label icons
            {
                Vector2 dimensions = new Vector2(20, 20);
                float iconXStep = 25;
                float iconXStart = xPos - (DataPoints[i].Icons.Count - 1) * iconXStep * 0.5f;
                float iconX = iconXStart + j * iconXStep;
                float iconY = -FontSize * 2;
                DrawImage(DataPoints[i].Icons[j], new Vector2(iconX, iconY), dimensions, DataPoints[i].IconTooltipTitles[j], DataPoints[i].IconTooltipTexts[j]);
            }
        }

        // Y-axis
        DrawRectangle(new Vector2(-AxisWidth / 2, ContainerHeight / 2), new Vector2(AxisWidth, ContainerHeight), AxisColor);
        DrawRectangle(new Vector2(ContainerWidth / 2, ContainerHeight - YMarginTop), new Vector2(ContainerWidth, AxisWidth), AxisStepColor);
        DrawText(((int)YMax).ToString(), new Vector2(-FontSize, ContainerHeight - YMarginTop), new Vector2(4 * FontSize, FontSize), AxisColor, FontSize);
        int yAxisSteps = (int)(YMax / YStep);
        if (YMax % YStep == 0) yAxisSteps--;
        for (int i = 0; i < yAxisSteps; i++)
        {
            float yStepValue = (i + 1) * YStep;
            float y = (yStepValue / YMax) * (ContainerHeight - YMarginTop);
            DrawRectangle(new Vector2(ContainerWidth / 2, y), new Vector2(ContainerWidth, AxisWidth), AxisStepColor);
            string label = yStepValue.ToString();
            DrawText(label, new Vector2(-FontSize, y), new Vector2(4 * FontSize, FontSize), AxisColor, FontSize);
        }
    }

    public void ShowRandomBarGraph()
    {
        int n = Random.Range(3, 9);
        int maxValue = Random.Range(4, 13) * 10;
        float spacing = Random.Range(1, 4);
        int step = Random.Range(1, 3) * 10;
        List<GraphDataPoint> testList = new List<GraphDataPoint>();
        for (int i = 0; i < n; i++)
        {
            testList.Add(new GraphDataPoint("P" + i, Random.Range(0, maxValue), new Color(Random.value, Random.value, Random.value)));
        }
        ShowBarGraph(testList, maxValue, step, spacing * 0.1f, Color.white, Color.grey);
    }

    #endregion

    #region Stacked Bar Graph

    public void ShowStackedBarGraph(
    List<StackedBar> bars,
    float yMax,
    string title,
    Color axisColor,
    Color? gridColor = null,
    int ySteps = 10)
    {
        ClearGraph();

        if (bars == null || bars.Count == 0) return;

        // Layout constants (mirrors line graph)
        float paddingAbs = 10f;
        float axisSizeAbs = 50f;
        float axisThicknessAbs = 3f;
        float axisTicksLength = 10f;
        int titleSize = 20;
        int axisFontSize = 16;

        Color yStepColor = gridColor ?? new Color(0.2f, 0.2f, 0.2f);

        // Compute yMax if not provided: use max stack sum with a bit of headroom
        float maxStackSum = 0f;
        foreach (var b in bars)
            maxStackSum = Mathf.Max(maxStackSum, b.Segments?.Sum(s => Mathf.Max(0f, s.Amount)) ?? 0f);
        if (yMax <= 0f) yMax = maxStackSum <= 0f ? 1f : maxStackSum * 1.05f;

        // Derived layout
        float circleSizeAbs = 16f; // keep space budget consistent with line graph
        float graphStartXAbs = paddingAbs + axisSizeAbs + circleSizeAbs / 2f;
        float graphStartYAbs = paddingAbs + axisSizeAbs + circleSizeAbs / 2f;

        float graphWidthAbs = ContainerWidth - 2f * paddingAbs - axisSizeAbs - circleSizeAbs;
        float graphHeightAbs = ContainerHeight - 2f * paddingAbs - axisSizeAbs - titleSize - circleSizeAbs;

        int n = Mathf.Max(bars.Count, 1);
        // No gaps between bars: each bar occupies a contiguous slice of the plot area.
        float barWidthAbs = graphWidthAbs / n;

        // Title
        DrawText(title,
            new Vector2(ContainerWidth / 2f, ContainerHeight - paddingAbs - titleSize / 2f),
            new Vector2(ContainerWidth, titleSize),
            axisColor, titleSize);

        // X axis
        float xAxisYPos = paddingAbs + axisSizeAbs - axisThicknessAbs / 2f;
        DrawRectangle(new Vector2(ContainerCenterX, xAxisYPos),
                      new Vector2(ContainerWidth - 2f * paddingAbs, axisThicknessAbs), axisColor);

        // Y axis
        float yAxisXPos = paddingAbs + axisSizeAbs - axisThicknessAbs / 2f;
        DrawRectangle(new Vector2(yAxisXPos, ContainerCenterY),
                      new Vector2(axisThicknessAbs, ContainerHeight - 2f * paddingAbs), axisColor);

        // Y grid + labels
        int steps = Mathf.Max(1, ySteps);
        float yStepAbs = graphHeightAbs / steps;
        float yValueStep = yMax / steps;
        for (int i = 0; i < steps; i++)
        {
            float yPos = graphStartYAbs + (i + 1) * yStepAbs;

            // grid line
            DrawRectangle(new Vector2(graphStartXAbs + graphWidthAbs / 2f, yPos),
                          new Vector2(graphWidthAbs, 2f), yStepColor, insertInBackground: true);

            // label
            float yValue = (i + 1) * yValueStep;
            string format = (yMax <= 1f) ? "N2" : (yMax <= 100f ? "N1" : "N0");
            DrawText(yValue.ToString(format),
                     new Vector2(paddingAbs + axisSizeAbs / 2f, yPos),
                     new Vector2(axisSizeAbs, yStepAbs),
                     axisColor, axisFontSize);
        }

        // Bars
        for (int i = 0; i < n; i++)
        {
            var bar = bars[i];
            float leftX = graphStartXAbs + i * barWidthAbs;
            float rightX = leftX + barWidthAbs;
            float centerX = (leftX + rightX) * 0.5f;

            // X tick
            DrawRectangle(new Vector2(centerX, xAxisYPos),
                          new Vector2(axisThicknessAbs, axisTicksLength), axisColor);

            // X label
            DrawText(bar.Label ?? "",
                     new Vector2(centerX, paddingAbs + axisSizeAbs / 2f),
                     new Vector2(barWidthAbs, axisSizeAbs),
                     axisColor, axisFontSize);

            // Stack segments: bottom → top, contiguous (no gaps)
            float runningHeight = 0f;
            if (bar.Segments != null && bar.Segments.Count > 0)
            {
                foreach (var seg in bar.Segments)
                {
                    float segVal = Mathf.Max(0f, seg.Amount);
                    if (segVal <= 0f) continue;

                    float segHeightAbs = (segVal / yMax) * graphHeightAbs;
                    float centerY = graphStartYAbs + runningHeight + segHeightAbs * 0.5f;

                    DrawRectangle(new Vector2(centerX, centerY),
                                  new Vector2(barWidthAbs, segHeightAbs),
                                  seg.Color);

                    runningHeight += segHeightAbs;
                }
            }
        }

        // Top border
        DrawRectangle(new Vector2(ContainerWidth / 2f, graphStartYAbs + graphHeightAbs),
                      new Vector2(ContainerWidth - 2f * paddingAbs, axisThicknessAbs),
                      axisColor);
    }

    public void ShowRandomStackedBarGraph(int bars = 10, int minSegments = 2, int maxSegments = 6)
    {
        bars = Mathf.Clamp(bars, 2, 40);
        minSegments = Mathf.Max(1, minSegments);
        maxSegments = Mathf.Max(minSegments, maxSegments);

        var list = new List<StackedBar>();
        float yMax = 0f;

        for (int i = 0; i < bars; i++)
        {
            int segCount = Random.Range(minSegments, maxSegments + 1);
            var segs = new List<StackedBarData>();
            int total = 0;
            for (int s = 0; s < segCount; s++)
            {
                // random positive portions
                int amt = Random.Range(1, 20);
                total += amt;
                segs.Add(new StackedBarData($"S{s}", amt, new Color(Random.value, Random.value, Random.value, 1f)));
            }
            yMax = Mathf.Max(yMax, total);
            list.Add(new StackedBar($"B{i}", segs));
        }

        // a bit of headroom
        yMax *= 1.1f;

        ShowStackedBarGraph(list, yMax, "Random Stacked Bars", axisColor: Color.white);
    }

    #endregion

    #region Line Graph

    // Back-compat convenience overload (single line)
    public void ShowLineGraph(List<GraphDataPoint> dataPoints, float yMax, string title, Color lineColor, Color axisColor)
    {
        var lines = new List<LineData> { new LineData(title, dataPoints, lineColor, thickness: 5f) };
        ShowLineGraph(lines, yMax, title, axisColor);
    }

    // New multi-line version
    public void ShowLineGraph(List<LineData> lines, float yMax, string title, Color axisColor, bool showDataPoints = true)
    {
        ClearGraph();

        if (lines == null || lines.Count == 0)
            return;

        // Use the first line’s labels for the X axis; enforce consistent length if you rely on labels per index.
        var primary = lines[0];
        int n = Mathf.Max((primary.Points?.Count ?? 0), 1);
        if (n < 2) n = 2; // avoid div-by-zero in xStep

        // ---- Layout constants
        float paddingAbs = 10f;  // outer padding
        float axisSizeAbs = 50f;  // reserved size for axis labels
        float circleSizeAbs = 16f;  // point marker size
        float axisThicknessAbs = 3f;
        float axisTicksLength = 10f;
        int titleSize = 20;
        int axisFontSize = (int)circleSizeAbs;

        float numYAxisSteps = 10f;
        float yStepWidth = 2f;
        Color yStepColor = new Color(0.2f, 0.2f, 0.2f);

        yMax = Mathf.Approximately(yMax, 0f) ? 1f : yMax;

        // ---- Derived layout
        float graphStartXAbs = paddingAbs + axisSizeAbs + circleSizeAbs / 2f;
        float graphStartYAbs = paddingAbs + axisSizeAbs + circleSizeAbs / 2f;

        float graphWidthAbs = ContainerWidth - 2f * paddingAbs - axisSizeAbs - circleSizeAbs;
        float graphHeightAbs = ContainerHeight - 2f * paddingAbs - axisSizeAbs - titleSize - circleSizeAbs;

        float xStep = graphWidthAbs / (n - 1);

        // ---- Title
        DrawText(title,
            new Vector2(ContainerWidth / 2f, ContainerHeight - paddingAbs - titleSize / 2f),
            new Vector2(ContainerWidth, titleSize),
            axisColor, // title in axis color looks consistent
            titleSize);

        // ---- X axis line + ticks + labels
        float xAxisYPos = paddingAbs + axisSizeAbs - axisThicknessAbs / 2f;
        DrawRectangle(new Vector2(ContainerCenterX, xAxisYPos),
                      new Vector2(ContainerWidth - 2f * paddingAbs, axisThicknessAbs), axisColor);

        for (int i = 0; i < n; i++)
        {
            float xPos = graphStartXAbs + i * xStep;
            DrawRectangle(new Vector2(xPos, xAxisYPos),
                          new Vector2(axisThicknessAbs, axisTicksLength), axisColor);

            string lbl = (i < (primary.Points?.Count ?? 0)) ? primary.Points[i].Label : "";
            DrawText(lbl,
                new Vector2(xPos, paddingAbs + axisSizeAbs / 2f),
                new Vector2(xStep, axisSizeAbs),
                axisColor, axisFontSize);
        }

        // ---- Y axis line + grid + labels
        float yAxisXPos = paddingAbs + axisSizeAbs - axisThicknessAbs / 2f;
        DrawRectangle(new Vector2(yAxisXPos, ContainerCenterY),
                      new Vector2(axisThicknessAbs, ContainerHeight - 2f * paddingAbs), axisColor);

        float yStepAbs = graphHeightAbs / numYAxisSteps;
        float yValueStep = yMax / numYAxisSteps;

        for (int i = 0; i < numYAxisSteps; i++)
        {
            float yPos = graphStartYAbs + (i + 1) * yStepAbs;

            // grid line
            DrawRectangle(new Vector2(graphStartXAbs + graphWidthAbs / 2f, yPos),
                          new Vector2(graphWidthAbs, yStepWidth), yStepColor, insertInBackground: true);

            // label
            float yValue = (i + 1) * yValueStep;
            string format = (yMax <= 1f) ? "N2" : (yMax <= 100f ? "N1" : "N0");
            DrawText(yValue.ToString(format),
                new Vector2(paddingAbs + axisSizeAbs / 2f, yPos),
                new Vector2(axisSizeAbs, yStepAbs),
                axisColor, axisFontSize);
        }

        // ---- Lines (each with own color & thickness)
        foreach (var line in lines)
        {
            if (line?.Points == null || line.Points.Count == 0) continue;

            // Build node positions from this line’s values (aligned by index to primary X)
            int count = Mathf.Min(line.Points.Count, n);
            var positions = new List<Vector2>(count);
            for (int i = 0; i < count; i++)
            {
                float xPos = graphStartXAbs + i * xStep;
                float norm = Mathf.Clamp01(line.Points[i].Value / yMax);
                float yPos = graphStartYAbs + graphHeightAbs * norm;
                positions.Add(new Vector2(xPos, yPos));
            }

            // Circles
            if (showDataPoints)
            {
                for (int i = 0; i < positions.Count; i++)
                    DrawCircle(positions[i], circleSizeAbs, line.LineColor);
            }

            // Segments
            for (int i = 0; i < positions.Count - 1; i++)
                DrawLine(positions[i], positions[i + 1], line.LineColor, Mathf.Max(1f, line.Thickness));
        }
    }

    public void ShowTestLineGraph()
    {
        int n = Random.Range(5, 16);
        var l1 = new List<GraphDataPoint>();
        var l2 = new List<GraphDataPoint>();
        for (int i = 0; i < n; i++)
        {
            l1.Add(new GraphDataPoint(i.ToString(), Random.value, Color.white));
            l2.Add(new GraphDataPoint(i.ToString(), Mathf.Clamp01(Random.value * 0.7f + 0.15f), Color.white));
        }
        var lines = new List<LineData>
    {
        new LineData("A", l1, new Color(0.2f,0.7f,1f), 5f),
        new LineData("B", l2, new Color(1f,0.5f,0.2f), 3f),
    };
        ShowLineGraph(lines, 1f, "Test Graph", axisColor: Color.white);
    }

    #endregion

    #region Elements

    private void DrawCircle(Vector2 anchoredPos, float size, Color color)
    {
        GameObject circleObject = new GameObject("circle", typeof(Image));
        circleObject.transform.SetParent(GraphContainer, false);
        circleObject.GetComponent<Image>().sprite = CircleSprite;
        circleObject.GetComponent<Image>().color = color;
        RectTransform rect = circleObject.GetComponent<RectTransform>();
        rect.anchoredPosition = anchoredPos;
        rect.sizeDelta = new Vector2(size, size);
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
    }

    private GameObject DrawRectangle(Vector2 centerPos, Vector2 dimensions, Color color, bool insertInBackground = false)
    {
        GameObject obj = new GameObject("rect", typeof(Image));
        obj.transform.SetParent(GraphContainer, false);
        obj.GetComponent<Image>().color = color;
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = centerPos;
        rect.sizeDelta = dimensions;
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        if (insertInBackground) obj.transform.SetSiblingIndex(0);
        return obj;
    }

    private TextMeshProUGUI DrawText(string text, Vector2 centerPos, Vector2 dimensions, Color c, int fontSize)
    {
        GameObject obj = new GameObject(text, typeof(TextMeshProUGUI));
        obj.transform.SetParent(GraphContainer, false);
        TextMeshProUGUI textObj = obj.GetComponent<TextMeshProUGUI>();
        textObj.text = text;
        textObj.color = c;
        textObj.fontSize = fontSize;
        textObj.alignment = TextAlignmentOptions.Midline;
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = centerPos;
        rect.sizeDelta = dimensions;
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);
        return textObj;
    }

    private Image DrawImage(Sprite sprite, Vector2 centerPos, Vector2 dimensions, string tooltipTitle = "", string tooltipText = "")
    {
        GameObject obj = new GameObject("ModifierIcon", typeof(Image));
        obj.transform.SetParent(GraphContainer, false);
        Image imgObj = obj.GetComponent<Image>();
        imgObj.sprite = sprite;
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = centerPos;
        rect.sizeDelta = dimensions;
        rect.anchorMin = new Vector2(0, 0);
        rect.anchorMax = new Vector2(0, 0);

        /*
        // Tooltip
        if(tooltipTitle != "")
        {
            TooltipTarget tooltipTarget = obj.AddComponent<TooltipTarget>();
            tooltipTarget.Title = tooltipTitle;
            tooltipTarget.Text = tooltipText;
        }
        */

        return imgObj;
    }

    private Image DrawLine(Vector2 from, Vector2 to, Color color, float thickness)
    {
        // Init object
        GameObject gameObject = new GameObject("Line", typeof(Image));
        gameObject.transform.SetParent(GraphContainer, false);
        Image img = gameObject.GetComponent<Image>();
        img.color = color;

        // Position and dimensions
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (to - from).normalized;
        float distance = Vector2.Distance(from, to);
        rectTransform.anchoredPosition = new Vector2((to.x + from.x) / 2, (to.y + from.y) / 2);
        rectTransform.sizeDelta = new Vector2(distance, thickness);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        // Rotation
        rectTransform.localEulerAngles = new Vector3(0, 0, GetDirectionVectorAngle(dir));
        // gameObject.transform.SetSiblingIndex(0);

        return img;
    }

    #endregion

    #region Private Helper Functions

    /// <summary>
    /// Converts a relative x value within the graph container to an absolute pixel value.
    /// </summary>
    private float RelToAbsX(float f) => ContainerWidth * f;

    /// <summary>
    /// Converts an absolute x value within the graph container to a relative one.
    /// </summary>
    private float AbsToRelX(float f) => f / ContainerWidth;

    /// <summary>
    /// Converts a relative y value within the graph container to an absolute pixel value.
    /// </summary>
    private float RelToAbsY(float f) => ContainerHeight * f;

    /// <summary>
    /// Converts an absolute y value within the graph container to a relative one.
    /// </summary>
    private float AbsToRelY(float f) => f / ContainerHeight;

    private float GetDirectionVectorAngle(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }

    #endregion

    #region Debug

    

    #endregion
}
