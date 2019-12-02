using MathNet.Numerics.LinearAlgebra;
using System;

namespace encoder.lib
{
  class Transformation
  {
    const int N = 8;
    private static Matrix<double> CalculationMatrix = Matrix<double>.Build.Dense(N, N);
    private const double OneDivSqrt2 = 0.707106781186547;

    // values for the following function, while N = 8 and n / k = 0..7
    // Math.Sqrt(2.0 / N) * Math.Cos((2 * n + 1) * ((k * Math.PI) / (2 * N)))
    private static double[,] Cosinus = {
      { 0.5, 0.490392640201615,0.461939766255643,0.415734806151273,0.353553390593274,0.277785116509801,0.191341716182545,0.0975451610080642},
      { 0.5,0.415734806151273,0.191341716182545,-0.0975451610080641,-0.353553390593274,-0.490392640201615,-0.461939766255643,-0.277785116509801},
      { 0.5,0.277785116509801,-0.191341716182545,-0.490392640201615,-0.353553390593274,0.0975451610080641,0.461939766255643,0.415734806151273},
      { 0.5,0.0975451610080642,-0.461939766255643,-0.277785116509801,0.353553390593274,0.415734806151273,-0.191341716182545,-0.490392640201615},
      { 0.5,-0.0975451610080641,-0.461939766255643,0.277785116509801,0.353553390593274,-0.415734806151273,-0.191341716182545,0.490392640201615},
      { 0.5,-0.277785116509801,-0.191341716182545,0.490392640201615,-0.353553390593273,-0.097545161008064,0.461939766255643,-0.415734806151273},
      { 0.5,-0.415734806151273,0.191341716182545,0.0975451610080644,-0.353553390593274,0.490392640201615,-0.461939766255643,0.277785116509801},
      { 0.5,-0.490392640201615,0.461939766255643,-0.415734806151273,0.353553390593273,-0.277785116509801,0.191341716182545,-0.0975451610080643},
    };

    // Math.Cos(((2 * x + 1) * i * Math.PI) / (2 * N)
    private static double[,] CosinusDirect = {
      { 1,0.98078528040323,0.923879532511287,0.831469612302545,0.707106781186548,0.555570233019602,0.38268343236509,0.195090322016128},
      { 1,0.831469612302545,0.38268343236509,-0.195090322016128,-0.707106781186547,-0.98078528040323,-0.923879532511287,-0.555570233019602},
      { 1,0.555570233019602,-0.38268343236509,-0.98078528040323,-0.707106781186548,0.195090322016128,0.923879532511287,0.831469612302546},
      { 1,0.195090322016128,-0.923879532511287,-0.555570233019602,0.707106781186547,0.831469612302546,-0.38268343236509,-0.980785280403231},
      { 1,-0.195090322016128,-0.923879532511287,0.555570233019602,0.707106781186548,-0.831469612302545,-0.382683432365091,0.98078528040323},
      { 1,-0.555570233019602,-0.38268343236509,0.98078528040323,-0.707106781186547,-0.195090322016128,0.923879532511287,-0.831469612302545},
      { 1,-0.831469612302545,0.38268343236509,0.195090322016129,-0.707106781186547,0.980785280403231,-0.923879532511286,0.555570233019602},
      { 1,-0.98078528040323,0.923879532511287,-0.831469612302545,0.707106781186547,-0.555570233019602,0.38268343236509,-0.195090322016129}
    };

    // (2.0 / 8) * Constant(i) * Constant(j)
    private static double[,] currentResult = {
      { 0.125, 0.176776695296637, 0.176776695296637, 0.176776695296637, 0.176776695296637, 0.176776695296637, 0.176776695296637, 0.176776695296637, },
      { 0.176776695296637, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, },
      { 0.176776695296637, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, },
      { 0.176776695296637, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, },
      { 0.176776695296637, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, },
      { 0.176776695296637, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, },
      { 0.176776695296637, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, },
      { 0.176776695296637, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, 0.25, }
    };


    public static Matrix<double> TransformDirectly(Matrix<double> input)
    {
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

      LogLine(resultMatrix.ToString());
      return resultMatrix;
    }

    public static Matrix<double> TransformSeparately(Matrix<double> input)
    {

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
      LogLine(resultMatrix.ToString());
      return resultMatrix;
    }

