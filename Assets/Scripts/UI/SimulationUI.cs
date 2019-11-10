using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SimulationUI : MonoBehaviour
{
    public Text Title;
    public SpeciesScoreboard SpeciesScoreboard;
    public SpeciesDistribution SpeciesDistribution;
    public GenomeVisualizer GenomeVisualizer;
    public EvolutionStatistics EvolutionStatistics;

    public void UpdateScoreboards(Population p)
    {
        SpeciesScoreboard.UpdateScoreboard(p);
        GenomeVisualizer.VisualizeSubject(p.Subjects.OrderByDescending(x => x.Genome.Fitness).First().Genome);
    }

    public void UpdateEvolutionStatistics(EvolutionInformation info)
    {
        EvolutionStatistics.UpdateStatistics(info);
    }

    public void UpdateSpeciesDistribution(Population p)
    {
        SpeciesDistribution.AddStep(p.Species);
    }
}
