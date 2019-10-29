using System.IO;
using encoder.lib;
using System;

namespace encoder.console
{
  class Encoder
  {
    private const int stepX = 8;
    private const int stepY = 8;
    private const bool isWindows = false;

    static void Main(string[] args)
    {
      readFromFileStreamAndWriteToFile("out.txt");
      writeFromBitStreamToFile("out.txt");
      writeJPEGHeader("test_5x5.ppm");
    }

    public static void writeJPEGHeader(string fileName)
    {

      string inputFilePath = isWindows ? @"../../../../assets/" + fileName : @"../assets/" + fileName;
      Picture rgbPicture = PPMReader.ReadFromPPMFile(inputFilePath, stepX, stepY);

      BitStream jpegStream = new BitStream();

      // JPEG init
      UInt16 startOfImage = 0xFFD8;


      // APP0 segment
      UInt16 startMarker = 0xFFE0;
      UInt16 lengthOfSegment = 16;







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

