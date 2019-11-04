using encoder.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace encoder.lib
{
    public class Element
    {
        public char Content { get; set; }
        public int Quantity { get; set; }

        public Element(char content)
        {
            Content = content;
            Quantity = 1;
        }
    }

    class Huffman
    {

        public static BitStream encoding(char[] input)

        {
            List<Element> elements = calculateProb(input);
            Tree forrest = growTree(elements);
            return null; 
        }
        
       
        public static List<Element> calculateProb(char[] input)
        {
            List<Element> tokens = new List<Element>();
            // creates List of unique input chars with quantity
            for (int i = 0; i < input.Length; i++)
            {
                char currentChar = input[i];

                Element nodeExists = tokens.Find(element => element.Content == currentChar);
                if (nodeExists != null)
                {
                    nodeExists.Quantity++;
                }
                else
                {
                    Element node = new Element(currentChar);
                    tokens.Add(node);
                }
            }

            List<Element> SortedNodes = tokens.OrderBy(o => o.Quantity).ToList();
            return SortedNodes;
        }
        public static Tree growTree(List<Element> elements)
        {
            List<Tree> forrest = new List<Tree>();
            foreach (Element element in elements)
            {
                Tree tree = new Tree();
                tree.add(element);
                forrest.Add(tree);
            }
            return null;
           
        
        }

    }


    class Tree
    {
        Node root;
        
        public void add(Element element)
        {
            if (root == null)
            {
                root = new Node(element);
            }
        }
            
    }
    class Node
    {
        Node Left { get; set; }
        Node Right { get; set; }
        Node Parent { get; set; }
        public Element Element { get; set; }

        public Node(Element element)
        {
            Element = element;
        }

    
    }

    
}
