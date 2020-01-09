using encoder.utils;
using System;
using System.IO;

namespace encoder.lib
{
  public class JPEGWriter
  {
    public static void WritePictureToJPEG(string jpegFileName, Picture picture)
    {
      string outputFilePath = Assets.GetFilePath(jpegFileName);

      BitStream jpegStream = new BitStream();

      // Write header segements
      WriteSOISegment(jpegStream);
      WriteAPP0Segment(jpegStream);
      WriteDQTSegment(jpegStream);
      WriteSOF0Segment(jpegStream, Convert.ToUInt16(picture.Height), Convert.ToUInt16(picture.Width));
      WriteDHTSegment(jpegStream, picture.huffmanTrees);
      WriteSOSSegment(jpegStream);
      WriteEOISegment(jpegStream);

      // Write to file
      writeToFile(jpegStream, outputFilePath);
    }

    /*
     * Write "Start of Scan"-Image
     */
    private static void WriteSOSSegment(BitStream jpegStream)
    {
      UInt16 startOfScan = 0xFFDA;
      UInt16 length = 6 + 2 * 3; // component count
      byte componentCount = 3;

      // 0000 (Y DC is HT 0) 0001 (Y AC is HT 1)
      byte yHuffmannTableID = (0 << 4) | 1;
      byte yComponentID = 1;

      // 0010 (Cb DC is HT 2) 0011 (Cb AC is HT 3)
      byte cbHuffmannTableID = (2 << 4) | 3;
      byte cbComponentID = 2;

      // 0010 (Cr DC is HT 2) 0011 (Cr AC is HT 3)
      byte crHuffmannTableID = (2 << 4) | 3;
      byte crComponentID = 3;

      byte startOfSpectralSelection = 0x00;
      byte endOfSpectralSelection = 0x3f;
      byte successiveApproximation = 0x00;

      jpegStream.writeWord(startOfScan);
      jpegStream.writeWord(length);
      jpegStream.writeByte(componentCount);
      jpegStream.writeByte(yComponentID);
      jpegStream.writeByte(yHuffmannTableID);
      jpegStream.writeByte(cbComponentID);
      jpegStream.writeByte(cbHuffmannTableID);
      jpegStream.writeByte(crComponentID);
      jpegStream.writeByte(crHuffmannTableID);
      jpegStream.writeByte(startOfSpectralSelection);
      jpegStream.writeByte(endOfSpectralSelection);
      jpegStream.writeByte(successiveApproximation);
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
     *  Write "DQT"-Segment
     */

    private static void WriteDQTSegment(BitStream bitStream)
    {
      int[][] qtTables = Quantization.GetQantizationTables();
      WriteDQTSegmentWithTables(bitStream, qtTables);
    }

    public static void WriteDQTSegmentWithTables(BitStream bitStream, int[][] qtTables)
    {
      UInt16 marker = 0xFFDB;
      UInt16 length = 67;

      int count = qtTables.Length;
      for (int number = 0; number < count; number++)
      {
        bitStream.writeWord(marker);
        bitStream.writeWord(length);

        // DQT Information 0000 + 0000 | 0001 | 0010 | 0011
        //byte dqtInformation = (byte)(number << 4);
        bitStream.writeByte((byte)number);

        // Write DQT table 64 * (0 + 1) -> precision = 0 -> 8 bit
        int qtCoefficientsLength = qtTables[number].Length;
        for (int i = 0; i < qtCoefficientsLength; i++)
        {
          bitStream.writeByte((byte)qtTables[number][i]);
        }
      }
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

    private static void WriteDHTSegment(BitStream bitStream, HuffmanTree[] trees)
    {
      UInt16 marker = 0xFFC4;

      // calculate lengths of all huffman trees
      int lengthOfAllTress = 0;
      for (int i = 0; i < trees.Length; i++)
      {
        lengthOfAllTress += trees[i].symbolsInTreeOrder.Length;
      }

      // length of segement = 2 Bytes for Length + (HT INFO = 1 Byte + HT DEPTH FREQUENCIES = 16 Byte) * (n of trees) + length of symbol array
      int length = 2 + (1 + 16) * trees.Length + lengthOfAllTress;
      byte[] lengthInBytes = BitConverter.GetBytes(length);

      // write all values
      bitStream.writeWord(marker);
      bitStream.writeByte(lengthInBytes[1]);
      bitStream.writeByte(lengthInBytes[0]);

      for (int i = 0; i < trees.Length; i++)
      {
        int tableType = ((i % 2) == 0) ? 0 : 1;

        // 000 (always 0) 0 (0 = DC, 1 = AC) 0001 (HT index)
        byte htInformation = (byte)(tableType << 4 | i);
        bitStream.writeByte(htInformation);

        foreach (var item in trees[i].frequenciesOfDepths)
        {
          bitStream.writeByte((byte)item);
        }

        foreach (var item in trees[i].symbolsInTreeOrder)
        {
          bitStream.writeBits(item, 8);
        }
      }
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