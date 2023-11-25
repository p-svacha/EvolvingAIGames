using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Genome {

    public int Id;
    public List<Node> InputNodes; //all input nodes of the network, DOES NOT CHANGE
    public List<Node> HiddenNodes;
    public List<Node> OutputNodes; //all output nodes of the network, DOES NOT CHANGE
    public Dictionary<int, Node> Nodes;//all (inputs, outputs, hidden) nodes of the network
    public Dictionary<int, Connection> Connections; // All Connections of the network accessible by id
    public int Depth; //longest amount of connections from an input node to an output node
    public float Fitness;
    public float AdjustedFitness;
    public Species Species;

    public float MaxConnectionWeight;
    public float MinConnectionWeight;

    public List<Connection> EnabledConnections
    {
        get
        {
            return Connections.Values.Where(x => x.Enabled).ToList();
        }
    }

    public Genome(int id, Dictionary<int, Node> nodes, Dictionary<int, Connection> connections, float maxConnectionWeight, float minConnectionWeight)
    {
        Id = id;
        Nodes = nodes;
        MaxConnectionWeight = maxConnectionWeight;
        MinConnectionWeight = minConnectionWeight;
        InputNodes = nodes.Values.Where(x => x.Type == NodeType.Input).ToList();
        HiddenNodes = nodes.Values.Where(x => x.Type == NodeType.Hidden).ToList();
        OutputNodes = nodes.Values.Where(x => x.Type == NodeType.Output).ToList();
        Connections = connections;
        CalculateDepths();
    }

    public Genome Copy()
    {
        Dictionary<int, Node> newNodes = new Dictionary<int, Node>();
        Dictionary<int, Connection> newConnections = new Dictionary<int, Connection>();
        foreach (Node n in Nodes.Values) newNodes.Add(n.Id, new Node(n.Id, n.Type));
        foreach (Connection c in Connections.Values)
        {
            Node sourceNode = newNodes[c.FromNode.Id];
            Node targetNode = newNodes[c.ToNode.Id];
            Connection newConnection = new Connection(c.Id, sourceNode, targetNode);
            newConnections.Add(c.Id, newConnection);
            sourceNode.OutConnections.Add(newConnection);
            targetNode.InConnections.Add(newConnection);
            newConnection.Weight = c.Weight;
            newConnection.Enabled = c.Enabled;
        }
        return new Genome(Id, newNodes, newConnections, MaxConnectionWeight, MinConnectionWeight);
    }

    public float[] FeedForward(float[] inputs)
    {
        if (inputs.Length != InputNodes.Count) throw new System.Exception("Wrong amount of inputs");

        // Validate inputs (maybe make this optional?)
        foreach (float f in inputs)
            if (f < 0 || f > 1) throw new System.Exception("Input value is not within 0-1 range. Input value with index " + inputs.ToList().IndexOf(f) + " = " + f + "!");

        // Set input values
        for (int i = 0; i < inputs.Length; i++)
            InputNodes[i].Value = inputs[i];

        // Calculate values for each depth layer
        for(int i = 1; i < Depth + 1; i++)
        {
            List<Node> layerNodes = Nodes.Values.Where(x => x.Depth == i).ToList();
            foreach(Node n in layerNodes)
            {
                n.Value = 0;
                foreach(Connection c in n.InConnections.Where(x => x.Enabled))
                {
                    n.Value += c.FromNode.Value * c.Weight;
                }
                n.Value = Sigmoid(n.Value);
            }
        }

        return OutputNodes.Select(x => x.Value).ToArray();
    }

    public void CalculateDepths()
    {
        foreach (Node n in Nodes.Values)
        {
            if (n.Type == NodeType.Hidden && n.InConnections.Count == 0 && n.OutConnections.Count == 0)
                throw new Exception("Hidden Layer Node with Id " + n.Id + " has no connections!");
            n.Depth = 0;
        }
        foreach(Node input in InputNodes)
                depthStep(input, 0);
        Depth = Nodes.Values.Max(x => x.Depth);
        foreach (Node n in OutputNodes) n.Depth = Depth;

        // Special starting case where there are no connections at all
        if (Depth == 0)
        {
            Depth = 1;
            foreach (Node n in OutputNodes) n.Depth = 1;
        }
    }

    private void depthStep(Node n, int depth)
    {
        if (depth > 50)
        {
            Debug.Log("The depth of a node in a genome is greater than 50. This is an indication that there is a circular flow in the network.");
        }
        else
        {
            n.Depth = depth;
            foreach (Connection c in n.OutConnections.Where(x => x.Enabled))
                if (depth >= c.ToNode.Depth) depthStep(c.ToNode, depth + 1);
        }
    }

    // Activation function - returns a value between 0 and 1
    private float Sigmoid(double value)
    {
        return 1.0f / (1.0f + (float)Math.Exp(-value));
    }
}
