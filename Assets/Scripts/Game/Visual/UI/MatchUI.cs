using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchUI : MonoBehaviour
{
    public Match Model;

    // Dimensions
    public float CanvasWidth;
    public float CanvasHeight;
    public float Factor;

    // Elements
    public VisualCard VisualCard;
    public Text TurnText;
    public Text Player1Name;
    public Text Player2Name;
    public Text Player1Health;
    public Text Player2Health;
    public CardValues Player1CV;
    public CardValues Player2CV;
    public GenomeVisualizer Player1GV;
    public GenomeVisualizer Player2GV;

    // Card Display
    public List<Sprite> CardSprites;
    public List<VisualCard> DisplayedCards;
    private float CardMarginX = 0.15f;
    private float CardMarginY = 0.1f;
    private float CardGapX = 1f; // 0.5 = The gap between two cards is the width of 0.5 cards

    public void UpdatePlayerHealth()
    {
        Player1Health.text = Model.Player1.Health + "/" + Model.Player1.MaxHealth;
        Player2Health.text = Model.Player2.Health + "/" + Model.Player2.MaxHealth;
    }

    public void UpdatePlayerNames()
    {
        Player1Name.text = Model.Player1.Name + " | " + Model.Player1.Brain.Wins + " - " + Model.Player1.Brain.Losses;
        Player2Name.text = Model.Player2.Name + " | " + Model.Player2.Brain.Wins + " - " + Model.Player2.Brain.Losses;
    }

    public void UpdateTurnText()
    {
        TurnText.text = "Turn " + Model.Turn;
    }

    public void ShowCards(List<Card> options, Player player)
    {
        // Position constants
        float xStep = (1 - 2 * CardMarginX) / (options.Count);
        float cardWidth = xStep * (1 / (1 + CardGapX));
        float xGap = xStep - cardWidth;
        float cardHeight = cardWidth * 1.5f * Factor;

        for (int i = 0; i < options.Count; i++)
        {
            // Instantiate Visual Card
            VisualCard vc = GameObject.Instantiate(VisualCard, transform);
            vc.Image.sprite = CardSprites[options[i].Id - 1];
            vc.Title.text = options[i].Name;
            vc.Description.text = options[i].Text;

            // Set Card Position
            float xStart = CardMarginX + i * xStep + xGap / 2;
            float xEnd = CardMarginX + i * xStep + cardWidth + xGap / 2;
            float yStart = player == Model.Player2 ? CardMarginY : 1 - CardMarginY - cardHeight;
            float yEnd = player == Model.Player2 ? CardMarginY + cardHeight : 1 - CardMarginY;

            RectTransform rectTransform = vc.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(0, 0);
            rectTransform.anchorMin = new Vector2(xStart, 1 - yEnd);
            rectTransform.anchorMax = new Vector2(xEnd, 1 - yStart);

            // Highlight selected Card
            if(options[i] == player.ChosenCard)
            {
                vc.GetComponent<Image>().color = Color.red;
            }

            DisplayedCards.Add(vc);
        }
    }

    public void UnshowAllCards()
    {
        foreach (VisualCard vc in DisplayedCards) GameObject.Destroy(vc.gameObject);
        DisplayedCards.Clear();
    }

    void Start()
    {
        CanvasWidth = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x;
        CanvasHeight = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.y;
        Factor = CanvasWidth / CanvasHeight;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
