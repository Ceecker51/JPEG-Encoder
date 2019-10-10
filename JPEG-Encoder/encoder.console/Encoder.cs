using System;

using encoder.lib;

namespace encoder.console
{
  class Encoder
  {
    private const int stepX = 16;
    private const int stepY = 16;
    private const string filePath = @"../images/test_with_borders.ppm";
    // private const string filePath = @"../../../../images/test_with_borders.ppm"; // for windows

    static void Main(string[] args)
    {
      // check for cli arguments
      bool isWindows = true;
      if (args.Length > 0)
      {
        isWindows = !args[0].ToString().Equals("-mac");
      }
            
      PixelMap pixelMap = PPMReader.ReadFromPPMFile(filePath, stepX, stepY, isWindows);

      for (int y = 0; y < pixelMap.Height; y++)
      {
        for (int x = 0; x < pixelMap.Width; x++)
        {
          Console.Write("({0},{1}) -> ", x, y);
          PrintWithColor(pixelMap.GetPixel(x, y));
        }
        Console.WriteLine("---");
      }

      for (int y = 0; y < pixelMap.Height; y++)
      {
        for (int x = 0; x < pixelMap.Width; x++)
        {
          Console.Write(pixelMap.GetPixel(x, y).Blue);
        }
        Console.WriteLine("");
      }
    }

    static void PrintWithColor(RGBColor color)
    {
      const int padding = 4;

      Console.BackgroundColor = ConsoleColor.Red;
      Console.Write(" ");
      Console.ResetColor();
      Console.Write(color.Red.ToString().PadRight(padding));

      Console.BackgroundColor = ConsoleColor.Green;
      Console.Write(" ");
      Console.ResetColor();
      Console.Write(color.Green.ToString().PadRight(padding));

      Console.BackgroundColor = ConsoleColor.Blue;
      Console.Write(" ");
      Console.ResetColor();
      Console.Write(color.Blue.ToString().PadRight(padding));

      Console.WriteLine();
    }
  }
}
