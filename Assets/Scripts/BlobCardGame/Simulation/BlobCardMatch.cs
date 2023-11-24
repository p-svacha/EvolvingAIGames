using System.Collections;
using System.Collections.Generic;
using static BlobCardGame.CardList;
using UnityEngine;
using System;
using System.Linq;

namespace BlobCardGame
{
    public class BlobCardMatch : Match
    {
        // Simulation model
        protected new BlobCardSimulationModel SimulationModel;  

        // Match rules
        public const int StartHealth = 30;
        public const int StartCardOptions = 1;
        public const int MinCardOptions = 1;
        public const int MaxCardOptions = 5;
        public const int MaxMinions = 50;
        public const int MaxMinionsPerType = 8;
        public const int FatigueDamageStartTurn = 20;

        public Color PlayerColor = Color.gray;

        // Players
        public PlayerBlob Player1 { get; private set; }
        public PlayerBlob Player2 { get; private set; }
        public PlayerBlob WinnerBlob { get; private set; }
        public PlayerBlob LoserBlob { get; private set; }

        // Minions
        public List<Minion> Minions; // List with minions from both players
        public int SummonOrder;

        // Phase
        public int Turn;
        public BlobCardMatchPhase Phase;
        public bool NextPhaseReady;

        // Effects
        public Queue<Action> Effects;

        // Visual
        public BlobCardMatchUI UI;

        public bool Log;
        public List<VisualAction> VisualActions;

        private float VisualBoardHeight = 8;
        private float MinionScale = 0.4f;
        private float MinionXGapPlan = 0.4f;
        private float MinionXGapAction = 0.1f;
        private float MinionYGapPlan = 0.1f;
        private float MinionYStartPlan = 0.35f; // The higher this value is, the closer the minions are to the player (0 < x < 0.5)
        private float MinionYStartAction = 0.1f; // The higher this value is, the closer the minions are to the player (0 < x < 0.5)

        #region Initialization and Match Start

        public BlobCardMatch(BlobCardSimulationModel model, Subject subject1, Subject subject2, MatchSimulationMode simulationMode) : base(model, subject1, subject2, simulationMode)
        {
            UI = model.MatchUI;
            SimulationModel = (BlobCardSimulationModel)base.SimulationModel;

            // Set players
            if(SimulationMode == MatchSimulationMode.Play)
            {
                Player1 = new HumanPlayer(this);
                Player2 = new AIPlayer(this, subject2);
            }
            else
            {
                Player1 = new AIPlayer(this, subject1);
                Player2 = new AIPlayer(this, subject2);
            }
            Player1.Initialize(Player2, StartHealth, StartCardOptions);
            Player2.Initialize(Player1, StartHealth, StartCardOptions);

            // Init game values
            Log = false;
            Minions = new List<Minion>();
            Effects = new Queue<Action>();
            VisualActions = new List<VisualAction>();
            Turn = 0;
            SummonOrder = 0;

            Phase = BlobCardMatchPhase.GameInitialized;
        }

        #endregion

        #region Game Cycle

        public override void OnMatchStart()
        {
            if (IsVisual)
            {
                // UI
                UI.UpdatePlayerBar();
                UI.UpdateTurnText();
                if (SimulationMode == MatchSimulationMode.Watch) UI.UpdatePlayerGenomes();

                // Summon Players
                Player1.Visual = GameObject.Instantiate(SimulationModel.VisualPlayerPrefab, new Vector3(0, 0, -(VisualBoardHeight / 2)), Quaternion.identity);
                Player2.Visual = GameObject.Instantiate(SimulationModel.VisualPlayerPrefab, new Vector3(0, 0, VisualBoardHeight / 2), Quaternion.identity);
                Player1.Color = PlayerColor;
                Player2.Color = PlayerColor;
                VisualActions.Add(new VA_SummonPlayer(Player1.Visual, PlayerColor));
                VisualActions.Add(new VA_SummonPlayer(Player2.Visual, PlayerColor));
            }

            // Let's go
            Phase = BlobCardMatchPhase.GameReady;
        }

