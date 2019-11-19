﻿using System;
using System.IO;

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
      //char[] input = { 's', 'a', '#', '#', 's', 'd', 'w', 's' };
      // char[] input2 = "aaaabbbbccccccddddddeeeeeeefffffffff".ToCharArray();
      //char[] input2 = "aaaabbbbccccddef".ToCharArray();
      //char[] input2 = "aabbbcccddddeeeeffffgggghhhhhiiiiijjjjjkkkkklllllmmmmmmnnnnnnoooooopppppppqqqqqqqrrrrrrrssssssssttttttttuuuuuuuuvvvvvvvvwwwwwwwwxxxxxxxxxyyyyyyyyy".ToCharArray();
      //char[] input2 = "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabbbbbbbbbbbbbbbbccccccccddddeefg".ToCharArray();
      // char[] input2 = "xxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxxaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaabbbbbbbbbbbbbbbbccccccccddddeefg".ToCharArray();
      char[] input2 = "eeeeeeeeeeeeeeeeeeeeeeeedddddddddddddddddddddddccccccccccbbbbbbbbbbbaaaaaaaaaaaxxxyyywvsr".ToCharArray();
      Console.WriteLine("Input content:");
      Console.WriteLine(new string(input2));
      Console.WriteLine();

      // Build huffman tree
      HuffmanTree tree = new HuffmanTree();
      tree.Build(input2);
      tree.Print();
      tree.RightBalance();
      tree.Print();
      foreach (var item in tree.frequenciesOfDepths)
      {
        Console.Write(item);
        Console.Write(", ");
      }
      Console.WriteLine();
      foreach (var item in tree.symbolsInTreeOrder)
      {
        Console.Write(item);
        Console.Write(", ");
      }
      Console.WriteLine();

      // // Encode symbols by huffman tree
      BitStream bitStream = tree.Encode(input2);
      bitStream.PrettyPrint();

      Console.WriteLine();

      bitStream.Reset();

      // // Decode symbols by huffman tree
      char[] decodedCode = tree.Decode(bitStream);

      Console.WriteLine("Decoded content:");
      Console.WriteLine(new string(decodedCode));

      WriteJPEGHeader("test.ppm", "out.jpg", tree);
    }

    public static void WriteJPEGHeader(string ppmFileName, string jpegFileName, HuffmanTree tree)
    {
      string inputFilePath = Asserts.GetFilePath(ppmFileName);
      string outputFilePath = Asserts.GetFilePath(jpegFileName);

      Picture rgbPicture = PPMReader.ReadFromPPMFile(inputFilePath, stepX, stepY);
      Picture yCbCrPicture = Picture.toYCbCr(rgbPicture);

      JPEGWriter.WritePictureToJPEG(outputFilePath, yCbCrPicture, tree);
    }

    public static void writeFromBitStreamToFile(string outputFilename)
    {
      string outputFilePath = Asserts.GetFilePath(outputFilename);

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

      string inputFilePath = Asserts.GetFilePath(inputFilename);
      string outputFilePath = Asserts.GetFilePath(outputFilename);

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
  }
}

