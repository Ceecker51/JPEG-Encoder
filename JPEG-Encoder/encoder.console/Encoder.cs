using System.IO;
using System.Text;
using encoder.lib;

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

      string outputFilename = "out.txt";
      string outputFilePath = isWindows ? @"../../../../assets/" + outputFilename : @"../assets/" + outputFilename;

      BitStream bitStream = new BitStream();

      // using (FileStream fileStream = new FileStream(inputFilePath, FileMode.Open))
      // {
      //   bitStream.readFromStream(fileStream);
      // }
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