        public override void Update()
        {
            // During game phases (only visual)
            switch (Phase)
            {
                case BlobCardMatchPhase.GameReady:
                    if (IsVisual)
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

                case BlobCardMatchPhase.TurnStart:
                    DequeueEffect();
                    break;

                case BlobCardMatchPhase.CardPick:
                    if (!UI.HideCardsButton.gameObject.activeSelf) UI.SetHideCardsButtonVisible(true);

                    if (Player1.ChosenCard != null && Player2.ChosenCard != null)
                    {
                        NextPhaseReady = true;
                    }
                    break;

                case BlobCardMatchPhase.CardEffect:
                    DequeueEffect();
                    break;

                case BlobCardMatchPhase.MinionsToAction:
                    VisualUpdate();
                    break;

                case BlobCardMatchPhase.MinionEffect:
                    DequeueEffect();
                    break;

                case BlobCardMatchPhase.MinionDeaths:
                    VisualUpdate();
                    break;

                case BlobCardMatchPhase.MinionsToPlan:
                    VisualUpdate();
                    break;

                case BlobCardMatchPhase.GameEnded:
                    break;
            }

            // Changing game phases (only visual)
            if (NextPhaseReady && Input.GetKeyDown(KeyCode.Space))
            {
                NextPhaseReady = false;

                switch (Phase)
                {
                    case BlobCardMatchPhase.GameReady:
                        StartTurn();
                        Phase = BlobCardMatchPhase.TurnStart;
                        break;

                    case BlobCardMatchPhase.TurnStart:
                        GetCardOptions();
                        Phase = BlobCardMatchPhase.CardPick;
                        break;

                    case BlobCardMatchPhase.CardPick:
                        // Hide cards
                        UI.UnshowAllCards();
                        UI.SetHideCardsButtonVisible(false);
                        InitializeCardEffects();
                        ApplyFatigueDamage();
                        Phase = BlobCardMatchPhase.CardEffect;
                        break;

                    case BlobCardMatchPhase.CardEffect:
                        RemoveSummonProtection();
                        if (Minions.Count > 0)
                        {
                            InitializeMoveMinionsEffect(toAction: true);
                            Phase = BlobCardMatchPhase.MinionsToAction;
                        }
                        else // Skip MoveMinion Phase if no minions
                        {
                            QueueMinionEffects();
                            Phase = BlobCardMatchPhase.MinionEffect;
                        }
                        break;

                    case BlobCardMatchPhase.MinionsToAction:
                        // Queue minion effects
                        QueueMinionEffects();
                        Phase = BlobCardMatchPhase.MinionEffect;
                        break;

                    case BlobCardMatchPhase.MinionEffect:
                        if (Minions.Where(x => x.Destabilized).Count() > 0)
                        {
                            DestroyDestabilizedMinions();
                            Phase = BlobCardMatchPhase.MinionDeaths;
                        }
                        else if (Minions.Count > 0) // Skip MinionDeaths if no destabilized minions
                        {
                            InitializeMoveMinionsEffect(toAction: false);
                            Phase = BlobCardMatchPhase.MinionsToPlan;
                        }
                        else // Skip to next turn if there are no minions
                        {
                            StartTurn();
                            Phase = BlobCardMatchPhase.TurnStart;
                        }
                        break;

                    case BlobCardMatchPhase.MinionDeaths:
                        if (Minions.Count > 0)
                        {
                            InitializeMoveMinionsEffect(toAction: false);
                            Phase = BlobCardMatchPhase.MinionsToPlan;
                        }
                        else // Skip to next turn if there are no minions
                        {
                            StartTurn();
                            Phase = BlobCardMatchPhase.TurnStart;
                        }
                        break;

                    case BlobCardMatchPhase.MinionsToPlan:
                        StartTurn();
                        Phase = BlobCardMatchPhase.TurnStart;
                        break;

                    case BlobCardMatchPhase.GameEnded:
                        break;
                }
            }
        }

