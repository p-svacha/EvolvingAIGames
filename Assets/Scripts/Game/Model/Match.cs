using System.Collections;
using System.Collections.Generic;
using static CardList;
using UnityEngine;
using System;
using System.Linq;

public class Match
{
    #region variablies and init 

    // Type
    public MatchType MatchType;

    // Rules
    public int StartHealth;
    public int StartCardOptions;
    public int MinCardOptions;
    public int MaxCardOptions;
    public int MaxMinionsPerType;
    public int MaxMinions;
    public int FatigueDamageStartTurn;

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

    public int VisualBoardHeight;

    private VisualPlayer V_Player;
    private VisualMinion V_Minion;

    private float MinionScale = 0.4f;
    private float MinionXGapPlan = 0.4f;
    private float MinionXGapAction = 0.1f;
    private float MinionYGapPlan = 0.1f;
    private float MinionYStartPlan = 0.35f; // The higher this value is, the closer the minions are to the player (0 < x < 0.5)
    private float MinionYStartAction = 0.1f; // The higher this value is, the closer the minions are to the player (0 < x < 0.5)

    public void InitGame(Player player1, Player player2, int health, int options, int minOptions, int maxOptions, int maxMinions, int maxMinionsPerType, int fatigueStart, bool visual, bool log)
    {
        // Set rules
        StartHealth = health;
        StartCardOptions = options;
        MinCardOptions = minOptions;
        MaxCardOptions = maxOptions;
        MaxMinions = maxMinions;
        MaxMinionsPerType = maxMinionsPerType;
        FatigueDamageStartTurn = fatigueStart;

        // Set players
        Player1 = player1;
        Player2 = player2;
        Player1.Initialize(Player2, StartHealth, StartCardOptions);
        Player2.Initialize(Player1, StartHealth, StartCardOptions);

        // Set match type
        if (Player1.GetType() == typeof(AIPlayer) && Player2.GetType() == typeof(AIPlayer))
        {
            MatchType = MatchType.AI_vs_AI;
        }
        else if (Player1.GetType() == typeof(HumanPlayer) && Player2.GetType() == typeof(AIPlayer))
        {
            MatchType = MatchType.Human_vs_AI;
        }
        else throw new Exception("Invalid Match Type. Only AI vs AI and Human vs AI allowed!");

        // Init game values
        Visual = visual;
        Log = log;
        Minions = new List<Minion>();
        Effects = new Queue<Action>();
        VisualActions = new List<VisualAction>();
        Turn = 0;
        SummonOrder = 0;

        Phase = MatchPhase.GameInitialized;
    }

    #endregion

    #region Game Cycle

