using MathNet.Numerics.LinearAlgebra;
using System;

namespace encoder.lib
{
  class Quantization
  {

    readonly static int N = 8;
    readonly static int[,] luminanceQT = {{16, 11, 10, 16, 24, 40, 51, 61},
                                {12,12,14,19,26,58,60,55},
                                {14,13,16,24,40,57,69,56},
                                {14,17,22,26,51,87,80,62},
                                {18,22,37,56,68,109,103,77},
                                {24,35,55,64,81,104,113,92},
                                {49,64,78,87,103,121,120,101},
                                {72,92,95,98,112,100,103,99}
                                };

    readonly static int[,] chrominanceQT = {{17, 18, 24, 47, 99,99,99,99},
                                {18,21,26,66,99,99,99,99},
                                {24,26,56,99,99,99,99,99},
                                {47,66,99,99,99,99,99,99},
                                {99,99,99,99,99,99,99,99},
                                {99,99,99,99,99,99,99,99},
                                {99,99,99,99,99,99,99,99},
                                {99,99,99,99,99,99,99,99},
                                };

    public static int[,] Quantisize(Matrix<float> channel, QTType type)
    {
      // TODO vektor umwandeln

      int[,] resultMatrix = new int[channel.RowCount, channel.ColumnCount];
      int[,] quantizationTable = type == QTType.LUMINANCE ? luminanceQT : chrominanceQT;

      for (int column = 0; column < channel.ColumnCount; column += N)
      {
        for (int row = 0; row < channel.RowCount; row += N)
        {
          Matrix<float> subMatrix = channel.SubMatrix(row, N, column, N);

          for (int subColumn = 0; subColumn < N; subColumn++)
          {
            for (int subRow = 0; subRow < N; subRow++)
            {
              resultMatrix[row, column] = (int)Math.Round(subMatrix[subRow, subColumn] / quantizationTable[subRow, subColumn]);
            }
          }

        }
      }

      return resultMatrix;
    }

  }

  enum QTType
  {
    LUMINANCE,
    CHROMINANCE
  }
}