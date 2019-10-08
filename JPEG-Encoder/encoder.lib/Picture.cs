using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace encoder.lib
{
    class Picture
    {
        public Pixel[,] pixels;

        public Picture(int x, int y)
        {
            pixels = new Pixel[y, x];
        }
    }
}
