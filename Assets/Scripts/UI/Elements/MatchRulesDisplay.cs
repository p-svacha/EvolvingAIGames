using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchRulesDisplay : UIElement
{

    public void UpdateStatistics(int startHealth, int startCardOptions, int fatigueDamageStartTurn, int maxMinions, int maxMinionsPerType)
    {
        Clear();

        // Title
        float titleSize = 1f / 6f;
        int fontSize = 16;
        string titleText = "Match Rules";
        AddText(titleText, fontSize, Color.black, FontStyle.Bold, 0, 0, 1, titleSize, Container, TextAnchor.MiddleCenter);

        float numColumns = 2;
        int nRows = 4;
        float yStep = 0;

        AddColumn(nRows, yStep, titleSize, yStep + (1 / numColumns), 1, true,
            new string[] { "General", "Start Health: " + startHealth, "Card Options: " + startCardOptions, "Fatigue Start Turn: " + fatigueDamageStartTurn });
        yStep += 1f / numColumns;

        AddColumn(nRows, yStep, titleSize, yStep + (1 / numColumns), 1, true,
            new string[] { "Minions", "Max Minions: " + maxMinions, "Max Minions / Type: " + maxMinionsPerType });
        yStep += 1f / numColumns;
    }

    private void AddColumn(int nRows, float xStart, float yStart, float xEnd, float yEnd, bool hasTitle, string[] content)
    {
        float step = 1f / nRows;
        int fontSize = 12;
        RectTransform column = AddPanel("column", Color.clear, xStart, yStart, xEnd, yEnd, Container);
        for(int i = 0; i < content.Length; i++)
            AddText(content[i], fontSize, Color.black, (i == 0 && hasTitle) ? FontStyle.Bold : FontStyle.Normal, 0, i * step, 1, (i + 1) * step, column, TextAnchor.MiddleLeft);
    }
}
