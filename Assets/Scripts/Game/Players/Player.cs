using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Player : Creature
{
    // Fixed values
    public int MaxHealth;

    // Changing values
    public int Health;
    public int NumCardOptions;
    public int Money;

    public Card ChosenCard;
    public VisualCard ChosenVisualCard;
    public List<Card> BestOptions; // List of cards with the shared highest value. (ChosenCard was chosen randomly from this list)

    // Card Statistics
    public Dictionary<int, int> CardsPicked;
    public Dictionary<int, int> CardsNotPicked;

    public Player(Match model, string name) : base(model)
    {
        Name = name;
        CardsPicked = new Dictionary<int, int>();
        CardsNotPicked = new Dictionary<int, int>();
        BestOptions = new List<Card>();
        for(int i = 0; i < CardList.Cards.Count; i++)
        {
            CardsPicked.Add(i, 0);
            CardsNotPicked.Add(i, 0);
        }
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
