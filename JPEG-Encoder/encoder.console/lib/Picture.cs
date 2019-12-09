using MathNet.Numerics.LinearAlgebra;
using System;

namespace encoder.lib
{
  public class Picture
  {
    public Matrix<float> Channel1;
    public Matrix<float> Channel2;
    public Matrix<float> Channel3;

    public int[,] iChannel1;
    public int[,] iChannel2;
    public int[,] iChannel3;

    public Picture(int width, int height, int maxValue)
    {
      Width = width;
      Height = height;
      MaxColorValue = maxValue;

      Channel1 = Matrix<float>.Build.Dense(width, height);
      Channel2 = Matrix<float>.Build.Dense(width, height);
      Channel3 = Matrix<float>.Build.Dense(width, height);
    }


    // GETTER / SETTER
    public int Width { get; set; }
    public int Height { get; set; }
    public int MaxColorValue { get; }

    public void SetPixel(int x, int y, Color color)
    {
      Channel1[x, y] = color.Channel1;
      Channel2[x, y] = color.Channel2;
      Channel3[x, y] = color.Channel3;
    }
    public Color GetPixel(int x, int y)
    {
      return new Color(Channel1[x, y], Channel2[x, y], Channel3[x, y]);
    }

    public Vector<float> GetPixelVector(int x, int y)
    {
      float[] channels = { Channel1[x, y], Channel2[x, y], Channel3[x, y] };
      return Vector<float>.Build.DenseOfArray(channels);
    }

    // TRANSFORMATION FUNCTIONS
    public static Picture toYCbCr(Picture picture)
    {
      float[,] transformationConstants = {{0.299f, 0.587f, 0.144f},
                                        {-0.1687f, -0.3312f, 0.5f},
                                        {0.5f,-0.4186f, 0.0813f}};

      float[] normalisationConstants = { 0f,
                                       (float)Math.Round(0.5f * picture.MaxColorValue),
                                       (float)Math.Round(0.5f * picture.MaxColorValue) };

      float[] colorTransformationOffset = { 128f, 128f, 128f };

      var transMatrix = Matrix<float>.Build.DenseOfArray(transformationConstants);
      var normVector = Vector<float>.Build.DenseOfArray(normalisationConstants);
      var offsetVector = Vector<float>.Build.DenseOfArray(colorTransformationOffset);

      Picture yCbCrPicture = new Picture(picture.Width, picture.Height, picture.MaxColorValue);

      for (int y = 0; y < yCbCrPicture.Height; y++)
      {
        for (int x = 0; x < yCbCrPicture.Width; x++)
        {
          Vector<float> rgbValues = picture.GetPixelVector(x, y);
          Vector<float> yCbCrValues = (transMatrix * rgbValues + normVector) - offsetVector;

          Color yCbCrColor = new Color(yCbCrValues[0], yCbCrValues[1], yCbCrValues[2]);
          yCbCrPicture.SetPixel(x, y, yCbCrColor);
        }
      }

      return yCbCrPicture;
    }

    // REDUCE FUNCTIONS //
    private Matrix<float> ReduceChannel(Matrix<float> channel, int reductionBy = 2)
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
      // else it's a rectangle
      else
      {
        stepHeight = (int)Math.Sqrt(reductionBy / 2);
        stepWidth = 2 * stepHeight;
      }

      int widthOfReductionMatrix = channel.ColumnCount / stepWidth;
      int heightOfReductionMatrix = channel.RowCount / stepHeight;

      return Matrix<float>.Build.Dense(widthOfReductionMatrix, heightOfReductionMatrix);
    }

    public void Reduce()
    {
      int reductionBy = 4;

      Channel2 = ReduceChannel(Channel2, reductionBy);
      Channel3 = ReduceChannel(Channel3, reductionBy);
    }

    public void Transform()
    {
      Channel1 = Transformation.TransformAraiThreaded(Channel1);
      Channel2 = Transformation.TransformAraiThreaded(Channel2);
      Channel3 = Transformation.TransformAraiThreaded(Channel3);
    }

    public void Quantisize()
    {
      iChannel1 = Quantization.Quantisize(Channel1, QTType.LUMINANCE);
      iChannel2 = Quantization.Quantisize(Channel2, QTType.CHROMINANCE);
      iChannel3 = Quantization.Quantisize(Channel3, QTType.CHROMINANCE);
    }

    // PRINT FUNCTIONS //
    public void Print()
    {
      Console.WriteLine("Printing PICTURE: ");
      Console.WriteLine("Channel 1");
      Console.WriteLine(Channel1.ToString());

      Console.WriteLine("Channel 2");
      Console.WriteLine(Channel2.ToString());

      Console.WriteLine("Channel 3");
      Console.WriteLine(Channel3.ToString());
    }
  }
}
