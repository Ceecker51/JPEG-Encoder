using System;
using System.IO;

namespace encoder.lib
{
  public class PPMReader
  {
    public static Picture ReadFromPPMFile(string filename, int stepX, int stepY)
    {
      // open file in stream
      FileStream ifs = new FileStream(filename, FileMode.Open);
      BinaryReader reader = new BinaryReader(ifs);

      // read the header
      PPMHeader header = ParseHeader(reader);
     
      // calculate stepped size
      Dimension originalSize = new Dimension { Width = header.Width, Height = header.Height };
      Dimension steppedSize = CalculateSteppedSizes(originalSize, stepX, stepY);

      // initialize Picture
      Picture picture = new Picture(steppedSize.Width, steppedSize.Height, header.MaxColorValue);
      for (int y = 0; y < originalSize.Height; y++)
      {
        for (int x = 0; x < originalSize.Width; x++)
        {
          picture.SetPixel(x, y, ReadColor(reader));
        }
      }

      reader.Close();
      ifs.Close();

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

      return picture;
    }

    private static PPMHeader ParseHeader(BinaryReader reader)
    {
      // 1.1 Read the magic number
      string magicNumber = ReadNextNonCommentLine(reader);
      if (magicNumber != "P3")
      {
        throw new PPMReaderException("Wrong format - Unknown magic number: " + magicNumber);
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
      string sMaxVal = ReadNextNonCommentLine(reader);
      if (!int.TryParse(sMaxVal, out int maxColorValue))
      {
        throw new PPMReaderException("Wrong format - max color value can not be parsed");
      }
      if (maxColorValue > 255)
      {
        throw new PPMReaderException("Wrong format - Not a 8-bit image");
      }

      return new PPMHeader { MagicNumber = magicNumber,
                             Width = width,
                             Height = height,
                             MaxColorValue = maxColorValue };
    }

    private static string ReadNextNonCommentLine(BinaryReader reader)
    {
      string s = ReadNextAnyLine(reader);
      while (s.StartsWith('#') || s == string.Empty)
        s = ReadNextAnyLine(reader);
      return s;
    }

    private static string ReadNextAnyLine(BinaryReader reader)
    {
      return ReadToSign(reader, '\n');
    }

    private static int ReadNextValue(BinaryReader reader)
    {
      string sValue = ReadToSign(reader, ' ');
      if (!int.TryParse(sValue, out int value))
      {
        throw new PPMReaderException("Can not parse single value");
      }
      return value;
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
      int red = ReadNextValue(reader);
      int green = ReadNextValue(reader);
      int blue = ReadNextValue(reader);

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

    struct PPMHeader
    {
      public string MagicNumber { get; set; }
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

