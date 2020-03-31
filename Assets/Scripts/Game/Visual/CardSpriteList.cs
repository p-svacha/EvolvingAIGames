using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public static class CardSpriteList
{
    private static List<Sprite> cardSprites;

    public static List<Sprite> CardSprites
    {
        get
        {
            if (cardSprites == null)
            {
                cardSprites = new List<Sprite>();
                int counter = 0;
                string numberWithLeadingZeros = counter.ToString("000");
                string path = "Assets/Prefabs/Cards/CardPictures/C" + numberWithLeadingZeros + ".png";
                Sprite sprite = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
                while (sprite != null)
                {
                    cardSprites.Add(sprite);
                    counter++;
                    numberWithLeadingZeros = counter.ToString("000");
                    path = "Assets/Prefabs/Cards/CardPictures/C" + numberWithLeadingZeros + ".png";
                    sprite = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
                }
            }
            return cardSprites;
        }
    }
}
