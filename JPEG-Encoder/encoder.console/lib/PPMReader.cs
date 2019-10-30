using System;
using System.IO;

namespace encoder.lib
{
  public class PPMReader
  {
    public static Picture ReadFromPPMFile(string filename, int stepX, int stepY)
    {
      // open file in stream
      FileStream fileStream = new FileStream(filename, FileMode.Open);
      BinaryReader reader = new BinaryReader(fileStream);

      // read the header
      PPMHeader header = ParseHeader(reader);
     
      // calculate stepped size
      int steppedX = SteppedSize(header.Width, stepX);
      int steppedY = SteppedSize(header.Height, stepY);

      // initialize Picture
      Picture picture = new Picture(steppedX, steppedY, header.MaxColorValue);
      for (int y = 0; y < header.Height; y++)
      {
        for (int x = 0; x < header.Width; x++)
        {
          picture.SetPixel(x, y, ReadColor(reader));
        }
      }

      reader.Close();
      fileStream.Close();

      // fill bottom left quarter with border values
      for (int y = header.Height; y < steppedY; y++)
      {
        for (int x = 0; x < header.Width; x++)
        {
          picture.SetPixel(x, y, picture.GetPixel(x, header.Height - 1));
        }
      }

      // ... top right quarter
      for (int y = 0; y < header.Height; y++)
      {
        for (int x = header.Width; x < steppedX; x++)
        {
          picture.SetPixel(x, y, picture.GetPixel(header.Width - 1, y));
        }
      }

      // ... bottom right quarter
      for (int y = header.Height; y < steppedY; y++)
      {
        for (int x = header.Width; x < steppedX; x++)
        {
          picture.SetPixel(x, y, picture.GetPixel(header.Width - 1, header.Height - 1));
        }
      }

      return picture;
    }

    private static PPMHeader ParseHeader(BinaryReader reader)
    {
      // 1.1 Read the magic number
      string plainFormatIdentifier = ReadNextNonCommentLine(reader);
      if (plainFormatIdentifier != "P3")
      {
        throw new PPMReaderException("Wrong format - Unknown magic number: " + plainFormatIdentifier);
      }

      // 1.2 Read width and height
      string widthHeight = ReadNextNonCommentLine(reader);
      string[] tokens = widthHeight.Split(' ');
      if (!int.TryParse(tokens[0], out int width))
      {
        throw new PPMReaderException("Wrong format - width can not be parsed");
      }
      if (!int.TryParse(tokens[1], out int height))
      {
        throw new PPMReaderException("Wrong format - height can not be parsed");
      }

      // 1.3 Read the max. color value
      string maxValue = ReadNextNonCommentLine(reader);
      if (!int.TryParse(maxValue, out int maxColorValue))
      {
        throw new PPMReaderException("Wrong format - max color value can not be parsed");
      }
      if (maxColorValue > 255)
      {
        throw new PPMReaderException("Wrong format - Not a 8-bit image");
      }

      return new PPMHeader { PlainFormatId = plainFormatIdentifier,
                             Width = width,
                             Height = height,
                             MaxColorValue = maxColorValue };
    }

    private static string ReadNextNonCommentLine(BinaryReader reader)
    {
      string line = ReadNextAnyLine(reader);
      while (line.StartsWith('#') || line == string.Empty)
        line = ReadNextAnyLine(reader);
      return line;
    }

    private static string ReadNextAnyLine(BinaryReader reader)
    {
      return ReadToSign(reader, '\n');
    }

    private static int ReadNextValue(BinaryReader reader)
    {
      string content = ReadToNextSeperator(reader);
      if (!int.TryParse(content, out int value))
      {
        throw new PPMReaderException("Can not parse single value");
      }
      return value;
    }

    private static string ReadToSign(BinaryReader reader, char sign)
    {
      string content = string.Empty;

      char currentChar;
      while ((currentChar = reader.ReadChar()) != sign)
        content += currentChar;

      return content.Trim();
    }

    private static string ReadToNextSeperator(BinaryReader reader)
    {
      string content = string.Empty;

      do
      {
        char currentChar;
        while (!Char.IsWhiteSpace(currentChar = reader.ReadChar()))
          content += currentChar;
        content = content.Trim();
      } while (string.IsNullOrWhiteSpace(content));

      return content;
    }

    private static Color ReadColor(BinaryReader reader)
    {      
      int red = ReadNextValue(reader);
      int green = ReadNextValue(reader);
      int blue = ReadNextValue(reader);

      return new Color(red, green, blue);
    }

    private static int SteppedSize(int size, int step)
    {
       return step * CalculateContainingSize(size, step);
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

    struct PPMHeader
    {
      public string PlainFormatId { get; set; }
      public int Width { get; set; }
      public int Height { get; set; }
      public int MaxColorValue { get; set; }
    }
  }

  public class PPMReaderException : System.Exception
  {
    public PPMReaderException(string message)
       : base(String.Format("Some custom error message. Value: {0}", message)) { }

  }
}

