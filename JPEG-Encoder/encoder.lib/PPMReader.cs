using System.IO;
using System;

namespace encoder.lib
{
  public class PPMReader
  {
    public static Picture ReadFromPPMFile(string filename, int stepX, int stepY)
    {
      BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open));
      //check for right format
      if (reader.ReadChar() != 'P' || reader.ReadChar() != '3')
      {
        throw new PPMReaderException("Wrong format - expecting .ppm");
      }

      // read newline
      reader.ReadChar();
      // reader.ReadChar(); // for windows

      // strip the comment
      char currentChar = reader.ReadChar();
      if (currentChar == '#')
      {
        while ((currentChar = reader.ReadChar()) != '\n')
        {

        }
      }

      dimensions originalSize = ParseSize(reader);
      dimensions steppedSize = CalculateSteppedSizes(originalSize, stepX, stepY);
      ParseMaxColorValue(reader); // todo: clamp

      // initialize Picture
      Picture picture = new Picture(steppedSize.width, steppedSize.height);

      RGBColor currentColor;
      RGBColor borderColor = new RGBColor(0, 0, 0);
      for (int y = 0; y < steppedSize.height; y++)
      {

        for (int x = 0; x < steppedSize.width; x++)
        {
          // pick pixel above if run out of height
          if (y >= originalSize.height)
          {
            picture.SetPixel(x, y, picture.GetPixel(x, y - 1));
            continue;
          }

          // pick Pixel to the left if run out of width
          if (x < originalSize.width)
          {
            currentColor = ReadColor(reader);
            borderColor = currentColor;
            picture.SetPixel(x, y, currentColor);
            continue;
          }

          picture.SetPixel(x, y, borderColor);
        }


      }

      return picture;
    }

    private static RGBColor ReadColor(BinaryReader reader)
    {
      string reds = "";
      string greens = "";
      string blues = "";

      char currentChar;

      while ((currentChar = reader.ReadChar()) != ' ')
        reds += currentChar;

      while ((currentChar = reader.ReadChar()) != ' ')
        greens += currentChar;

      while ((currentChar = reader.ReadChar()) != ' ')
        blues += currentChar;

      int red = int.Parse(reds);
      int green = int.Parse(greens);
      int blue = int.Parse(blues);

      return new RGBColor(red, green, blue);
    }

    private static dimensions CalculateSteppedSizes(dimensions originalSize, int stepX, int stepY)
    {
      return new dimensions
      {
        width = stepX * CalculateContainingSize(originalSize.width, stepX),
        height = stepY * CalculateContainingSize(originalSize.height, stepY)
      };
    }

    private static int CalculateContainingSize(int original, int step)
    {
      int count = 1;
      int accumulator = step;
      while (accumulator < original)
      {
        accumulator += step;
        count++;
      }
      Console.WriteLine(count);
      return count;

    }



    struct dimensions { public int width; public int height; };

    private static dimensions ParseSize(BinaryReader reader)
    {
      char currentChar;
      string widths = "", heights = "";
      while ((currentChar = reader.ReadChar()) != ' ')
        widths += currentChar;
      while ((currentChar = reader.ReadChar()) >= '0' && currentChar <= '9')
        heights += currentChar;

      int width = int.Parse(widths);
      int height = int.Parse(heights);
      return new dimensions { width = width, height = height };
    }

    private static void ParseMaxColorValue(BinaryReader reader)
    {
      // reader.ReadChar(); // for windows

      if (reader.ReadChar() != '2' || reader.ReadChar() != '5' || reader.ReadChar() != '5')
      {
        throw new PPMReaderException("Reading max color value failed");
      }
      // skip carriage return and newline
      reader.ReadChar();
    }
  }
  public class PPMReaderException : System.Exception
  {
    public PPMReaderException(string message)
       : base(String.Format("Some custom error message. Value: {0}", message)) { }

  }
}

