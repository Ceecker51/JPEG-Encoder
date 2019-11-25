using MathNet.Numerics.LinearAlgebra;
using System;

namespace encoder.lib
{
  class Transformation
  {
    const int N = 8;

    public static Matrix<double> transformDirectly(Matrix<double> input)
    {
      var resultMatrix = Matrix<double>.Build.Dense(input.RowCount, input.ColumnCount);
      Console.WriteLine(input.ToString());

      // iterate over all 8x8 matrices
      for (int column = 0; column < input.ColumnCount; column += 8)
      {
        for (int row = 0; row < input.RowCount; row += 8)
        {
          var matrix = input.SubMatrix(row, N, column, N);
          var transformationMatrix = calculateValuesDirectly(matrix);

          // put tranformation matrix into result matrix
          resultMatrix.SetSubMatrix(row, column, transformationMatrix);
        }
      }
      Console.WriteLine(resultMatrix.ToString());
      return resultMatrix;
    }

    private static Matrix<double> calculateValuesDirectly(Matrix<double> matrix)
    {
      var result = Matrix<double>.Build.Dense(N, N);
      for (int j = 0; j < N; j++)
      {
        for (int i = 0; i < N; i++)
        {
          double currentResult = 2 / N * Constant(i) * Constant(j);
          for (int y = 0; y < N; y++)
          {
            for (int x = 0; x < N; x++)
            {
              currentResult += matrix[y, x] * Math.Cos(((2 * x + 1) * i * Math.PI) / (2 * N))
                                * Math.Cos(((2 * y + 1) * j * Math.PI) / (2 * N));

            }
          }
          result[j, i] = currentResult;

        }
      }

      return result;
    }

    private static double Constant(double n)
    {
      return n != 0
        ? 1
        : (1 / Math.Sqrt(2));
    }
  }
}