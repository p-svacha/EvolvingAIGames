using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EvolutionStatistics : UIElement
{

    public void UpdateStatistics(EvolutionInformation info)
    {
        Clear();

        // Title
        float titleSize = 1f / 6f;
        int fontSize = 16;
        string titleText = "Evolution Information for Generation " + info.Generation + " (" + info.EvolutionTime + " ms)";
        AddText(titleText, fontSize, Color.black, FontStyle.Bold, 0, 0, 1, titleSize, Container, TextAnchor.MiddleCenter);

        float numColumns = 6;
        int nRows = 7;
        float yStep = 0;

        AddColumn(nRows, yStep, titleSize, yStep + (1 / numColumns), 1, true,
            new string[] { "# Subjects: " + info.NumSubjects, "# Offsprings: " + info.NumOffsprings, "# Best: " + info.NumBestSubjectsTakenOver, "# Random: " + info.NumRandomSubjectsTakenOver });
        yStep += 1f / numColumns;

        AddColumn(nRows, yStep, titleSize, yStep + (1 / numColumns), 1, true,
            new string[] { "# Species: " + info.NumSpecies, "# Previous: " + info.NumPreviousSpecies, "# New: " + info.NumNewSpecies, "# Eliminated: " + info.NumEliminatedSpecies, "# Empty: " + info.NumEmptySpecies, "CTH: " + info.CompatibilityThreshhold });
        yStep += 1f / numColumns;

        AddColumn(nRows, yStep, titleSize, yStep + (1 / numColumns), 1, true,
            new string[] { "Gen " + (info.Generation - 1) + " Fitness", "Max: " + (int)info.MaxFitness, "Average: " + (int)info.AverageFitness });
        yStep += 1f / numColumns;

        AddColumn(nRows, yStep, titleSize, yStep + (1 / numColumns), 1, true,
            new string[] { "Misc", "# AdpChecks: " + info.NumSubjectsCheckedForAdoption, "# Immune: " + info.NumImmuneToMutationSubjects, "Rank Limit: " + info.RankLimit, "#Gens <Limit: " + info.GensAllowedBelowLimit, "Takeovers immune: " + info.MutationImmunityForTakeOvers });
        yStep += 1f / numColumns;

        AddColumn(nRows, yStep, titleSize, yStep + (1 / numColumns), 1, true,
            new string[] { "# Topology Mutations: " + info.MutationInfo.NumTopologyMutations, "# Con (new): " + info.MutationInfo.NumNewConnectionsMutations + " (" + info.MutationInfo.NumNewUniqueConnectionsMutations + ")", "# Node (new): " + info.MutationInfo.NumNewNodeMutations + " (" + info.MutationInfo.NumNewUniqueNodeMutations + ")" });
        yStep += 1f / numColumns;

        AddColumn(nRows, yStep, titleSize, yStep + (1 / numColumns), 1, true,
            new string[] { "# Weight Mutations: " + info.MutationInfo.NumWeightMutations, "# Replaces: " + info.MutationInfo.NumReplaceMutations, "# Shifts: " + info.MutationInfo.NumShiftMutations, "# Scales: " + info.MutationInfo.NumScaleMutations, "# Inverts: " + info.MutationInfo.NumInvertMutations, "# Swaps: " + info.MutationInfo.NumSwapMutations });
        yStep += 1f / numColumns;
    }

    private void AddColumn(int nRows, float xStart, float yStart, float xEnd, float yEnd, bool hasTitle, string[] content)
    {
        float step = 1f / nRows;
        int fontSize = 12;
        RectTransform column = AddPanel("column", Color.grey, xStart, yStart, xEnd, yEnd, Container);
        for(int i = 0; i < content.Length; i++)
            AddText(content[i], fontSize, Color.black, (i == 0 && hasTitle) ? FontStyle.Bold : FontStyle.Normal, 0, i * step, 1, (i + 1) * step, column, TextAnchor.MiddleLeft);
    }
}
