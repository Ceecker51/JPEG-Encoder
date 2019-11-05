using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace encoder.lib
{
    public class HuffmanTree
    {
        public List<Element> Elements = new List<Element>();

        public Node Root { get; set; }
        private List<Node> nodes = new List<Node>();

        public HuffmanTree(List<Element> elements)
        {
            Elements = elements;

            growTree();
        }

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

            Console.Write(currentNode.Element.Symbol);

            if (currentNode.Right!= null)
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
            foreach (Element element in Elements)
            {
                Console.Write(element.Symbol + ": ");
                BitArray bits = dictionary[element.Symbol];
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
                        output.Add(position.Element.Symbol);
                        position = Root;
                    }
                }
                else if (bit == 0)
                {
                    position = position.Left;

                    if (position.Left == null)
                    {
                        output.Add(position.Element.Symbol);
                        position = Root;
                    }
                }
            }
            return output.ToArray();
        }

        // Huffman Algorithmus zum Bauen eines Baumes angewendet
        private void growTree()
        {
            // add for each element a single node in tree
            foreach (Element element in Elements)
            {
                Node node = new Node(element);
                nodes.Add(node);
            }

            while (nodes.Count > 1)
            {
                List<Node> sortedNodes = nodes.OrderBy(node => node.Element.Frequence).ToList();

                if (sortedNodes.Count >= 2)
                {
                    List<Node> taken = sortedNodes.Take(2).ToList();

                    // combine the lowest frequence nodes together in a new element
                    Element mergedElement = new Element('*');
                    mergedElement.Frequence = taken[0].Element.Frequence + taken[1].Element.Frequence;

                    // create the parent node of the combinded elements
                    Node parent = new Node(mergedElement);
                    parent.Left = sortedNodes[0];
                    parent.Right = sortedNodes[1];

                    // Remove combined elements from node list and add parent node
                    nodes.Remove(taken[0]);
                    nodes.Remove(taken[1]);
                    nodes.Add(parent);
                }

                // set the new root element
                Root = nodes.FirstOrDefault();
            }
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
                dic.Add(node.Element.Symbol, bits);

                data.RemoveAt(data.Count - 1);
            }
        }

        public static HuffmanTree Build(char[] input)
        {
            List<Element> elements = calculateProb(input);
            return new HuffmanTree(elements);
        }

        //berechnet Häufigkeit jedes einzelnen Zeichens und speichert Infos in Liste
        public static List<Element> calculateProb(char[] input)
        {
            List<Element> tokens = new List<Element>();

            // creates List of unique input chars with quantity
            for (int i = 0; i < input.Length; i++)
            {
                char currentChar = input[i];

                Element nodeExists = tokens.Find(element => element.Symbol == currentChar);
                if (nodeExists != null)
                {
                    nodeExists.Frequence++;
                }
                else
                {
                    Element node = new Element(currentChar);
                    tokens.Add(node);
                }
            }

            return tokens;
        }
    }

    public class Element
    {
        public char Symbol { get; set; }
        public int Frequence { get; set; }

        public Element(char content)
        {
            Symbol = content;
            Frequence = 1;
        }
    }

    public class Node
    {
        public Element Element { get; set; }

        public Node Left { get; set; }
        public Node Right { get; set; }

        public Node(Element element)
        {
            Element = element;
        }
    }
}
