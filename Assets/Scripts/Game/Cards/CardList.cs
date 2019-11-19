using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CardList
{
    public static List<Card> Cards;

    public static void InitCardList()
    {
        int counter = 1;

        Cards = new List<Card>();

        Cards.Add(new C001_DoNothing(counter++));
        Cards.Add(new C002_SummonYellow(counter++));
        Cards.Add(new C003_SummonRed(counter++));
        Cards.Add(new C004_SummonBlue(counter++));
        Cards.Add(new C005_SummonGreen(counter++));
        Cards.Add(new C006_DestroyTwo(counter++));
        Cards.Add(new C007_DealDirect(counter++));
        Cards.Add(new C008_DestroyRedAndYellow(counter++));
        Cards.Add(new C009_DestroyGreenAndBlue(counter++));
        Cards.Add(new C010_SecondChance(counter++));
        Cards.Add(new C011_DecoyArmy(counter++));
    }
}
