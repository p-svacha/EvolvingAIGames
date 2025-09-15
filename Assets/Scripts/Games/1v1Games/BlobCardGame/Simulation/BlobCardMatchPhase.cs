using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BlobCardMatchPhase
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
