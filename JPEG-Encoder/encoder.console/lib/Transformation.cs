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

    public static Matrix<double> TransformArai(Matrix<double> input)
    {
      Console.WriteLine(input.ToString());

      var resultMatrix = Matrix<double>.Build.Dense(input.RowCount, input.ColumnCount);

      // iterate over all 8x8 matrices
      for (int column = 0; column < input.ColumnCount; column += 8)
      {
        for (int row = 0; row < input.RowCount; row++)
        {
          // 0. Schritt
          double x0 = input[row, column];
          double x1 = input[row, column + 1];
          double x2 = input[row, column + 2];
          double x3 = input[row, column + 3];
          double x4 = input[row, column + 4];
          double x5 = input[row, column + 5];
          double x6 = input[row, column + 6];
          double x7 = input[row, column + 7];

          x0 = 1;
          x1 = 2;
          x2 = 3;
          x3 = 2;
          x4 = 1;
          x5 = 2;
          x6 = 3;
          x7 = 4;

          // 1. Schritt
          double t0 = x0 + x7;
          double t1 = x1 + x6;
          double t2 = x2 + x5;
          double t3 = x3 + x4;
          double t4 = x3 - x4;
          double t5 = x2 - x5;
          double t6 = x1 - x6;
          double t7 = x0 - x7;

          // 2. Schritt
          double tt0 = t0 + t3;
          double tt1 = t1 + t2;
          double tt2 = t1 - t2;
          double tt3 = t0 - t3;
          double tt4 = -t4 - t5;
          double tt5 = t5 + t6;
          double tt6 = t6 + t7;

          // 3.Schritt
          double ttt0 = tt0 + tt1;
          double ttt1 = tt0 - tt1;
          double ttt2 = tt2 + tt3;

          // 4. Schritt
          double a1 = ConstantK(4);
          double a2 = ConstantK(2) - ConstantK(6);
          double a3 = ConstantK(4);
          double a4 = ConstantK(6) + ConstantK(2);
          double a5 = ConstantK(6);

          double tttt2 = ttt2 * a1;
          double tttt4 = -(tt4 * a2) - (tt4 + tt6) * a5; // Zwischenspeichern
          double tttt5 = tt5 * a3;
          double tttt6 = (tt6 * a4) - (tt4 + tt6) * a5;

          // 5. Schritt
          double ttttt2 = tttt2 + tt3;
          double ttttt3 = tt3 - tttt2;
          double ttttt5 = tttt5 + t7;
          double ttttt7 = t7 - tttt5;

          // 6. Schritt
          double tttttt4 = tttt4 + ttttt7;
          double tttttt5 = ttttt5 + tttt6;
          double tttttt6 = ttttt5 - tttt6;
          double tttttt7 = -tttt4 + ttttt7;

          // 7. Schritt
          double y0 = ttt0 * ConstantS(0);
          double y4 = ttt1 * ConstantS(4);
          double y2 = ttttt2 * ConstantS(2);
          double y6 = ttttt3 * ConstantS(6);
          double y5 = tttttt4 * ConstantS(5);
          double y1 = tttttt5 * ConstantS(1);
          double y7 = tttttt6 * ConstantS(7);
          double y3 = tttttt7 * ConstantS(3);

          double[,] data = { { y0, y1, y2, y3, y4, y5, y6, y7 } };
          var matrix = Matrix<double>.Build.DenseOfArray(data);

          // put tranformation matrix into result matrix
          resultMatrix.SetSubMatrix(row, column, matrix);
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
          matrixA[k, n] = Constant(k) * Math.Sqrt(2.0 / N) * Math.Cos((2 * n + 1) * ((k * Math.PI) / (2 * N)));
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

    private static double ConstantK(double k)
    {
      return Math.Cos((k * Math.PI) / 16.0);
    }

    private static double ConstantS(double k)
    {
      if (k == 0)
        return 1.0 / (2 * Math.Sqrt(2));

      return 1.0 / (4 * ConstantK(k));
    }
  }
}