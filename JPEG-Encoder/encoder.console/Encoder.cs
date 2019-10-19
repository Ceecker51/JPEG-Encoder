﻿using encoder.lib;

namespace encoder.console
{
  class Encoder
  {
    private const int stepX = 8;
    private const int stepY = 8;
    private const bool isWindows = true;

    static void Main(string[] args)
    {
      //TODO: input as CLI argument
      string inputFilename = "test_5x5.ppm.ppm";
      string inputFilePath = isWindows ? @"../../../../images/" + inputFilename : @"../images/" + inputFilename;

      Picture rgbPicture = PPMReader.ReadFromPPMFile(inputFilePath, stepX, stepY);
      rgbPicture.Print();

      Picture yCbCrPicture = Picture.toYCbCr(rgbPicture);
      yCbCrPicture.Print();

      string outputFilename = "out_" + inputFilename;
      string outputFilePath = isWindows ? @"../../../../images/" + outputFilename : @"../images/" + outputFilename;

      PPMWriter.WritePictureToPPM(outputFilePath, yCbCrPicture);

      yCbCrPicture.ReduceY(2);
      yCbCrPicture.ReduceCb(4);
      yCbCrPicture.ReduceCr(8);

      yCbCrPicture.Print();
    }
  }
}
