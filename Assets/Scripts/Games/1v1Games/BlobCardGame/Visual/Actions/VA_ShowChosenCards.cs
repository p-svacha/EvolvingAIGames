using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BlobCardGame
{
    public class VA_ShowChosenCards : VisualAction
    {
        PlayerBlob Player;
        Card Card;
        VisualCard VisualCard;
        BlobCardMatchUI MatchUI;

        public Vector3 SourcePosition;
        public Vector3 TargetPosition;

        public VA_ShowChosenCards(PlayerBlob player, Card card, BlobCardMatchUI matchUI)
        {
            Frames = DefaultFrames * 1.5f;
            Player = player;
            Card = card;
            MatchUI = matchUI;
        }

        public override void OnStart()
        {
            base.OnStart();

            float cardWidth = 200;
            float cardHeight = 300;

            VisualCard = GameObject.Instantiate(MatchUI.VisualCard, MatchUI.transform);
            VisualCard.ShowCardContent(Card);
            RectTransform rect = VisualCard.GetComponent<RectTransform>();
            rect.sizeDelta = new Vector2(cardWidth, cardHeight);

            float travelDistance = 100;
            float startX = MatchUI.CanvasWidth / 2 - travelDistance / 2;
            float yMargin = 200;
            float y = Player == MatchUI.BlobMatch.Player1 ? yMargin : MatchUI.CanvasHeight - yMargin;

            SourcePosition = new Vector3(startX, y, 0);
            TargetPosition = SourcePosition + new Vector3(travelDistance, 0, 0);

            VisualCard.transform.position = SourcePosition;
        }

        public override void Update()
        {
            base.Update();

            VisualCard.transform.position = Vector3.Lerp(SourcePosition, TargetPosition, CurrentFrame / Frames);
        }

        public override void OnDone()
        {
            base.OnDone();

            GameObject.Destroy(VisualCard.gameObject);
        }
    }
}
