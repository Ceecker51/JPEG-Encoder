using MathNet.Numerics.LinearAlgebra;

namespace encoder.lib
{
  public class Picture
  {
    private Matrix<double> channel1;
    private Matrix<double> channel2;
    private Matrix<double> channel3;

    public Picture(int width, int height)
    {
      Width = width;
      Height = height;

      channel1 = Matrix<double>.Build.Dense(width, height);
      channel2 = Matrix<double>.Build.Dense(width, height);
      channel3 = Matrix<double>.Build.Dense(width, height);

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

      transMatrix.DenseOfArray(transformationConstants);
      normVector.DenseOfArray(normalisationConstants);




      return null;
    }

    public void SetPixel(int x, int y, Color color)
    {
      channel1[x, y] = color.Channel1;
      channel2[x, y] = color.Channel2;
      channel3[x, y] = color.Channel3;
    }
    public Color GetPixel(int x, int y)
    {
      return new Color((int)channel1[x, y], (int)channel2[x, y], (int)channel3[x, y]);
    }

  }
}
