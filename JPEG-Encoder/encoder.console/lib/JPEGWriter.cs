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
      WriteSOISegment(jpegStream);
      WriteAPP0Segment(jpegStream);
      WriteSOF0Segment(jpegStream, Convert.ToUInt16(picture.Height), Convert.ToUInt16(picture.Width));
      WriteDHTSegment(jpegStream);
      WriteEOISegment(jpegStream);

      // Write to file
      writeToFile(jpegStream, file);
    }

    /*
     * Write "Start of Image"-Segment
     */
    private static void WriteSOISegment(BitStream jpegStream)
    {
      UInt16 startOfImage = 0xFFD8;
      jpegStream.writeWord(startOfImage);
    }

    /*
     * Write "End of Image"-Segment
     */
    private static void WriteEOISegment(BitStream jpegStream)
    {
      UInt16 endOfImage = 0xFFD9;
      jpegStream.writeWord(endOfImage);
    }

    /*
     *  Write "Application"-Segment
     */
    private static void WriteAPP0Segment(BitStream bitStream)
    {
      UInt16 startMarker = 0xFFE0;
      UInt16 lengthOfSegment = 16; // length >= 16
      byte versionhi = 1; // major revision number (1)
      byte versionlo = 1; // minor revision number (0..2)
      byte xyunits = 0;   // pixel size (0 = no units, 1 = dots/inch, 2 = dots/cm)
      UInt16 xdensity = 0x0048;
      UInt16 ydensity = 0x0048;
      byte thumbnwidth = 0;
      byte thumbnheight = 0;

      bitStream.writeWord(startMarker);
      bitStream.writeWord(lengthOfSegment);
      bitStream.writeByte((byte)'J');
      bitStream.writeByte((byte)'F');
      bitStream.writeByte((byte)'I');
      bitStream.writeByte((byte)'F');
      bitStream.writeByte(0x0);
      bitStream.writeByte(versionhi);
      bitStream.writeByte(versionlo);
      bitStream.writeByte(xyunits);
      bitStream.writeWord(xdensity);
      bitStream.writeWord(ydensity);
      bitStream.writeByte(thumbnheight);
      bitStream.writeByte(thumbnwidth);
    }

    /*
     * Write "Start of Frame 0"-Segment
     */
    private static void WriteSOF0Segment(BitStream bitStream, UInt16 ht, UInt16 wid)
    {
      UInt16 marker = 0xFFC0;
      UInt16 length = 17;   // 8 + nrofcomponets * 3
      byte precision = 8;   // in bits/sample
      byte nrofcomponents = 3;

      byte IdY = 1;     // ID (1 = Y, 2 = Cb, 3 = Cr)
      byte HVY = 0x22;  // sampling factor (no sampling = 0x22, sampling factor 2 = 0x11)
      byte QTY = 0;     // number of quantization

      byte IdCb = 2;
      byte HVCb = 0x11;
      byte QTCb = 1;

      byte IdCr = 3;
      byte HVCr = 0x11;
      byte QTCr = 1;

      bitStream.writeWord(marker);
      bitStream.writeWord(length);
      bitStream.writeByte(precision);
      bitStream.writeWord(ht);
      bitStream.writeWord(wid);
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

    private static void WriteDHTSegment(BitStream bitStream)
    {
      UInt16 marker = 0xFFC4;
      UInt16 length = 17;   // ?? calculate Length

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