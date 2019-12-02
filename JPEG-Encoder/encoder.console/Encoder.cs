using System;
using System.IO;
using MathNet.Numerics.LinearAlgebra;
using System.Diagnostics;

using encoder.lib;
using encoder.utils;

namespace encoder.console
{
  class Encoder
  {
    private const int stepX = 8;
    private const int stepY = 8;

    static void Main(string[] args)
    {
      // TestHuffman();
      TestTransformations();
    }

    public static void TestTransformations()
    {
      var watch = new Stopwatch();
      watch.Start();

      // var picture = PPMReader.ReadFromPPMFile("mountain.ppm", stepX, stepY);
      // var yCbCrPicture = Picture.toYCbCr(picture);

      // load random image
      int width = 3840;
      int height = 2160;
      var input = Matrix<double>.Build.Dense(width, height);
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          input[x, y] = (x + y * 8) % 256;
        }
      }
      // Console.WriteLine(input);

      watch.Stop();
      Console.WriteLine("It took {0} ms to load the picture.\n", watch.ElapsedMilliseconds);

      for (int i = 0; i < 10; i++)
      {
        // Console.WriteLine("Direct");
        // measureTime(input, Transformation.TransformDirectly);

        // Console.WriteLine("Separate");
        // measureTime(input, Transformation.TransformSeparately);

        //Console.WriteLine("Arai");
        measureTime(input, Transformation.TransformArai);
      }

      // var transform = Transformation.TransformArai(input);
      // Console.WriteLine(Transformation.InverseTransform(transform).ToString());
    }

    public static void measureTime(Matrix<double> channel, Func<Matrix<double>, Matrix<double>> f)
    {
      var watch = new Stopwatch();
      watch.Start();
      var channel2Trans = f(channel);
      watch.Stop();
      Console.WriteLine("└─ {0} ms\n", watch.ElapsedMilliseconds);
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
      WriteJPEGHeader("test.ppm", "out.jpg", trees);
    }

    public static void WriteJPEGHeader(string ppmFileName, string jpegFileName, HuffmanTree[] trees)
    {
      string inputFilePath = Assets.GetFilePath(ppmFileName);
      string outputFilePath = Assets.GetFilePath(jpegFileName);

      Picture rgbPicture = PPMReader.ReadFromPPMFile(inputFilePath, stepX, stepY);
      Picture yCbCrPicture = Picture.toYCbCr(rgbPicture);

      JPEGWriter.WritePictureToJPEG(outputFilePath, yCbCrPicture, trees);
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

