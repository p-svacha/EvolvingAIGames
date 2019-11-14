﻿using System.Collections;
using System.Collections.Generic;
using static CardList;
using UnityEngine;
using System;
using System.Linq;

public class Match
{
    // Rules
    public int StartHealth;
    public int MaxMinionsPerType = 10;
    public int MaxMinions = 20;
    public int FatigueDamageStartTurn = 20;

    // Players
    public Player Player1;
    public Player Player2;
    public Player Winner;
    public Player Loser;
    public Color PlayerColor = Color.gray;

    // Minions
    public List<Minion> Minions; // List with minions from both players
    public int SummonOrder;

    // Phase
    public int Turn;
    public MatchPhase Phase;
    public bool NextPhaseReady;

    // Effects
    public Queue<Action> Effects;

    // Visual
    public bool Log;
    public bool Visual;
    public MatchUI MatchUI;
    public List<VisualAction> VisualActions;

    public int VisualBoardWidth;
    public int VisualBoardHeight;

    private VisualPlayer V_Player;
    private VisualMinion V_Minion;

    private float MinionScale = 0.4f;
    private float MinionXGapPlan = 0.8f;
    private float MinionXGapAction = 0.5f;
    private float MinionYGapPlan = 0.5f;
    private float MinionYStartPlan = 0.35f; // The higher this value is, the closer the minions are to the player (0 < x < 0.5)
    private float MinionYStartAction = 0.1f; // The higher this value is, the closer the minions are to the player (0 < x < 0.5)


    public void InitGame(Player player1, Player player2, int health, int options, bool log)
    {
        // Set players
        StartHealth = health;
        Player1 = player1;
        Player2 = player2;
        Player1.Initialize(Player2, StartHealth, options);
        Player2.Initialize(Player1, StartHealth, options);

        // Init game values
        Log = log;
        Minions = new List<Minion>();
        Effects = new Queue<Action>();
        VisualActions = new List<VisualAction>();
        Turn = 0;
        SummonOrder = 0;

        Phase = MatchPhase.GameInitialized;
    }

