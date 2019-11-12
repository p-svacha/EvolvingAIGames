using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardList
{
    public static List<Card> Cards;

    public static void InitCardList(MatchModel model)
    {
        Cards = new List<Card>();

        Cards.Add(new C001_DoNothing(model));
        Cards.Add(new C002_SummonYellow(model));
        Cards.Add(new C003_SummonRed(model));
        Cards.Add(new C004_SummonBlue(model));
        Cards.Add(new C005_SummonGreen(model));
    }
}