    public static Matrix<double> TransformArai(Matrix<double> input)
    {

      int rowCount = input.RowCount;
      int columnCount = input.ColumnCount;
      double[,] resultMatrix = new double[rowCount, columnCount];

      for (int column = 0; column < columnCount; column += 8)
      {
        for (int row = 0; row < rowCount; row++)
        {

          double[] resultValues = CalculateAraiValues(input[row, column],
            input[row, column + 1],
            input[row, column + 2],
            input[row, column + 3],
            input[row, column + 4],
            input[row, column + 5],
            input[row, column + 6],
            input[row, column + 7]
          );

          resultMatrix[row, column] = resultValues[0];
          resultMatrix[row, column + 1] = resultValues[1];
          resultMatrix[row, column + 2] = resultValues[2];
          resultMatrix[row, column + 3] = resultValues[3];
          resultMatrix[row, column + 4] = resultValues[4];
          resultMatrix[row, column + 5] = resultValues[5];
          resultMatrix[row, column + 6] = resultValues[6];
          resultMatrix[row, column + 7] = resultValues[7];

        }
      }


      for (int row = 0; row < rowCount; row += 8)
      {
        for (int column = 0; column < columnCount; column++)
        {

          double[] resultValues = CalculateAraiValues(resultMatrix[row, column],
              resultMatrix[row + 1, column],
              resultMatrix[row + 2, column],
              resultMatrix[row + 3, column],
              resultMatrix[row + 4, column],
              resultMatrix[row + 5, column],
              resultMatrix[row + 6, column],
              resultMatrix[row + 7, column]
          );

          resultMatrix[row, column] = resultValues[0];
          resultMatrix[row + 1, column] = resultValues[1];
          resultMatrix[row + 2, column] = resultValues[2];
          resultMatrix[row + 3, column] = resultValues[3];
          resultMatrix[row + 4, column] = resultValues[4];
          resultMatrix[row + 5, column] = resultValues[5];
          resultMatrix[row + 6, column] = resultValues[6];
          resultMatrix[row + 7, column] = resultValues[7];
        }
      }

      Matrix<double> result = Matrix<double>.Build.DenseOfArray(resultMatrix);
      LogLine(result.ToString());
      return result;
    }

    public static Matrix<double> InverseTransform(Matrix<double> input)
    {
      LogLine(input.ToString());

      var resultMatrix = Matrix<double>.Build.Dense(input.RowCount, input.ColumnCount);

      // iterate over all 8x8 matrices
      for (int column = 0; column < input.ColumnCount; column += 8)
      {
        for (int row = 0; row < input.RowCount; row += 8)
        {
          var matrix = input.SubMatrix(row, N, column, N);
          var transformationMatrix = CalculateValuesInversely(matrix);

          // put tranformation matrix into result matrix
          resultMatrix.SetSubMatrix(row, column, transformationMatrix);
        }
      }
      LogLine(resultMatrix.ToString());
      return resultMatrix;
    }

    private static double[] CalculateAraiValues(double x0, double x1, double x2, double x3, double x4, double x5, double x6, double x7)
    {
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

      double[] result = { y0, y1, y2, y3, y4, y5, y6, y7 };
      return result;
    }

    private static Matrix<double> CalculateValuesSeparately(Matrix<double> matrix)
    {

      for (int n = 0; n < N; n++)
      {
        for (int k = 0; k < N; k++)
        {
          CalculationMatrix[k, n] = (k == 0 ? OneDivSqrt2 : 1) * Cosinus[n, k];
        }
      }

      return CalculationMatrix * matrix * CalculationMatrix.Transpose();

    }

    private static Matrix<double> CalculateValuesDirectly(Matrix<double> matrix)
    {
      for (int j = 0; j < N; j++)
      {
        for (int i = 0; i < N; i++)
        {
          // double currentResult = (2.0 / N) * Constant(i) * Constant(j);
          double sum = 0;
          for (int y = 0; y < N; y++)
          {
            for (int x = 0; x < N; x++)
            {
              sum += matrix[y, x] * CosinusDirect[x, i] * CosinusDirect[y, j];
            }
          }
          CalculationMatrix[j, i] = currentResult[i, j] * sum;
        }
      }

      return CalculationMatrix;
    }

    private static Matrix<double> CalculateValuesInversely(Matrix<double> matrix)
    {
      var result = Matrix<double>.Build.Dense(N, N);
      for (int y = 0; y < N; y++)
      {
        for (int x = 0; x < N; x++)
        {
          double sum = 0;
          for (int i = 0; i < N; i++)
          {
            for (int j = 0; j < N; j++)
            {
              double constants = (2.0 / N) * Constant(i) * Constant(j);
              sum += constants * matrix[j, i] * Math.Cos(((2 * x + 1) * i * Math.PI) / (2 * N))
                                * Math.Cos(((2 * y + 1) * j * Math.PI) / (2 * N));
            }
          }
          result[y, x] = sum;
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

    private static void LogLine(string message = null)
    {
#if DEBUG
      Console.WriteLine(message);
#endif
    }
  }

}