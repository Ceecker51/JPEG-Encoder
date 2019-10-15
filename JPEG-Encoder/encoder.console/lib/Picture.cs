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
    public int Width { get; set; }
    public int Height { get; set; }

    public int MaxColorValue { get; }

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

    public void Print()
    {
      Console.WriteLine("Printing PICTURE: ");
      for (int y = 0; y < Height; y++)
      {
        for (int x = 0; x < Width; x++)
        {
          Console.Write("({0},{1}) -> ", x, y);
          Console.WriteLine("[{0},{1},{2}]", GetPixel(x, y).Channel1, GetPixel(x, y).Channel2, GetPixel(x, y).Channel3);
        }
        Console.WriteLine("---");
      }

    }
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

  }
}
