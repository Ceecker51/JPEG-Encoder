using MathNet.Numerics.LinearAlgebra;

namespace encoder.lib
{
  class Transformation
  {
    public static Matrix<double> transformDirectly(Matrix<double> input)
    {
      // iterate over all 8x8 matrices
      for (int column = 0; column < input.ColumnCount; column++)
      {
        for (int row = 0; row < input.RowCount; row++)
        {
          var matrix = input.SubMatrix(row, 8, column, 8);



        }
      }
      return null;
    }
  }
}