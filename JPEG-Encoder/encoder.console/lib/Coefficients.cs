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

      // run length encoding for each block
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

      // while there are still values ...
      while (currentIndex < input.Length)
      {
        // grab current value
        int currentValue = input[currentIndex];
        currentIndex++;

        // if there are more than 15 zeros (special case)
        if (currentValue == 0 && nullCounter == 15)
        {
          // add new tuple to the front with (15, 0)
          result.Insert(0, ((nullCounter, currentValue)));
          nullCounter = 0;
          continue;
        }

        // increment null counter
        if (currentValue == 0)
        {
          nullCounter++;
          continue;
        }

        // add new tuple to the front e.g. (4, 64) -> 4 zeros and value 64
        result.Insert(0, ((nullCounter, currentValue)));
        nullCounter = 0;
      }

      // if there are some zeros left check for EOB (End of Block)
      if (nullCounter > 0 || result[0].Item2 == 0)
      {
        // (6,0) (15,0) (15,0) (15,0) (15,0) (4,64) ... --> (0,0)
        result = result.SkipWhile(tuple => tuple.Item2 == 0).ToList();
        result.Insert(0, ((0, 0)));
      }

      // reverse list
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
        ACEncode encoding = new ACEncode(tuple.Item1, category, bitMask);
        acEncodings.Add(encoding);
      });

      return acEncodings;
    }

    private static (int, int) GetCategory(int acValue)
    {
      int absoluteValue = Math.Abs(acValue);
      const int MAX_CATEGORY = 15;

      for (int category = 0; category < MAX_CATEGORY; category++)
      {
        int upperBound = UpperBound(category);
        if (absoluteValue <= upperBound)
        {
          return (category, GetBitmask(acValue, upperBound));
        }
      }

      // should not happen ... 
      return (-1, 0);
    }

    private static int UpperBound(int exponent)
    {
      return (int)Math.Pow(2, exponent) - 1;
    }

    private static int GetBitmask(int acValue, int upperBound)
    {
      if (acValue == 0)
      {
        return -1;
      }

      if (acValue < 0)
      {
        return upperBound + acValue;
      }

      return acValue;
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

      // 0000 0100 | 0000 0101 --> 0100 0101 -> 0x45
      Flag = (char)((zeros << 4) + category);
    }

    public string Print()
    {
      return string.Format("0:{0}, CAT:{1}, BITM:{2}, FLAG:{3}", Zeros, Category, Bitmask, Flag + 65);
    }
  }
}