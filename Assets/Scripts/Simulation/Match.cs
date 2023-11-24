using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A match is one duel between two subjects within a generation of the simulation.
/// <br/> Match-specific data will be processed in this class.
/// <br/> The match outcome will be recorded in the subjects histories.
/// </summary>
public abstract class Match
{
    // General
    protected SimulationModel SimulationModel;
    public MatchSimulationMode SimulationMode { get; private set; }
    public bool IsVisual => SimulationMode != MatchSimulationMode.SimulateInBackground;
    public MatchPhase MatchPhase { get; private set; }

    // Players
    public Subject Subject1 { get; private set; }
    public Subject Subject2 { get; private set; }

    public Subject Winner { get; private set; }
    public Subject Loser { get; private set; }

    // UI
    public MatchUI MatchUI { get; private set; }

    public Match(SimulationModel model, Subject subject1, Subject subject2, MatchSimulationMode simulationMode)
    {
        SimulationModel = model;
        MatchUI = model.MatchUI;
        Subject1 = subject1;
        Subject2 = subject2;
        SimulationMode = simulationMode;
        MatchPhase = MatchPhase.Initialized;

        // Set opponents
        if (SimulationMode != MatchSimulationMode.Play)
        {
            subject1.Opponents.Add(subject2);
            subject2.Opponents.Add(subject1);
        }
    }

    /// <summary>
    /// Gets called each frame while a match is running.
    /// </summary>
    public abstract void Update();

    /// <summary>
    /// Ends the match and sets a winner.
    /// </summary>
    public virtual void EndGame(Subject winner)
    {
        if(winner == Subject1)
        {
            Winner = Subject1;
            Loser = Subject2;
        }
        else
        {
            Winner = Subject2;
            Loser = Subject1;
        }

        if(SimulationMode != MatchSimulationMode.Play)
        {
            Winner.Wins++;
            Loser.Losses++;
        }

        MatchPhase = MatchPhase.Finished;

        OnMatchEnd();
    }

    /// <summary>
    /// Starts the match.
    /// </summary>
    public void StartMatchSimulation()
    {
        MatchPhase = MatchPhase.Running;
        OnMatchStart();
    }

    /// <summary>
    /// Gets called when the match is starting.
    /// </summary>
    public virtual void OnMatchStart() { }

    /// <summary>
    /// Gets called when the match is done and a winner is set.
    /// </summary>
    public virtual void OnMatchEnd() { }
}