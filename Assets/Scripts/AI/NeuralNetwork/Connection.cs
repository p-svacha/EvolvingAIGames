using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection {

    public int Id;
    public Node FromNode; //source node, where the connection comes from
    public Node ToNode; //target node, where the connection goes to
    public float Weight; // [-1, 1]
    public bool Enabled;

    // UI
    public GameObject VisualConnection;

    public Connection(int id, Node from, Node to)
    {
        Id = id;
        FromNode = from;
        ToNode = to;
        Enabled = true;
    }

}
