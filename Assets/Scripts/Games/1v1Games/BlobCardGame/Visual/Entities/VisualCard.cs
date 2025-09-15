﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace BlobCardGame
{
    public class VisualCard : MonoBehaviour
        , IPointerClickHandler
    {
        // Visual Elements
        public Image Image;
        public Text Title;
        public Text Description;
        public Text CostText;

        // Owner and card
        public Card Card;
        public PlayerBlob Player;

        // Attributes
        public bool Selectable;

        public void InitVisualCard(Card card, PlayerBlob player, bool selectable)
        {
            Card = card;
            Player = player;
            Selectable = selectable;
        }

        // Sets the content of the Visual to display a certain card.
        public void ShowCardContent(Card card)
        {
            Image.sprite = CardSpriteList.CardSprites[card.Id];
            Title.text = card.Name;
            Description.text = card.Text;
            CostText.text = card.Cost + "";
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (Selectable) SetCardAsChosen();
        }

        private void SetCardAsChosen()
        {
            if (Player.ChosenVisualCard != null)
                Player.ChosenVisualCard.HighlightCard(Color.white);
            Player.ChosenCard = Card;
            Player.ChosenVisualCard = this;
            HighlightCard(Color.red);
        }

        public void HighlightCard(Color color)
        {
            GetComponent<Image>().color = color;
        }
    }
}
