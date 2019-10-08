using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace encoder.lib
{
  public class Picture
  {
    private Pixel[,] pixels;

    public Picture(int width, int height)
    {
      Width = width;
      Height = height;
      pixels = new Pixel[height, width];
      MaxColorValue = 255;
    }
    public int Width { get; set; }
    public int Height { get; set; }

    public int MaxColorValue { get; }

    public void SetPixel(int x, int y, RGBColor color)
    {
      Pixel pixel = new Pixel()
      {
        Color = color
      };
      pixels[y, x] = pixel;

    }
    public Pixel GetPixel(int x, int y)
    {
      return pixels[y, x];
    }

  }
}
