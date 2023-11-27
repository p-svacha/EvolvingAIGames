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

    void Start()
    {
        ContainerWidth = GraphContainer.rect.width;
        ContainerHeight = GraphContainer.rect.height;

        ContainerCenterX = ContainerWidth / 2f;
        ContainerCenterY = ContainerHeight / 2f;
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

    #endregion

    #region Line Graph

    public void ShowLineGraph(List<GraphDataPoint> dataPoints, float yMax, string title)
    {
        ClearGraph();

        // Const layout attributes
        float paddingAbs = 10; // space to edge of container on all sides
        float axisSizeAbs = 50; // How much space the axis take up
        float circleSizeAbs = 16; // Size of the value nodes
        float lineThicknessAbs = 5; // Thickness of the line in the graph

        int titleSize = 20;

        float axisThicknessAbs = 3;
        float axisTicksLength = 10;

        float numYAxisSteps = 10;
        float yStepWidth = 2;
        Color yStepColor = Color.gray;

        // Init important layout values
        float graphStartXAbs = paddingAbs + axisSizeAbs + circleSizeAbs / 2;
        float graphStartYAbs = paddingAbs + axisSizeAbs + circleSizeAbs / 2;

        float graphWidthAbs = ContainerWidth - 2 * paddingAbs - axisSizeAbs - circleSizeAbs;
        float graphHeightAbs = ContainerHeight - 2 * paddingAbs - axisSizeAbs - titleSize - circleSizeAbs;

        float xStep = graphWidthAbs / (dataPoints.Count - 1);

        // Init absolute value node positions
        List<Vector2> nodePositions = new List<Vector2>();
        for (int i = 0; i < dataPoints.Count; i++)
        {
            float xPos = graphStartXAbs + (i * xStep);
            float yPos = graphStartYAbs + (graphHeightAbs * (dataPoints[i].Value / yMax));
            nodePositions.Add(new Vector2(xPos, yPos));
        }

        // Create value circle nodes
        for (int i = 0; i < dataPoints.Count; i++) DrawCircle(nodePositions[i], circleSizeAbs, dataPoints[i].Color);

        // Create connections
        for (int i = 0; i < dataPoints.Count - 1; i++) DrawLine(nodePositions[i], nodePositions[i + 1], Color.black, lineThicknessAbs);

        // Title
        DrawText(title, new Vector2(ContainerWidth / 2f, ContainerHeight - paddingAbs - titleSize / 2f), new Vector2(ContainerWidth, titleSize), Color.black, titleSize);

        // X axis
        int axisFontSize = (int)(circleSizeAbs);
        float xAxisYPos = paddingAbs + axisSizeAbs - axisThicknessAbs / 2f;
        DrawRectangle(new Vector2(ContainerCenterX, xAxisYPos), new Vector2(ContainerWidth - 2 * paddingAbs, axisThicknessAbs), Color.black);
        for (int i = 0; i < dataPoints.Count; i++)
        {
            DrawRectangle(new Vector2(nodePositions[i].x, xAxisYPos), new Vector2(axisThicknessAbs, axisTicksLength), Color.black);
            DrawText(dataPoints[i].Label, new Vector2(nodePositions[i].x, paddingAbs + axisSizeAbs / 2f), new Vector2(xStep, axisSizeAbs), Color.black, axisFontSize);
        }

        // Y axis
        float yAxisXPos = paddingAbs + axisSizeAbs - axisThicknessAbs / 2f;
        DrawRectangle(new Vector2(yAxisXPos, ContainerCenterY), new Vector2(axisThicknessAbs, ContainerHeight - 2 * paddingAbs), Color.black);
        float yStepAbs = graphHeightAbs / numYAxisSteps;
        float yValueStep = yMax / numYAxisSteps;
        for(int i = 0; i < numYAxisSteps; i++)
        {
            float yPos = graphStartYAbs + (i + 1) * yStepAbs;
            DrawRectangle(new Vector2(graphStartXAbs + graphWidthAbs / 2f, yPos), new Vector2(graphWidthAbs, yStepWidth), yStepColor, insertInBackground: true);

            // Value texts
            float yValue = (i + 1) * yValueStep;
            string format = "";
            if (yMax <= 1) format = "N2";
            else if (yMax <= 100) format = "N1";
            else format = "N0";
            DrawText(yValue.ToString(format), new Vector2(paddingAbs + axisSizeAbs / 2f, yPos), new Vector2(axisSizeAbs, yStepAbs), Color.black, axisFontSize);
        }
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
        gameObject.transform.SetSiblingIndex(0);

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

    public void ShowRandomLineGraph()
    {
        int n = Random.Range(3, 20);
        List<GraphDataPoint> values = new List<GraphDataPoint>();
        for (int i = 0; i < n; i++) values.Add(new GraphDataPoint(i.ToString(), Random.value, Color.white));
        ShowLineGraph(values, 1f, "Test Graph");
    }

    #endregion
}
