using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// A subject represents a participant in one generation of the simulation process with a static genome.
/// <br/> In match-based simulations, subjects will create a match-specific player instance for each match that handles match-specific data.
/// </summary>
public class Subject {

    public string Name { get; set; }
    public int OverallRank { get; set; }
    public Genome Genome { get; set; }
    public bool ImmuneToMutation { get; set; }

    public Subject(Genome g)
    {
        Genome = g;
    }
}
