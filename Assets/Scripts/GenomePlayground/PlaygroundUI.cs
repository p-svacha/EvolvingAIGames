using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PlaygroundUI : MonoBehaviour
{
    private Genome Parent1Genome;
    private Genome Parent2Genome;

    private TopologyMutator TopologyMutator;
    private WeightMutator WeightMutator;

    List<NewNodeMutation> NewNodeMutations;
    List<NewConnectionMutation> NewConnectionMutations;

    private int NumInputs;
    private int NumOutputs;

    // Visual Elements
    public Text NumInputNodesText;
    public Text NumOutputNodesText;
    public Button ResetBoth_Button;
    public Button TopologyMutationToBoth_Button;

    public GenomeVisualizer Parent1GV;
    public GenomeVisualizer Parent2GV;
    public GenomeVisualizer ChildGV;

    public Button P1_MutateTopology_Button;
    public Button P1_MutateWeight_Button;
    public Button P2_MutateTopology_Button;
    public Button P2_MutateWeight_Button;
    public Button CreateChild_Button;

    // Start is called before the first frame update
    void Start()
    {
        TopologyMutator = new TopologyMutator();
        WeightMutator = new WeightMutator();
        NewNodeMutations = new List<NewNodeMutation>();
        NewConnectionMutations = new List<NewConnectionMutation>();

        ResetBoth_Button.onClick.AddListener(ResetBoth_OnClick);
        TopologyMutationToBoth_Button.onClick.AddListener(TopologyMutationToBoth_OnClick);

        P1_MutateTopology_Button.onClick.AddListener(P1_MutateTopology_OnClick);
        P1_MutateWeight_Button.onClick.AddListener(P1_MutateWeight_OnClick);

        P2_MutateWeight_Button.onClick.AddListener(P2_MutateWeight_OnClick);
        P2_MutateTopology_Button.onClick.AddListener(P2_MutateTopology_OnClick);

        CreateChild_Button.onClick.AddListener(CreateChild_OnClick);
    }

    private void ResetBoth_OnClick()
    {
        int.TryParse(NumInputNodesText.text, out NumInputs);
        int.TryParse(NumOutputNodesText.text, out NumOutputs);

        if (NumInputs < 1) NumInputs = 1;
        if (NumInputs > 50) NumInputs = 50;
        if (NumOutputs < 1) NumOutputs = 1;
        if (NumOutputs > 50) NumOutputs = 50;

        TopologyMutator.NodeId = NumInputs + NumOutputs;

        NewNodeMutations.Clear();
        NewConnectionMutations.Clear();
        ResetGenome(Parent1GV);
        ResetGenome(Parent2GV);
        ChildGV.VisualizeGenome(Parent1Genome, false, true);
    }

    private void P1_MutateTopology_OnClick()
    {
        MutateTopology(Parent1GV, Parent1Genome);
    }

    private void P2_MutateTopology_OnClick()
    {
        MutateTopology(Parent2GV, Parent2Genome);
    }

    private void P1_MutateWeight_OnClick()
    {
        MutateWeight(Parent1GV, Parent1Genome);
    }

    private void P2_MutateWeight_OnClick()
    {
        MutateWeight(Parent2GV, Parent2Genome);
    }

    private void CreateChild_OnClick()
    {
        Parent1Genome.AdjustedFitness = 2;
        Parent2Genome.AdjustedFitness = 1;
        ChildGV.VisualizeGenome(CrossoverAlgorithm.Crossover(Parent1Genome, Parent2Genome), false, true);
    }

    private void TopologyMutationToBoth_OnClick()
    {
        if (Parent1Genome != null && Parent2Genome != null)
        {
            MutateTopology(Parent1GV, Parent1Genome);

            Parent2Genome.Nodes.Clear();
            Parent2Genome.InputNodes.Clear();
            Parent2Genome.HiddenNodes.Clear();
            Parent2Genome.OutputNodes.Clear();  
            foreach(Node n in Parent1Genome.Nodes.Values)
            {
                Node newNode = new Node(n.Id, n.Type);
                Parent2Genome.Nodes.Add(n.Id, newNode);
                if (n.Type == NodeType.Input) Parent2Genome.InputNodes.Add(newNode);
                if (n.Type == NodeType.Hidden) Parent2Genome.HiddenNodes.Add(newNode);
                if (n.Type == NodeType.Output) Parent2Genome.OutputNodes.Add(newNode);
            }

            Parent2Genome.Connections.Clear();
            foreach(Connection c in Parent1Genome.Connections.Values)
            {
                Node fromNode = Parent2Genome.Nodes[c.FromNode.Id];
                Node toNode = Parent2Genome.Nodes[c.ToNode.Id];
                Connection newCon = new Connection(c.Id, fromNode, toNode);
                newCon.Enabled = c.Enabled;
                newCon.Weight = c.Weight;
                fromNode.OutConnections.Add(newCon);
                toNode.InConnections.Add(newCon);
                Parent2Genome.Connections.Add(c.Id, newCon);
            }

            Parent2Genome.CalculateDepths();
            
            Parent2GV.VisualizeGenome(Parent2Genome, false, true);
        }
    }

    private void ResetGenome(GenomeVisualizer gv)
    {
        Dictionary<int, Node> nodes = new Dictionary<int, Node>();
        Dictionary<int, Connection> initialConnections = new Dictionary<int, Connection>();

        // Create input nodes
        for (int i = 0; i < NumInputs; i++)
            nodes.Add(i, new Node(i, NodeType.Input));

        // Create output nodes
        for (int i = NumInputs; i < (NumInputs + NumOutputs); i++)
            nodes.Add(i, new Node(i, NodeType.Output));

        if (gv == Parent1GV)
        {
            Parent1Genome = new Genome(CrossoverAlgorithm.GenomeId++, nodes, initialConnections, 2, -2);
            gv.VisualizeGenome(Parent1Genome, false, true);
        }
        else
        {
            Parent2Genome = new Genome(CrossoverAlgorithm.GenomeId++, nodes, initialConnections, 2, -2);
            gv.VisualizeGenome(Parent2Genome, false, true);
        }
        
    }

    private void MutateTopology(GenomeVisualizer gv, Genome g)
    {
        if (g != null)
        {
            TopologyMutator.MutateTopology(g, null, NewNodeMutations, NewConnectionMutations);
            gv.VisualizeGenome(g, false, true);
        }
    }

    private void MutateWeight(GenomeVisualizer gv, Genome g)
    {
        if (g != null)
        {
            WeightMutator.MutateWeight(g, null);
            gv.VisualizeGenome(g, false, true);
        }
    }
}