    public void StartMatch(VisualPlayer v_player = null, VisualMinion v_minion = null, int visualBoardHeight = 0, MatchUI matchUI = null)
    {
        V_Player = v_player;
        V_Minion = v_minion;
        VisualBoardHeight = visualBoardHeight;

        if (Visual)
        {
            // UI
            MatchUI = matchUI;
            MatchUI.Model = this;
            MatchUI.UpdatePlayerHealthText();
            MatchUI.UpdatePlayerNames();
            MatchUI.UpdateTurnText();
            if (MatchType == MatchType.AI_vs_AI)
                MatchUI.UpdatePlayerGenomes();

            // Summon Players
            Player1.Visual = GameObject.Instantiate(V_Player, new Vector3(0, 0, -(VisualBoardHeight / 2)), Quaternion.identity);
            Player2.Visual = GameObject.Instantiate(V_Player, new Vector3(0, 0, VisualBoardHeight / 2), Quaternion.identity);
            Player1.Color = PlayerColor;
            Player2.Color = PlayerColor;
            VisualActions.Add(new VA_SummonPlayer(Player1.Visual, PlayerColor));
            VisualActions.Add(new VA_SummonPlayer(Player2.Visual, PlayerColor));

            
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
                VisualUpdate();
                break;

            case MatchPhase.MinionEffect:
                DequeueEffect();
                break;

            case MatchPhase.MinionDeaths:
                VisualUpdate();
                break;

            case MatchPhase.MinionsToPlan:
                VisualUpdate();
                break;

            case MatchPhase.GameEnded:
                break;
        }

        // Changing game phases
        if(NextPhaseReady && Input.GetKeyDown(KeyCode.Space))
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
                    MatchUI.UnshowAllCards();

                    // Queue effects that show what cards were chosen + card effects
                    Effects.Enqueue(() => { VisualActions.Add(new VA_ShowChosenCards(Player1, Player1.ChosenCard, MatchUI)); });
                    Effects.Enqueue(() => { Player1.ChosenCard.Action(this, Player1, Player2); });
                    Effects.Enqueue(() => { VisualActions.Add(new VA_ShowChosenCards(Player2, Player2.ChosenCard, MatchUI)); });
                    Effects.Enqueue(() => { Player2.ChosenCard.Action(this, Player2, Player1); });
                    
                    ApplyFatigueDamage();
                    Phase = MatchPhase.CardEffect;
                    break;

                case MatchPhase.CardEffect:
                    RemoveSummonProtection();
                    // Initiate Moveminion effect
                    if(Minions.Count > 0)
                    {
                        VisualActions.Add(new VA_MoveMinions(this, true));
                        Phase = MatchPhase.MinionsToAction;
                    }
                    else // Skip MoveMinion Phase if no minions
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
                    if(Minions.Where(x => x.Destabilized).Count() > 0)
                    {
                        DestroyDestabilizedMinions();
                        Phase = MatchPhase.MinionDeaths;
                    }
                    else if (Minions.Count > 0) // Skip MinionDeaths if no destabilized minions
                    {
                        VisualActions.Add(new VA_MoveMinions(this, false));
                        Phase = MatchPhase.MinionsToPlan;
                    }
                    else // Skip MoveMinion Phase if no minions
                    {
                        PickCards();
                        Phase = MatchPhase.CardPick;
                    }
                    break;

                case MatchPhase.MinionDeaths:
                    if (Minions.Count > 0)
                    {
                        VisualActions.Add(new VA_MoveMinions(this, false));
                        Phase = MatchPhase.MinionsToPlan;
                    }
                    else // Skip MoveMinion Phase if no minions
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
                        RemoveSummonProtection();
                        QueueMinionEffects();
                        Phase = MatchPhase.MinionEffect;
                    }
                    break;

