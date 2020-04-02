using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class SpeciesScoreboard : UIElement
{
    public int NumSpecies = 10;
    private List<Species> species;

    private List<Tuple<string, float>> Columns; // Column name, column start, column end

    private void Start()
    {
        Columns = new List<Tuple<string, float>>()
        {
            new Tuple<string, float>("Id", 0.1f),
            new Tuple<string, float>("Subjects", 0.3f),
            new Tuple<string, float>("# Under Limit", 0.2f),
            new Tuple<string, float>("Max Fitness", 0.2f),
            new Tuple<string, float>("Avg Fitness", 0.2f),
        };
    }

    public void UpdateScoreboard(Population p)
    {
        Clear();

        // Get best subjects of population
        int amount = Mathf.Min(NumSpecies, p.Species.Count);
        species = p.Species.OrderByDescending(x => x.AverageFitness).Take(amount).ToList();

        // Calculate step size
        float step = (float)(1 / ((float)NumSpecies + 2));
        int fontSize = 16;

        // Add title
        AddText("Species Standings", fontSize, Color.black, FontStyle.Bold, 0, 0, 1, 1f / (NumSpecies + 1), Container);

        // Add title Panel
        Color titleColor = Color.black;
        RectTransform titlePanel = AddPanel("TitlePanel", Color.clear, 0, (1f / (NumSpecies + 1)), 1, (1f / (NumSpecies + 1)) + step, Container);
        for(int i = 0; i < Columns.Count; i++)
        {
            float xStart = Columns.Where(x => Columns.IndexOf(x) < i).Sum(x => x.Item2);
            AddText(Columns[i].Item1, fontSize, titleColor, FontStyle.Normal, xStart, 0, xStart + Columns[i].Item2, 1, titlePanel);
        }

        // Add Species Panels
        for (int i = 0; i < amount; i++)
        {
            Color fontColor = species[i].Color.r + species[i].Color.g + species[i].Color.b < 1f ? Color.white : Color.black;
            RectTransform panel = AddPanel((i + 2) + "", species[i].Color, 0, (1f / (NumSpecies + 1)) + ((i + 1) * step), 1, (1f / (NumSpecies + 1)) + ((i + 2) * step), Container);
            for(int j = 0; j < Columns.Count; j++)
            {
                string text = "///";
                switch(j)
                {
                    case 0: text = species[i].Id + ""; break;
                    case 1: text = species[i].Size + ""; break;
                    case 2: text = species[i].GenerationsWithoutImprovement + ""; break;
                    case 3: text = species[i].MaxFitness + ""; break;
                    case 4: text = species[i].AverageFitness.ToString("0.0") + ""; break;
                }
                float xStart = Columns.Where(x => Columns.IndexOf(x) < j).Sum(x => x.Item2);
                AddText(text, fontSize, fontColor, FontStyle.Normal, xStart, 0, xStart + Columns[j].Item2, 1, panel);
            }
        }
    }
}
