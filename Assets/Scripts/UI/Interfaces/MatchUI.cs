using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MatchUI : MonoBehaviour
{
    public Match Match;

    // Dimensions
    public float CanvasWidth;
    public float CanvasHeight;
    public float Factor;

    // Elements
    public VisualCard VisualCard;
    public VisualCard VisualCardHidden;
    public Text TurnText;

    public Playerbar Player1Bar;
    public Playerbar Player2Bar;

    public CardValues Player1CV;
    public CardValues Player2CV;
    public GenomeVisualizer Player1GenomeVis;
    public GenomeVisualizer Player2GenomeVis;

    public Button HideCardsButton;
    public Text HideCardsButtonText;

    // Effects
    public ParticleSystem PS_CardOptionIncrease;

    // Card Display
    public Texture DestabilizedTexture;
    public List<VisualCard> DisplayedCards;
    private float CardMarginX = 0.15f;
    private float CardMarginY = 0.1f;
    private float CardGapX; // 0.5 = The gap between two cards is the width of 0.5 cards
    private bool CardsAreHidden;

    void Start()
    {
        CanvasWidth = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.x;
        CanvasHeight = GameObject.Find("Canvas").GetComponent<RectTransform>().sizeDelta.y;
        Factor = CanvasWidth / CanvasHeight;

        HideCardsButtonText.text = "Hide Cards";
        HideCardsButton.onClick.AddListener(HideCardsButtonOnClick);
        HideCardsButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// Updates the player bars, which inclues name, health and money
    /// </summary>
    public void UpdatePlayerBar()
    {
        Player1Bar.UpdatePlayerbar(Match, Match.Player1);
        Player2Bar.UpdatePlayerbar(Match, Match.Player2);
    }

    public void UpdatePlayerGenomes()
    {
        Player1GenomeVis.VisualizeGenome(((AIPlayer)Match.Player1).Brain.Genome, true, false);
        Player2GenomeVis.VisualizeGenome(((AIPlayer)Match.Player2).Brain.Genome, true, false);
    }

    public void UpdateTurnText()
    {
        TurnText.text = "Turn " + Match.Turn;
    }

    public void ShowCards(List<Card> options, Player player, bool hidden, bool selectable)
    {
        CardGapX = options.Count <= 2 ? 2f : options.Count <= 3 ? 1f : options.Count <= 5 ? 0.5f : 0.1f;

        // Position constants
        float xStep = (1 - 2 * CardMarginX) / (options.Count);
        float cardWidth = xStep * (1 / (1 + CardGapX));
        float xGap = xStep - cardWidth;
        float cardHeight = cardWidth * 1.5f * Factor;

        for (int i = 0; i < options.Count; i++)
        {
            // Instantiate Visual Card
            VisualCard vc;
            if (hidden)
            {
                vc = GameObject.Instantiate(VisualCardHidden, transform);
            }
            else
            {
                vc = GameObject.Instantiate(VisualCard, transform);
                vc.ShowCardContent(options[i]);
                if (options[i] == player.ChosenCard) vc.HighlightCard(Color.red);
                else if (player.BestOptions.Contains(options[i])) vc.HighlightCard(Color.yellow);
            }

            // Set Card Attributes
            vc.InitVisualCard(options[i], player, selectable);

            // Set Card Position
            float xStart = CardMarginX + i * xStep + xGap / 2;
            float xEnd = CardMarginX + i * xStep + cardWidth + xGap / 2;
            float yStart = player == Match.Player2 ? CardMarginY : 1 - CardMarginY - cardHeight;
            float yEnd = player == Match.Player2 ? CardMarginY + cardHeight : 1 - CardMarginY;

            RectTransform rectTransform = vc.GetComponent<RectTransform>();
            rectTransform.anchoredPosition = new Vector2(0, 0);
            rectTransform.sizeDelta = new Vector2(0, 0);
            rectTransform.anchorMin = new Vector2(xStart, 1 - yEnd);
            rectTransform.anchorMax = new Vector2(xEnd, 1 - yStart);

            DisplayedCards.Add(vc);
        }
    }

    public void UnshowAllCards()
    {
        foreach (VisualCard vc in DisplayedCards) GameObject.Destroy(vc.gameObject);
        DisplayedCards.Clear();
    }

    private void HideCardsButtonOnClick()
    {
        if(CardsAreHidden)
        {
            HideCardsButtonText.text = "Hide Cards";
            foreach (VisualCard card in DisplayedCards) card.gameObject.SetActive(true);
        }
        else
        {
            HideCardsButtonText.text = "Show Cards";
            foreach (VisualCard card in DisplayedCards) card.gameObject.SetActive(false);
        }
        CardsAreHidden = !CardsAreHidden;
    }

    public void SetHideCardsButtonVisible(bool vis)
    {
        HideCardsButton.gameObject.SetActive(vis);
    }

}
