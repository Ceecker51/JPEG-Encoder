using MathNet.Numerics.LinearAlgebra;
using System;
using System.Threading;
using System.Diagnostics;

namespace encoder.lib
{
  class Transformation
  {
    const int N = 8;
    private static Matrix<float> CalculationMatrix = Matrix<float>.Build.Dense(N, N);
    private const float OneDivSqrt2 = 0.707106781186547f;

    // values for the following function, while N = 8 and n / k = 0..7
    // Math.Sqrt(2.0 / N) * Math.Cos((2 * n + 1) * ((k * Math.PI) / (2 * N)))
    private static float[,] Cosinus = {
      { 0.5f, 0.490392640201615f,0.461939766255643f,0.415734806151273f,0.353553390593274f,0.277785116509801f,0.191341716182545f,0.0975451610080642f},
      { 0.5f,0.415734806151273f,0.191341716182545f,-0.0975451610080641f,-0.353553390593274f,-0.490392640201615f,-0.461939766255643f,-0.277785116509801f},
      { 0.5f,0.277785116509801f,-0.191341716182545f,-0.490392640201615f,-0.353553390593274f,0.0975451610080641f,0.461939766255643f,0.415734806151273f},
      { 0.5f,0.0975451610080642f,-0.461939766255643f,-0.277785116509801f,0.353553390593274f,0.415734806151273f,-0.191341716182545f,-0.490392640201615f},
      { 0.5f,-0.0975451610080641f,-0.461939766255643f,0.277785116509801f,0.353553390593274f,-0.415734806151273f,-0.191341716182545f,0.490392640201615f},
      { 0.5f,-0.277785116509801f,-0.191341716182545f,0.490392640201615f,-0.353553390593273f,-0.097545161008064f,0.461939766255643f,-0.415734806151273f},
      { 0.5f,-0.415734806151273f,0.191341716182545f,0.0975451610080644f,-0.353553390593274f,0.490392640201615f,-0.461939766255643f,0.277785116509801f},
      { 0.5f,-0.490392640201615f,0.461939766255643f,-0.415734806151273f,0.353553390593273f,-0.277785116509801f,0.191341716182545f,-0.0975451610080643f},
    };

    // Math.Cos(((2 * x + 1) * i * Math.PI) / (2 * N)
    private static float[,] CosinusDirect = {
      { 1f,0.98078528040323f,0.923879532511287f,0.831469612302545f,0.707106781186548f,0.555570233019602f,0.38268343236509f,0.195090322016128f},
      { 1f,0.831469612302545f,0.38268343236509f,-0.195090322016128f,-0.707106781186547f,-0.98078528040323f,-0.923879532511287f,-0.555570233019602f},
      { 1f,0.555570233019602f,-0.38268343236509f,-0.98078528040323f,-0.707106781186548f,0.195090322016128f,0.923879532511287f,0.831469612302546f},
      { 1f,0.195090322016128f,-0.923879532511287f,-0.555570233019602f,0.707106781186547f,0.831469612302546f,-0.38268343236509f,-0.980785280403231f},
      { 1f,-0.195090322016128f,-0.923879532511287f,0.555570233019602f,0.707106781186548f,-0.831469612302545f,-0.382683432365091f,0.98078528040323f},
      { 1f,-0.555570233019602f,-0.38268343236509f,0.98078528040323f,-0.707106781186547f,-0.195090322016128f,0.923879532511287f,-0.831469612302545f},
      { 1f,-0.831469612302545f,0.38268343236509f,0.195090322016129f,-0.707106781186547f,0.980785280403231f,-0.923879532511286f,0.555570233019602f},
      { 1f,-0.98078528040323f,0.923879532511287f,-0.831469612302545f,0.707106781186547f,-0.555570233019602f,0.38268343236509f,-0.195090322016129f}
    };

    // (2.0 / 8) * Constant(i) * Constant(j)
    private static float[,] currentResult = {
      { 0.125f, 0.176776695296637f, 0.176776695296637f, 0.176776695296637f, 0.176776695296637f, 0.176776695296637f, 0.176776695296637f, 0.176776695296637f, },
      { 0.176776695296637f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, },
      { 0.176776695296637f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, },
      { 0.176776695296637f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, },
      { 0.176776695296637f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, },
      { 0.176776695296637f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, },
      { 0.176776695296637f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, },
      { 0.176776695296637f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, }
    };

