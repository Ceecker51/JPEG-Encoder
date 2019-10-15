using System;

using encoder.lib;

namespace encoder.console
{
  class Encoder
  {
    private const int stepX = 8;
    private const int stepY = 8;

    static void Main(string[] args)
    {
      // check for operation system
      bool isWindows = true;
      if (args.Length > 0)
      {
        isWindows = !args[0].ToString().Equals("-mac");
      }

      string fileName = "test_5x5.ppm";
      string filePath = isWindows ? @"../../../../images/" + fileName : @"../images/" + fileName;


      Picture rgbPicture = PPMReader.ReadFromPPMFile(filePath, stepX, stepY, isWindows);
      rgbPicture.Print();

      Picture yCbCrPicture = Picture.toYCbCr(rgbPicture);

      yCbCrPicture.Print();
      yCbCrPicture.ReduceCb(2);
      yCbCrPicture.Print();

    }
  }
}