        private void CompleteWholeMatch()
        {
            while (Phase != BlobCardMatchPhase.GameEnded)
            {
                switch (Phase)
                {
                    case BlobCardMatchPhase.GameReady:
                        Phase = BlobCardMatchPhase.TurnStart;
                        break;

                    case BlobCardMatchPhase.TurnStart:
                        StartTurn();
                        Phase = BlobCardMatchPhase.CardPick;
                        break;

                    case BlobCardMatchPhase.CardPick:
                        GetCardOptions();
                        InitializeCardEffects();
                        ApplyFatigueDamage();
                        Phase = BlobCardMatchPhase.CardEffect;
                        break;

                    case BlobCardMatchPhase.CardEffect:
                        DequeueEffect();
                        if (Phase != BlobCardMatchPhase.GameEnded)
                        {
                            RemoveSummonProtection();
                            QueueMinionEffects();
                            Phase = BlobCardMatchPhase.MinionEffect;
                        }
                        break;

                    case BlobCardMatchPhase.MinionEffect:
                        DequeueEffect();
                        if (Phase != BlobCardMatchPhase.GameEnded)
                        {
                            DestroyDestabilizedMinions();
                            Phase = BlobCardMatchPhase.TurnStart;
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// Actions that occur at the start of a turn.
        /// </summary>
        private void StartTurn()
        {
            // Next turn
            Turn++;
            if (Log)
                Debug.Log("##################### Starting Turn " + Turn + " #####################");

            // Give each player money
            GiveMoney(Player1, Player1, 1);
            GiveMoney(Player2, Player2, 1);

        }

        /// <summary>
        /// Gets card options for each player. AI players also instantly chose a card (as set a card as ChosenCard)
        /// </summary>
        private void GetCardOptions()
        {
            // Pick Cards
            Player1.ChosenCard = null;
            Player2.ChosenCard = null;
            List<Card> Player1RandomCards = GetCardOptionsFor(Player1);
            List<Card> Player2RandomCards = GetCardOptionsFor(Player2);
            Player1.PickCard(Player1RandomCards);
            Player2.PickCard(Player2RandomCards);

            // Visual
            if (IsVisual)
            {
                if (SimulationMode == MatchSimulationMode.Watch)
                {
                    UI.ShowCards(Player1RandomCards, Player1, false, false);
                    UI.ShowCards(Player2RandomCards, Player2, false, false);
                }
                else
                {
                    UI.ShowCards(Player1RandomCards, Player1, false, true);
                    UI.ShowCards(Player2RandomCards, Player2, true, false); // Hide cards for opponent in human match
                }

                UI.UpdateTurnText();
                // Show Genomes is AI vs AI match
                if (SimulationMode == MatchSimulationMode.Watch) UI.UpdatePlayerGenomes();

            }
            if (Log)
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

        /// <summary>
        /// Adjust player money and initialize card effects according to the players ChosenCards
        /// </summary>
        private void InitializeCardEffects()
        {
            if (Player1.ChosenCard == null || Player2.ChosenCard == null) throw new Exception("Called InitializeCardEffects() without both players having chosen a card.");

            // Reduce money of players according to card costs
            Player1.Money -= Player1.ChosenCard.Cost;
            Player2.Money -= Player2.ChosenCard.Cost;

            // Update Player bar to show new money
            if (IsVisual) UI.UpdatePlayerBar();

            // Queue actions of the chosen cards (and show chosen cards if visual)
            if (IsVisual) Effects.Enqueue(() => { VisualActions.Add(new VA_ShowChosenCards(Player1, Player1.ChosenCard, UI)); });
            Effects.Enqueue(() => { Player1.ChosenCard.Action(this, Player1, Player2); });
            if (IsVisual) Effects.Enqueue(() => { VisualActions.Add(new VA_ShowChosenCards(Player2, Player2.ChosenCard, UI)); });
            Effects.Enqueue(() => { Player2.ChosenCard.Action(this, Player2, Player1); });
        }

        private void ApplyFatigueDamage()
        {
            if (Turn > FatigueDamageStartTurn)
            {
                int dmg = Turn - FatigueDamageStartTurn;
                Effects.Enqueue(() => { DealDamage(Player1, Player2, dmg); });
                Effects.Enqueue(() => { DealDamage(Player2, Player1, dmg); });
                if (Log)
                {
                    Debug.Log("Applying " + dmg + " fatigue damage each.");
                }
            }
        }

        /// <summary>
        /// Initializes the MoveMinions Visual Effect. toAction declares if the minions should go to their action position or plan position.
        /// </summary>
        /// <param name="toAction"></param>
        private void InitializeMoveMinionsEffect(bool toAction)
        {
            Dictionary<Minion, Vector3> minionPositions = new Dictionary<Minion, Vector3>();
            foreach (Minion m in Minions)
            {
                minionPositions.Add(m, toAction ? GetActionPosition(m) : GetPlanPosition(m));
            }
            VisualActions.Add(new VA_MoveMinions(minionPositions));
        }

        private void DestroyDestabilizedMinions()
        {
            List<Minion> destabilizedMinions = Minions.Where(x => x.Destabilized).ToList();
            if (destabilizedMinions.Count == 0) return;
            foreach (Minion m in destabilizedMinions)
            {
                Minions.Remove(m);
            }
            if (IsVisual)
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
            foreach (Minion m in Minions.Where(x => x.HasSummonProtection))
            {
                m.SetSummonProtection(this, false);
            }
            if (Log)
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
            if (IsVisual)
            {
                if (VisualActions.Count > 0 && VisualActions[0].Done)
                {
                    UI.UpdatePlayerBar();
                    VisualActions.RemoveAt(0);
                }

                if (Effects.Count > 0 && VisualActions.Count == 0) // && (VisualActions.Count == 0 || VisualActions[0].Done))
                {
                    UI.UpdatePlayerBar();
                    CheckGameOver();

                    if (Phase != BlobCardMatchPhase.GameEnded) Effects.Dequeue().Invoke();
                }
                else if (VisualActions.Count > 0 && !VisualActions[0].Done)
                {
                    VisualActions[0].Update();
                }
                else
                {
                    UI.UpdatePlayerBar();
                    CheckGameOver();

                    NextPhaseReady = true;
                }
            }

            // Non-visual, do everything in one update-step
            else
            {
                while (Effects.Count > 0)
                {
                    Effects.Dequeue().Invoke();
                    if (CheckGameOver()) break;
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
                EndGame(winner: ((AIPlayer)Player2).Brain);
                return true;
            }
            if (Player2.Health <= 0) 
            {
                EndGame(winner: ((AIPlayer)Player1).Brain);
                return true;
            }
            return false;
        }

        public override void OnMatchEnd()
        {
            Phase = BlobCardMatchPhase.GameEnded;
            if(((AIPlayer)Player1).Brain == Winner)
            {
                WinnerBlob = Player1;
                LoserBlob = Player2;
            }
            else
            {
                WinnerBlob = Player2;
                LoserBlob = Player1;
            }
            if (IsVisual)
            {
                UI.UpdatePlayerBar();
                GameObject.Destroy(Player1.Visual.gameObject);
                GameObject.Destroy(Player2.Visual.gameObject);
                foreach (Minion m in Minions) GameObject.Destroy(m.Visual.gameObject);
            }
            if (Log) Debug.Log(Winner.Name + " won against " + Loser.Name + " after " + Turn + " turns.");
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

        public void SummonMinions(Creature source, List<Tuple<MinionType, PlayerBlob>> list, bool summonProtection)
        {
            if (list.Count == 0) return;

            List<Minion> createdMinions = new List<Minion>();
            List<Vector3> targetPositions = new List<Vector3>();

            for (int i = 0; i < list.Count; i++)
            {
                MinionType type = list[i].Item1;
                PlayerBlob player = list[i].Item2;
                if (NumMinions(player, type) < MaxMinionsPerType && NumMinions(player) < MaxMinions)
                {
                    Minion newMinion = CreateNewMinion(type, player);
                    Minions.Add(newMinion);
                    createdMinions.Add(newMinion);

                    if (IsVisual)
                    {
                        newMinion.Visual = GameObject.Instantiate(SimulationModel.VisualMinionPrefab);
                        newMinion.Visual.GetComponent<Renderer>().material.color = newMinion.Color;
                        targetPositions.Add(GetPlanPosition(newMinion));
                    }

                    newMinion.SetSummonProtection(this, summonProtection);
                }
            }

            if (IsVisual)
            {
                VisualActions.Add(new VA_SummonMinions(createdMinions.Select(x => x.Visual).ToList(), source.Visual, targetPositions, MinionScale));
            }

            if (Log)
            {
                string names = "";
                foreach (Minion minion in createdMinions) names += minion.Name + ", ";
                names = names.TrimEnd(new char[] { ',', ' ' });
                Debug.Log(source.Name + " summoned " + names);
            }
        }

        public void DestabilizeMinions(Creature source, List<Minion> targets)
        {
            if (targets.Count == 0) return;

            foreach (Minion m in targets)
            {
                m.Destabilized = true;
            }

            if (IsVisual)
            {
                VisualActions.Add(new VA_DestabilizeMinions(source.Visual, targets.Select(x => x.Visual).ToList(), new Color(1, 0, 1), UI.DestabilizedTexture));
            }

            if (Log)
            {
                string names = "";
                foreach (Minion minion in targets) names += minion.Name + ", ";
                names = names.TrimEnd(new char[] { ',', ' ' });
                Debug.Log(source.Name + " destabilized " + names);
            }

        }

        public void DestroyMinions(Creature source, List<Minion> targets)
        {
            if (targets.Count == 0) return;

            foreach (Minion target in targets)
            {
                Minions.Remove(target);
            }

            if (IsVisual)
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

        public void DealDamage(Creature source, PlayerBlob target, int amount)
        {
            if (amount == 0) return;

            target.Health = Mathf.Max(0, target.Health - amount);

            if (IsVisual)
            {
                if (source == target) VisualActions.Add(new VA_DamageSelf(source.Visual, amount));
                else VisualActions.Add(new VA_DealDamage(source.Visual, target.Visual, amount, source.Color));
            }
            if (Log)
            {
                Debug.Log(source.Name + " dealt " + amount + " damage to " + target.Name + " (now at " + target.Health + ")");
            }
        }

        public void Heal(Creature source, PlayerBlob target, int amount)
        {
            if (target.Health >= target.MaxHealth || amount == 0) return;

            target.Health = Mathf.Min(target.MaxHealth, target.Health + amount);

            if (IsVisual)
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

        public void AddCardOption(Creature source, PlayerBlob target, int amount)
        {
            if (target.NumCardOptions == MaxCardOptions || amount == 0) return;

            if (MaxCardOptions - target.NumCardOptions < amount) amount = MaxCardOptions - target.NumCardOptions;

            target.NumCardOptions += amount;

            if (IsVisual)
            {
                VisualActions.Add(new VA_IncreaseCardOption(target.Visual, UI));
            }
            if (Log)
            {
                Debug.Log(source.Name + " has increased " + target.Name + "'s card options by " + amount + ".");
            }
        }

        public void GiveMoney(Creature source, PlayerBlob target, int amount)
        {
            if (amount == 0) return;

            target.Money += amount;

            if (Log) Debug.Log(source.Name + " has increased " + target.Name + "'s gold by " + amount + ".");
        }

        public void StealMinions(Creature source, PlayerBlob newOwner, List<Minion> targets, bool giveMinionsSummonProtection = false)
        {
            if (targets.Count == 0) return;

            foreach (Minion m in targets)
            {
                m.SetOwner(newOwner);
                if (giveMinionsSummonProtection) m.SetSummonProtection(this, true);
            }

            if (IsVisual)
            {
                Dictionary<Minion, Vector3> minionPositions = new Dictionary<Minion, Vector3>();
                foreach (Minion m in targets)
                {
                    minionPositions.Add(m, GetPlanPosition(m));
                }
                VisualActions.Add(new VA_MoveMinions(minionPositions));
            }

            if (Log)
            {
                string names = "";
                foreach (Minion minion in targets) names += minion.Name + ", ";
                names = names.TrimEnd(new char[] { ',', ' ' });
                Debug.Log(names + " got stolen by " + source.Name + " and given to " + newOwner.Name);
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

        private List<Card> GetCardOptionsFor(PlayerBlob player)
        {
            // Create Lists
            List<Card> possibleCards = new List<Card>();
            List<Card> options = new List<Card>();

            // Add x random options from other cards, where x is the amount of options of the player
            // Only affordable cards appear
            possibleCards.AddRange(Cards.Where(x => !x.AlwaysAppears && x.Cost <= player.Money));
            for (int i = 0; i < player.NumCardOptions; i++)
            {
                Card c = possibleCards[UnityEngine.Random.Range(0, possibleCards.Count)];
                options.Add(c);
                possibleCards.Remove(c);
            }

            // Add cards that always appear
            options.AddRange(Cards.Where(x => x.AlwaysAppears));

            return options;
        }

        private Minion CreateNewMinion(MinionType type, PlayerBlob player)
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

        public int NumMinions(PlayerBlob player, bool withoutSummonProtection = false)
        {
            if (withoutSummonProtection) return Minions.Where(x => x.Owner == player && !x.HasSummonProtection).Count();
            else return Minions.Where(x => x.Owner == player).Count();
        }

        public int NumMinions(PlayerBlob player, MinionType type)
        {
            return Minions.Where(x => x.Owner == player && x.Type == type).Count();
        }

        public List<Minion> RandomMinionsFromPlayer(PlayerBlob player, int amount, bool withoutSummonProtection = false, bool withoutDestabilized = false)
        {
            List<Minion> list = Minions.Where(x => x.Owner == player).ToList();
            if (withoutSummonProtection) list = list.Where(x => !x.HasSummonProtection).ToList();
            if (withoutDestabilized) list = list.Where(x => !x.Destabilized).ToList();
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

        public List<Minion> AllMinionsOfPlayer(PlayerBlob player, bool withoutSummonProtection = false)
        {
            List<Minion> list = Minions.Where(x => x.Owner == player).ToList();
            if (withoutSummonProtection) list = list.Where(x => !x.HasSummonProtection).ToList();
            return list;
        }

        public List<Minion> AllMinionsOfType(PlayerBlob player, MinionType type, bool withoutSummonProtection = false)
        {
            List<Minion> list = Minions.Where(x => x.Owner == player && x.Type == type).ToList();
            if (withoutSummonProtection) list = list.Where(x => !x.HasSummonProtection).ToList();
            return list;
        }

        /// <summary>
        /// Returns a random minion type.
        /// </summary>
        public MinionType RandomMinionType()
        {
            Array types = Enum.GetValues(typeof(MinionType));
            return (MinionType)types.GetValue(UnityEngine.Random.Range(0, types.Length));
        }

        /// <summary>
        /// Returns n different random minion types.
        /// </summary>
        public List<MinionType> RandomMinionTypes(int amount)
        {
            List<MinionType> allTypes = Enum.GetValues(typeof(MinionType)).Cast<MinionType>().ToList();
            List<MinionType> types = new List<MinionType>();
            for (int i = 0; i < amount; i++)
            {
                MinionType type = allTypes[UnityEngine.Random.Range(0, allTypes.Count)];
                types.Add(type);
                allTypes.Remove(type);
            }
            return types;
        }

        #endregion

    }
}
