using MathNet.Numerics.LinearAlgebra;
using System;

namespace encoder.lib
{
  class Transformation
  {
    const int N = 8;

    public static Matrix<double> TransformDirectly(Matrix<double> input)
    {
      Console.WriteLine(input.ToString());

      var resultMatrix = Matrix<double>.Build.Dense(input.RowCount, input.ColumnCount);

      // iterate over all 8x8 matrices
      for (int column = 0; column < input.ColumnCount; column += 8)
      {
        for (int row = 0; row < input.RowCount; row += 8)
        {
          var matrix = input.SubMatrix(row, N, column, N);
          var transformationMatrix = CalculateValuesDirectly(matrix);

          // put tranformation matrix into result matrix
          resultMatrix.SetSubMatrix(row, column, transformationMatrix);
        }
      }
      Console.WriteLine(resultMatrix.ToString());
      return resultMatrix;
    }

    public static Matrix<double> TransformSeparately(Matrix<double> input)
    {
      Console.WriteLine(input.ToString());

      var resultMatrix = Matrix<double>.Build.Dense(input.RowCount, input.ColumnCount);

      // iterate over all 8x8 matrices
      for (int column = 0; column < input.ColumnCount; column += 8)
      {
        for (int row = 0; row < input.RowCount; row += 8)
        {
          var matrix = input.SubMatrix(row, N, column, N);
          var transformationMatrix = CalculateValuesSeparately(matrix);

          // put tranformation matrix into result matrix
          resultMatrix.SetSubMatrix(row, column, transformationMatrix);
        }
      }
      Console.WriteLine(resultMatrix.ToString());
      return resultMatrix;
    }

    private static Matrix<double> CalculateValuesSeparately(Matrix<double> matrix)
    {
      var matrixA = Matrix<double>.Build.Dense(N, N);

      for (int n = 0; n < N; n++)
      {
        for (int k = 0; k < N; k++)
        {
          matrixA[k, n] = Constant(k) * Math.Sqrt(2 / N) * Math.Cos((2 * n + 1) * ((k * Math.PI) / (2 * N)));
        }
      }

      return matrixA * matrix * matrixA.Transpose();

    }

    private static Matrix<double> CalculateValuesDirectly(Matrix<double> matrix)
    {
      var result = Matrix<double>.Build.Dense(N, N);
      for (int j = 0; j < N; j++)
      {
        for (int i = 0; i < N; i++)
        {
          double currentResult = (2.0 / N) * Constant(i) * Constant(j);
          double sum = 0;
          for (int y = 0; y < N; y++)
          {
            for (int x = 0; x < N; x++)
            {
              sum += matrix[y, x] * Math.Cos(((2 * x + 1) * i * Math.PI) / (2 * N))
                                * Math.Cos(((2 * y + 1) * j * Math.PI) / (2 * N));
            }
          }
          result[j, i] = currentResult * sum;
        }
      }

      return result;
    }

    private static double Constant(double n)
    {
      if (n == 0)
      {
        return (1.0 / Math.Sqrt(2));
      }
      return 1;
    }
  }
}