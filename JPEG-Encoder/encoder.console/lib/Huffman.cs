using System;
using System.Collections.Generic;
using System.Linq;

namespace encoder.lib
{
    class Huffman
    {
        static Tree tree;

        //encode einen beliebigen char Array zu Bitstream
        public static BitStream encoding(char[] input)
        {
            BitStream outputStream = new BitStream();

            List<Element> elements = calculateProb(input);
            tree = growTree(elements);

            // Create glossary for the characters
            Dictionary<char, string> dictionary = tree.createDictionary();
            foreach (char token in input)
            {
                string value = dictionary[token];
                foreach (char number in value)
                {
                    int temp = (int)char.GetNumericValue(number);
                    outputStream.writeBit(temp);
                }
            }
            return outputStream;
        }

        //decode einen Bitstream mit den static Tree
        public static char[] decoding(BitStream stream)
        {
            List<char> output = new List<char>();

            Node position = tree.Root;
            foreach (int bit in stream.readBits())
            {
                if (bit == 1)
                {
                    position = position.Right;

                    if (position.Right == null)
                    {
                        output.Add(position.Element.Symbol);
                        position = tree.Root;
                    }
                }
                else if (bit == 0)
                {
                    position = position.Left;

                    if (position.Left == null)
                    {
                        output.Add(position.Element.Symbol);
                        position = tree.Root;
                    }
                }
            }
            return output.ToArray();
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

        // Huffman Algorithmus zum Bauen eines Baumes angewendet
        public static Tree growTree(List<Element> elements)
        {
            List<Tree> forrest = new List<Tree>();
            //erster Schritt huffman
            foreach (Element element in elements)
            {
                Tree tree = new Tree();
                tree.add(element);
                forrest.Add(tree);
            }
            //zweiter und dritter Schritt huffman
            List<Tree> SortedTrees = forrest.OrderBy(tree => tree.Root.Element.Frequence).ToList();

            while (1 != SortedTrees.Count)
            {
                Tree mergedTree = new Tree();
                Element mergedElement = new Element(' ');
                mergedElement.Frequence = SortedTrees[0].Root.Element.Frequence + SortedTrees[1].Root.Element.Frequence;
                mergedTree.add(mergedElement);
                mergedTree.merge(SortedTrees[0]);
                mergedTree.merge(SortedTrees[1]);

                SortedTrees.RemoveAt(0);
                SortedTrees.RemoveAt(0);
                SortedTrees.Add(mergedTree);

                SortedTrees = SortedTrees.OrderBy(tree => tree.Root.Element.Frequence).ToList();

            }
            return SortedTrees[0];
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

    class Node
    {   
        public Element Element { get; set; }

        public Node Left { get; set; }
        public Node Right { get; set; }

        public Node(Element element)
        {
            Element = element;
        }
    }

    class Tree
    {
        public Node Root { get; set; }

        public void add(Element element)
        {
            if (Root == null)
            {
                Root = new Node(element);
                return;
            }
        }

        public void merge(Tree tree)
        {
            if (Root.Left == null)
            {
                Root.Left = tree.Root;
                return;
            }
            if (Root.Right == null)
            {
                Root.Right = tree.Root;
            }
        }

        // erstellt ein dictionary zum schnellen encoden
        public Dictionary<char, string> createDictionary()
        {
            List<int> bits = new List<int>();
            Dictionary<char, string> dictionary = new Dictionary<char, string>();
            if (Root == null)
            {
                Console.WriteLine("<empty>");
            }
            else
            {
                Node next = Root;
                rekursivDeeper(next.Left, bits, 0, dictionary);
                rekursivDeeper(next.Right, bits, 1, dictionary);
            }
            return dictionary;
        }

        // von createDictionary benutzt um den Baum Rekursiv ablaufen zu können
        public void rekursivDeeper(Node node, List<int> bits, int direction, Dictionary<char, string> dic)
        {
            bits.Add(direction);

            if (node.Left != null)
            {
                rekursivDeeper(node.Left, bits, 0, dic);
                rekursivDeeper(node.Right, bits, 1, dic);

                bits.RemoveAt(bits.Count - 1);
            }
            else
            {
                Console.Write(node.Element.Symbol + ": ");
                string code = string.Empty;

                foreach (int bit in bits)
                {
                    code = code + bit;
                    Console.Write(bit);
                }
                Console.WriteLine();

                dic.Add(node.Element.Symbol, code);
                bits.RemoveAt(bits.Count - 1);
            }
        }
    }
}
