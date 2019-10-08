using System.IO;

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
        return null;
      }

      char currentChar;
      if ((currentChar = reader.ReadChar()) == '#')
      {
        while ((currentChar = reader.ReadChar()) != '\n')
        {

        }
      }

      //read width and height
      string widths = "", heights = "";
      while ((currentChar = reader.ReadChar()) != ' ')
        widths += currentChar;
      while ((currentChar = reader.ReadChar()) >= '0' && currentChar <= '9')
        heights += currentChar;

      int width = int.Parse(widths);
      int height = int.Parse(heights);

      // skip newline
      reader.ReadChar();

      //read max color value
      if (reader.ReadChar() != '2' || reader.ReadChar() != '5' || reader.ReadChar() != '5')
      {
        return null;
      }
      // skip carriage return and newline
      reader.ReadChar();
      reader.ReadChar();

      // initialize Picture
      Picture picture = new Picture(width, height);
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
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
  }
}
