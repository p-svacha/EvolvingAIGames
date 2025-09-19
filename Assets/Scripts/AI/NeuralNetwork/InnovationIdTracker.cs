using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class that handles the assignment and tracking of all node and connection id's within a simulation of an evolving population.
/// </summary>
public static class InnovationIdTracker
{
    private static int _nextConn = 0;
    private static int _nextNode = 0;

    // (fromNodeId,toNodeId) -> innovation id
    private static readonly Dictionary<(int, int), int> _connInnov = new();

    // splitConnectionId -> (newNodeId, inConnId, outConnId)
    private static readonly Dictionary<int, (int nodeId, int inId, int outId)> _splitInnov = new();

    public static void Initialize(int nextNodeId, int nextConnId)
    {
        _nextNode = nextNodeId;
        _nextConn = nextConnId;
    }

    public static void RegisterExistingConnectionInnovation(int from, int to, int id)
    {
        var key = (from, to);
        if (_connInnov.TryGetValue(key, out var existing))
        {
            if (existing != id)
                Debug.LogWarning($"Innovation mismatch for ({from}->{to}): {existing} vs {id}");
            return;
        }
        _connInnov[key] = id;
        if (id >= _nextConn) _nextConn = id + 1; // keep counter ahead
    }

    public static int GetOrCreateConnectionInnovation(int fromNodeId, int toNodeId)
    {
        var key = (fromNodeId, toNodeId);
        if (_connInnov.TryGetValue(key, out var id)) return id;
        id = _nextConn++;
        _connInnov[key] = id;
        return id;
    }

    public static (int nodeId, int inId, int outId) GetOrCreateSplitInnovation(int splitConnectionId)
    {
        if (_splitInnov.TryGetValue(splitConnectionId, out var triple)) return triple;
        int nodeId = _nextNode++;
        int inId = _nextConn++;
        int outId = _nextConn++;
        triple = (nodeId, inId, outId);
        _splitInnov[splitConnectionId] = triple;
        return triple;
    }
}
