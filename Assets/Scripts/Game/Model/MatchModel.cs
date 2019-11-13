using System.Collections;
using System.Collections.Generic;
using static CardList;
using UnityEngine;
using System;
using System.Linq;

public class MatchModel : MonoBehaviour
{
    // Rules
    public int MaxMinions = 10;

    // Players
    public Player Player1;
    public Player Player2;
    public Player Winner;
    public Color PlayerColor = Color.gray;

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
    private float MinionXGapPlan = 0.8f;
    private float MinionXGapAction = 0.5f;
    private float MinionYGapPlan = 0.5f;
    private float MinionYStartPlan = 0.35f; // The higher this value is, the closer the minions are to the player (0 < x < 0.5)
    private float MinionYStartAction = 0.1f; // The higher this value is, the closer the minions are to the player (0 < x < 0.5)
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
        if (Visual)
        {
            // UI
            MatchUI = matchUI;
            MatchUI.Model = this;
            MatchUI.UpdatePlayerHealth();

            // Summon Players
            Player1.Visual = GameObject.Instantiate(V_Player, new Vector3(0, 0, -(VisualBoardHeight / 2)), Quaternion.identity);
            Player2.Visual = GameObject.Instantiate(V_Player, new Vector3(0, 0, VisualBoardHeight / 2), Quaternion.identity);
            Player1.Color = PlayerColor;
            Player2.Color = PlayerColor;
            VisualActions.Add(new VA_SummonPlayer(Player1.Visual, PlayerColor));
            VisualActions.Add(new VA_SummonPlayer(Player2.Visual, PlayerColor));

            // UI Elements
            MatchUI.Player1GV.VisualizeSubject(Player1.Brain.Genome);
            MatchUI.Player2GV.VisualizeSubject(Player2.Brain.Genome);
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

            case GamePhase.MinionsToAction:
                MoveMinionsUpdate();
                break;

            case GamePhase.MinionEffect:
                DequeueEffect();
                break;

            case GamePhase.MinionsToPlan:
                MoveMinionsUpdate();
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
                    Phase = GamePhase.CardPick;
                    break;

                case GamePhase.CardPick:
                    // Hide cards
                    if(Visual)
                    {
                        MatchUI.UnshowAllCards();
                    }
                    // Queue card effects
                    Effects.Enqueue(() => { Player1.ChosenCard.Action(Player1, Player2); });
                    Effects.Enqueue(() => { Player2.ChosenCard.Action(Player2, Player1); });
                    Phase = GamePhase.CardEffect;
                    break;

                case GamePhase.CardEffect:
                    // Initiate Moveminion effect
                    if(Visual)
                    {
                        VisualActions.Add(new VA_MoveMinions(this, true));
                        Phase = GamePhase.MinionsToAction;
                    }
                    else // Skip MoveMinion Phase if non-visual
                    {
                        QueueMinionEffects();
                        Phase = GamePhase.MinionEffect;
                    }
                    break;

                case GamePhase.MinionsToAction:
                    // Queue minion effects
                    QueueMinionEffects();
                    Phase = GamePhase.MinionEffect;
                    break;

                case GamePhase.MinionEffect:
                    if (Visual)
                    {
                        VisualActions.Add(new VA_MoveMinions(this, false));
                        Phase = GamePhase.MinionsToPlan;
                    }
                    else // Skip MoveMinion Phase if non-visual
                    {
                        PickCards();
                        Phase = GamePhase.CardPick;
                    }
                    break;

                case GamePhase.MinionsToPlan:
                    PickCards();
                    Phase = GamePhase.CardPick;
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
    }

    private void QueueMinionEffects()
    {
        foreach (Minion m in Minions.OrderBy(x => x.OrderNum))
        {
            Effects.Enqueue(() => { if(!m.Destroyed) m.Action(); });
        }
    }

