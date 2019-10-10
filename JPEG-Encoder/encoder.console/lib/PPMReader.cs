using System.IO;
using System;

namespace encoder.lib
{
  public class PPMReader
  {
    public static PixelMap ReadFromPPMFile(string filename, int stepX, int stepY, Boolean isWindows)
    {
      int headerItemCount = 0;

      // check if file exists
      if (!File.Exists(filename))
      {
        throw new FileNotFoundException("The file " + filename + " does not exist", filename);
      }

      // open file in stream
      BinaryReader reader = new BinaryReader(new FileStream(filename, FileMode.Open));

      // 1. Read the Header
      while (headerItemCount < 2)
      {
        char nextChar = (char)reader.PeekChar();
        if (nextChar == '#') // comment
        {
          char currentChar;
          while ((currentChar = reader.ReadChar()) != '\n') ; // ignore the rest of the line.
        }
        else if (Char.IsWhiteSpace(nextChar)) // whitespace
        {
          reader.ReadChar(); // ignore whitespace
        }
        else
        {
          switch (headerItemCount)
          {
            case 0: // next item is magic number
              // Read the first 2 characters and determine the type of pixelmap.
              string magicNumber = ReadMagicNumber(reader);
              headerItemCount++;
              break;
            case 1: // next item is width and height
              headerItemCount++;
              break;
            default:
                throw new PPMReaderException("Error parsing the file header");
          }
        }             
      }

      dimensions originalSize = ParseSize(reader);
      dimensions steppedSize = CalculateSteppedSizes(originalSize, stepX, stepY);

      if (isWindows)
        reader.ReadChar();

      ParseMaxColorValue(reader); // todo: clamp

      // initialize Picture
      PixelMap pixelMap = new PixelMap(steppedSize.width, steppedSize.height);

      // fill in pixels
      for (int y = 0; y < originalSize.height; y++)
      {
        for (int x = 0; x < originalSize.width; x++)
        {
          pixelMap.SetPixel(x, y, ReadColor(reader));
        }
      }

      // fill bottom left quarter with border values
      for (int y = originalSize.height; y < steppedSize.height; y++)
      {
        for (int x = 0; x < originalSize.width; x++)
        {
          pixelMap.SetPixel(x, y, pixelMap.GetPixel(x, originalSize.height - 1));
        }
      }

      // ... top right quarter
      for (int y = 0; y < originalSize.height; y++)
      {
        for (int x = originalSize.width; x < steppedSize.width; x++)
        {
          pixelMap.SetPixel(x, y, pixelMap.GetPixel(originalSize.width - 1, y));
        }
      }

      // ... bottom right quarter
      for (int y = originalSize.height; y < steppedSize.height; y++)
      {
        for (int x = originalSize.width; x < steppedSize.width; x++)
        {
          pixelMap.SetPixel(x, y, pixelMap.GetPixel(originalSize.width - 1, originalSize.height - 1));
        }
      }

      reader.Close();

      return pixelMap;
    }

    private static string ReadMagicNumber(BinaryReader reader)
    {
      char[] chars = reader.ReadChars(2);
      //check for right format
      if (chars[0] != 'P' || chars[1] != '3')
      {
        throw new PPMReaderException("Wrong format - expecting .ppm");
      }
      return chars[0].ToString() + chars[1].ToString();
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
      return count;

    }



    struct dimensions { public int width; public int height; };

    private static dimensions ParseSize(BinaryReader reader)
    {
      int width = ReadValue(reader);
      int height = ReadValue(reader);
      return new dimensions { width = width, height = height };
    }

    private static int ReadValue(BinaryReader reader)
    {
      string value = string.Empty;

      char nextChar;
      while (!Char.IsWhiteSpace(nextChar = (char)reader.PeekChar()))
      {
        value += reader.ReadChar().ToString();
      }
      reader.ReadChar(); // ignore the whitespace
      return int.Parse(value);
    }

    private static void ParseMaxColorValue(BinaryReader reader)
    {


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

