using encoder.lib;
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
      byte[] testString = uniEncoding.GetBytes("b");
      // "Zwölf laxe Typen qualmen verdächtig süße Objekte");
      byte[] euro = uniEncoding.GetBytes(
            "€");


      using (MemoryStream memStream = new MemoryStream(100))
      {
        memStream.Write(testString, 0, testString.Length);

        memStream.Seek(0, SeekOrigin.Begin);

        var bitStream = new BitStream(memStream);

        foreach (bool bit in bitStream.readBitsStackOverFlowStyle())
        {

        }

      }
    }

  }

  class BitStream
  {
    private Stream stream;
    private int byteNumber;

    public BitStream(Stream input)
    {
      this.stream = input;
      this.byteNumber = 0;
    }

    public void write()
    {

    }

    public IEnumerable<bool> readBitsStackOverFlowStyle()
    {
      // https://stackoverflow.com/questions/1315839/how-to-write-read-bits-from-to-a-stream-c
      if (stream == null) throw new NullReferenceException("No input stream provided");
      if (!stream.CanRead) throw new ArgumentException("Not able to read from input");

      int readByte;
      while ((readByte = stream.ReadByte()) >= 0)
      {
        for (int i = 7; i >= 0; i--)
        {
          int bit = ((readByte >> i) & 1);
          Console.Write("{0}", bit);
          yield return bit == 1;
        }

        if (byteNumber == 1) Console.WriteLine();
        else Console.Write(" ");

        byteNumber = ++byteNumber % 2;

      }
    }

  }
}
