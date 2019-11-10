using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// !!!!!!Elements holding this script must have a rect transform preset (like center middle or whatever)!!!!!
/// </summary>
public class UIElement : MonoBehaviour
{
    protected List<GameObject> objects;

    protected RectTransform Container;
    public float ContainerWidth, ContainerHeight, ContainerX, ContainerY, Margin;


    public void Awake()
    {
        objects = new List<GameObject>();
        Container = gameObject.GetComponent<RectTransform>();
        ContainerWidth = Container.sizeDelta.x - 2 * Margin;
        ContainerHeight = Container.sizeDelta.y - 2 * Margin;
        ContainerX = Container.anchoredPosition.x;
        ContainerY = Container.anchoredPosition.y;
    }


    /// <summary>
    /// Add a panel element. xStart, xEnd, yStart, yEnd are percentage values (between 0 and 1).
    /// </summary>
    protected RectTransform AddPanel(string name, Color backgroundColor, float xStart, float yStart, float xEnd, float yEnd, RectTransform parent)
    {
        GameObject panel = new GameObject(name);
        panel.transform.SetParent(parent, false);

        Image image = panel.AddComponent<Image>();
        image.color = backgroundColor;

        RectTransform rectTransform = panel.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(0, 0);
        rectTransform.anchorMin = new Vector2(xStart, 1 - yEnd);
        rectTransform.anchorMax = new Vector2(xEnd, 1 - yStart);
        objects.Add(panel);

        return rectTransform;
    }


    /// <summary>
    /// Add a text element. xStart, xEnd, yStart, yEnd are percentage values (between 0 and 1).
    /// </summary>
    protected GameObject AddText(string content, int fontSize, Color fontColor, FontStyle fontStyle, float xStart, float yStart, float xEnd, float yEnd, RectTransform parent, TextAnchor textAnchor = TextAnchor.MiddleCenter)
    {
        GameObject textElement = new GameObject(content);
        textElement.transform.SetParent(parent, false);
        Text text = textElement.AddComponent<Text>();
        text.text = content;
        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = ArialFont;
        text.material = ArialFont.material;
        text.fontStyle = fontStyle;
        text.color = fontColor;
        text.fontSize = fontSize;
        text.alignment = textAnchor;

        RectTransform textRect = textElement.GetComponent<RectTransform>();
        textRect.anchoredPosition = new Vector2(0, 0);
        textRect.sizeDelta = new Vector2(0, 0);
        textRect.anchorMin = new Vector2(xStart, 1 - yEnd);
        textRect.anchorMax = new Vector2(xEnd, 1 - yStart);
        objects.Add(textElement);

        return textElement;
    }

    /// <summary>
    /// Destroys all elements in this UI Element.
    /// </summary>
    protected void Clear()
    {
        foreach (GameObject go in objects) GameObject.Destroy(go);
        objects.Clear();
    }
}
