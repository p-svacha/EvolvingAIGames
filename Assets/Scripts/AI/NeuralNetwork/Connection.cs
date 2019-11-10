using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Connection {

    public int InnovationNumber;
    public Node In; //source node, where the connection comes from
    public Node Out; //target node, where the connection goes to
    public float Weight; // [-1, 1]
    public bool Enabled;

    // UI
    public GameObject VisualConnection;

    public Connection(int innovationNumber, Node input, Node output)
    {
        InnovationNumber = innovationNumber;
        In = input;
        Out = output;
        Enabled = true;
    }

}
