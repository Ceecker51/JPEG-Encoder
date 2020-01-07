using System.Collections.Generic;
using System.Collections;

using FluentAssertions;
using NUnit.Framework;

using encoder.lib;
using encoder.utils;

namespace encoder.test
{
  class CoefficientsTest
  {
    [Test]
    public void Test_SelectDCValues()
    {
      // generate random array
      int[,] input = ArrayHelper.GetTwoDimensionalArrayOfLength(16);
      // do the zick zack
      List<int[]> output = ZickZack.ZickZackSortChannel(input);

      // Action
      int[] expected = new[] { 0, 8, 128, 136 };
      int[] actual = Coefficients.SelectDCValues(output);

      // Assertion
      actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Test_CalculateDCDifferences()
    {
      // generate random array
      int[,] input = ArrayHelper.GetTwoDimensionalArrayOfLength(16);
      // do the zick zack
      List<int[]> output = ZickZack.ZickZackSortChannel(input);

      // Action
      int[] expected = new[] { 0, 8, 120, 8 };
      int[] actual = Coefficients.CalculateDCDifferences(output);

      // Assertion
      actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Test_RunLengthEncodeDCValues()
    {
      // generate random array
      int[,] input = ArrayHelper.GetTwoDimensionalArrayOfLength(16);
      // do the zick zack
      List<int[]> output = ZickZack.ZickZackSortChannel(input);

      // Action
      var actual = Coefficients.EncodeDCValueDifferences(output);

      DCEncode[] dcStructs = { new DCEncode(0, -1),
                               new DCEncode(4, 8),
                               new DCEncode(7, 120),
                               new DCEncode(4, 8)
                              };

      List<DCEncode> expected = new List<DCEncode>(dcStructs);

      // Assertion
      actual.Should().BeEquivalentTo(expected);

    }

    [Test]
    public void Test_SelectACValues()
    {
      int[] block = { 128, 57, 45, 0, 0, 0, 0, 23,
                        0, -30, -16, 0, 0, 1, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0,
                        0, 0, 0, 0, 0, 0, 0, 0 };

      int[] expected = { 57, 45, 0, 0, 0, 0, 23,
                          0, -30, -16, 0, 0, 1, 0, 0,
                          0, 0, 0, 0, 0, 0, 0, 0,
                          0, 0, 0, 0, 0, 0, 0, 0,
                          0, 0, 0, 0, 0, 0, 0, 0,
                          0, 0, 0, 0, 0, 0, 0, 0,
                          0, 0, 0, 0, 0, 0, 0, 0,
                          0, 0, 0, 0, 0, 0, 0, 0 };

      int[] actual = Coefficients.SelectACValues(block);

      actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Test_RunLengthHelper_Normal()
    {
      int[] acValues = Coefficients.SelectACValues(
        new[] { 128, 57, 45, 0, 0, 0, 0, 23,
                  0, -30, -16, 0, 0, 1, 0, 0,
                  0, 0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0, 0, 0
        });

      (int, int)[] tuples = { (0, 57), (0, 45), (4, 23), (1, -30), (0, -16), (2, 1), (0, 0) };
      List<(int, int)> expected = new List<(int, int)>(tuples);

      List<(int, int)> actual = Coefficients.RunLengthHelper(acValues);

      actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Test_RunLengthHelper_WithMaxAmountOfZeros()
    {
      int[] acValues = Coefficients.SelectACValues(
        new[] { 128, 57, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 3, 0, 0, 0,
                  0, 2, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 0, 0, 0, 0, 0,
                  0, 0, 0, 895, 0, 0, 0, 0
        });

      (int, int)[] tuples = { (0, 57), (15, 0), (2, 3), (4, 2), (15, 0), (15, 0), (1, 895), (0, 0) };
      List<(int, int)> expected = new List<(int, int)>(tuples);

      List<(int, int)> actual = Coefficients.RunLengthHelper(acValues);

      actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Test_GetCategory()
    {
      int[] input = { 0, 1, 3, 7, -12, -31, 60, 63, 127, 255, 256, 511, 1023, 2047, 4095, 8191, 16383, 32767, 32768 };
      (int, int)[] expected = { (0, -1), (1, 1), (2, 3), (3, 7), (4, 3), (5, 0), (6, 60), (6, 63), (7, 127), (8, 255), (9, 256), (9, 511), (10, 1023), (11, 2047), (12, 4095), (13, 8191), (14, 16383), (15, 32767), (-1, -1) };

      (int, int)[] actual = new (int, int)[input.Length];
      for (int i = 0; i < input.Length; i++)
      {
        actual[i] = Coefficients.GetCategory(input[i]);
      }

      actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Test_GenerateYDCTree()
    {
      DCEncode[] dcStructs = { new DCEncode(8, 250),
                               new DCEncode(8, 250),
                               new DCEncode(4, 8),
                               new DCEncode(7, 120),
                              };

      HuffmanTree tree = Picture.GenerateYDCTree(new List<DCEncode>(dcStructs));
      string actual = tree.DictToString(tree.createDictionary());
      string expected = "8: 0|4: 10|7: 110|";

      actual.Should().Equals(expected);
    }

    [Test]
    public void Test_GenerateCbCrDCTree()
    {
      var dcStructs1 = new List<DCEncode>(new[]{
        new DCEncode(8, 250),
        new DCEncode(8, 250),
      });

      var dcStructs2 = new List<DCEncode>(new[]{
        new DCEncode(4, 8),
        new DCEncode(7, 120),
      });

      HuffmanTree tree = Picture.GenerateCbCrDCTree(dcStructs1, dcStructs2);
      string actual = tree.DictToString(tree.createDictionary());
      string expected = "8: 0|4: 10|7: 110|";

      actual.Should().Equals(expected);
    }

    [Test]
    public void Test_GenerateYACTree()
    {
      var acStructs = new List<ACEncode>(new[] {
        new ACEncode(0, 6, 63),
        new ACEncode(0, 6, 63),
        new ACEncode(4, 5, 31),
        new ACEncode(2, 7, 127)
      });
      var acList = new List<List<ACEncode>>();
      acList.Add(acStructs);

      HuffmanTree tree = Picture.GenerateYACTree(acList);
      string actual = tree.DictToString(tree.createDictionary());
      string expected = "6: 0|69: 10|39: 110|";

      actual.Should().StartWith(expected);
    }

    [Test]
    public void Test_GenerateYACTree_2()
    {

      var acStructs = new List<ACEncode>(new[] {
        new ACEncode(4, 5, 31),
        new ACEncode(4, 5, 31),
        new ACEncode(4, 5, 31),
        new ACEncode(4, 5, 31),
        new ACEncode(2, 7, 127),
        new ACEncode(2, 7, 127),
        new ACEncode(1, 1, 1),
        new ACEncode(9, 3, 3)
      });

      var acList = new List<List<ACEncode>>();
      acList.Add(acStructs);

      HuffmanTree tree = Picture.GenerateYACTree(acList);
      string actual = tree.DictToString(tree.createDictionary());
      string expected = "69: 0|39: 10|17: 110|147: 1110|";

      actual.Should().StartWith(expected);
    }

    [Test]
    public void Test_GenerateCbCrACTree()
    {

      var acStructs1 = new List<ACEncode>(new[] {
        new ACEncode(4, 5, 31),
        new ACEncode(4, 5, 31),
        new ACEncode(4, 5, 31),
        new ACEncode(4, 5, 31)
      });

      var acStructs2 = new List<ACEncode>(new[] {
        new ACEncode(2, 7, 127),
        new ACEncode(2, 7, 127),
        new ACEncode(1, 1, 1),
        new ACEncode(9, 3, 3)
      });

      var acList1 = new List<List<ACEncode>>();
      acList1.Add(acStructs1);

      var acList2 = new List<List<ACEncode>>();
      acList2.Add(acStructs2);

      HuffmanTree tree = Picture.GenerateCbCrACTree(acList1, acList2);
      string actual = tree.DictToString(tree.createDictionary());
      string expected = "69: 0|39: 10|17: 110|147: 1110|";

      actual.Should().StartWith(expected);
    }
  }
}
