using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node {

    public int Id;
    public NodeType Type;
    public float Value;
    public int Depth; //how many connections this node is away from an input node, 0 for input nodes

    public List<Connection> InConnections;
    public List<Connection> OutConnections;

    //UI
    public GameObject VisualNode;
    public float VisualYPosition;

    public Node(int id, NodeType type)
    {
        Id = id;
        Type = type;
        InConnections = new List<Connection>();
        OutConnections = new List<Connection>();
    }

    public bool IsConnectedTo(Node n)
    {
        return InConnections.Exists(x => x.In == n) || OutConnections.Exists(x => x.Out == n);
    }
}
