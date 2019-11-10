using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card
{
    public MatchModel Model;
    public string Name;
    public string Text;

    public Card(MatchModel model)
    {
        Model = model;
    }

    public abstract void Action(Player self, Player enemy);
}
