using MathNet.Numerics.LinearAlgebra;
using System;
using System.Collections.Generic;
using System.Text;

namespace encoder.utils
{
  class PictureHelper
  {
    public static Matrix<float> GenerateRandomChannel(int width, int height)
    {
      Matrix<float> result = Matrix<float>.Build.Dense(width, height);
      for (int y = 0; y < height; y++)
      {
        for (int x = 0; x < width; x++)
        {
          result[x, y] = (x + y * 8) % 256;
        }
      }

      return result;
    }
  }
}
