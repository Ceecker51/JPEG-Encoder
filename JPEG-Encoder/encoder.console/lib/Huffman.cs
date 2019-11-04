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
            return tokens;

        }
        public static Tree growTree(List<Element> elements)
        {
            List<Tree> forrest = new List<Tree>();  
            //erster schritt huffman
            foreach (Element element in elements)
            {
                Tree tree = new Tree();
                tree.add(element);
                forrest.Add(tree);
            }
            //zweiter schritt huffman
            List<Tree> SortedTrees = forrest.OrderBy(tree => tree.Root.Element.Quantity).ToList();
        
            while (1 != SortedTrees.Count)
            {
                Tree mergedTree = new Tree();
                Element mergedElement = new Element(' ');
                mergedElement.Quantity = SortedTrees[0].Root.Element.Quantity + SortedTrees[1].Root.Element.Quantity;
                mergedTree.add(mergedElement);
                mergedTree.merge(SortedTrees[0]);
                mergedTree.merge(SortedTrees[1]);

                SortedTrees.RemoveAt(0);
                SortedTrees.RemoveAt(0);
                SortedTrees.Add(mergedTree);

                SortedTrees = SortedTrees.OrderBy(tree => tree.Root.Element.Quantity).ToList();

            }
            return SortedTrees[0];

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
            
    }
    class Node
    {
        public Node Left { get; set; }
        public Node Right { get; set; }
        public Node Parent { get; set; }
        public Element Element{ get; set; }

        public Node(Element element)
        {
            Element = element;
        }

    
    }

    
}
