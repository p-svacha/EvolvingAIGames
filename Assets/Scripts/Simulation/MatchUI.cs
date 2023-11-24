using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The base class of the match UI that gets activated when a match is visually shown.
/// </summary>
public abstract class MatchUI : MonoBehaviour
{
    protected Match Match;

    public void Init(Match match)
    {
        Match = match;
        gameObject.SetActive(true);
        OnInit();
    }

    /// <summary>
    /// Gets called when the UI gets activated.
    /// </summary>
    public abstract void OnInit();
}
