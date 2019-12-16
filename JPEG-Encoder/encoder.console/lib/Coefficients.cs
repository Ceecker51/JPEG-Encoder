using System;
using encoder.utils;
using System.Collections.Generic;
using System.Linq;

namespace encoder.console
{
  class Coefficients
  {
    const int N = 8;

    public static int[] CalculateDCDifferences(int[,] channel)
    {
      int width = channel.GetLength(0);
      int height = channel.GetLength(1);

      int[] dcValues = new int[(width / N) * (height / N)];
      int counter = 0;

      // grab dc values
      for (int row = 0; row < height; row += N)
      {
        for (int column = 0; column < width; column += N)
        {
          dcValues[counter] = channel[row, column];
          counter++;
        }
      }

      // calculate differences (starting with second item)
      for (int i = 1; i < dcValues.Length; i++)
      {
        int difference = dcValues[i] - dcValues[i - 1];
        dcValues[i] = difference;
      }

      return dcValues;
    }

    public static int[] RunLengthEncodeACValues(int[,] channel)
    {

      int width = channel.GetLength(0);
      int height = channel.GetLength(1);

      List<List<(int, int)>> result = new List<List<(int, int)>>();

      // grab ac values
      for (int row = 0; row < height; row += N)
      {
        for (int column = 0; column < width; column += N)
        {
          int[,] subArray = ArrayHelper.Get8X8SubArray(channel, row, column);
          result.Add(RunLengthEncodeACValuesPerBlock(subArray));
        }
      }

      return null;
    }

    private static List<(int, int)> RunLengthEncodeACValuesPerBlock(int[,] values)
    {
      // Tuple<int, int>[] acValues = new Tuple<int, int>[63];
      int[] acValues = new int[N * N - 1];
      int counter = 0;

      // grab ac values
      for (int row = 0; row < N; row += 1)
      {
        for (int column = 0; column < N; column += 1)
        {
          if ((row % N) + (column % N) == 0)
          {
            continue;
          }
          acValues[counter] = values[row, column];
          counter++;
        }
      }

      return null;
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
        // while (true)
        // {
        //   if (result[0].Item2 != 0)
        //   {
        //     break;
        //   }
        //   result.RemoveAt(0);
        // }
        result = result.SkipWhile(tuple => tuple.Item2 == 0).ToList();
        result.Insert(0, ((0, 0)));
      }
      result.Reverse();

      return result;
    }

    public static void CategoryEncoding(List<(int, int)> acValues)
    {
      List<int> bitMasks = new List<int>();
      List<(int, int)> result = acValues.Select(tuple =>
      {
        var (category, bitMask) = GetCategory(tuple.Item2);
        tuple.Item2 = category;
        if (category == 0)
        {
          bitMasks.Add(-1);
        }
        else
        {
          bitMasks.Add(bitMask);
        }
        return tuple;
      }).ToList();

      for (int i = 0; i < bitMasks.Count; i++)
      {
        Console.Write(result[i].ToString() + ", " + bitMasks[i].ToString());
        Console.WriteLine();
      }

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
  }

}