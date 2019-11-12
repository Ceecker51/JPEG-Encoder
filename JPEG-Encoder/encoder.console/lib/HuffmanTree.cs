using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace encoder.lib
{
  public class HuffmanTree
  {
    public Dictionary<char, int> frequencies = new Dictionary<char, int>();
    public Node Root { get; set; }

    private List<Node> nodes = new List<Node>();
    private List<Node> weightedNodes = new List<Node>();

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
      if (currentNode.Left != null)
      {
        Print(currentNode.Left);
      }

      Console.Write(currentNode.Symbol);

      if (currentNode.Right != null)
      {
        Print(currentNode.Right);
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

    //decode einen Bitstream mit den static Tree
    public char[] Decode(BitStream stream)
    {
      List<char> output = new List<char>();

      Node position = Root;
      foreach (int bit in stream.readBits())
      {
        if (bit == 1)
        {
          position = position.Right;

          if (position.Right == null)
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
        Travers(node.Right, data, true, dic);

        data.RemoveAt(data.Count - 1);
      }
      else
      {
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
            Symbol = '*',
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
    public void RightBalance()
    {
      calculateNodeDepths(Root, 0);
      weightedNodes = weightedNodes.OrderBy(node => node.Depth).ToList();

      Console.WriteLine("right balanced tree (für William <3)");
      weightedNodes.ForEach(node => Console.WriteLine(node.Depth + " -> " + node.Symbol));

      Node newRoot = new Node() { Symbol = '*', Depth = 0 };
      var currentDepth = 1;
      addLeaves(newRoot, currentDepth);
      Root = newRoot;
  
    }

        private void addLeaves(Node currentNode, int currentDepth)
        {
            //erst links checken
            //List<Node> validItems = weightedNodes.Where(node => node.Depth == currentDepth).ToList();
            if (weightedNodes[0].Depth == currentDepth)
            {
                //Leaf zuweisen
                Node nextNode = weightedNodes[0];
                currentNode.Left = nextNode;
                weightedNodes.RemoveAt(0);
            }
            else
            {
                //links deeper
                Node nextNode = new Node() { Symbol = '*', Depth = currentDepth } ;
                currentNode.Left = nextNode;
                addLeaves(nextNode, currentDepth + 1);
            }
            //dann rechts checken
            if (weightedNodes[0].Depth == currentDepth)
            {
                //Leaf zuweisen
                Node nextNode = weightedNodes[0];
                currentNode.Right = nextNode;
                weightedNodes.RemoveAt(0);
            }
            else
            {
                //rechts deeper
                Node nextNode = new Node() { Symbol = '*', Depth = currentDepth };
                currentNode.Right = nextNode;
                addLeaves(nextNode, currentDepth + 1);
            }
        }

        private void calculateNodeDepths(Node currentNode, int currentDepth)
        {
            if (currentNode.Left == null)
            {
                currentNode.Depth = currentDepth;
                weightedNodes.Add(currentNode);
                return;
            }
            calculateNodeDepths(currentNode.Left, currentDepth + 1);
            calculateNodeDepths(currentNode.Right, currentDepth + 1);
        }
  }


  public class Node
  {
    public char Symbol { get; set; }
    public int Frequence { get; set; }
    public int Depth { get; set; }

    public Node Left { get; set; }
    public Node Right { get; set; }

  }
}
