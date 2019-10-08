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
          PrintWithColor(pixel);
        }
        Console.WriteLine("---");
      }
    }

    static void PrintWithColor(Pixel pixel)
    {
      const int padding = 4;

      Console.BackgroundColor = ConsoleColor.Red;
      Console.Write(" ");
      Console.ResetColor();
      Console.Write(pixel.Color.Red.ToString().PadRight(padding));

      Console.BackgroundColor = ConsoleColor.Green;
      Console.Write(" ");
      Console.ResetColor();
      Console.Write(pixel.Color.Red.ToString().PadRight(padding));

      Console.BackgroundColor = ConsoleColor.Blue;
      Console.Write(" ");
      Console.ResetColor();
      Console.Write(pixel.Color.Red.ToString().PadRight(padding));

      Console.WriteLine();
    }
  }
}
