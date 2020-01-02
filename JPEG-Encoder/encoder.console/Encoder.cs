﻿using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;
using System.Collections.Generic;

using encoder.lib;
using encoder.utils;
using System.Text;

namespace encoder.console
{
  class Encoder
  {
    private const int stepX = 8;
    private const int stepY = 8;

    static void Main(string[] args)
    {
      // TestHuffman();
      // TestTransformations();
      // TestQuantization();

      //FlowTest();
      ZickZackTest();
      // CoefficientEncoding();
      // HuffmanTreeACDC();

      Console.WriteLine("Please press any key to continue ...");
      Console.ReadKey();
    }

    public static void HuffmanTreeACDC()
    {
      // sonstiges Zeug
      ACEncode encode = new ACEncode(4, 5, 45);
      Console.WriteLine(encode.Print());

      byte[] numbers = { 0x06, 0x45, 0x15, 0x04, 0x21, 0x00 };
      char[] input = Encoding.UTF8.GetChars(numbers);

      // Build HuffmanTree
      //char[] input = "eeeeeeeeeeeeeeeeeeeeeeeedddddddddddddddddddddddccccccccccbbbbbbbbbbbaaaaaaaaaaaxxxyyywvsr".ToCharArray();
      HuffmanTree tree = new HuffmanTree();
      tree.Build(input);
      tree.Print();

      BitStream bitStream = tree.Encode(input);

      // Write into file
      string outputFilePath = Assets.GetFilePath("test.txt");
      using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create))
      {
        bitStream.writeToStream(outputFileStream);
      }
    }

    public static void CoefficientEncoding()
    {
      // var arr = new int[] { 57, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 3, 0, 0, 0, 0, 2, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 895, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
      var arr = new int[] { 57, 45, 0, 0, 0, 0, 23, 0, -30, -8, 0, 0, 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };
      var result1 = Coefficients.RunLengthHelper(arr);
      result1.ForEach(t => Console.WriteLine(t.ToString()));
      Coefficients.CategoryEncoding(result1);
    }

    public static void ZickZackTest()
    {
      // generate random array
      int[,] input = ArrayHelper.GetTwoDimensionalArrayOfLength(64);
      ArrayHelper.PrintArray(input);
      Console.WriteLine();

      // do the zick zack
      List<int[]> output = ZickZack.ZickZackSortChannel(input);
   
      ArrayHelper.PrintArray(output[0]);
      ArrayHelper.PrintArray(output[1]);
      ArrayHelper.PrintArray(output[2]);

      // calculate DC values
      int[] dcValues = Coefficients.CalculateDCDifferences(output);

      ArrayHelper.PrintArray(dcValues);
    }

    public static void FlowTest()
    {
      // load picture and convert to YCbCr
      //var picture = PPMReader.ReadFromPPMFile("test_5x5.ppm", stepX, stepY);
      //var yCbCrPicture = Picture.toYCbCr(picture);
      var yCbCrPicture = new Picture(8, 8, 255);
      yCbCrPicture.MakeRandom();
      
      // subsampling
      yCbCrPicture.Reduce();

      // transform channels
      yCbCrPicture.Transform();

      // Quantisize channels
      yCbCrPicture.Quantisize();

      // Execute ZickZack
      yCbCrPicture.ZickZackSort();

      // Calculate DC/AC coefficients
      yCbCrPicture.CalculateCoefficients();

      // sonstiges Zeug
      byte[] numbers = { 0x06, 0x45, 0x15, 0x04, 0x21, 0x0 };
      char[] input = Encoding.Unicode.GetChars(numbers);

      // Build HuffmanTree
      HuffmanTree tree = new HuffmanTree();
      tree.Build(input);
      tree.RightBalance();
      HuffmanTree[] trees = { tree };

      // write JPEG
      WriteJPEGHeader("test.ppm", "out.jpg", null, trees);
    }

    public static void TestQuantization()
    {
      var input = PictureHelper.GenerateRandomChannel(32, 32);
      var output = Transformation.TransformArai(input);
      Console.WriteLine(output.ToString(32, 32));
      var result = Quantization.Quantisize(output, QTType.CHROMINANCE);

      var rowLength = result.GetLength(0);
      var columnLength = result.GetLength(1);

      for (int i = 0; i < rowLength; i++)
      {
        for (int j = 0; j < columnLength; j++)
        {
          Console.Write(result[i, j] + ", ");
        }
        Console.WriteLine();
      }
    }

    public static void TestTransformations()
    {
      // var picture = PPMReader.ReadFromPPMFile("mountain.ppm", stepX, stepY);
      // var yCbCrPicture = Picture.toYCbCr(picture);

      // load random image
      int width = 3840;
      int height = 2160;
      var input = Matrix<float>.Build.Dense(width, height);
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          input[x, y] = (x + y * 8) % 256;
        }
      }
      //Console.WriteLine(input);

      long[] times;

      Console.WriteLine("8 Threads");

      Console.WriteLine("Direct (" + 2 + " times)");
      times = measureTime(input, Transformation.TransformDirectly, 2);
      Console.WriteLine("Mean: " + calculateMean(times) + " ms");
      Console.WriteLine();

      Console.WriteLine("Separate (" + 3 + " times)");
      times = measureTime(input, Transformation.TransformSeparately, 3);
      Console.WriteLine("Mean: " + calculateMean(times) + " ms");
      Console.WriteLine();

      Console.WriteLine("Arai (" + 50 + " times)");
      times = measureTime(input, Transformation.TransformArai, 50);
      Console.WriteLine("Mean: " + calculateMean(times) + " ms");
      Console.WriteLine();

      Console.WriteLine("Arai Threaded (" + 50 + " times)");
      times = measureTime(input, Transformation.TransformAraiThreaded, 50);
      Console.WriteLine("Mean: " + calculateMean(times) + " ms");
      Console.WriteLine();

      // var transform = Transformation.TransformAraiThreaded(input);
      // Console.WriteLine(Transformation.InverseTransform(transform).ToString());
    }

    public static double calculateMean(long[] times)
    {
      long sum = 0;
      for (int i = 0; i < times.Length; i++)
      {
        sum += times[i];
      }

      return sum / times.Length;
    }

    public static long[] measureTime(Matrix<float> channel, Func<Matrix<float>, Matrix<float>> f, int count)
    {
      long[] times = new long[count];

      for (int i = 0; i < count; i++)
      {
        var watch = new Stopwatch();
        watch.Start();
        var channel2Trans = f(channel);
        watch.Stop();

        times[i] = watch.ElapsedMilliseconds;
      }

      return times;
    }

    public static void TestHuffman()
    {
      //char[] input = { 's', 'a', '#', '#', 's', 'd', 'w', 's' };
      // char[] input2 = "aaaabbbbccccccddddddeeeeeeefffffffff".ToCharArray();
      //char[] input2 = "aaaabbbbccccddef".ToCharArray();
      //char[] input2 = "aabbbcccddddeeeeffffgggghhhhhiiiiijjjjjkkkkklllllmmmmmmnnnnnnoooooopppppppqqqqqqqrrrrrrrssssssssttttttttuuuuuuuuvvvvvvvvwwwwwwwwxxxxxxxxxyyyyyyyyy".ToCharArray();
      //char[] input2 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabbbbbbbbbbbbbbbbccccccccddddeefg".ToCharArray();
      // char[] input2 = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabbbbbbbbbbbbbbbbccccccccddddeefg".ToCharArray();
      char[] input2 = "eeeeeeeeeeeeeeeeeeeeeeeedddddddddddddddddddddddccccccccccbbbbbbbbbbbaaaaaaaaaaaxxxyyywvsr".ToCharArray();
      LogLine("Input content:");
      LogLine(new string(input2));
      LogLine();

      // // Build huffman tree
      HuffmanTree tree = new HuffmanTree();
      tree.Build(input2);
      tree.Print();
      tree.RightBalance();
      tree.Print();

      // Encode symbols by huffman tree
      BitStream bitStream = tree.Encode(input2);
#if DEBUG
      bitStream.PrettyPrint();
      LogLine();
#endif

      bitStream.Reset();

      // Decode symbols by huffman tree
      char[] decodedCode = tree.Decode(bitStream);

      LogLine("Decoded content:");
      LogLine(new string(decodedCode));
      HuffmanTree[] trees = { tree };
      WriteJPEGHeader("test.ppm", "out.jpg", null, trees);
    }

    public static void WriteJPEGHeader(string ppmFileName, string jpegFileName, int[,] qtTables, HuffmanTree[] trees)
    {
      Picture rgbPicture = PPMReader.ReadFromPPMFile(ppmFileName, stepX, stepY);
      Picture yCbCrPicture = Picture.toYCbCr(rgbPicture);

      JPEGWriter.WritePictureToJPEG(jpegFileName, yCbCrPicture, qtTables, trees);
    }

    public static void writeFromBitStreamToFile(string outputFilename)
    {
      string outputFilePath = Assets.GetFilePath(outputFilename);

      BitStream bitStream = new BitStream();

      // 'A' or 65
      bitStream.writeBits('A', 8);
      bitStream.PrettyPrint();

      using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create))
      {
        bitStream.writeToStream(outputFileStream);
      }
    }

    public static void readFromFileStreamAndWriteToFile(string outputFilename)
    {
      string inputFilename = "test.txt";

      string inputFilePath = Assets.GetFilePath(inputFilename);
      string outputFilePath = Assets.GetFilePath(outputFilename);

      BitStream bitStream = new BitStream();

      using (FileStream fileStream = new FileStream(inputFilePath, FileMode.Open))
      {
        bitStream.readFromStream(fileStream);
      }
      bitStream.PrettyPrint();

      using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create))
      {
        bitStream.writeToStream(outputFileStream);
      }
    }

    private static void LogLine(string message = null)
    {
#if DEBUG
      Console.WriteLine(message);
#endif
    }

    private static void Log(string message = null)
    {
#if DEBUG
      Console.Write(message);
#endif
    }

  }

}

