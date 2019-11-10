using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

// The object holding this script must be anchored to the bottom left!!
public class SpeciesDistribution : UIElement {

    List<List<Species>> Steps = new List<List<Species>>();
    public int MaxSteps = 20;

    public void AddStep(List<Species> speciesStep)
    {
        float xStepLength = ContainerWidth / MaxSteps;
        List<Species> species = new List<Species>();
        foreach (Species s in speciesStep)
        {
            Species newS = new Species(s.Id, s.Representative, s.Color, s.GenerationsWithoutImprovement);
            newS.AverageFitness = s.AverageFitness;
            newS.OffspringCount = s.OffspringCount;
            foreach (Subject sub in s.Subjects) newS.Subjects.Add(sub);
            species.Add(newS);
        }
        foreach (GameObject go in objects) GameObject.Destroy(go);
        objects.Clear();
        if (Steps.Count == MaxSteps) Steps.RemoveAt(0);
        Steps.Add(species);

        for (int i = 0; i < Steps.Count; i++)
        {
            List<Species> step = Steps[i];
            float totalSubjects = step.Sum(x => x.Size);

            List<Species> ordered = step.OrderBy(x => x.Id).ToList();
            float yPos = 0;
            foreach(Species spc in step)
            {
                float yLength = (spc.Size / totalSubjects) * ContainerHeight;
                yPos += yLength;

                GameObject gameObject = new GameObject("speciesbar", typeof(Image));
                gameObject.transform.SetParent(Container, false);
                gameObject.GetComponent<Image>().color = spc.Color;
                RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(0, 0);
                rectTransform.sizeDelta = new Vector2(xStepLength, yLength);
                rectTransform.anchoredPosition = new Vector2(i * xStepLength + (xStepLength / 2), yPos - (yLength / 2));
                objects.Add(gameObject);
            }
        }

    }
}
