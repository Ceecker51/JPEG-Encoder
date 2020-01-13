using System;
using System.Collections.Generic;
using System.Linq;

using MathNet.Numerics.LinearAlgebra;

using encoder.utils;

namespace encoder.lib
{
  public class Picture
  {
    const int N = 8;

    public Matrix<float> ChannelY;
    public Matrix<float> ChannelCb;
    public Matrix<float> ChannelCr;

    public int[,] iChannelY; 
    public int[,] iChannelCb;
    public int[,] iChannelCr;

    public List<int[]> zickZackChannelY;
    public List<int[]> zickZackChannelCb;
    public List<int[]> zickZackChannelCr;

    public List<DCEncode> dcValuesY;
    public List<DCEncode> dcValuesCb;
    public List<DCEncode> dcValuesCr;

    public List<List<ACEncode>> acEncodedY;
    public List<List<ACEncode>> acEncodedCb;
    public List<List<ACEncode>> acEncodedCr;

    public HuffmanTree[] huffmanTrees;

    public Picture(int width, int height, int maxValue)
    {
      Width = width;
      Height = height;
      MaxColorValue = maxValue;

      ChannelY = Matrix<float>.Build.Dense(width, height);
      ChannelCb = Matrix<float>.Build.Dense(width, height);
      ChannelCr = Matrix<float>.Build.Dense(width, height);
    }

    // GETTER / SETTER
    public int Width { get; set; }
    public int Height { get; set; }
    public int MaxColorValue { get; }

    public void SetPixel(int x, int y, Color color)
    {
      ChannelY[x, y] = color.Channel1;
      ChannelCb[x, y] = color.Channel2;
      ChannelCr[x, y] = color.Channel3;
    }
    public Color GetPixel(int x, int y)
    {
      return new Color(ChannelY[x, y], ChannelCb[x, y], ChannelCr[x, y]);
    }

    public Vector<float> GetPixelVector(int x, int y)
    {
      float[] channels = { ChannelY[x, y], ChannelCb[x, y], ChannelCr[x, y] };
      return Vector<float>.Build.DenseOfArray(channels);
    }

    public void MakeRandom()
    {
      ChannelY = PictureHelper.GenerateRandomChannel(Width, Height);
      ChannelCb = PictureHelper.GenerateRandomChannel(Width, Height);
      ChannelCr = PictureHelper.GenerateRandomChannel(Width, Height);
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

      ChannelCb = ReduceChannel(ChannelCb, reductionBy);
      ChannelCr = ReduceChannel(ChannelCr, reductionBy);
    }

    public void Transform()
    {
      ChannelY = Transformation.TransformArai(ChannelY);
      ChannelCb = Transformation.TransformArai(ChannelCb);
      ChannelCr = Transformation.TransformArai(ChannelCr);
    }

    public void Quantisize()
    {
      iChannelY = Quantization.Quantisize(ChannelY, QTType.LUMINANCE);
      iChannelCb = Quantization.Quantisize(ChannelCb, QTType.CHROMINANCE);
      iChannelCr = Quantization.Quantisize(ChannelCr, QTType.CHROMINANCE);
    }

    public void ZickZackSort()
    {
      zickZackChannelY = ZickZack.ZickZackSortChannel(iChannelY);
      zickZackChannelCb = ZickZack.ZickZackSortChannel(iChannelCb);
      zickZackChannelCr = ZickZack.ZickZackSortChannel(iChannelCr);
    }

    internal void CalculateCoefficients()
    {
      // DC values
      dcValuesY = Coefficients.EncodeDCValueDifferences(zickZackChannelY);
      dcValuesCb = Coefficients.EncodeDCValueDifferences(zickZackChannelCb);
      dcValuesCr = Coefficients.EncodeDCValueDifferences(zickZackChannelCr);

      // AC values
      acEncodedY = Coefficients.RunLengthEncodeACValues(zickZackChannelY);
      acEncodedCb = Coefficients.RunLengthEncodeACValues(zickZackChannelCb);
      acEncodedCr = Coefficients.RunLengthEncodeACValues(zickZackChannelCr);
    }

    internal void GenerateHuffmanTrees()
    {
      HuffmanTree[] trees = new HuffmanTree[4];

      // Y - DC
      trees[0] = GenerateYDCTree(dcValuesY);

      // Y - AC
      trees[1] = GenerateYACTree(acEncodedY);

      // CbCr - DC
      trees[2] = GenerateCbCrDCTree(dcValuesCb, dcValuesCr);

      // CbCr - AC
      trees[3] = GenerateCbCrACTree(acEncodedCb, acEncodedCr);

      huffmanTrees = trees;
    }

    public static HuffmanTree GenerateYDCTree(List<DCEncode> dcValuesChannel1)
    {
      char[] yDCValues = dcValuesChannel1.Select(dcValue => (char)dcValue.Category).ToArray();
      HuffmanTree yDCTree = new HuffmanTree();
      yDCTree.Build(yDCValues);
      yDCTree.RightBalance();
      yDCTree.CreateLookUpDictionary();
      return yDCTree;
    }

    public static HuffmanTree GenerateYACTree(List<List<ACEncode>> acEncodedChannel1)
    {
      char[] yACValues = acEncodedChannel1
          .SelectMany(acEncodedBlock => acEncodedBlock
            .Select(acValue => (char)acValue.Flag)).ToArray();

      HuffmanTree yACTree = new HuffmanTree();
      yACTree.Build(yACValues);
      yACTree.RightBalance();
      yACTree.CreateLookUpDictionary();

      return yACTree;
    }

    public static HuffmanTree GenerateCbCrDCTree(List<DCEncode> dcValuesChannel2, List<DCEncode> dcValuesChannel3)
    {
      char[] cbCrDCValues = dcValuesChannel2
                            .Concat(dcValuesChannel3)
                            .Select(dcValue => (char)dcValue.Category).ToArray();

      HuffmanTree cbCrDCTree = new HuffmanTree();
      cbCrDCTree.Build(cbCrDCValues);
      cbCrDCTree.RightBalance();
      cbCrDCTree.CreateLookUpDictionary();

      return cbCrDCTree;
    }

    public static HuffmanTree GenerateCbCrACTree(List<List<ACEncode>> acEncodedChannel2, List<List<ACEncode>> acEncodedChannel3)
    {
      char[] cbCrACValues = acEncodedChannel2
                            .Concat(acEncodedChannel3)
                            .SelectMany(acEncodedBlock => acEncodedBlock
                              .Select(acValue => (char)acValue.Flag)).ToArray();

      HuffmanTree cbCrACTree = new HuffmanTree();
      cbCrACTree.Build(cbCrACValues);
      cbCrACTree.RightBalance();
      cbCrACTree.CreateLookUpDictionary();

      return cbCrACTree;
    }

    // PRINT FUNCTIONS //
    public void Print()
    {
      Console.WriteLine("Printing PICTURE: ");
      Console.WriteLine("Channel 1");
      Console.WriteLine(ChannelY.ToString());

      Console.WriteLine("Channel 2");
      Console.WriteLine(ChannelCb.ToString());

      Console.WriteLine("Channel 3");
      Console.WriteLine(ChannelCr.ToString());
    }
  }
}
