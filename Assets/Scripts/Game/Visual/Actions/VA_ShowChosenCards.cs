using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VA_ShowChosenCards : VisualAction 
{
    Player Player;
    Card Card;
    VisualCard VisualCard;
    MatchUI MatchUI;

    public Vector3 SourcePosition;
    public Vector3 TargetPosition;

    public VA_ShowChosenCards(Player player, Card card, MatchUI matchUI)
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
        float y = Player == MatchUI.Match.Player1 ? yMargin : MatchUI.CanvasHeight - yMargin;

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
