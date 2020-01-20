using System;
using System.Collections.Generic;
using System.Text;

using NUnit.Framework;
using FluentAssertions;
using encoder.lib;

namespace encoder.console.test
{
  class HuffmanTreeTest
  {
    [Test]
    public void Test_Constructor()
    {
      HuffmanTree tree = new HuffmanTree();

      Assert.IsNull(tree.Root);
      Assert.IsEmpty(tree.Frequencies);
    }

    [Test]
    public void Test_Build()
    {
      char[] input = { 's', 'a', '#', '#', 's', 'd', 'w', 's' };

      string expectedStructure = @"(#\0(#\0(#\w)(#\#))(#\0(#\0(#\a)(#\d))(#\s)))";
      Dictionary<char, int> expected = new Dictionary<char, int>();
      expected.Add('s', 3);
      expected.Add('a', 1);
      expected.Add('#', 2);
      expected.Add('d', 1);
      expected.Add('w', 1);

      HuffmanTree actual = new HuffmanTree();
      actual.Build(input);

      actual.Frequencies.Should().BeEquivalentTo(expected);
      Assert.AreEqual(expectedStructure, actual.ToString());
    }

    [Test]
    public void Test_DepthContrain()
    {
      string input = "eeeeeeeeeeeeeeeeeeeeeeeedddddddddddddddddddddddccccccccccbbbbbbbbbbbaaaaaaaaaaaxxxyyywvsr";

      HuffmanTree tree = new HuffmanTree();
      tree.Build(input.ToCharArray());
      tree.RightBalance();
    }
  }
}
