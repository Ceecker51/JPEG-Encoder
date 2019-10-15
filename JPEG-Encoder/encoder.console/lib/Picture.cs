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

    public static Picture toYCbCr(Picture picture)
    {
      double[,] transformationConstants = {{0.299, 0.587, 0.144},
                                        {-0.1687, -0.3312, 0.5},
                                        {0.5,-0.4186, 0.0813}};

      double[] normalisationConstants = { 0,
                                       0.5 * picture.MaxColorValue,
                                       0.5 * picture.MaxColorValue };

      var transMatrix = Matrix<double>.Build;
      var normVector = Vector<double>.Build;





      return null;
    }

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
