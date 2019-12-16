using System;

namespace encoder.console
{
  class Coefficients
  {

    public static int[] CalculateDCDifferences(int[,] channel)
    {
      int width = channel.GetLength(0);
      int height = channel.GetLength(1);

      int[] dcValues = new int[(width / 8) * (height / 8)];
      int counter = 0;

      // grab dc values
      for (int row = 0; row < height; row += 8)
      {
        for (int column = 0; column < width; column += 8)
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

      return null;


    }

    public static (int, int) GetCategory(int coefficient)
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