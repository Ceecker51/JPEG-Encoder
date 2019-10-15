using MathNet.Numerics.LinearAlgebra;
using System;

namespace encoder.lib
{
  public class Picture
  {
    private Matrix<double> channel1;
    private Matrix<double> channel2;
    private Matrix<double> channel3;

    public Picture(int width, int height)
    {
      Width = width;
      Height = height;

      channel1 = Matrix<double>.Build.Dense(width, height);
      channel2 = Matrix<double>.Build.Dense(width, height);
      channel3 = Matrix<double>.Build.Dense(width, height);

      MaxColorValue = 255;
    }


    // GETTER / SETTER
    public int Width { get; set; }
    public int Height { get; set; }

    public int MaxColorValue { get; }

    public void SetPixel(int x, int y, Color color)
    {
      channel1[x, y] = color.Channel1;
      channel2[x, y] = color.Channel2;
      channel3[x, y] = color.Channel3;
    }
    public Color GetPixel(int x, int y)
    {
      return new Color(channel1[x, y], channel2[x, y], channel3[x, y]);
    }

    public Vector<double> GetPixelVector(int x, int y)
    {
      double[] channels = { channel1[x, y], channel2[x, y], channel3[x, y] };
      return Vector<double>.Build.DenseOfArray(channels);
    }

    // TRANSFORMATION FUNCTIONS
    public static Picture toYCbCr(Picture picture)
    {
      double[,] transformationConstants = {{0.299, 0.587, 0.144},
                                        {-0.1687, -0.3312, 0.5},
                                        {0.5,-0.4186, 0.0813}};

      double[] normalisationConstants = { 0,
                                       Math.Round(0.5 * picture.MaxColorValue),
                                       Math.Round(0.5 * picture.MaxColorValue) };

      var transMatrix = Matrix<double>.Build.DenseOfArray(transformationConstants);
      var normVector = Vector<double>.Build.DenseOfArray(normalisationConstants);

      Picture yCbCrPicture = new Picture(picture.Width, picture.Height);

      for (int y = 0; y < yCbCrPicture.Height; y++)
      {
        for (int x = 0; x < yCbCrPicture.Width; x++)
        {
          Vector<double> rgbValues = picture.GetPixelVector(x, y);
          Vector<double> yCbCrValues = transMatrix * rgbValues + normVector;

          Color yCbCrColor = new Color(yCbCrValues[0], yCbCrValues[1], yCbCrValues[2]);

          yCbCrPicture.SetPixel(x, y, yCbCrColor);
        }
      }

      return yCbCrPicture;
    }

    // REDUCE FUNCTIONS //
    public Matrix<double> ReduceChannel(Matrix<double> channel, int reductionBy = 2)
    {

      if (reductionBy % 2 != 0)
      {
        throw new ArgumentException("Reduction values have to be multiples of two");
      }

      if (reductionBy > channel.ColumnCount || reductionBy > channel.RowCount)
      {
        throw new ArgumentException("Reduction exceeds picture size");
      }


      // calculate reduced width and height
      int stepWidth = 0;
      int stepHeight = 0;

      // if sqrt is even, reduced block will be a square
      if (Math.Sqrt(reductionBy) % 2 == 0)
      {
        stepWidth = (int)Math.Sqrt(reductionBy);
        stepHeight = stepWidth;

      }
      else if (reductionBy == 2)
      {
        stepWidth = 2;
        stepHeight = 1;
      }
      // else it's a rectangle
      else
      {
        stepHeight = (int)Math.Sqrt(reductionBy / 2);
        stepWidth = 2 * stepHeight;
      }

      int widthOfReductionMatrix = channel.ColumnCount / stepWidth;
      int heightOfReductionMatrix = channel.RowCount / stepHeight;

      var reducedChannel = Matrix<double>.Build.Dense(widthOfReductionMatrix, heightOfReductionMatrix);

      // show reduction Values placed in a zero Matrix
      var zeroMatrix = Matrix<double>.Build.Dense(channel.ColumnCount, channel.RowCount);

      for (int i = 0; i < heightOfReductionMatrix; i++)
      {
        for (int j = 0; j < widthOfReductionMatrix; j++)
        {
          // sum all values in a matrix block
          var subMatrix = channel.SubMatrix(i * stepHeight, stepHeight, j * stepWidth, stepWidth);
          var mean = subMatrix.RowSums().Sum() / reductionBy;
          reducedChannel[j, i] = mean;

          // set reduction values in zero matrix
          zeroMatrix[j * stepWidth, i * stepHeight] = mean;
        }
      }

      // print zero matrix
      Console.WriteLine("--------- zero matrix ------------ ");
      Console.WriteLine(zeroMatrix.ToString());
      Console.WriteLine();

      return reducedChannel;

    }

    public void ReduceY(int reductionBy)
    {
      channel1 = ReduceChannel(channel1, reductionBy);
    }

    public void ReduceCb(int reductionBy)
    {
      channel2 = ReduceChannel(channel2, reductionBy);
    }

    public void ReduceCr(int reductionBy)
    {
      channel3 = ReduceChannel(channel3, reductionBy);
    }

    // PRINT FUNCTIONS //
    public void Print()
    {
      Console.WriteLine("Printing PICTURE: ");
      Console.WriteLine("Channel 1");
      Console.WriteLine(channel1.ToString());

      Console.WriteLine("Channel 2");
      Console.WriteLine(channel2.ToString());

      Console.WriteLine("Channel 3");
      Console.WriteLine(channel3.ToString());
    }
  }
}
