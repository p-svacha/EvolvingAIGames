using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SpeciesScoreboard : UIElement
{
    public int NumSpecies = 10;
    private List<Species> species;

    public void UpdateScoreboard(Population p)
    {
        Clear();

        // Get best subjects of population
        int rows = Mathf.Max(NumSpecies, p.Species.Count);
        int amount = Mathf.Min(rows, p.Species.Count);
        species = p.Species.OrderByDescending(x => x.AverageFitness).Take(amount).ToList();

        // Calculate step size
        float step = (float)(1 / ((float)rows + 1));
        int fontSize = 16;

        // Add title
        AddText("Species Standings", fontSize, Color.black, FontStyle.Bold, 0, 0, 1, 1f / (NumSpecies + 1), Container);

        // Add Driver Panels
        for (int i = 0; i < amount; i++)
        {
            Color fontColor = species[i].Color.r + species[i].Color.g + species[i].Color.b < 1f ? Color.white : Color.black;
            RectTransform panel = AddPanel((i + 1) + "", species[i].Color, 0, (1f / (NumSpecies + 1)) + (i * step), 1, (1f / (NumSpecies + 1)) + ((i + 1) * step), Container);
            AddText(species[i].Size + " Cars", fontSize, fontColor, FontStyle.Normal, 0.1f, 0, 0.6f, 1, panel, TextAnchor.MiddleLeft);
            AddText(species[i].GenerationsWithoutImprovement + "", fontSize/4*3, fontColor, FontStyle.Normal, 0.6f, 0, 0.7f, 1, panel, TextAnchor.MiddleCenter);
            AddText((int)species[i].AverageFitness + "", fontSize, fontColor, FontStyle.Normal, 0.7f, 0, 0.9f, 1, panel, TextAnchor.MiddleRight);
        }
    }
}
