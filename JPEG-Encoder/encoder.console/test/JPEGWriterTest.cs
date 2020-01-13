using NUnit.Framework;
using FluentAssertions;

using System;
using System.Collections.Generic;
using System.Text;
using encoder.lib;

namespace encoder.test
{
  class JPEGWriterTest
  {
    [Test]
    public void Test_ChannelToArrayY()
    {
      List<DCEncode> dcValues = new List<DCEncode>();
      for (int i = 0; i < 16; i++)
      {
        dcValues.Add(new DCEncode(i, i));
      }

      List<List<ACEncode>> acValues = new List<List<ACEncode>>();
      for (int i = 0; i < 16; i++)
      {
        List<ACEncode> acBlock = new List<ACEncode>();
        for (int j = 0; j < 63; j++)
        {
          if (j == 0)
          {
            acBlock.Add(new ACEncode(0, i, i));
            continue;
          }

          acBlock.Add(new ACEncode(0, 3, 4));
        }

        acValues.Add(acBlock);
      }

      //var actual = JPEGWriter.ChannelToArrayY(dcValues, acValues, 4);
      // TODO expected + Assertion
    }
  }
}
