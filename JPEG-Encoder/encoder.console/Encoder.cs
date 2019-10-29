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
    private const bool isWindows = false;

    static void Main(string[] args)
    {
      string inputFilename = "test.txt";
      string inputFilePath = isWindows ? @"../../../../assets/" + inputFilename : @"../assets/" + inputFilename;

      // Picture rgbPicture = PPMReader.ReadFromPPMFile(inputFilePath, stepX, stepY);
      // rgbPicture.Print();

      // Picture yCbCrPicture = Picture.toYCbCr(rgbPicture);
      // yCbCrPicture.Print();

      // string outputFilename = "out_" + inputFilename;
      // string outputFilePath = isWindows ? @"../../../../assets/" + outputFilename : @"../assets/" + outputFilename;

      // PPMWriter.WritePictureToPPM(outputFilePath, yCbCrPicture);

      // yCbCrPicture.ReduceY(2);
      // yCbCrPicture.ReduceCb(4);
      // yCbCrPicture.ReduceCr(8);

      // yCbCrPicture.Print();
    }

    public static void bitStreamStuffMemoryStream()
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
        //var bitStream = new BitStream(memStream);
        //bitStream.prettyPrint();
      }
    }
  }

  class BitStream
  {
    private Stream stream;
        private byte buffer;
        private int bufferLength;
    public BitStream()
    {
            this.stream = new MemoryStream();
            buffer = 0;
            bufferLength = 0;
    }

    public void writeBit(int bit)
    {
            if (stream == null) throw new NullReferenceException("No input stream provided");
            if (!stream.CanWrite) throw new ArgumentException("Not able to write to stream");

            buffer = (byte)((buffer << 1) + bit);
            bufferLength++;
           
            if(bufferLength == 8)
            {
                this.stream.WriteByte(buffer);
                buffer = 0;
                bufferLength = 0;
            }
                    
    }

    

    public void readFromStream(Stream inputStream)
    {
            if (inputStream == null) throw new NullReferenceException("No input stream provided");
            if (!inputStream.CanWrite) throw new ArgumentException("Not able to write to stream");

            //var bitStream = new BitStream(fileStream);
            //bitStream.prettyPrint();
            
    }

    public IEnumerable<int> readBitsStackOverFlowStyle()
    {
      // https://stackoverflow.com/questions/1315839/how-to-write-read-bits-from-to-a-stream-c
      if (stream == null) throw new NullReferenceException("No input stream provided");
      if (!stream.CanRead) throw new ArgumentException("Not able to read from input");

      int readByte;
      while ((readByte = stream.ReadByte()) >= 0)
      {
        for (int i = 7; i >= 0; i--)
        {
          // shift right to byte position i, then set every bit 0 except the last one with "& 1"
          int bit = ((readByte >> i) & 1);
          yield return bit;
        }
      }
    }

    public void prettyPrint()
    {
      int bitCounter = 0;
      foreach (int bit in readBitsStackOverFlowStyle())
      {
        Console.Write(bit);
        bitCounter++;

        if (bitCounter == 4) Console.Write(" ");

        if (bitCounter == 8)
        {
          Console.WriteLine();
          bitCounter = 0;
        }

      }
    }
  }
}
