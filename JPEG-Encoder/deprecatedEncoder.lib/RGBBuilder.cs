using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace encoder.lib
{
    class RGBBuilder : IPixelBuilder
    {
        private Pixel _pixel = new Pixel();

        public void WithColourPart1()
        {
            _pixel.ColourPart1 = 0;
        }

        public void WithColourPart2()
        {
            _pixel.ColourPart2 = 0;
        }

        public void WithColourPart3()
        {
            _pixel.ColourPart3 = 0;
        }

        public Pixel GetPixelItem()
        {
            return _pixel;
        }
    }
}