    public void StartMatch(bool visual = false, VisualPlayer v_player = null, VisualMinion v_minion = null, int visualBoardWidth = 0, int visualBoardHeight = 0, MatchUI matchUI = null)
    {
        Visual = visual;
        V_Player = v_player;
        V_Minion = v_minion;
        VisualBoardWidth = visualBoardWidth;
        VisualBoardHeight = visualBoardHeight;

        if (Visual)
        {
            // UI
            MatchUI = matchUI;
            MatchUI.Model = this;
            MatchUI.UpdatePlayerHealth();
            MatchUI.UpdatePlayerNames();
            MatchUI.UpdateTurnText();

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

        // Let's go
        Phase = MatchPhase.GameReady;
    }

    public void Update()
    {
       // During game phases
        switch(Phase)
        {
            case MatchPhase.GameReady:
                if (Visual)
                {
                    foreach (VisualAction va in VisualActions) va.Update();
                    VisualActions = VisualActions.Where(x => !x.Done).ToList();
                    if (VisualActions.Count == 0) NextPhaseReady = true;
                }
                else
                {
                    CompleteWholeMatch();
                }
                break;

            case MatchPhase.CardPick:
                if (Player1.ChosenCard != null && Player2.ChosenCard != null)
                {
                    NextPhaseReady = true;
                }
                break;

            case MatchPhase.CardEffect:
                DequeueEffect();
                break;

            case MatchPhase.MinionsToAction:
                MoveMinionsUpdate();
                break;

            case MatchPhase.MinionEffect:
                DequeueEffect();
                break;

            case MatchPhase.MinionsToPlan:
                MoveMinionsUpdate();
                break;

            case MatchPhase.GameEnded:
                break;
        }

        // Changing game phases
        if(NextPhaseReady && (!Visual || Input.GetKeyDown(KeyCode.Space)))
        {
            NextPhaseReady = false;

            switch(Phase)
            {
                case MatchPhase.GameReady:
                    PickCards();
                    Phase = MatchPhase.CardPick;
                    break;

                case MatchPhase.CardPick:
                    // Hide cards
                    if(Visual)
                    {
                        MatchUI.UnshowAllCards();
                    }
                    // Queue card effects
                    Effects.Enqueue(() => { Player1.ChosenCard.Action(this, Player1, Player2); });
                    Effects.Enqueue(() => { Player2.ChosenCard.Action(this, Player2, Player1); });
                    ApplyFatigueDamage();
                    Phase = MatchPhase.CardEffect;
                    break;

                case MatchPhase.CardEffect:
                    // Initiate Moveminion effect
                    if(Visual && Minions.Count > 0)
                    {
                        VisualActions.Add(new VA_MoveMinions(this, true));
                        Phase = MatchPhase.MinionsToAction;
                    }
                    else // Skip MoveMinion Phase if non-visual
                    {
                        QueueMinionEffects();
                        Phase = MatchPhase.MinionEffect;
                    }
                    break;

                case MatchPhase.MinionsToAction:
                    // Queue minion effects
                    QueueMinionEffects();
                    Phase = MatchPhase.MinionEffect;
                    break;

                case MatchPhase.MinionEffect:
                    if (Visual && Minions.Count > 0)
                    {
                        VisualActions.Add(new VA_MoveMinions(this, false));
                        Phase = MatchPhase.MinionsToPlan;
                    }
                    else // Skip MoveMinion Phase if non-visual
                    {
                        PickCards();
                        Phase = MatchPhase.CardPick;
                    }
                    break;

                case MatchPhase.MinionsToPlan:
                    PickCards();
                    Phase = MatchPhase.CardPick;
                    break;

                case MatchPhase.GameEnded:
                    break;
            }
        }
    }

    private void CompleteWholeMatch()
    {
        while(Phase != MatchPhase.GameEnded)
        {
            // During game phases
            switch (Phase)
            {
                case MatchPhase.GameReady:
                    PickCards();
                    Phase = MatchPhase.CardPick;
                    break;

                case MatchPhase.CardPick:
                    Effects.Enqueue(() => { Player1.ChosenCard.Action(this, Player1, Player2); });
                    Effects.Enqueue(() => { Player2.ChosenCard.Action(this, Player2, Player1); });
                    ApplyFatigueDamage();
                    Phase = MatchPhase.CardEffect;
                    break;

                case MatchPhase.CardEffect:
                    DequeueEffect();
                    if (Phase != MatchPhase.GameEnded)
                    {
                        QueueMinionEffects();
                        Phase = MatchPhase.MinionEffect;
                    }
                    break;

                case MatchPhase.MinionEffect:
                    DequeueEffect();
                    if (Phase != MatchPhase.GameEnded)
                    {
                        PickCards();
                        Phase = MatchPhase.CardPick;
                    }
                    break;
            }
        }
    }

    private void PickCards()
    {
        // Next Turn
        Turn++;
        if (Log)
        {
            Debug.Log("##################### Starting Turn " + Turn + " #####################");
        }

        // Pick Cards
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
            MatchUI.UpdateTurnText();
        }
        if(Log)
        {
            string optionString = "";
            foreach (Card c in Player1RandomCards) optionString += c.Name + ", ";
            optionString = optionString.TrimEnd(new char[] { ',', ' ' });
            Debug.Log(Player1.Name + " chose " + Player1.ChosenCard.Name + " from " + optionString);

            optionString = "";
            foreach (Card c in Player2RandomCards) optionString += c.Name + ", ";
            optionString = optionString.TrimEnd(new char[] { ',', ' ' });
            Debug.Log(Player2.Name + " chose " + Player2.ChosenCard.Name + " from " + optionString);
        }
    }

    private void ApplyFatigueDamage()
    {
        if(Turn > FatigueDamageStartTurn)
        {
            int dmg = Turn - FatigueDamageStartTurn;
            Effects.Enqueue(() => { Damage(Player1, Player2, dmg); });
            Effects.Enqueue(() => { Damage(Player2, Player1, dmg); });
            if (Log)
            {
                Debug.Log("Applying " + dmg + " fatigue damage each.");
            }
        }
    }

    private void QueueMinionEffects()
    {
        foreach (Minion m in Minions.OrderBy(x => x.OrderNum))
        {
            Effects.Enqueue(() => { if(!m.Destroyed) m.Action(); });
        }
    }

