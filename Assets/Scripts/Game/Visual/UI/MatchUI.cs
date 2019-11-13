using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchUI : MonoBehaviour
{
    public MatchModel Model;

    // Dimensions
    public float CanvasWidth;
    public float CanvasHeight;

    // Elements
    public VisualCard VisualCard;
    public Text Player1Health;
    public Text Player2Health;
    public CardValues Player1CV;
    public CardValues Player2CV;
    public GenomeVisualizer Player1GV;
    public GenomeVisualizer Player2GV;

    // Card Display
    public List<VisualCard> DisplayedCards;
    private float CardBorderXMarginRel = 0.3f;
    public float CardBorderXMargin;
    private float CardBorderYMarginRel = 0.3f;
    public float CardBorderYMargin;
    private float CardWidthRel = 0.1f;
    public float CardWidth;
    public float CardHeight;

    public void UpdatePlayerHealth()
    {
        Player1Health.text = Model.Player1.Health + "/" + Model.Player1.MaxHealth;
        Player2Health.text = Model.Player2.Health + "/" + Model.Player2.MaxHealth;
    }

    public void ShowCards(List<Card> options, Player p)
    {
        for(int i = 0; i < options.Count; i++)
        {
            VisualCard vc = GameObject.Instantiate(VisualCard, transform);
            vc.Title.text = options[i].Name;
            vc.Description.text = options[i].Text;

            vc.GetComponent<RectTransform>().sizeDelta = new Vector2(CardWidth, CardHeight);
            float step = (CanvasWidth - 2 * CardBorderXMargin) / (options.Count - 1);
            float xPos = CardBorderXMargin + (i * step);
            float yPos = p == Model.Player1 ? CardBorderYMargin : CanvasHeight - CardBorderYMargin;
            vc.GetComponent<RectTransform>().localPosition = new Vector3(xPos, yPos, 0);

            if(options[i] == p.ChosenCard)
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
        CanvasWidth = GetComponent<RectTransform>().sizeDelta.x;
        CanvasHeight = GetComponent<RectTransform>().sizeDelta.y;

        CardBorderXMargin = CanvasWidth * CardBorderXMarginRel;
        CardBorderYMargin = CanvasHeight * CardBorderYMarginRel;
        CardWidth = CanvasWidth * CardWidthRel;
        CardHeight = CardWidth * 1.5f;
    }

    // Update is called once per frame
    void Update()
    {

    }
}
