﻿using System;
using System.Collections.Generic;
using System.Text;

namespace encoder.utils
{
  static class ArrayHelper
  {
    public static int[,] GetTwoDimensionalArrayOfLength(int N)
    {
      int counter = 0;
      int[,] array = new int[N, N];

      for (int i = 0; i < N; i++)
      {
        for (int j = 0; j < N; j++)
        {
          array[i, j] = counter;
          counter++;
        }
      }

      return array;
    }

    public static int[,] Get8X8SubArray(int[,] source, int rowStart, int columnStart)
    {
      int[,] result = new int[8, 8];
      for (int row = rowStart; row < rowStart + 8; row++)
      {
        for (int column = columnStart; column < columnStart + 8; column++)
        {
          result[row - rowStart, column - columnStart] = source[column, row];
        }
      }
      return result;
    }

    public static void PrintArray(int[] array)
    {
      for (int i = 0; i < array.Length; i++)
      {
        int index = i + 1;
        if ((index % 8) == 0)
        {
          Console.Write(array[i] + "\t");
          Console.WriteLine();
        }
        else
        {
          Console.Write(array[i] + "\t");
        }
      }
    }

    public static void PrintArray(int[,] array)
    {
      int yLength = array.GetLength(0);
      int xLegnth = array.GetLength(1);

      for (int y = 0; y < yLength; y++)
      {
        for (int x = 0; x < yLength; x++)
        {
          Console.Write(array[y, x] + "\t");
        }
        Console.WriteLine();
      }
    }
  }
}
