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
            char[] input = { 's', 'a', '#', '#', 's', 'd', 'w' ,'s'};
            BitStream stream = Huffman.encoding(input);
            stream.prettyPrint();
            stream.reset();
            char[] decodedCode = Huffman.decoding(stream);
            foreach (char item in decodedCode)
            {
                Console.Write(item);
            }
    }

    public static void writeJPEGHeader(string ppmFileName, string jpegFileName)
    {
      string inputFilePath = Asserts.GetFilePath(ppmFileName);
      string outputFilePath = Asserts.GetFilePath(jpegFileName);

      Picture rgbPicture = PPMReader.ReadFromPPMFile(inputFilePath, stepX, stepY);
      Picture yCbCrPicture = Picture.toYCbCr(rgbPicture);

      JPEGWriter.WritePictureToJPEG(outputFilePath, yCbCrPicture);
    }

    public static void writeFromBitStreamToFile(string outputFilename)
    {
      string outputFilePath = Asserts.GetFilePath(outputFilename);

      BitStream bitStream = new BitStream();

      // 'A' or 65
      bitStream.writeBits('A', 8);
      bitStream.prettyPrint();

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
      bitStream.prettyPrint();

      using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create))
      {
        bitStream.writeToStream(outputFileStream);
      }
    }
  }
}

