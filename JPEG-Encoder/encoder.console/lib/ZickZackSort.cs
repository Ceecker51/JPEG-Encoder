using System;
namespace encoder.lib
{
  class ZickZack
  {
    public static int[] Sort(int[,] input)
    {
      int yLength = input.GetLength(0);
      int xLength = input.GetLength(1);

      // check dimensions
      if (xLength != yLength)
      {
        throw new InvalidOperationException("Array dimension must be square");
      }

      // create result array
      int resultLength = xLength * yLength;
      int[] result = new int[resultLength];

      // initialze helper variables
      int x = 0;
      int y = 0;
      Direction direction = Direction.UPRIGHT;    // false = runter | true = hoch
      bool changeSteps = true;  // true = increase steps | false= decrease steps

      int xStart = 0;
      int yStart = 0;

      // execute ZikZak algo
      for (int i = 0; i < resultLength; i++)
      {
        // top left corner: save directly to result array
        if (i == 0)
        {
          result[i] = input[y, x];
        }
        // when at the end of a diagonal...
        else if (xStart == y && yStart == x)
        {
          // ... and in bottom left corner...
          if (x == 0 && y == yLength - 1)
          {
            // ... change directions taken at the end of a diagonal 
            // so that it goes right at the lower end and left at the upper end 
            changeSteps = false;
          }

          // ... and arrived at the top ...
          if (direction == Direction.UPRIGHT)
          {
            // ... alternate between going down and going right
            if (changeSteps)
            {
              x++;
            }
            else
            {
              y++;
            }
          }
          // ... and arrived at the bottom ...
          else
          {
            // ... alternate between going down and going right
            if (changeSteps)
            {
              y++;
            }
            else
            {
              x++;
            }
          }

          // change diagonal direction 
          direction = direction == Direction.UPRIGHT ? Direction.DOWNLEFT : Direction.UPRIGHT;

          // save where diagonal started
          xStart = x;
          yStart = y;
        }
        // ... when traversing the diagonal ...
        else
        {
          // ... continue upwards.
          if (direction == Direction.UPRIGHT)
          {
            y--;
            x++;
          }
          // ... continue downwards.
          else
          {
            y++;
            x--;
          }
        }

        // save current position to result array
        result[i] = input[y, x];
      }

      return result;
    }

  }

  enum Direction
  {
    DOWNLEFT, UPRIGHT
  }
}