    private void DequeueEffect()
    {
        if (VisualActions.Count > 0 && VisualActions[0].Done) VisualActions.RemoveAt(0);

        if (Effects.Count > 0 && (VisualActions.Count == 0 || VisualActions[0].Done))
        {
            MatchUI.UpdatePlayerHealth();

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
                MatchUI.UpdatePlayerHealth();
                Debug.Log(Winner.Name + " won!");
            }
        }
        else if (VisualActions.Count > 0 && !VisualActions[0].Done)
        {
            VisualActions[0].Update();
        }
        else
        {
            MatchUI.UpdatePlayerHealth();

            NextPhaseReady = true;
        }
    }

    private void MoveMinionsUpdate()
    {
        if (VisualActions.Count > 0)
        {
            if (VisualActions[0].Done)
            {
                VisualActions.Clear();
                NextPhaseReady = true;
            }
            else
            {
                VisualActions[0].Update();
            }
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

            case MinionType.Yellow:
                minion = new M02_Yellow(this, player, player.Enemy, SummonOrder++);
                break;

            case MinionType.Blue:
                minion = new M03_Blue(this, player, player.Enemy, SummonOrder++);
                break;

            case MinionType.Green:
                minion = new M04_Green(this, player, player.Enemy, SummonOrder++);
                break;
        }
        Minions.Add(minion);

        if (Visual)
        {
            minion.Visual = GameObject.Instantiate(V_RedMinion);
            minion.Visual.GetComponent<Renderer>().material.color = minion.Color;
            Vector3 targetPosition = GetPlanPosition(minion);
            VisualActions.Add(new VA_SummonMinion(minion.Visual, source.Visual.transform.position, targetPosition, MinionScale));
        }
    }

    public void DestroyRandomMinion(Creature source, Player targetPlayer)
    {
        Minion target = RandomMinionFromPlayer(targetPlayer);

        if (target != null)
        {
            target.Destroyed = true;
            Minions.Remove(target);

            if (Visual)
            {
                VisualActions.Add(new VA_DestroyMinion(source.Visual, target.Visual, Color.black));
            }
        }
    }

    public void Damage(Creature source, Player target, int amount)
    {
        target.Health = Mathf.Max(0, target.Health - amount);

        if(Visual)
        {
            VisualActions.Add(new VA_DealDamage(source.Visual, target.Visual, amount, source.Color));
        }
    }

    public void Heal(Creature source, Player target, int amount)
    {
        target.Health = Mathf.Min(target.MaxHealth, target.Health + amount);

        if (Visual)
        {
            VisualActions.Add(new VA_Heal(source.Visual, target.Visual, amount, source.Color));
        }
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
    public int NumMinions(Player player)
    {
        return Minions.Where(x => x.Owner == player).Count();
    }

    public int NumMinions(Player player, MinionType type)
    {
        return Minions.Where(x => x.Owner == player && x.Type == type).Count();
    }

    private Minion RandomMinionFromPlayer(Player player)
    {
        List<Minion> list = Minions.Where(x => x.Owner == player).ToList();
        if (list.Count == 0) return null;
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public MinionType RandomMinionType()
    {
        Array types = Enum.GetValues(typeof(MinionType));
        return (MinionType)types.GetValue(UnityEngine.Random.Range(0, types.Length));
    }

    // Visual
    public Vector3 GetPlanPosition(Minion m)
    {
        List<Minion> orderedTypeList = Minions.Where(x => x.Type == m.Type && x.Owner == m.Owner).OrderBy(x => x.OrderNum).ToList();
        float position = orderedTypeList.IndexOf(m);
        float yPos;
        if (m.Owner == Player1)
        {
            yPos = -(VisualBoardHeight * MinionYStartPlan) + ((position - 1) * MinionYGapPlan);
        }
        else
        {
            yPos = (VisualBoardHeight * MinionYStartPlan) - ((position - 1) * MinionYGapPlan);
        }

        return new Vector3(-(VisualBoardWidth / 2) + ((float)m.Type * MinionXGapPlan), 0, yPos);
    }

    public Vector3 GetActionPosition(Minion m)
    {
        List<Minion> orderedTypeList = Minions.OrderBy(x => x.OrderNum).ToList();
        float xPos = -(VisualBoardWidth / 2) + orderedTypeList.IndexOf(m) * MinionXGapAction;
        float yPos = m.Owner == Player1 ? -(VisualBoardHeight * MinionYStartAction) : (VisualBoardHeight * MinionYStartAction);
        return new Vector3(xPos, 0, yPos);
    }


}
