﻿using encoder.lib;
using System.IO;
using System.Collections.Generic;
using System;
using System.Text;

namespace encoder.console
{
  class Encoder
  {
    private const int stepX = 8;
    private const int stepY = 8;
    private const bool isWindows = true;

    static void Main(string[] args)
    {
      // string inputFilename = "triumphant.ppm";
      // string inputFilePath = isWindows ? @"../../../../images/" + inputFilename : @"../images/" + inputFilename;

      // Picture rgbPicture = PPMReader.ReadFromPPMFile(inputFilePath, stepX, stepY);
      // rgbPicture.Print();

      // Picture yCbCrPicture = Picture.toYCbCr(rgbPicture);
      // yCbCrPicture.Print();

      // string outputFilename = "out_" + inputFilename;
      // string outputFilePath = isWindows ? @"../../../../images/" + outputFilename : @"../images/" + outputFilename;

      // PPMWriter.WritePictureToPPM(outputFilePath, yCbCrPicture);

      // yCbCrPicture.ReduceY(2);
      // yCbCrPicture.ReduceCb(4);
      // yCbCrPicture.ReduceCr(8);

      // yCbCrPicture.Print();
      bitStreamStuff();
    }

    public static void bitStreamStuff()
    {
      UnicodeEncoding uniEncoding = new UnicodeEncoding();
      byte[] testString = uniEncoding.GetBytes(
            "Zwölf laxe Typen qualmen verdächtig süße Objekte");
      byte[] euro = uniEncoding.GetBytes(
            "€");
      MemoryStream memStream = new MemoryStream(100);

      memStream.Write(euro, 0, euro.Length);

      memStream.Seek(0, SeekOrigin.Begin);

      foreach (bool bit in BitStream.readBitsStackOverFlowStyle(memStream))
      {

      }



      memStream.Close();
    }

  }

  class BitStream
  {
    public void write()
    {

    }

    public static IEnumerable<bool> readBitsStackOverFlowStyle(Stream input)
    {
      // https://stackoverflow.com/questions/1315839/how-to-write-read-bits-from-to-a-stream-c
      if (input == null) throw new ArgumentNullException("No input stream provided");
      if (!input.CanRead) throw new ArgumentException("Not able to read from input");
      Console.WriteLine("yo");
      int readByte;
      while ((readByte = input.ReadByte()) >= 0)
      {
        for (int i = 7; i >= 0; i--)
        {
          Console.WriteLine("{0}, {1}, {2}", (readByte >> i), (readByte >> i) & 1, ((readByte >> i) & 1) == 1);
          yield return ((readByte >> i) & 1) == 1;
        }
      }
    }

  }
}
