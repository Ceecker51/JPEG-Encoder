using System;
using System.IO;

namespace encoder.lib
{
  public class PPMReader
  {
    public static Picture ReadFromPPMFile(string filename, int stepX, int stepY, Boolean isWindows)
    {
      int headerItemCount = 0;

      // open file in stream
      FileStream ifs = new FileStream(filename, FileMode.Open);
      BinaryReader reader = new BinaryReader(ifs);

      // 1. Read the header
      // 1.1 Read the magic number
      string magicNumber = NextNonCommentLine(reader);
      if (magicNumber != "P3")
      {
        throw new PPMReaderException("Unknown magic number: " + magicNumber);
      }

      int width = 0;
      int height = 0;
      int maxColorValue = 0;
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
            case 0: // next item is width and height
              width = ReadValue(reader);
              height = ReadValue(reader);
              headerItemCount++;
              break;
            case 1: // next item is the max color value
              maxColorValue = ReadValue(reader);
              headerItemCount++;
              break;
            default:
              throw new PPMReaderException("Error parsing the file header");
          }
        }
      }

      Dimension originalSize = new Dimension { Width = width, Height = height };
      Dimension steppedSize = CalculateSteppedSizes(originalSize, stepX, stepY);

      // initialize Picture
      Picture picture = new Picture(steppedSize.Width, steppedSize.Height);

      // fill in pixels
      for (int y = 0; y < originalSize.Height; y++)
      {
        for (int x = 0; x < originalSize.Width; x++)
        {
          picture.SetPixel(x, y, ReadColor(reader));
        }
      }

      // fill bottom left quarter with border values
      for (int y = originalSize.Height; y < steppedSize.Height; y++)
      {
        for (int x = 0; x < originalSize.Width; x++)
        {
          picture.SetPixel(x, y, picture.GetPixel(x, originalSize.Height - 1));
        }
      }

      // ... top right quarter
      for (int y = 0; y < originalSize.Height; y++)
      {
        for (int x = originalSize.Width; x < steppedSize.Width; x++)
        {
          picture.SetPixel(x, y, picture.GetPixel(originalSize.Width - 1, y));
        }
      }

      // ... bottom right quarter
      for (int y = originalSize.Height; y < steppedSize.Height; y++)
      {
        for (int x = originalSize.Width; x < steppedSize.Width; x++)
        {
          picture.SetPixel(x, y, picture.GetPixel(originalSize.Width - 1, originalSize.Height - 1));
        }
      }

      reader.Close();

      return picture;
    }

    private static string NextAnyLine(BinaryReader reader)
    {
      return ReadToSign(reader, '\n');
    }

    private static string NextNonCommentLine(BinaryReader reader)
    {
      string s = NextAnyLine(reader);
      while (s.StartsWith('#') || s == string.Empty)
        s = NextAnyLine(reader);
      return s;
    }

    private static string ReadToSign(BinaryReader reader, char sign)
    {
      string s = string.Empty;

      char currentChar;
      while ((currentChar = reader.ReadChar()) != sign)
        s += currentChar;

      return s.Trim();
    }

    private static Color ReadColor(BinaryReader reader)
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

      return new Color(red, green, blue);
    }

    private static Dimension CalculateSteppedSizes(Dimension originalSize, int stepX, int stepY)
    {
      return new Dimension
      {
        Width = stepX * CalculateContainingSize(originalSize.Width, stepX),
        Height = stepY * CalculateContainingSize(originalSize.Height, stepY)
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

    struct Dimension
    {
      public int Width { get; set; }
      public int Height { get; set; }
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
  }

  public class PPMReaderException : System.Exception
  {
    public PPMReaderException(string message)
       : base(String.Format("Some custom error message. Value: {0}", message)) { }

  }
}

