using System;

using encoder.lib;

namespace encoder.console
{
  class Encoder
  {
    static void Main(string[] args)
    {
      Picture picture = PPMReader.ReadFromPPMFile(@"C:\Users\willi\Documents\Multimedia II\JPEG-Encoder\JPEG-Encoder\encoder.console\test.ppm");

      for (int y = 0; y < picture.Height; y++)
      {
        for (int x = 0; x < picture.Width; x++)
        {
          Pixel pixel = picture.GetPixel(x, y);
          Console.Write(pixel);
          Console.WriteLine();
        }
      }

      Console.ReadKey();
    }
  }
}
