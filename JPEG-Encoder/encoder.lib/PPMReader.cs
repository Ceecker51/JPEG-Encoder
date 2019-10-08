using System.IO;
using System;

namespace encoder.lib
{
  public class PPMReader
  {
    public static Picture ReadFromPPMFile(string Filename)
    {
      BinaryReader reader = new BinaryReader(new FileStream(Filename, FileMode.Open));

      //check for right format
      if (reader.ReadChar() != 'P' || reader.ReadChar() != '3')
      {
        throw new PPMReaderException("Wrong format - expecting .ppm");
      }

      // read newline
      reader.ReadChar();

      // strip the comment
      char currentChar = reader.ReadChar();
      if (currentChar == '#')
      {
        while ((currentChar = reader.ReadChar()) != '\n')
        {

        }
      }

      dimensions size = ParseSize(reader);
      ParseMaxColorValue(reader);

      // initialize Picture
      Picture picture = new Picture(size.width, size.height);
      for (int y = 0; y < size.height; y++)
      {
        for (int x = 0; x < size.width; x++)
        {
          string reds = "", greens = "", blues = "";

          // read red
          while ((currentChar = reader.ReadChar()) != ' ')
            reds += currentChar;

          while ((currentChar = reader.ReadChar()) != ' ')
            greens += currentChar;

          while ((currentChar = reader.ReadChar()) != ' ')
            blues += currentChar;

          int red = int.Parse(reds);
          int green = int.Parse(greens);
          int blue = int.Parse(blues);

          RGBColor color = new RGBColor(red, green, blue);
          picture.SetPixel(x, y, color);
        }
      }

      return picture;
    }

    struct dimensions { public int width; public int height; };

    static dimensions ParseSize(BinaryReader reader)
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

    static void ParseMaxColorValue(BinaryReader reader)
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

