using System;

using encoder.lib;

namespace encoder.console
{
  class Encoder
  {
    private const int stepX = 16;
    private const int stepY = 16;
    private const string filePath = @"../images/test_with_borders.ppm";

    static void Main(string[] args)
    {
      Picture picture = PPMReader.ReadFromPPMFile(filePath, stepX, stepY);

      Console.WriteLine("-> picture is {0} x {1}", picture.Width, picture.Height);

      for (int y = 0; y < picture.Height; y++)
      {
        for (int x = 0; x < picture.Width; x++)
        {
          Pixel pixel = picture.GetPixel(x, y);
          Console.Write("({0},{1}) -> ", x, y);
          PrintWithColor(pixel);
        }
        Console.WriteLine("---");
      }

      for (int y = 0; y < picture.Height; y++)
      {
        for (int x = 0; x < picture.Width; x++)
        {
          Console.Write(picture.GetPixel(x, y).Color.Blue);
        }
        Console.WriteLine("");
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
