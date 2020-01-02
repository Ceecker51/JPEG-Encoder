using System;
using System.Collections.Generic;
using System.Linq;

namespace encoder.lib
{
  class Coefficients
  {
    const int N = 8;

    public static int[] CalculateDCDifferences(List<int[]> zickZackChannel)
    {
      int[] dcValues = zickZackChannel.Select(block => block[0]).ToArray();

      // calculate differences (starting with second item)
      for (int i = 1; i < dcValues.Length; i++)
      {
        int difference = dcValues[i] - dcValues[i - 1];
        dcValues[i] = difference;
      }

      return dcValues;
    }

    public static List<List<ACEncode>> RunLengthEncodeACValues(List<int[]> blocks)
    {
      List<List<ACEncode>> result = new List<List<ACEncode>>();

      // grab ac values
      foreach (int[] block in blocks)
      {
        result.Add(RunLengthEncodeACValuesPerBlock(block));
      }
      
      return result;
    }

    private static List<ACEncode> RunLengthEncodeACValuesPerBlock(int[] values)
    {
      // drop dc element from block
      int[] acValues = values.Skip(1).ToArray();

      // Encode length
      List<(int, int)> lengthEncoded = RunLengthHelper(acValues);

      // Encode category
      List<ACEncode> acEncodings = CategoryEncoding(lengthEncoded);

      return acEncodings;
    }

    public static List<(int, int)> RunLengthHelper(int[] input)
    {
      List<(int, int)> result = new List<(int, int)>();
      int currentIndex = 0;
      int nullCounter = 0;

      while (currentIndex < input.Length)
      {
        int currentValue = input[currentIndex];
        currentIndex++;

        if (currentValue == 0 && nullCounter == 15)
        {
          result.Insert(0, ((nullCounter, currentValue)));
          nullCounter = 0;
          continue;
        }

        if (currentValue == 0)
        {
          nullCounter++;
          continue;
        }

        result.Insert(0, ((nullCounter, currentValue)));
        nullCounter = 0;
      }

      if (nullCounter > 0)
      {
        result = result.SkipWhile(tuple => tuple.Item2 == 0).ToList();
        result.Insert(0, ((0, 0)));
      }
      result.Reverse();

      return result;
    }

    public static List<ACEncode> CategoryEncoding(List<(int, int)> acValues)
    {
      List<ACEncode> acEncodings = new List<ACEncode>();

      acValues.ForEach(tuple =>
      {
        var (category, bitMask) = GetCategory(tuple.Item2);

        // bundle zeros, category and bitmask into struct
        ACEncode encoding = new ACEncode(tuple.Item1, category, (category == 0) ? -1 : bitMask);
        acEncodings.Add(encoding);
      });

      return acEncodings;
    }

    private static (int, int) GetCategory(int coefficient)
    {
      int absoluteValue = Math.Abs(coefficient);
      const int MAX_CATEGORY = 15;

      for (int i = 0; i < MAX_CATEGORY; i++)
      {
        int upperBound = UpperBound(i);
        if (absoluteValue <= upperBound)
        {
          return (i, GetBitmask(coefficient, upperBound));
        }
      }

      // should not happen.. 
      return (-1, 0);
    }

    private static int UpperBound(int exponent)
    {
      return (int)Math.Pow(2, exponent) - 1;
    }

    private static int GetBitmask(int value, int upperBound)
    {
      if (value == 0)
      {
        return 0;
      }

      if (value < 0)
      {
        return upperBound + value;
      }

      return upperBound - (upperBound - value);
    }

    public static void PrintACValues(List<ACEncode> acEncodings)
    {
      for (int i = 0; i < acEncodings.Count; i++)
      {
        Console.Write(acEncodings[i].Print());
        Console.WriteLine();
      }
    }
  }

  struct ACEncode
  {
    readonly int Zeros;
    readonly int Category;
    readonly int Bitmask;
    readonly char Flag;

    public ACEncode(int zeros, int category, int bitmask)
    {
      Zeros = zeros;
      Category = category;
      Bitmask = bitmask;

      Flag = (char)((zeros << 4) + category);
    }

    public string Print()
    {
      return string.Format("0:{0}, CAT:{1}, BITM:{2}, FLAG:{3}", Zeros, Category, Bitmask, Flag + 65);
    }
  }
}