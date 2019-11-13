using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Card
{
    public int Id;
    public MatchModel Model;
    public string Name;
    public string Text;

    public Card(MatchModel model, int id)
    {
        Model = model;
        Id = id;
    }

    public abstract void Action(Player self, Player enemy);
}
