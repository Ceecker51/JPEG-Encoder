using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace encoder.lib
{
  public class HuffmanTree
  {
    // Constants
    private const int MAX_DEPTH = 5;

    // Fields
    private Dictionary<char, int> frequencies;
    private List<Node> nodesWithDepth = new List<Node>();

    public Dictionary<int, BitArray> TreeDictionary = new Dictionary<int, BitArray>();
    public int[] frequenciesOfDepths = new int[16];
    public char[] symbolsInTreeOrder;

    // Properties
    public Node Root { get; private set; }
    public Dictionary<char, int> Frequencies { get { return frequencies; } }

    public void Build(char[] input)
    {
      // create a list of unique input chars with quantity
      Dictionary<char, int> frequencies = new Dictionary<char, int>();
      for (int i = 0; i < input.Length; i++)
      {
        if (!frequencies.ContainsKey(input[i]))
        {
          frequencies.Add(input[i], 0);
        }
        frequencies[input[i]]++;
      }

      Build(frequencies);
    }

    public void Build(Dictionary<char, int> frequencies)
    {
      this.frequencies = frequencies;

      // add for each element a single node in tree
      List<Node> nodes = new List<Node>();
      foreach (KeyValuePair<char, int> element in frequencies)
      {
        Node node = new Node(element.Key, element.Value);
        nodes.Add(node);
      }

      BuildTree(nodes);
    }

    private void BuildTree(List<Node> nodes)
    {
      while (nodes.Count > 1)
      {
        // sort nodes by frequency
        List<Node> sortedNodes = nodes.OrderBy(node => node.Frequency).ToList();

        if (sortedNodes.Count >= 2)
        {
          // Select right element from list and remove it from list
          Node left = sortedNodes[0];
          nodes.Remove(sortedNodes[0]);

          // Select right element from list and remove it from list
          Node right = sortedNodes[1];
          nodes.Remove(sortedNodes[1]);

          // combinde elements under one new parent node
          Node parent = new Node(left, right);
          nodes.Add(parent);
        }
      }

      // set the new root element
      Root = nodes.FirstOrDefault();
    }

    /// <summary>
    ///   create right balanced tree
    /// </summary>
    public void RightBalance()
    {
      // add depth property to all nodes
      calculateNodeDepths(Root, 0);

      // sort weighted nodes by depth then frequency
      nodesWithDepth = nodesWithDepth.OrderBy(node => node.Depth)
                                     .ThenByDescending(node => node.Frequency)
                                     .ToList();

      Console.WriteLine("size: " + nodesWithDepth.Count);
      // constrain depth of tree (only if it's constrainable)
      if (nodesWithDepth.Last().Depth > MAX_DEPTH) DepthConstrain();

      Console.WriteLine("size: " + nodesWithDepth.Count);
      // sort weighted nodes by depth then frequency
      nodesWithDepth = nodesWithDepth.OrderBy(node => node.Depth)
                                     .ThenByDescending(node => node.Frequency)
                                     .ToList();

      CreateDHTDictionary(nodesWithDepth);

      // create new root and add leaves
      Node newRoot = new Node();
      if (Root.Left == null && Root.Right == null)
      {
        newRoot.Left = Root;
        Root = newRoot;
        return;
      }

      var currentDepth = 1;
      addLeaves(newRoot, currentDepth);

      // replace root
      Root = newRoot;
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
      LogLine("--> " + totalCost);

      // set all nodes that are too deep to Max_DEPTH
      nodesWithDepth.ForEach(node => { if (node.Depth > MAX_DEPTH) node.Depth = MAX_DEPTH; });

      // get nodes lower then max depth, sort by depth and frequency
      var shallowNodes = nodesWithDepth.Where(node => node.Depth < MAX_DEPTH)
                                       .OrderByDescending(node => node.Depth)
                                       .ThenBy(node => node.Frequency)
                                       .ToList();

      // lower nodes to fit all nodes below MAX_DEPTH
      bool noDebt = payDebts(shallowNodes, (int)totalCost);

      if (!noDebt)
      {
        //create more debt
        int newTotalCost = (int)totalCost;
        for (int i = 0; i < nodesWithDepth.Count; i++)
        {
          Console.WriteLine("Durchlauf " + i);

          if (nodesWithDepth[i].Depth == MAX_DEPTH)
          {
            nodesWithDepth[i].Depth--;
            newTotalCost++;
            shallowNodes = nodesWithDepth.Where(node => node.Depth < MAX_DEPTH)
                                       .OrderByDescending(node => node.Depth)
                                       .ThenBy(node => node.Frequency)
                                       .ToList();
            noDebt = payDebts(shallowNodes, newTotalCost);
            if (noDebt)
            {
              Console.WriteLine("HAT Funktioniert !!");
              break;
            }
          }
        }
      }
      if (!noDebt) throw new Exception(String.Format("Not able to restrict to max depth {0}.. 🤪", MAX_DEPTH));
    }

    private bool payDebts(List<Node> nodes, int currentDebt)
    {
      if (currentDebt == 0)
      {
        return true;
      }

      int currentDebtCopy = currentDebt;
      for (int i = 0; i < nodes.Count; i++)
      {
        int depthDifference = MAX_DEPTH - nodes[i].Depth;
        if (depthDifference == 0) continue;

        // calculate how much debt can be paid off with this node
        int retCredit = (int)Math.Pow(2, depthDifference - 1);
        if (retCredit <= currentDebtCopy)
        {
          currentDebtCopy -= retCredit;
          nodes[i].Depth++;

          // sort by depth then frequency
          /*
          nodes = nodes.OrderByDescending(node => node.Depth)
                       .ThenBy(node => node.Frequence)
                       .ToList();
                */
          bool noDebt = payDebts(nodes, currentDebtCopy);

          // return if debt is 0 
          if (noDebt) return true;

          // reset current dept + move node upwards
          currentDebtCopy = currentDebt;
          nodes[i].Depth--;
        }
        else
        {
          continue;
        }
      }

      return false;
    }

    private double calculateCost(int depthDifference)
    {
      double sum = 0;
      for (int i = 1; i <= depthDifference; i++)
      {
        sum += Math.Pow(0.5, i);
      }

      LogLine("Calculated Costs: " + sum);

      // when adding up costs it should always return whole number
      return sum;
    }

    private void CreateDHTDictionary(List<Node> nodes)
    {
      symbolsInTreeOrder = new char[nodes.Count];

      for (int i = 0; i < nodes.Count; i++)
      {
        if (i == nodes.Count - 1)
        {
          frequenciesOfDepths[nodes[i].Depth]++;
        }
        else
        {
          frequenciesOfDepths[nodes[i].Depth - 1]++;
        }
        symbolsInTreeOrder[i] = nodes[i].Symbol;
      }
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
        Node nextNode = new Node(Node.DEFAULT_SYMBOL, 0, currentDepth);
        currentNode.Left = nextNode;
        addLeaves(nextNode, currentDepth + 1);
      }

      // handle right side: check if currentDepth fits first Node
      if (nodesWithDepth[0].Depth == currentDepth)
      {
        if (nodesWithDepth.Count == 1)
        {
          // last added node is moved one deeper and to the left
          Node nextNode = new Node(currentDepth);
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
        Node nextNode = new Node(currentDepth);
        currentNode.Right = nextNode;
        addLeaves(nextNode, currentDepth + 1);
      }
    }

    public void EncodeInt(int input, BitStream outputStream)
    {
      BitArray bits = TreeDictionary[input];
      foreach (bool bit in bits)
      {
        int value = (bit ? 1 : 0);
        outputStream.writeBit(value);
      }
    }

    //encode einen beliebigen char Array zu Bitstream
    public BitStream Encode(char[] input)
    {
      BitStream outputStream = new BitStream();

      // Create glossary for the characters
      CreateLookUpDictionary();

      // Print dictionary
      foreach (KeyValuePair<char, int> element in frequencies)
      {
        Log(element.Key + ": ");
        BitArray bits = TreeDictionary[element.Key];
        foreach (bool bit in bits)
        {
          Log((bit ? 1 : 0));
        }
        LogLine();
      }
      LogLine();

      // write to Bitstream
      foreach (int token in input)
      {
        BitArray bits = TreeDictionary[token];
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

    public void CreateLookUpDictionary()
    {
      TreeDictionary = createDictionary();
    }

    // erstellt ein dictionary zum schnellen encoden
    public Dictionary<int, BitArray> createDictionary()
    {
      List<bool> bits = new List<bool>();
      Dictionary<int, BitArray> dictionary = new Dictionary<int, BitArray>();

      LogLine("Dictionary:");

      if (Root == null)
      {
        LogLine("<empty>");
      }
      else
      {
        Node next = Root;
        Travers(next.Left, bits, false, dictionary);

        // if root only has one node on the left side -> do not travers right side
        if (next.Right != null)
        {
          Travers(next.Right, bits, true, dictionary);
        }
      }

      return dictionary;
    }

    // von createDictionary benutzt um den Baum Rekursiv ablaufen zu können
    private void Travers(Node node, List<bool> data, bool direction, Dictionary<int, BitArray> dic)
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

    #region Printing Stuff

    public override string ToString()
    {
      if (Root == null) return "Baum ist leer";
      return ToStringHelper(Root);
    }

    public void Print()
    {
      LogLine("Huffman-Tree:");

      if (Root == null) LogLine("Baum ist leer");
      else Print(Root);

      LogLine();
      LogLine();
    }

    private void Print(Node currentNode)
    {
      Log("(");

      if (currentNode.Right == null && currentNode.Left == null)
      {
        Log("#\\");
        Log(currentNode.Symbol);
        Log(")");
        return;
      }

      Log("#\\");
      Log(currentNode.Depth);

      if (currentNode.Left != null)
      {
        Print(currentNode.Left);
      }

      if (currentNode.Right != null)
      {
        Print(currentNode.Right);
      }

      Log(")");
    }

    private string ToStringHelper(Node currentNode)
    {
      string result = "";
      result += "(";

      if (currentNode.Right == null && currentNode.Left == null)
      {
        result += @"#\";
        result += currentNode.Symbol;
        result += ")";
        return result;
      }

      result += @"#\";
      result += currentNode.Depth;

      if (currentNode.Left != null)
      {
        result += ToStringHelper(currentNode.Left);
      }

      if (currentNode.Right != null)
      {
        result += ToStringHelper(currentNode.Right);
      }

      result += ")";

      return result;
    }

    public string DictToString(Dictionary<int, BitArray> dictionary)
    {
      string result = "";

      foreach (KeyValuePair<char, int> element in frequencies)
      {
        result += (int)element.Key + ": ";
        BitArray bits = dictionary[element.Key];
        foreach (bool bit in bits)
        {
          result += bit ? 1 : 0;
        }
        result += "|";
      }

      return result;
    }

    #endregion Printing Stuff

    #region Logger
    private static void LogLine(string message = null)
    {
#if DEBUG
      Console.WriteLine(message);
#endif
    }

    private static void Log(string message = null)
    {
#if DEBUG
      Console.Write(message);
#endif
    }

    private static void Log(int message = 0)
    {
#if DEBUG
      Console.Write(message);
#endif
    }

    private static void Log(char message = ' ')
    {
#if DEBUG
      Console.Write(message);
#endif
    }
    #endregion
  }

  public class Node
  {
    public const char DEFAULT_SYMBOL = 'x';

    public char Symbol { get; set; }
    public int Frequency { get; set; }
    public int Depth { get; set; }

    public Node Left { get; set; }
    public Node Right { get; set; }

    public Node() : this(DEFAULT_SYMBOL, -1, 0) { }
    public Node(char symbol, int frequency) : this(symbol, frequency, 0) { }
    public Node(int depth) : this(DEFAULT_SYMBOL, -1, depth) { }
    
    public Node(char symbol, int frequency, int depth)
    {
      Symbol = symbol;
      Frequency = frequency;
      Depth = depth;
    }

    public Node(Node left, Node right)
    {
      Symbol = DEFAULT_SYMBOL;
      Frequency = left.Frequency + right.Frequency;
      Depth = 0;
      Left = left;
      Right = right;
    }
  }
}
