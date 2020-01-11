using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardList
{
    public static List<Card> Cards;

    public static void InitCardList()
    {
        Cards = new List<Card>();

        Cards.Add(new C000_DoNothing());
        Cards.Add(new C001_SummonYellow());
        Cards.Add(new C002_SummonRed());
        Cards.Add(new C003_SummonBlue());
        Cards.Add(new C004_GreenBlood());
        Cards.Add(new C005_DestroyTwo());
        Cards.Add(new C006_DirectAction());
        Cards.Add(new C007_DestroyRedAndYellow());
        Cards.Add(new C008_DestroyGreenAndBlue());
        Cards.Add(new C009_SecondChance());
        Cards.Add(new C010_DecoyArmy());
        Cards.Add(new C011_Invest());
    }
}
