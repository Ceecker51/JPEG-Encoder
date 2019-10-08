using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace encoder.lib
{
    interface IPixelBuilder
    {
        void WithColourPart1();
        void WithColourPart2();
        void WithColourPart3();
        Pixel GetPixelItem();
    }
}
