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
    public List<Node> Nodes;//all (inputs, outputs, hidden) nodes of the network
    public List<Connection> Connections; //all connections (between 2 nodes) of the network
    public int Depth; //longest amount of connections from an input node to an output node
    public float Fitness;
    public float AdjustedFitness;
    public Species Species;

    public Genome(int id, List<Node> inputs, List<Node> outputs, List<Connection> initialConnections)
    {
        Id = id;
        InputNodes = inputs;
        OutputNodes = outputs;
        Connections = initialConnections;
        Nodes = new List<Node>();
        HiddenNodes = new List<Node>();
        Nodes.AddRange(inputs);
        Nodes.AddRange(outputs);
        CalculateDepths();
    }

    public Genome(int id, List<Node> nodes, List<Connection> connections)
    {
        Id = id;
        Nodes = nodes;
        InputNodes = nodes.Where(x => x.Type == NodeType.Input).ToList();
        HiddenNodes = nodes.Where(x => x.Type == NodeType.Hidden).ToList();
        OutputNodes = nodes.Where(x => x.Type == NodeType.Output).ToList();
        Connections = connections;
        CalculateDepths();
    }

    public Genome Copy()
    {
        List<Node> newNodes = new List<Node>();
        List<Connection> newConnections = new List<Connection>();
        foreach (Node n in Nodes) newNodes.Add(new Node(n.Id, n.Type));
        foreach (Connection c in Connections)
        {
            Node sourceNode = newNodes.First(x => x.Id == c.In.Id);
            Node targetNode = newNodes.First(x => x.Id == c.Out.Id);
            Connection newConnection = new Connection(c.InnovationNumber, sourceNode, targetNode);
            newConnections.Add(newConnection);
            sourceNode.OutConnections.Add(newConnection);
            targetNode.InConnections.Add(newConnection);
            newConnection.Weight = c.Weight;
            newConnection.Enabled = c.Enabled;
        }
        return new Genome(Id, newNodes, newConnections);
    }

    public float[] FeedForward(float[] inputs)
    {
        if (inputs.Length != InputNodes.Count) throw new System.Exception("Wrong amount of inputs");

        // Set input values
        for (int i = 0; i < inputs.Length; i++)
            InputNodes[i].Value = inputs[i];

        // Calculate values for each depth layer
        for(int i = 1; i < Depth + 1; i++)
        {
            List<Node> layerNodes = Nodes.Where(x => x.Depth == i).ToList();
            foreach(Node n in layerNodes)
            {
                n.Value = 0;
                foreach(Connection c in n.InConnections.Where(x => x.Enabled))
                {
                    n.Value += c.In.Value * c.Weight;
                }
                n.Value = Sigmoid(n.Value);
            }
        }

        return OutputNodes.Select(x => x.Value).ToArray();
    }

    public void CalculateDepths()
    {
        foreach (Node n in Nodes) n.Depth = 0;
        foreach(Node input in InputNodes)
                depthStep(input, 0);
        Depth = Nodes.Max(x => x.Depth);
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
                if (depth >= c.Out.Depth) depthStep(c.Out, depth + 1);
        }
    }

    // Activation function - returns a value between 0 and 1
    private float Sigmoid(double value)
    {
        return 1.0f / (1.0f + (float)Math.Exp(-value));
    }
}
