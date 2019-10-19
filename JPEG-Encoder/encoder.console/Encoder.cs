using encoder.lib;

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
      string fileName = "test_5x5.ppm";
      string filePath = isWindows ? @"../../../../images/" + fileName : @"../images/" + fileName;

      Picture rgbPicture = PPMReader.ReadFromPPMFile(filePath, stepX, stepY);
      rgbPicture.Print();

      Picture yCbCrPicture = Picture.toYCbCr(rgbPicture);
      yCbCrPicture.Print();

      yCbCrPicture.ReduceY(2);
      yCbCrPicture.ReduceCb(4);
      yCbCrPicture.ReduceCr(8);

      yCbCrPicture.Print();
    }
  }
}
