using MathNet.Numerics.LinearAlgebra;

namespace encoder.lib
{
  public class Picture
  {
    private Matrix<double> red;
    private Matrix<double> green;
    private Matrix<double> blue;

    public Picture(int width, int height)
    {
      Width = width;
      Height = height;

      red = Matrix<double>.Build.Dense(width, height);
      green = Matrix<double>.Build.Dense(width, height);
      blue = Matrix<double>.Build.Dense(width, height);

      MaxColorValue = 255;
    }
    public int Width { get; set; }
    public int Height { get; set; }

    public int MaxColorValue { get; }

    public void SetPixel(int x, int y, RGBColor color)
    {
      red[x, y] = color.Red;
      green[x, y] = color.Green;
      blue[x, y] = color.Blue;
    }
    public RGBColor GetPixel(int x, int y)
    {
      return new RGBColor((int)red[x, y], (int)green[x, y], (int)blue[x, y]);
    }

  }
}
