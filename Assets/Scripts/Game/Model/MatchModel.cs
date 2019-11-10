using System.Collections;
using System.Collections.Generic;
using static CardList;
using UnityEngine;
using System;
using System.Linq;

public class MatchModel : MonoBehaviour
{
    // Players
    public Player Player1;
    public Player Player2;
    public Player Winner;

    // Minions
    public List<Minion> Minions; // List with minions from both players
    public int SummonOrder;

    // Phase
    public int Turn;
    public GamePhase Phase;
    public bool NextPhaseReady;

    // Effects
    public Queue<Action> Effects;

    // Visual
    public bool Visual;
    public MatchUI MatchUI;
    public List<VisualAction> VisualActions;

    public int VisualBoardWidth;
    public int VisualBoardHeight;

    public VisualPlayer V_Player;

    private float MinionScale = 0.4f;
    private float MinionXGap = 0.8f;
    private float MinionYGap = 0.5f;
    public VisualMinion V_RedMinion;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void InitGame(Player player1, Player player2, int health, int options, bool visual, int visualBoardWidth = 0, int visualBoardHeight = 0, MatchUI matchUI = null)
    {
        // Set players
        Player1 = player1;
        Player2 = player2;
        Player1.Initialize(Player2, health, options);
        Player2.Initialize(Player1, health, options);

        // Init game values
        Visual = visual;
        Minions = new List<Minion>();
        Effects = new Queue<Action>();
        VisualActions = new List<VisualAction>();
        Turn = 0;
        SummonOrder = 0;

        // Check visuals
        Visual = visual;
        VisualBoardWidth = visualBoardWidth;
        VisualBoardHeight = visualBoardHeight;
        if (visual)
        {
            // UI
            MatchUI = matchUI;
            MatchUI.Model = this;
            MatchUI.UpdatePlayerHealth();

            // Summon Players
            Player1.Visual = GameObject.Instantiate(V_Player, new Vector3(0, 0, -(VisualBoardHeight / 2)), Quaternion.identity);
            Player2.Visual = GameObject.Instantiate(V_Player, new Vector3(0, 0, VisualBoardHeight / 2), Quaternion.identity);
            VisualActions.Add(new VA_SummonPlayer(Player1.Visual));
            VisualActions.Add(new VA_SummonPlayer(Player2.Visual));
        }

        // Init cards
        CardList.InitCardList(this); // later refactor this to the simulation model to only execute once

        // Let's go
        Phase = GamePhase.GameReady;
    }

    // Update is called once per frame
    void Update()
    {

       // During game phases
        switch(Phase)
        {
            case GamePhase.GameReady:
                if (Visual)
                {
                    foreach (VisualAction va in VisualActions) va.Update();
                    VisualActions = VisualActions.Where(x => !x.Done).ToList();
                    if (VisualActions.Count == 0) NextPhaseReady = true;
                }
                else NextPhaseReady = true;
                break;

            case GamePhase.CardPick:
                if (Player1.ChosenCard != null && Player2.ChosenCard != null)
                {
                    NextPhaseReady = true;
                }
                break;

            case GamePhase.CardEffect:
                DequeueEffect();
                break;

            case GamePhase.MinionEffect:
                DequeueEffect();
                break;

            case GamePhase.GameEnded:
                break;
        }

        // Changing game phases
        if(Input.GetKeyDown(KeyCode.Space) && NextPhaseReady)
        {
            NextPhaseReady = false;

            switch(Phase)
            {
                case GamePhase.GameReady:
                    PickCards();
                    break;

                case GamePhase.CardPick:
                    if(Visual)
                    {
                        MatchUI.UnshowAllCards();
                    }
                    Effects.Enqueue(() => { Player1.ChosenCard.Action(Player1, Player2); });
                    Effects.Enqueue(() => { Player2.ChosenCard.Action(Player2, Player1); });
                    Phase = GamePhase.CardEffect;
                    break;

                case GamePhase.CardEffect:
                    foreach(Minion m in Minions.OrderBy(x => x.OrderNum))
                    {
                        Effects.Enqueue(() => { m.Action(); });
                    }
                    Phase = GamePhase.MinionEffect;
                    break;

                case GamePhase.MinionEffect:
                    PickCards();
                    break;

                case GamePhase.GameEnded:
                    break;
            }
        }
    }

