using System;
using System.IO;

using encoder.lib;

namespace encoder.console
{
  class Encoder
  {
    private const int stepX = 8;
    private const int stepY = 8;
    private const bool isWindows = true;

    static void Main(string[] args)
    {
      readFromFileStreamAndWriteToFile("out.txt");
      writeFromBitStreamToFile("out.txt");
      writeJPEGHeader("test_5x5.ppm", "output_5x5.jpg");
    }

    public static void writeJPEGHeader(string ppmFileName, string jpegFileName)
    {
      string inputFilePath = isWindows ? @"../../../../assets/" + ppmFileName : @"../assets/" + ppmFileName;
      string outputFilePath = isWindows ? @"../../../../assets/out_" + jpegFileName : @"../assets/out_" + jpegFileName;

      Picture rgbPicture = PPMReader.ReadFromPPMFile(inputFilePath, stepX, stepY);
      Picture yCbCrPicture = Picture.toYCbCr(rgbPicture);

      JPEGWriter.WritePictureToJPEG(outputFilePath, yCbCrPicture);
    }

    public static void writeFromBitStreamToFile(string outputFilename)
    {
      string outputFilePath = isWindows ? @"../../../../assets/" + outputFilename : @"../assets/" + outputFilename;

      BitStream bitStream = new BitStream();

      // 'A' or 65
      bitStream.writeBit(0);
      bitStream.writeBit(1);
      bitStream.writeBit(0);
      bitStream.writeBit(0);
      bitStream.writeBit(0);
      bitStream.writeBit(0);
      bitStream.writeBit(0);
      bitStream.writeBit(1);
      bitStream.prettyPrint();

      using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create))
      {
        bitStream.writeToStream(outputFileStream);
      }
    }

    public static void readFromFileStreamAndWriteToFile(string outputFilename)
    {
      string inputFilename = "test.txt";

      string inputFilePath = isWindows ? @"../../../../assets/" + inputFilename : @"../assets/" + inputFilename;
      string outputFilePath = isWindows ? @"../../../../assets/" + outputFilename : @"../assets/" + outputFilename;

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

