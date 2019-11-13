using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardList
{
    public static List<Card> Cards;

    public static void InitCardList(MatchModel model)
    {
        int counter = 1;

        Cards = new List<Card>();

        Cards.Add(new C001_DoNothing(model, counter++));
        Cards.Add(new C002_SummonYellow(model, counter++));
        Cards.Add(new C003_SummonRed(model, counter++));
        Cards.Add(new C004_SummonBlue(model, counter++));
        Cards.Add(new C005_SummonGreen(model, counter++));
    }
}
