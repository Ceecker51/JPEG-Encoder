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
      writeJPEGHeader("test_5x5.ppm", "output_5x5.jpg");
    }

    public static void writeJPEGHeader(string inputFileName, string outputFileName)
    {

      string inputFilePath = isWindows ? @"../../../../assets/" + inputFileName : @"../assets/" + inputFileName;

      Picture rgbPicture = PPMReader.ReadFromPPMFile(inputFilePath, stepX, stepY);
      Picture yCbCrPicture = Picture.toYCbCr(rgbPicture);

      string outputFilePath = isWindows ? @"../../../../assets/out_" + outputFileName : @"../assets/out_" + outputFileName;
      BitStream jpegStream = new BitStream();

      // JPEG init
      UInt16 startOfImage = 0xFFD8;
      writeHex(jpegStream, startOfImage);
      // Write Segments
      writeAPP0Segment(jpegStream);
      writeSOF0Segment(jpegStream, Convert.ToUInt16(yCbCrPicture.Height), Convert.ToUInt16(yCbCrPicture.Width));

      // Write to file
      writeToFile(jpegStream, outputFilePath);
    }

    public static void writeAPP0Segment(BitStream bitStream)
        {
            // APP0 segment
            UInt16 startMarker = 0xFFE0;
            UInt16 lengthOfSegment = 16; //length = 16
            byte versionhi = 1;
            byte versionlo = 1;
            byte xyunits = 0;   // 0 = no units, normal density
            UInt16 xdensity = 1;  // 1
            UInt16 ydensity = 1;  // 1
            byte thumbnwidth = 0; // 0
            byte thumbnheight = 0; // 0

            //Write APP0 section
            writeHex(bitStream, startMarker);
            writeHex(bitStream, lengthOfSegment);
            bitStream.writeByte((byte)'J');
            bitStream.writeByte((byte)'F');
            bitStream.writeByte((byte)'I');
            bitStream.writeByte((byte)'F');
            bitStream.writeByte(0x0);
            bitStream.writeByte(versionhi);
            bitStream.writeByte(versionlo);
            bitStream.writeByte(xyunits);
            writeHex(bitStream, xdensity);
            writeHex(bitStream, ydensity);
            bitStream.writeByte(thumbnheight);
            bitStream.writeByte(thumbnwidth);
        }

        public static void writeSOF0Segment(BitStream bitStream, UInt16 ht, UInt16 wid)
        {
            // SOF0 Segment
            UInt16 marker = 0xFFC0;
            UInt16 length = 17;
            byte precision = 8;
            byte nrofcomponents = 3;

            byte IdY = 1;
            byte QTY = 0;
            byte HVY = 0x11;

            byte IdCb = 2; // = 2
            byte HVCb = 0x11;
            byte QTCb = 1; // 1

            byte IdCr = 3; // = 3
            byte HVCr = 0x11;
            byte QTCr = 1;

            // Write SOF0 segment
            writeHex(bitStream, marker);
            writeHex(bitStream, length);
            bitStream.writeByte(precision);
            writeHex(bitStream, ht);
            writeHex(bitStream, wid);
            bitStream.writeByte(nrofcomponents);
            bitStream.writeByte(IdY);
            bitStream.writeByte(HVY);
            bitStream.writeByte(QTY);
            bitStream.writeByte(IdCb);
            bitStream.writeByte(HVCb);
            bitStream.writeByte(QTCb);
            bitStream.writeByte(IdCr);
            bitStream.writeByte(HVCr);
            bitStream.writeByte(QTCr);
        }

    public static void writeHex(BitStream bitStream ,UInt16 hexValue)
    {
      byte[] byteArray = BitConverter.GetBytes(hexValue);
      Array.Reverse(byteArray);
      bitStream.writeBytes(byteArray);
    }

    public static void writeToFile(BitStream bitStream, string outputFilePath)
    {

      using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create))
      {
        bitStream.writeToStream(outputFileStream);
      }
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

