using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MatchPhase
{
    GameInitialized,
    GameReady,
    TurnStart,
    CardPick,
    CardEffect,
    MinionsToAction,
    MinionEffect,
    MinionDeaths,
    MinionsToPlan,
    GameEnded
}