    private void PickCards()
    {
        Turn++;
        Player1.ChosenCard = null;
        Player2.ChosenCard = null;
        List<Card> Player1RandomCards = RandomCards(Player1.NumCardOptions);
        List<Card> Player2RandomCards = RandomCards(Player2.NumCardOptions);
        Player1.PickCard(Player1RandomCards);
        Player2.PickCard(Player2RandomCards);
        if (Visual)
        {
            MatchUI.ShowCards(Player1RandomCards, Player1);
            MatchUI.ShowCards(Player2RandomCards, Player2);
        }
        Phase = GamePhase.CardPick;
    }

    private void DequeueEffect()
    {
        if (VisualActions.Count > 0 && VisualActions[0].Done) VisualActions.RemoveAt(0);

        if (Effects.Count > 0 && (VisualActions.Count == 0 || VisualActions[0].Done))
        {
            Effects.Dequeue().Invoke();

            if(Player1.Health <= 0)
            {
                Winner = Player2;
                Phase = GamePhase.GameEnded;
            }
            else if (Player2.Health <= 0)
            {
                Winner = Player1;
                Phase = GamePhase.GameEnded;
            }
            if(Phase == GamePhase.GameEnded)
            {
                Debug.Log(Winner.Name + " won!");
            }
        }
        else if (VisualActions.Count > 0 && !VisualActions[0].Done)
        {
            VisualActions[0].Update();
        }
        else
        {
            NextPhaseReady = true;
        }
    }

    public void SummonMinion(Creature source, MinionType type, Player player)
    {
        Minion minion = null;
        switch(type)
        {
            case MinionType.Red:
                minion = new M01_Red(this, player, player.Enemy, SummonOrder++);
                break;
        }
        Minions.Add(minion);

        if (Visual)
        {
            minion.Visual = GameObject.Instantiate(V_RedMinion);
            Vector3 targetPosition = GetPlanPosition(minion);
            VisualActions.Add(new VA_SummonMinion(minion.Visual, source.Visual.transform.position, targetPosition, MinionScale));
        }

        Debug.Log(player.Name + " summons a " + minion.Name);
    }

    public void Damage(Creature source, Player target, int amount)
    {
        target.Health = Mathf.Max(0, target.Health - amount);

        Debug.Log(source.Name + " deals " + amount + " damage to " + target.Name + " (now at " + target.Health + ")");
    }

    public void Heal(Creature source, Player target, int amount)
    {
        target.Health = Mathf.Min(target.MaxHealth, target.Health + amount);

        Debug.Log(source.Name + " heals " + target.Name + " for " + amount + " (now at " + target.Health + ")");
    }

    private List<Card> RandomCards(int amount)
    {
        List<Card> cards = new List<Card>();
        for (int i = 0; i < amount; i++)
        {
            Card c = null;
            do
            {
                c = Cards[UnityEngine.Random.Range(0, Cards.Count)];
            } while (cards.Contains(c));
            cards.Add(c);
        }
        return cards;
    }


    // LINQ
    public Vector3 GetPlanPosition(Minion m)
    {
        List<Minion> orderedTypeList = Minions.Where(x => x.Type == m.Type && x.Owner == m.Owner).OrderBy(x => x.OrderNum).ToList();
        float position = orderedTypeList.IndexOf(m);
        float yPos;
        if(m.Owner == Player1)
        {
            yPos = -(VisualBoardHeight / 2.5f) + ((position-1) * MinionYGap);
        }
        else
        {
            yPos = (VisualBoardHeight / 2.5f) - ((position-1) * MinionYGap);
        }

        return new Vector3(-(VisualBoardWidth / 2) + ((float)m.Type * MinionXGap), 0, yPos);
    }

    public Vector3 GetActionPosition(Minion m)
    {
        List<Minion> orderedTypeList = Minions.OrderBy(x => x.OrderNum).ToList();
        float position = orderedTypeList.IndexOf(m);
        float yPos = VisualBoardHeight / 3;
        return new Vector3(position * MinionXGap, 0, yPos);
    }

    public int NumMinions(Player player)
    {
        return Minions.Where(x => x.Owner == player).Count();
    }
}
