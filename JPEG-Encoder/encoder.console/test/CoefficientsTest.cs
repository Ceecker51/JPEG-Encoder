using System.Collections.Generic;

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
      int[] expected = new[] { 0, 8, 120, 16 };
      int[] actual = Coefficients.CalculateDCDifferences(output);

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
      (int, int)[] expected = { (-1, 0), (1, 1), (2, 3), (3, 7), (4, 4), (5, 0), (6, 60), (6, 63), (7, 127), (8, 255), (9, 256), (9, 511), (10, 1023), (11, 2047), (12, 4095), (13, 8191), (14, 16383), (15, 32767), (-1, 0) };

      (int, int)[] actual = new (int, int)[input.Length];
      for (int i = 0; i < input.Length; i++)
      {
        actual[i] = Coefficients.GetCategory(input[i]);
      }

      actual.Should().BeEquivalentTo(actual);
    }
  }
}
