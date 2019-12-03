using MathNet.Numerics.LinearAlgebra;
using System;
using System.Threading;

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
      for (int column = 0; column < input.ColumnCount; column += N)
      {
        for (int row = 0; row < input.RowCount; row += N)
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
      for (int column = 0; column < input.ColumnCount; column += N)
      {
        for (int row = 0; row < input.RowCount; row += N)
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

      for (int column = 0; column < columnCount; column += N)
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

      for (int row = 0; row < rowCount; row += N)
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

    public static Matrix<double> TransformAraiThreaded(Matrix<double> input)
    {
      int rowCount = input.RowCount;
      int columnCount = input.ColumnCount;

      int amountBlocks = columnCount / 4;
      int amountOfBlocksPerCore = (int) Math.Floor(amountBlocks / 8.0) * 8;

      AraiState state1 = new AraiState(input.SubMatrix(0, rowCount, 0, amountOfBlocksPerCore));
      AraiState state2 = new AraiState(input.SubMatrix(0, rowCount, amountOfBlocksPerCore, amountOfBlocksPerCore + 8));
      AraiState state3 = new AraiState(input.SubMatrix(0, rowCount, amountOfBlocksPerCore * 2 + 8, amountOfBlocksPerCore));
      AraiState state4 = new AraiState(input.SubMatrix(0, rowCount, amountOfBlocksPerCore * 3 + 8, amountOfBlocksPerCore + 8));

      Thread thread1 = new Thread(new ThreadStart(state1.TransformArai));
      Thread thread2 = new Thread(new ThreadStart(state2.TransformArai));
      Thread thread3 = new Thread(new ThreadStart(state3.TransformArai));
      Thread thread4 = new Thread(new ThreadStart(state4.TransformArai));

      thread1.Start();
      thread2.Start();
      thread3.Start();
      thread4.Start();

      thread1.Join();
      thread2.Join();
      thread3.Join();
      thread4.Join();

      Matrix<double> result = Matrix<double>.Build.Dense(rowCount, columnCount);
      result.SetSubMatrix(0, 0, state1.Result);
      result.SetSubMatrix(0, amountOfBlocksPerCore, state2.Result);
      result.SetSubMatrix(0, amountOfBlocksPerCore * 2 + 8, state3.Result);
      result.SetSubMatrix(0, amountOfBlocksPerCore * 3 + 8, state4.Result);
      return result;
    }

    public static Matrix<double> InverseTransform(Matrix<double> input)
    {
      LogLine(input.ToString());

      var resultMatrix = Matrix<double>.Build.Dense(input.RowCount, input.ColumnCount);

      // iterate over all 8x8 matrices
      for (int column = 0; column < input.ColumnCount; column += N)
      {
        for (int row = 0; row < input.RowCount; row += N)
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
      double v0, v1, v2, v3, v4, v5, v6, v7, v8, v9,
             v10, v11, v12, v13, v14, v15, v16, v17, v18, v19,
             v20, v21, v22, v23, v24, v25, v26, v27, v28;

      // 1. Schritt
      v0 = x0 + x7;
      v1 = x1 + x6;
      v2 = x2 + x5;
      v3 = x3 + x4;
      v4 = x3 - x4;
      v5 = x2 - x5;
      v6 = x1 - x6;
      v7 = x0 - x7;

      // 2. Schritt
      v8 = v0 + v3;
      v9 = v1 + v2;
      v10 = v1 - v2;
      v11 = v0 - v3;
      v12 = -v4 - v5;
      v13 = (v5 + v6) * 0.707106781186548; // a3
      v14 = v6 + v7;

      // 3.Schritt
      v15 = v8 + v9;
      v16 = v8 - v9;
      v17 = (v10 + v11) * 0.707106781186548; // a1
      v18 = (v12 + v14) * 0.38268343236509; // a5

      // 4. Schritt
      v19 = -(v12 * 0.541196100146197) - v18; // a2
      v20 = (v14 * 1.306562964876377) - v18; // a4

      // 5. Schritt
      v21 = v17 + v11;
      v22 = v11 - v17;
      v23 = v13 + v7;
      v24 = v7 - v13;

      // 6. Schritt
      v25 = v19 + v24;
      v26 = v23 + v20;
      v27 = v23 - v20;
      v28 = -v19 + v24;

      // 7. Schritt
      return new[] { v15 * 0.353553390593274,
                     v26 * 0.25489778955208,
                     v21 * 0.270598050073099,
                     v28 * 0.300672443467523,
                     v16 * 0.353553390593274,
                     v25 * 0.449988111568208,
                     v22 * 0.653281482438188,
                     v27 * 1.28145772387075
      };
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

  class AraiState {
    public Matrix<double> Input { get; private set; }
    public Matrix<double> Result { get; private set; }

    public AraiState(Matrix<double> input)
    {
      Input = input;
    }

    public void TransformArai()
    {
      Result = Transformation.TransformArai(Input);
    }
  }
}