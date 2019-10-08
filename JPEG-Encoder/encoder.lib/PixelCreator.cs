using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace encoder.lib
{
    class PixelCreator
    {
        public void Construct(IPixelBuilder pixelBuilder)
        {
            pixelBuilder.WithColourPart1();
            pixelBuilder.WithColourPart2();
            pixelBuilder.WithColourPart3();

        }
    }
}
