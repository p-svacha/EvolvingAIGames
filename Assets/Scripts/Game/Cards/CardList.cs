using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardList
{
    public static List<Card> Cards;

    public static void InitCardList(MatchModel model)
    {
        Cards = new List<Card>();

        Cards.Add(new C001_SummonRed(model));
        Cards.Add(new C002_Summon2Reds(model));
        Cards.Add(new C003_Summon3Reds(model));
    }
}