                case MatchPhase.MinionEffect:
                    DequeueEffect();
                    if (Phase != MatchPhase.GameEnded)
                    {
                        DestroyDestabilizedMinions();
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
            Debug.Log("##################### Starting Turn " + Turn + " #####################");

        // Pick Cards
        Player1.ChosenCard = null;
        Player2.ChosenCard = null;
        List<Card> Player1RandomCards = GetCardOptions(Player1.NumCardOptions);
        List<Card> Player2RandomCards = GetCardOptions(Player2.NumCardOptions);
        Player1.PickCard(Player1RandomCards);
        Player2.PickCard(Player2RandomCards);

        // Visual
        if (Visual)
        {
            if (MatchType == MatchType.AI_vs_AI)
            {
                MatchUI.ShowCards(Player1RandomCards, Player1, false, false);
                MatchUI.ShowCards(Player2RandomCards, Player2, false, false);
            }
            else
            {
                MatchUI.ShowCards(Player1RandomCards, Player1, false, true);
                MatchUI.ShowCards(Player2RandomCards, Player2, true, false); // Hide cards for opponent in human match
            }

            MatchUI.UpdateTurnText();
            // Show Genomes is AI vs AI match
            if (MatchType == MatchType.AI_vs_AI)
            {
                MatchUI.UpdatePlayerGenomes();
            }
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

    private void DestroyDestabilizedMinions()
    {
        List<Minion> destabilizedMinions = Minions.Where(x => x.Destabilized).ToList();
        if (destabilizedMinions.Count == 0) return;
        foreach(Minion m in destabilizedMinions)
        {
            Minions.Remove(m);
        }
        if(Visual)
        {
            VisualActions.Add(new VA_MinionDeaths(destabilizedMinions.Select(x => x.Visual).ToList(), MinionScale));
        }
        if (Log)
        {
            string names = "";
            foreach (Minion minion in destabilizedMinions) names += minion.Name + ", ";
            names = names.TrimEnd(new char[] { ',', ' ' });
            Debug.Log(names + " died from destabilization");
        }
    }

    private void RemoveSummonProtection()
    {
        foreach (Minion m in Minions.Where(x => x.HasSummonProtection)) m.HasSummonProtection = false;
        if(Log)
        {
            Debug.Log("Minions lost summon protection.");
        }
    }

    private void QueueMinionEffects()
    {
        foreach (Minion m in Minions.OrderBy(x => x.OrderNum))
        {
            Effects.Enqueue(() => { m.Action(); });
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
            if (VisualActions.Count > 0 && VisualActions[0].Done)
            {
                MatchUI.UpdatePlayerHealthText();
                VisualActions.RemoveAt(0);
            }

            if (Effects.Count > 0 && VisualActions.Count == 0) // && (VisualActions.Count == 0 || VisualActions[0].Done))
            {
                MatchUI.UpdatePlayerHealthText();
                CheckGameOver();

                if(Phase != MatchPhase.GameEnded) Effects.Dequeue().Invoke();
            }
            else if (VisualActions.Count > 0 && !VisualActions[0].Done)
            {
                VisualActions[0].Update();
            }
            else
            {
                MatchUI.UpdatePlayerHealthText();
                CheckGameOver();

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
            if (MatchType == MatchType.AI_vs_AI)
            {
                ((AIPlayer)Player2).Brain.Wins++;
                ((AIPlayer)Player1).Brain.Losses++;
            }
            Phase = MatchPhase.GameEnded;
        }
        else if (Player2.Health <= 0)
        {
            Winner = Player1;
            Loser = Player2;
            if (MatchType == MatchType.AI_vs_AI)
            {
                ((AIPlayer)Player1).Brain.Wins++;
                ((AIPlayer)Player2).Brain.Losses++;
            }
            Phase = MatchPhase.GameEnded;
        }
        if (Phase == MatchPhase.GameEnded)
        {
            //if (Player1.Name == "0.0" || Player2.Name == "0-0") Debug.Log("0-0 finished a game " + Player1.Name + "," + Player2.Name); // test if one player finished one match per round

            if (MatchType == MatchType.AI_vs_AI)
            {
                ((AIPlayer)Player1).Brain.DamageDealt = StartHealth - Player2.Health;
                ((AIPlayer)Player2).Brain.DamageReceived = StartHealth - Player2.Health;

                ((AIPlayer)Player1).Brain.DamageReceived = StartHealth - Player1.Health;
                ((AIPlayer)Player2).Brain.DamageDealt = StartHealth - Player1.Health;
            }

            if (Visual)
            {
                MatchUI.UpdatePlayerHealthText();
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
    private void VisualUpdate()
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

    #endregion

    #region Game Commands - Functions that can be called by Cards

    public void SummonMinion(Creature source, MinionType type, Player player, bool summonProtection)
    {
        Minion minion = null;
        if (NumMinions(player, type) < MaxMinionsPerType && NumMinions(player) < MaxMinions)
        {
            minion = CreateNewMinion(type, player);
            if (summonProtection) minion.HasSummonProtection = true;
            Minions.Add(minion);

            if (Visual)
            {
                minion.Visual = GameObject.Instantiate(V_Minion);
                minion.Visual.GetComponent<Renderer>().material.color = minion.Color;
                Vector3 targetPosition = GetPlanPosition(minion);
                VisualActions.Add(new VA_SummonMinions(new List<VisualEntity>() { minion.Visual }, source.Visual, new List<Vector3>() { targetPosition }, MinionScale));
            }
            if (Log)
            {
                Debug.Log(source.Name + " summoned " + minion.Name);
            }
        }
    }

    public void SummonMultipleMinions(Creature source, List<Tuple<MinionType, Player>> list, bool summonProtection)
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
                if (summonProtection) newMinion.HasSummonProtection = true;
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
            VisualActions.Add(new VA_SummonMinions(createdMinions.Select(x => x.Visual).ToList(), source.Visual, targetPositions, MinionScale));
        }
        if(Log)
        {
            string names = "";
            foreach (Minion minion in createdMinions) names += minion.Name + ", ";
            names = names.TrimEnd(new char[] { ',', ' ' });
            Debug.Log(source.Name + " summoned " + names);
        }
    }

    public void DestroyMinion(Creature source, Minion target)
    {
        if (target != null)
        {
            Minions.Remove(target);

            if (Visual)
            {
                VisualActions.Add(new VA_DestroyMinions(source.Visual, new List<VisualEntity>() { target.Visual }, Color.black));
            }
            if (Log)
            {
                Debug.Log(source.Name + " destroyed " + target.Name);
            }
        }
    }

    public void DestabilizeMinion(Creature source, Minion target)
    {
        if (target != null)
        {
            target.Destabilized = true;

            if (Visual)
            {
                VisualActions.Add(new VA_DestabilizeMinions(source.Visual, new List<VisualEntity>() { target.Visual }, new Color(1, 0, 1), MatchUI.DestabilizedTexture));
            }
            if (Log)
            {
                Debug.Log(source.Name + " destabilized " + target.Name);
            }
        }
    }

    public void DestroyMultipleMinions(Creature source, List<Minion> targets)
    {
        if (targets.Count > 0)
        {
            foreach (Minion target in targets)
            {
                Minions.Remove(target);
            }

            if (Visual)
            {
                VisualActions.Add(new VA_DestroyMinions(source.Visual, targets.Select(x => x.Visual).ToList(), Color.black));
            }

            if (Log)
            {
                string names = "";
                foreach (Minion minion in targets) names += minion.Name + ", ";
                names = names.TrimEnd(new char[] { ',', ' ' });
                Debug.Log(source.Name + " destroyed " + names);
            }
        }
    }

    public void Damage(Creature source, Player target, int amount)
    {
        target.Health = Mathf.Max(0, target.Health - amount);

        if(Visual)
        {
            if (source == target) VisualActions.Add(new VA_DamageSelf(source.Visual, amount));
            else VisualActions.Add(new VA_DealDamage(source.Visual, target.Visual, amount, source.Color));
        }
        if(Log)
        {
            Debug.Log(source.Name + " dealt " + amount + " damage to " + target.Name + " (now at " + target.Health + ")");
        }
    }

    public void Heal(Creature source, Player target, int amount)
    {
        if (target.Health >= target.MaxHealth || amount == 0) return;


        target.Health = Mathf.Min(target.MaxHealth, target.Health + amount);

        if (Visual)
        {
            if (source == target)
                VisualActions.Add(new VA_HealSelf(source.Visual, amount));
            else
                VisualActions.Add(new VA_Heal(source.Visual, target.Visual, amount, source.Color));
        }
        if (Log)
        {
            Debug.Log(source.Name + " healed " + target.Name + " for " + amount + " (now at " + target.Health + ")");
        }
    }

    public void AddCardOption(Creature source, Player target, int amount)
    {
        if (target.NumCardOptions == MaxCardOptions) return;
        if (MaxCardOptions - target.NumCardOptions < amount) amount = MaxCardOptions - target.NumCardOptions;

        target.NumCardOptions += amount;

        if(Visual)
        {
            VisualActions.Add(new VA_IncreaseCardOption(target.Visual, MatchUI));
        }
        if(Log)
        {
            Debug.Log(source.Name + " has increased " + target.Name + "'s card options by " + amount + ".");
        }
    }

    #endregion

    #region Helper Functions

    public Vector3 GetPlanPosition(Minion m)
    {
        List<Minion> orderedTypeList = Minions.Where(x => x.Type == m.Type && x.Owner == m.Owner).OrderBy(x => x.OrderNum).ToList();
        float position = orderedTypeList.IndexOf(m);
        float yPos;
        bool secondColumn = (position + 1) > MaxMinionsPerType / 2;

        float visualWidth = (Enum.GetNames(typeof(MinionType)).Length * (2 * MinionScale + MinionXGapPlan)) - MinionXGapPlan;
        float xPos = -(visualWidth / 2) + ((float)(m.Type - 1) * (MinionXGapPlan + 2 * MinionScale)) + MinionScale / 2;
        if (secondColumn) xPos += MinionScale;

        if (m.Owner == Player1)
        {
            if (secondColumn) yPos = -(VisualBoardHeight * MinionYStartPlan) + ((position - (MaxMinionsPerType / 2) - 1) * (MinionYGapPlan + MinionScale));
            else yPos = -(VisualBoardHeight * MinionYStartPlan) + ((position - 1) * (MinionYGapPlan + MinionScale));
        }
        else
        {
            if (secondColumn) yPos = (VisualBoardHeight * MinionYStartPlan) - ((position - MaxMinionsPerType / 2 - 1) * (MinionYGapPlan + MinionScale));
            else yPos = (VisualBoardHeight * MinionYStartPlan) - ((position - 1) * (MinionYGapPlan + MinionScale));
        }

        return new Vector3(xPos, 0, yPos);
    }

    public Vector3 GetActionPosition(Minion m)
    {
        List<Minion> orderedTypeList = Minions.OrderBy(x => x.OrderNum).ToList();
        float visualWidth = (Minions.Count * (MinionScale + MinionXGapAction)) - MinionXGapAction;
        float xPos = -(visualWidth / 2) + (orderedTypeList.IndexOf(m) * (MinionXGapAction + MinionScale)) + MinionScale / 2;
        float yPos = m.Owner == Player1 ? -(VisualBoardHeight * MinionYStartAction) : (VisualBoardHeight * MinionYStartAction);
        return new Vector3(xPos, 0, yPos);
    }

    private List<Card> GetCardOptions(int amount)
    {
        List<Card> cardListCopy = new List<Card>();
        cardListCopy.AddRange(Cards.Where(x => !x.AlwaysAppears));
        List<Card> options = new List<Card>();
        for (int i = 0; i < amount; i++)
        {
            Card c = cardListCopy[UnityEngine.Random.Range(0, cardListCopy.Count)];
            options.Add(c);
            cardListCopy.Remove(c);
        }
        options.AddRange(Cards.Where(x => x.AlwaysAppears));
        return options;
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

            case MinionType.Grey:
                return new M05_Grey(this, player, player.Enemy, SummonOrder++);
        }
        throw new Exception("Minion type not handled in minion creation!");
    }

    #endregion

    #region LINQ

    public int NumMinions(Player player, bool withoutSummonProtection = false)
    {
        if(withoutSummonProtection) return Minions.Where(x => x.Owner == player && !x.HasSummonProtection).Count();
        else return Minions.Where(x => x.Owner == player).Count();
    }

    public int NumMinions(Player player, MinionType type)
    {
        return Minions.Where(x => x.Owner == player && x.Type == type).Count();
    }

    public Minion RandomMinionFromPlayer(Player player, bool withoutDestabilized = false)
    {
        List<Minion> list = Minions.Where(x => x.Owner == player).ToList();
        if (withoutDestabilized) list = list.Where(x => !x.Destabilized).ToList();
        if (list.Count == 0) return null;
        return list[UnityEngine.Random.Range(0, list.Count)];
    }

    public List<Minion> RandomMinionsFromPlayer(Player player, int amount, bool withoutSummonProtection = false)
    {
        List<Minion> list = Minions.Where(x => x.Owner == player).ToList();
        if (withoutSummonProtection) list = list.Where(x => !x.HasSummonProtection).ToList();
        if (amount >= list.Count) return list;
        else
        {
            List<Minion> returnList = new List<Minion>();
            for (int i = 0; i < amount; i++)
            {
                Minion minion = list[UnityEngine.Random.Range(0, list.Count)];
                returnList.Add(minion);
                list.Remove(minion);
            }
            return returnList;
        }
    }

    public List<Minion> AllMinionsOfType(Player player, MinionType type, bool withoutSummonProtection = false)
    {
        List<Minion> list = Minions.Where(x => x.Owner == player && x.Type == type).ToList();
        if (withoutSummonProtection) list = list.Where(x => !x.HasSummonProtection).ToList();
        return list;
    }

    public MinionType RandomMinionType()
    {
        Array types = Enum.GetValues(typeof(MinionType));
        return (MinionType)types.GetValue(UnityEngine.Random.Range(0, types.Length));
    }

    #endregion

}