    public static Matrix<float> TransformDirectly(Matrix<float> input)
    {
      var resultMatrix = Matrix<float>.Build.Dense(input.RowCount, input.ColumnCount);

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

    public static Matrix<float> TransformSeparately(Matrix<float> input)
    {
      var resultMatrix = Matrix<float>.Build.Dense(input.RowCount, input.ColumnCount);

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

    public static Matrix<float> TransformArai(Matrix<float> input)
    {
      int rowCount = input.RowCount;
      int columnCount = input.ColumnCount;
      float[,] resultMatrix = new float[rowCount, columnCount];

      for (int column = 0; column < columnCount; column += N)
      {
        for (int row = 0; row < rowCount; row++)
        {

          float[] resultValues = CalculateAraiValues(input[row, column],
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

          float[] resultValues = CalculateAraiValues(resultMatrix[row, column],
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

      Matrix<float> result = Matrix<float>.Build.DenseOfArray(resultMatrix);
      LogLine(result.ToString());
      return result;
    }

    public static Matrix<float> TransformAraiThreaded(Matrix<float> input)
    {
      int rowCount = input.RowCount;
      int columnCount = input.ColumnCount;

      int threadCount = 4;

      int amountBlocks = columnCount / 8;
      int amountOfBlocksPerCore = amountBlocks / threadCount;
      int restBlocks = amountBlocks % threadCount;

      AraiState[] states = new AraiState[threadCount];
      Thread[] threads = new Thread[threadCount];

      int start = 0, end = 0;
      for (int i = 0; i < threadCount; i++)
      {
        if (restBlocks > 0)
        {
          end = (amountOfBlocksPerCore + 1) * 8;
          restBlocks--;
        }
        else
        {
          end = (amountOfBlocksPerCore) * 8;
        }

        Matrix<float> subMatrix = input.SubMatrix(0, rowCount, start, end);
        AraiState state = new AraiState(start, end, subMatrix);
        Thread thread = new Thread(new ThreadStart(state.TransformArai));

        states[i] = state;
        threads[i] = thread;

        start += end;
      }

      for (int i = 0; i < threadCount; i++)
      {
        threads[i].Start();
      }

      for (int i = 0; i < threadCount; i++)
      {
        threads[i].Join();
      }

      Matrix<float> result = Matrix<float>.Build.Dense(rowCount, columnCount);
      for (int i = 0; i < threadCount; i++)
      {
        result.SetSubMatrix(0, states[i].Start, states[i].Result);
      }

      return result;
    }

    public static Matrix<float> InverseTransform(Matrix<float> input)
    {
      LogLine(input.ToString());

      var resultMatrix = Matrix<float>.Build.Dense(input.RowCount, input.ColumnCount);

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

    private static float[] CalculateAraiValues(float x0, float x1, float x2, float x3, float x4, float x5, float x6, float x7)
    {
      float v0, v1, v2, v3, v4, v5, v6, v7, v8, v9,
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
      v13 = (v5 + v6) * 0.707106781186548f; // a3
      v14 = v6 + v7;

      // 3.Schritt
      v15 = v8 + v9;
      v16 = v8 - v9;
      v17 = (v10 + v11) * 0.707106781186548f; // a1
      v18 = (v12 + v14) * 0.38268343236509f; // a5

      // 4. Schritt
      v19 = -(v12 * 0.541196100146197f) - v18; // a2
      v20 = (v14 * 1.306562964876377f) - v18; // a4

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
      return new[] { v15 * 0.353553390593274f,
                     v26 * 0.25489778955208f,
                     v21 * 0.270598050073099f,
                     v28 * 0.300672443467523f,
                     v16 * 0.353553390593274f,
                     v25 * 0.449988111568208f,
                     v22 * 0.653281482438188f,
                     v27 * 1.28145772387075f
      };
    }

    private static Matrix<float> CalculateValuesSeparately(Matrix<float> matrix)
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

    private static Matrix<float> CalculateValuesDirectly(Matrix<float> matrix)
    {
      for (int j = 0; j < N; j++)
      {
        for (int i = 0; i < N; i++)
        {
          // double currentResult = (2.0 / N) * Constant(i) * Constant(j);
          float sum = 0;
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

    private static Matrix<float> CalculateValuesInversely(Matrix<float> matrix)
    {
      var result = Matrix<float>.Build.Dense(N, N);
      for (int y = 0; y < N; y++)
      {
        for (int x = 0; x < N; x++)
        {
          float sum = 0;
          for (int i = 0; i < N; i++)
          {
            for (int j = 0; j < N; j++)
            {
              float constants = (2.0f / N) * Constant(i) * Constant(j);
              sum += (float)(constants * matrix[j, i] * Math.Cos(((2 * x + 1f) * i * Math.PI) / (2 * N))
                                * Math.Cos(((2 * y + 1) * j * Math.PI) / (2 * N)));
            }
          }
          result[y, x] = sum;
        }
      }

      return result;
    }

    private static float Constant(float n)
    {
      if (n == 0)
      {
        return (float)(1.0 / Math.Sqrt(2));
      }
      return 1;
    }

    private static float ConstantK(float k)
    {
      return (float)Math.Cos((k * Math.PI) / 16.0);
    }

    private static float ConstantS(float k)
    {
      if (k == 0)
        return 1.0f / (2.0f * (float)Math.Sqrt(2));

      return (float)1.0 / (4 * ConstantK(k));
    }

    private static void LogLine(string message = null)
    {
#if DEBUG
      Console.WriteLine(message);
#endif
    }
  }

  class AraiState
  {

    public int Start { get; private set; }
    public int End { get; private set; }

    public Matrix<float> Input { get; private set; }
    public Matrix<float> Result { get; private set; }

    public AraiState(int start, int end, Matrix<float> input)
    {
      Start = start;
      End = end;
      Input = input;
    }

    public void TransformArai()
    {
      Result = Transformation.TransformArai(Input);
    }
  }
}