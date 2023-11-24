using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BlobCardGame
{
    public static class CardList
    {
        public static List<Card> Cards;

        public static void InitCardList()
        {
            Cards = new List<Card>();

            Cards.Add(new C000_SkipTurn());
            Cards.Add(new C001_YellowTrio());
            Cards.Add(new C002_SummonRed());
            Cards.Add(new C003_SummonBlue());
            Cards.Add(new C004_GreenBlood());
            Cards.Add(new C005_DestroyTwo());
            Cards.Add(new C006_DirectAction());
            Cards.Add(new C007_DestroyRedAndYellow());
            Cards.Add(new C008_TargetBomb());
            Cards.Add(new C009_SecondChance());
            Cards.Add(new C010_DecoyArmy());
            Cards.Add(new C011_Invest());
            Cards.Add(new C012_RedArmy());
            Cards.Add(new C013_Sacrifice());
            Cards.Add(new C014_Steal());
            Cards.Add(new C015_RedDuo());
            Cards.Add(new C016_Rainbow());
            Cards.Add(new C017_TacticalNuke());
            Cards.Add(new C018_Shotgun());
            Cards.Add(new C019_ClassicDuo());

            if (Cards.GroupBy(x => x.Id).Count() != Cards.Count) throw new System.Exception("Not all card id's are unique!");
        }
    }
}