    /// <summary>
    /// Updates the current action or starts the next one if current is done. When all actions are done, it sais next phase ready.
    /// </summary>
    private void DequeueEffect()
    {
        // Visual
        if (Visual)
        {
            if (VisualActions.Count > 0 && VisualActions[0].Done) VisualActions.RemoveAt(0);

            if (Effects.Count > 0 && (VisualActions.Count == 0 || VisualActions[0].Done))
            {
                MatchUI.UpdatePlayerHealth();
                Effects.Dequeue().Invoke();
                CheckGameOver();
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

        // Non-visual, do everything in one update-step
        else
        {
            while(Effects.Count > 0)
            {
                Effects.Dequeue().Invoke();
                if(CheckGameOver()) break;
            }
        }
    }

    /// <summary>
    /// Checks if a player is dead and if yets updates the player stats and ends the game.
    /// </summary>
    private bool CheckGameOver()
    {
        if (Player1.Health <= 0)
        {
            Winner = Player2;
            Loser = Player1;

            Player2.Brain.Wins++;
            Player1.Brain.Losses++;
            Phase = MatchPhase.GameEnded;
        }
        else if (Player2.Health <= 0)
        {
            Winner = Player1;
            Loser = Player2;
            Player1.Brain.Wins++;
            Player2.Brain.Losses++;
            Phase = MatchPhase.GameEnded;
        }
        if (Phase == MatchPhase.GameEnded)
        {
            //if (Player1.Name == "0.0" || Player2.Name == "0-0") Debug.Log("0-0 finished a game " + Player1.Name + "," + Player2.Name); // test if one player finished one match per round

            Player1.Brain.DamageDealt = StartHealth - Player2.Health;
            Player2.Brain.DamageReceived = StartHealth - Player2.Health;

            Player1.Brain.DamageReceived = StartHealth - Player1.Health;
            Player2.Brain.DamageDealt = StartHealth - Player1.Health;

            if (Visual)
            {
                MatchUI.UpdatePlayerHealth();
                GameObject.Destroy(Player1.Visual.gameObject);
                GameObject.Destroy(Player2.Visual.gameObject);
                foreach (Minion m in Minions) GameObject.Destroy(m.Visual.gameObject);
            }
            if(Log) Debug.Log(Winner.Name + " won against " + Loser.Name + " after " + Turn + " turns.");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Updates the minions positions while moving from planning phase to action phase or vice versa.
    /// </summary>
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
        if (NumMinions(player, type) < MaxMinionsPerType && NumMinions(player) < MaxMinions)
        {
            minion = CreateNewMinion(type, player);
            Minions.Add(minion);

            if (Visual)
            {
                minion.Visual = GameObject.Instantiate(V_Minion);
                minion.Visual.GetComponent<Renderer>().material.color = minion.Color;
                Vector3 targetPosition = GetPlanPosition(minion);
                VisualActions.Add(new VA_SummonMinion(minion.Visual, source.Visual.transform.position, targetPosition, MinionScale));
            }
            if (Log)
            {
                Debug.Log(source.Name + " summoned " + minion.Name);
            }
        }
    }

    public void SummonMultipleMinions(Creature source, List<Tuple<MinionType, Player>> list)
    {
        List<Minion> createdMinions = new List<Minion>();
        List<Vector3> targetPositions = new List<Vector3>();

        for (int i = 0; i < list.Count; i++)
        {
            MinionType type = list[i].Item1;
            Player player = list[i].Item2;
            if (NumMinions(player, type) < MaxMinionsPerType && NumMinions(player) < MaxMinions)
            {
                Minion newMinion = CreateNewMinion(type, player);
                Minions.Add(newMinion);
                createdMinions.Add(newMinion);
            }
        }

        if(Visual)
        {
            foreach(Minion minion in createdMinions)
            {
                minion.Visual = GameObject.Instantiate(V_Minion);
                minion.Visual.GetComponent<Renderer>().material.color = minion.Color;
                targetPositions.Add(GetPlanPosition(minion));
            }
            VisualActions.Add(new VA_SummonMultipleMinions(createdMinions.Select(x => x.Visual).ToList(), source.Visual.transform.position, targetPositions, MinionScale));
        }
        if(Log)
        {
            string names = "";
            foreach (Minion minion in createdMinions) names += minion.Name + ", ";
            names = names.TrimEnd(new char[] { ',', ' ' });
            Debug.Log(source.Name + " summoned " + names);
        }
    }

    public void DestroyRandomMinion(Creature source, Minion target)
    {
        if (target != null)
        {
            target.Destroyed = true;
            Minions.Remove(target);

            if (Visual)
            {
                VisualActions.Add(new VA_DestroyMinion(source.Visual, target.Visual, Color.black));
            }
            if (Log)
            {
                Debug.Log(source.Name + " destroyed " + target.Name);
            }
        }
    }

    public void DestroyMultipleRandomMinions(Creature source, List<Minion> targets)
    {
        foreach(Minion target in targets)
        {
            target.Destroyed = true;
            Minions.Remove(target);
        }

        if(Visual)
        {
            VisualActions.Add(new VA_DestroyMultipleMinions(source.Visual, targets.Select(x => x.Visual).ToList(), Color.black));
        }

        if (Log)
        {
            string names = "";
            foreach (Minion minion in targets) names += minion.Name + ", ";
            names = names.TrimEnd(new char[] { ',', ' ' });
            Debug.Log(source.Name + " destroyed " + names);
        }
    }

    public void Damage(Creature source, Player target, int amount)
    {
        target.Health = Mathf.Max(0, target.Health - amount);

        if(Visual)
        {
            VisualActions.Add(new VA_DealDamage(source.Visual, target.Visual, amount, source.Color));
        }
        if(Log)
        {
            Debug.Log(source.Name + " dealt " + amount + " damage to " + target.Name + " (now at " + target.Health + ")");
        }
    }

    public void Heal(Creature source, Player target, int amount)
    {
        if (target.Health < target.MaxHealth)
        {
            target.Health = Mathf.Min(target.MaxHealth, target.Health + amount);

            if (Visual)
            {
                VisualActions.Add(new VA_Heal(source.Visual, target.Visual, amount, source.Color));
            }
            if (Log)
            {
                Debug.Log(source.Name + " healed " + target.Name + " for " + amount + " (now at " + target.Health + ")");
            }
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

    private Minion CreateNewMinion(MinionType type, Player player)
    {
        switch (type)
        {
            case MinionType.Red:
                return new M01_Red(this, player, player.Enemy, SummonOrder++);

            case MinionType.Yellow:
                return new M02_Yellow(this, player, player.Enemy, SummonOrder++);

            case MinionType.Blue:
                return new M03_Blue(this, player, player.Enemy, SummonOrder++);

            case MinionType.Green:
                return new M04_Green(this, player, player.Enemy, SummonOrder++);
        }
        throw new Exception("Minion type not handled in minion creation!");
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

    public Minion RandomMinionFromPlayer(Player player)
    {
        List<Minion> list = Minions.Where(x => x.Owner == player).ToList();
        if (list.Count == 0) return null;
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public List<Minion> RandomMinionsFromPlayer(Player player, int amount)
    {
        if (amount >= NumMinions(player)) return Minions.Where(x => x.Owner == player).ToList();
        else
        {
            List<Minion> returnList = new List<Minion>();
            List<Minion> copy = new List<Minion>();
            copy.AddRange(Minions.Where(x => x.Owner == player));
            for(int i = 0; i < amount; i++)
            {
                Minion minion = copy[UnityEngine.Random.Range(0, copy.Count)];
                returnList.Add(minion);
                copy.Remove(minion);
            }
            return returnList;
        }
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
        bool secondColumn = (position + 1) > MaxMinionsPerType / 2;

        float xPos = -(VisualBoardWidth / 2) + ((float)m.Type * (MinionXGapPlan + MinionScale));
        if (secondColumn) xPos += MinionScale;

        if (m.Owner == Player1)
        {
            if(secondColumn) yPos = -(VisualBoardHeight * MinionYStartPlan) + ((position - (MaxMinionsPerType / 2) - 1) * MinionYGapPlan);
            else yPos = -(VisualBoardHeight * MinionYStartPlan) + ((position - 1) * MinionYGapPlan);
        }
        else
        {
            if (secondColumn) yPos = (VisualBoardHeight * MinionYStartPlan) - ((position - MaxMinionsPerType / 2 - 1) * MinionYGapPlan);
            else yPos = (VisualBoardHeight * MinionYStartPlan) - ((position - 1) * MinionYGapPlan);
        }

        return new Vector3(xPos, 0, yPos);
    }

    public Vector3 GetActionPosition(Minion m)
    {
        List<Minion> orderedTypeList = Minions.OrderBy(x => x.OrderNum).ToList();
        float xPos = -(VisualBoardWidth / 2) + orderedTypeList.IndexOf(m) * MinionXGapAction;
        float yPos = m.Owner == Player1 ? -(VisualBoardHeight * MinionYStartAction) : (VisualBoardHeight * MinionYStartAction);
        return new Vector3(xPos, 0, yPos);
    }


}