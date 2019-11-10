using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// The object holding this script must be anchored to the bottom left!!
public class GenomeVisualizer : UIElement {

    public Sprite CircleSprite;
    public Text FitnessText;

    public float CircleSize;


    public void VisualizeSubject(Genome g)
    {
        ClearVisualization();

        // Set background according to species
        if(g.Species != null) gameObject.GetComponent<Image>().color = g.Species.Color;

        // Set fitness text
        if(FitnessText!= null) FitnessText.text = g.Fitness.ToString("0.#") + " / " + g.AdjustedFitness.ToString("0.#");

        // Set fixed y positions of inputs and outputs
        for (int i = 0; i < g.InputNodes.Count; i++)
        {
            if(g.InputNodes.Count == 1) g.InputNodes[i].VisualYPosition = Margin + (ContainerHeight / (g.InputNodes.Count + 1)) * (i + 1);
            else g.InputNodes[i].VisualYPosition = Margin + (ContainerHeight / (g.InputNodes.Count - 1)) * i;
        }
        for (int i = 0; i < g.OutputNodes.Count; i++)
        {
            if (g.OutputNodes.Count == 1) g.OutputNodes[i].VisualYPosition = Margin + (ContainerHeight / (g.OutputNodes.Count + 1)) * (i + 1);
            else g.OutputNodes[i].VisualYPosition = Margin + (ContainerHeight / (g.OutputNodes.Count - 1)) * i;
        }

        // Set position of all nodes
        for (int i = 0; i < g.Depth + 1; i++)
        {
            float xPosition = Margin + (ContainerWidth / g.Depth) * i;
            List<Node> depthLayerNodes = g.Nodes.Where(x => x.Depth == i).ToList();
            for(int j = 0; j < depthLayerNodes.Count; j++)
            {
                Node node = depthLayerNodes[j];
                if (node.Type == NodeType.Hidden)
                {
                    List<Node> connectedNodes = node.InConnections.Select(x => x.In).Concat(node.OutConnections.Select(x => x.Out)).ToList();
                    node.VisualYPosition = connectedNodes.Average(x => x.VisualYPosition);
                }
                node.VisualNode = CreateCircle(new Vector2(xPosition, node.VisualYPosition), Color.black, node.Id + ""); //node.Value.ToString("0.##")
            }
        }

        foreach (Connection c in g.Connections.Where(x => x.Enabled)) c.VisualConnection = CreateDotConnection(c.In.VisualNode.transform.localPosition, c.Out.VisualNode.transform.localPosition, c.Weight <= 0 ? Color.white : Color.black, (Math.Abs(c.Weight) * 6) + 0.5f, "");
    }

    public void ClearVisualization()
    {
        foreach (GameObject go in objects) GameObject.Destroy(go);
        objects.Clear();
        gameObject.GetComponent<Image>().color = Color.clear;
    }

    public void UpdateValues(Genome g, bool displayOutputsAsBool)
    {
        foreach (Node n in g.Nodes)
        {
            if (n.Type == NodeType.Output && displayOutputsAsBool)
            {
                Color color = n.Value > 0.5f ? new Color(1, 1, 1) : new Color(0, 0, 0);
                n.VisualNode.GetComponent<Image>().color = color;
            }
            else
                n.VisualNode.GetComponent<Image>().color = new Color(n.Value, n.Value, n.Value);
        }
    }

    private GameObject CreateCircle(Vector2 anchoredPosition, Color color, string nodeText)
    {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(Container, false);
        gameObject.GetComponent<Image>().sprite = CircleSprite;
        gameObject.GetComponent<Image>().color = color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(CircleSize, CircleSize);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        objects.Add(gameObject);

        GameObject textElement = new GameObject("circleText");
        textElement.transform.SetParent(Container, false);
        Text text = textElement.AddComponent<Text>();
        text.text = nodeText;
        Font ArialFont = (Font)Resources.GetBuiltinResource(typeof(Font), "Arial.ttf");
        text.font = ArialFont;
        text.material = ArialFont.material;
        text.color = Color.red;
        text.fontSize = (int)(CircleSize * 0.75);
        text.alignment = TextAnchor.MiddleCenter;
        RectTransform textRect = textElement.GetComponent<RectTransform>();
        textRect.anchoredPosition = anchoredPosition;
        textRect.sizeDelta = new Vector2(CircleSize * 2, CircleSize * 2);
        textRect.anchorMin = new Vector2(0, 0);
        textRect.anchorMax = new Vector2(0, 0);
        objects.Add(textElement);

        return gameObject;
    }

    private GameObject CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB, Color color, float thickness, string conText)
    {
        GameObject gameObject = new GameObject("dotConnection", typeof(Image));
        gameObject.transform.SetParent(Container, false);
        gameObject.GetComponent<Image>().color = color;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchoredPosition = new Vector2((dotPositionB.x + dotPositionA.x) / 2, (dotPositionB.y + dotPositionA.y) / 2);
        rectTransform.sizeDelta = new Vector2(distance, thickness);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);

        //rectTransform.anchoredPosition = (dotPositionA - new Vector2(graphPositionX, graphPositionY)) + dir * distance * .5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, GetAngleFromVectorFloat(dir));
        gameObject.transform.SetSiblingIndex(0);
        objects.Add(gameObject);

        return gameObject;
    }

    private float GetAngleFromVectorFloat(Vector3 dir)
    {
        dir = dir.normalized;
        float n = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        if (n < 0) n += 360;

        return n;
    }
}
