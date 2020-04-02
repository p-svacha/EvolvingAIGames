using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection {

    public int InnovationNumber;
    public Node From; //source node, where the connection comes from
    public Node To; //target node, where the connection goes to
    public float Weight; // [-1, 1]
    public bool Enabled;

    // UI
    public GameObject VisualConnection;

    public Connection(int innovationNumber, Node from, Node to)
    {
        InnovationNumber = innovationNumber;
        From = from;
        To = to;
        Enabled = true;
    }

}
