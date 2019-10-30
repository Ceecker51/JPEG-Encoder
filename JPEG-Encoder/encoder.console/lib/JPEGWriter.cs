using System;
using System.IO;

namespace encoder.lib
{
  public class JPEGWriter
  {
    public static void WritePictureToJPEG(string file, Picture picture)
    {
      BitStream jpegStream = new BitStream();

      // Write header segements
      writeSOISegment(jpegStream);
      writeAPP0Segment(jpegStream);
      writeSOF0Segment(jpegStream, Convert.ToUInt16(picture.Height), Convert.ToUInt16(picture.Width));
      writeEOISegment(jpegStream);

      // Write to file
      writeToFile(jpegStream, file);
    }

    private static void writeSOISegment(BitStream jpegStream)
    {
      UInt16 startOfImage = 0xFFD8;
      jpegStream.writeHex(startOfImage);
    }

    private static void writeEOISegment(BitStream jpegStream)
    {
      UInt16 endOfImage = 0xFFD9;
      jpegStream.writeHex(endOfImage);
    }

    private static void writeAPP0Segment(BitStream bitStream)
    {
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
      bitStream.writeHex(startMarker);
      bitStream.writeHex(lengthOfSegment);
      bitStream.writeByte((byte)'J');
      bitStream.writeByte((byte)'F');
      bitStream.writeByte((byte)'I');
      bitStream.writeByte((byte)'F');
      bitStream.writeByte(0x0);
      bitStream.writeByte(versionhi);
      bitStream.writeByte(versionlo);
      bitStream.writeByte(xyunits);
      bitStream.writeHex(xdensity);
      bitStream.writeHex(ydensity);
      bitStream.writeByte(thumbnheight);
      bitStream.writeByte(thumbnwidth);
    }

    private static void writeSOF0Segment(BitStream bitStream, UInt16 ht, UInt16 wid)
    {
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
      bitStream.writeHex(marker);
      bitStream.writeHex(length);
      bitStream.writeByte(precision);
      bitStream.writeHex(ht);
      bitStream.writeHex(wid);
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

    private static void writeToFile(BitStream bitStream, string outputFilePath)
    {
      using (FileStream outputFileStream = new FileStream(outputFilePath, FileMode.Create))
      {
        bitStream.writeToStream(outputFileStream);
      }
    }
  }
}