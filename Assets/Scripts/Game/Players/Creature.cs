using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Creatures are possible sources for actions, includes Players and Minions.
public abstract class Creature
{
    public Match Model;
    public string Name;
    public Player Enemy;
    public VisualEntity Visual;
    public Color Color;


    public Creature(Match model)
    {
        Model = model;
    }
}
