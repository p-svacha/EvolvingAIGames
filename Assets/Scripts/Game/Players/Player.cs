using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Player : Creature
{
    public Subject Brain;

    public int MaxHealth;
    public int Health;
    public int NumCardOptions;
    public Card ChosenCard;

    public Player(MatchModel model, string name, Subject brain) : base(model)
    {
        Name = name;
        Brain = brain;
    }

    public void Initialize(Player enemy, int health, int options)
    {
        Enemy = enemy;
        MaxHealth = health;
        Health = health;
        NumCardOptions = options;
    }

    public abstract void PickCard(List<Card> options);
}
