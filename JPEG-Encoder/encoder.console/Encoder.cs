using System;

using encoder.lib;

namespace encoder.console
{
  class Encoder
  {
    private const int stepX = 16;
    private const int stepY = 16;

    static void Main(string[] args)
    {
      // Picture yCbCrPic = Picture.toYCbCr(rGBPic)
      // check for operation system
      bool isWindows = true;
      if (args.Length > 0)
      {
        isWindows = !args[0].ToString().Equals("-mac");
      }

      string fileName = "test_with_borders.ppm";
      string filePath = isWindows ? @"../../../../images/" + fileName : @"../images/" + fileName;

      Picture picture = PPMReader.ReadFromPPMFile(filePath, stepX, stepY, isWindows);

      for (int y = 0; y < picture.Height; y++)
      {
        for (int x = 0; x < picture.Width; x++)
        {
          Console.Write("({0},{1}) -> ", x, y);
          PrintWithColor(picture.GetPixel(x, y));
        }
        Console.WriteLine("---");
      }

      for (int y = 0; y < picture.Height; y++)
      {
        for (int x = 0; x < picture.Width; x++)
        {
          Console.Write(picture.GetPixel(x, y).Channel3);
        }
        Console.WriteLine("");
      }
    }

    static void PrintWithColor(Color color)
    {
      const int padding = 4;

      Console.BackgroundColor = ConsoleColor.Red;
      Console.Write(" ");
      Console.ResetColor();
      Console.Write(color.Channel1.ToString().PadRight(padding));

      Console.BackgroundColor = ConsoleColor.Green;
      Console.Write(" ");
      Console.ResetColor();
      Console.Write(color.Channel2.ToString().PadRight(padding));

      Console.BackgroundColor = ConsoleColor.Blue;
      Console.Write(" ");
      Console.ResetColor();
      Console.Write(color.Channel3.ToString().PadRight(padding));

      Console.WriteLine();
    }
  }
}
