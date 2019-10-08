using System;

using encoder.lib;

namespace encoder.console
{
  class Encoder
  {
    static void Main(string[] args)
    {
      Picture picture = PPMReader.ReadFromPPMFile(@"../images/test.ppm");

      Console.WriteLine("-> picture is {0} x {1}", picture.Width, picture.Height);

      for (int y = 0; y < picture.Height; y++)
      {
        for (int x = 0; x < picture.Width; x++)
        {
          Pixel pixel = picture.GetPixel(x, y);
          Console.WriteLine(pixel);
        }
        Console.WriteLine("---");
      }
    }
  }
}
