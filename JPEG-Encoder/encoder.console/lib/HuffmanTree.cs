using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace encoder.lib
{
  public class HuffmanTree
  {
    private const int MAX_DEPTH = 3;

    public Dictionary<char, int> frequencies = new Dictionary<char, int>();
    public Node Root { get; set; }

    private List<Node> nodes = new List<Node>();
    private List<Node> nodesWithDepth = new List<Node>();

    public void Print()
    {
      Console.WriteLine("Huffman-Tree:");

      if (Root == null) Console.WriteLine("Baum ist leer");
      else Print(Root);

      Console.WriteLine();
      Console.WriteLine();
    }

    private void Print(Node currentNode)
    {
      if (currentNode.Left != null && currentNode.Right == null)
      {
        Console.Write("(");
        Console.Write(currentNode.Left.Symbol);
        Console.Write(currentNode.Symbol);
        Console.Write("");
        Console.Write(")");
        return;

      }
      if (currentNode.Left != null)
      {
        Console.Write("(");
        Print(currentNode.Left);
      }

      Console.Write(currentNode.Symbol);

      if (currentNode.Right != null)
      {
        Print(currentNode.Right);
        Console.Write(")");
      }
    }

    //encode einen beliebigen char Array zu Bitstream
    public BitStream Encode(char[] input)
    {
      BitStream outputStream = new BitStream();

      // Create glossary for the characters
      Dictionary<char, BitArray> dictionary = createDictionary();

      // Print dictionary
      foreach (KeyValuePair<char, int> element in frequencies)
      {
        Console.Write(element.Key + ": ");
        BitArray bits = dictionary[element.Key];
        foreach (bool bit in bits)
        {
          Console.Write((bit ? 1 : 0));
        }
        Console.WriteLine();
      }
      Console.WriteLine();

      // write to Bitstream
      foreach (char token in input)
      {
        BitArray bits = dictionary[token];
        foreach (bool bit in bits)
        {
          int value = (bit ? 1 : 0);
          outputStream.writeBit(value);
        }
      }
      return outputStream;
    }

    //decode bitstream with static tree
    public char[] Decode(BitStream stream)
    {
      List<char> output = new List<char>();

      Node position = Root;
      foreach (int bit in stream.readBits())
      {
        if (bit == 1)
        {
          position = position.Right;

          if (position.Left == null)
          {
            output.Add(position.Symbol);
            position = Root;
          }
        }
        else if (bit == 0)
        {
          position = position.Left;

          if (position.Left == null)
          {
            output.Add(position.Symbol);
            position = Root;
          }
        }
      }
      return output.ToArray();
    }

    // erstellt ein dictionary zum schnellen encoden
    private Dictionary<char, BitArray> createDictionary()
    {
      List<bool> bits = new List<bool>();
      Dictionary<char, BitArray> dictionary = new Dictionary<char, BitArray>();

      Console.WriteLine("Dictionary:");

      if (Root == null)
      {
        Console.WriteLine("<empty>");
      }
      else
      {
        Node next = Root;
        Travers(next.Left, bits, false, dictionary);
        Travers(next.Right, bits, true, dictionary);
      }

      return dictionary;
    }

    // von createDictionary benutzt um den Baum Rekursiv ablaufen zu können
    private void Travers(Node node, List<bool> data, bool direction, Dictionary<char, BitArray> dic)
    {
      data.Add(direction);

      if (node.Left != null)
      {
        Travers(node.Left, data, false, dic);

        // the right hand node will be null at the end, hence skip the traversing
        if (node.Right != null)
        {
          Travers(node.Right, data, true, dic);
        }
        data.RemoveAt(data.Count - 1);
      }
      else
      {
        // transform list of bool to array
        BitArray bits = new BitArray(data.ToArray());
        dic.Add(node.Symbol, bits);

        data.RemoveAt(data.Count - 1);
      }
    }

    public void Build(char[] input)
    {
      // creates List of unique input chars with quantity
      for (int i = 0; i < input.Length; i++)
      {
        if (!frequencies.ContainsKey(input[i]))
        {
          frequencies.Add(input[i], 0);
        }

        frequencies[input[i]]++;
      }

      // add for each element a single node in tree
      foreach (KeyValuePair<char, int> element in frequencies)
      {
        Node node = new Node();
        node.Symbol = element.Key;
        node.Frequence = element.Value;
        nodes.Add(node);
      }

      while (nodes.Count > 1)
      {
        List<Node> sortedNodes = nodes.OrderBy(node => node.Frequence).ToList();

        if (sortedNodes.Count >= 2)
        {
          List<Node> taken = sortedNodes.Take(2).ToList();

          // create the parent node of the combinded elements
          Node parent = new Node()
          {
            Symbol = '^',
            Frequence = sortedNodes[0].Frequence + sortedNodes[1].Frequence,
            Left = sortedNodes[0],
            Right = sortedNodes[1]
          };

          // Remove combined elements from node list and add parent node
          nodes.Remove(taken[0]);
          nodes.Remove(taken[1]);
          nodes.Add(parent);
        }

        // set the new root element
        Root = nodes.FirstOrDefault();
      }
    }

    private void DepthConstrain()
    {
      // select nodes which are too deep
      var nodesTooDeep = nodesWithDepth.Where(node => node.Depth > MAX_DEPTH).ToList();

      // calculate total cost
      double totalCost = 0;
      foreach (var node in nodesTooDeep)
      {
        int difference = node.Depth - MAX_DEPTH;
        totalCost += calculateCost(difference);
      }
      Console.WriteLine("--> " + totalCost);

      // set all nodes that are too deep to Max_DEPTH
      nodesWithDepth.ForEach(node => { if (node.Depth > MAX_DEPTH) node.Depth = MAX_DEPTH; });
      
      // get nodes lower then max depth, sort by depth and frequency
      var shallowNodes = nodesWithDepth.Where(node => node.Depth < MAX_DEPTH)
                                       .OrderByDescending(node => node.Depth)
                                       .ThenBy(node => node.Frequence)
                                       .ToList();

      // pay depts
      // 1. Fall: Möglichst einen unteren Pfad ändern
      (var changedNodes, var noDebt) = payDebts(cloneNodes(shallowNodes), (int) totalCost);
      if (!noDebt) Console.WriteLine("😣");

      changedNodes.AddRange(nodesWithDepth.Where(node => node.Depth >= MAX_DEPTH).ToList());
      nodesWithDepth = changedNodes;
    }

    private List<Node> cloneNodes(List<Node> oldNodes)
    {
      List<Node> newNodes = new List<Node>(oldNodes.Count);
      oldNodes.ForEach(node => newNodes.Add(new Node(node)));
      return newNodes;
    }

    private Tuple<List<Node>, bool> payDebts(List<Node> nodes, int currentDebt)
    {
      if (currentDebt == 0)
      {
        return new Tuple<List<Node>, bool>(nodes, true);
      }

      for (int i = 0; i < nodes.Count; i++)
      {
        int depthDifference = MAX_DEPTH - nodes[i].Depth;
        if (depthDifference == 0) continue;

        int retCredit = (int) Math.Pow(2, depthDifference - 1);
        if (retCredit <= currentDebt)
        {
          currentDebt -= retCredit;
          nodes[i].Depth++;

          nodes = nodes.OrderByDescending(node => node.Depth)
                       .ThenBy(node => node.Frequence)
                       .ToList();

          var tuple = payDebts(cloneNodes(nodes), currentDebt);
          if (tuple.Item2)
          {
            return tuple;
          }
        }
        break;
      }

      return new Tuple<List<Node>, bool>(nodes, false);
    }

    private double calculateCost(int depthDifference)
    {
      double sum = 0;
      for (int i = 1; i <= depthDifference; i++)
      {
        sum += Math.Pow(0.5, i);
      }

      Console.WriteLine("Calculated Costs: " + sum);
      
      // when adding up costs it should always return whole number
      return sum;
    }

    /// <summary>
    ///   create right balanced tree
    /// </summary>
    public void RightBalance()
    {
      // add depth property to all nodes
      calculateNodeDepths(Root, 0);

      // sort weighted nodes by depth
      nodesWithDepth = nodesWithDepth.OrderBy(node => node.Depth).ToList();

      // depth constrains
      DepthConstrain();

      // TODO: sort by Freuquency
      nodesWithDepth = nodesWithDepth.OrderBy(node => node.Depth).ToList();

      // create new root and add leaves
      Node newRoot = new Node() { Symbol = '^', Depth = 0 };
      var currentDepth = 1;
      addLeaves(newRoot, currentDepth);

      // replace root
      Root = newRoot;
    }


    /// <summary>
    ///   helper method to create right balanced tree
    /// </summary>
    private void addLeaves(Node currentNode, int currentDepth)
    {
      // handle left side: check if currentDepth fits first Node
      if (nodesWithDepth[0].Depth == currentDepth)
      {
        // set left side leaf
        Node nextNode = nodesWithDepth[0];
        currentNode.Left = nextNode;
        nodesWithDepth.RemoveAt(0);
      }
      else
      {
        // create interims node and add leaves recursively
        Node nextNode = new Node() { Symbol = '^', Depth = currentDepth };
        currentNode.Left = nextNode;
        addLeaves(nextNode, currentDepth + 1);
      }

      // handle right side: check if currentDepth fits first Node
      if (nodesWithDepth[0].Depth == currentDepth)
      {
        if (nodesWithDepth.Count == 1)
        {
          // last added node is moved one deeper and to the left
          Node nextNode = new Node() { Symbol = '^', Depth = currentDepth };
          currentNode.Right = nextNode;
          nextNode.Left = nodesWithDepth[0];
          nodesWithDepth.RemoveAt(0);
        }
        else
        {
          // set right side leaf
          Node nextNode = nodesWithDepth[0];
          currentNode.Right = nextNode;
          nodesWithDepth.RemoveAt(0);
        }
      }
      else
      {
        // create interims node and add leaves recursively
        Node nextNode = new Node() { Symbol = '^', Depth = currentDepth };
        currentNode.Right = nextNode;
        addLeaves(nextNode, currentDepth + 1);
      }
    }

    private void calculateNodeDepths(Node currentNode, int currentDepth)
    {
      if (currentNode.Left == null)
      {
        currentNode.Depth = currentDepth;
        nodesWithDepth.Add(currentNode);
        return;
      }
      calculateNodeDepths(currentNode.Left, currentDepth + 1);
      calculateNodeDepths(currentNode.Right, currentDepth + 1);
    }
  }


  public class Node
  {
    public Node() { }
    public Node(Node node)
    {
      this.Symbol = node.Symbol;
      this.Frequence = node.Frequence;
      this.Depth = node.Depth;
    }

    public char Symbol { get; set; }
    public int Frequence { get; set; }
    public int Depth { get; set; }

    public Node Left { get; set; }
    public Node Right { get; set; }
  }
}
