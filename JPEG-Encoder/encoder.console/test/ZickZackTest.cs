using encoder.lib;
using encoder.utils;
using FluentAssertions;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace encoder.test
{
  class ZickZackTest
  {
    [Test]
    public void Test_SortBlock()
    {
      int[,] block = ArrayHelper.GetTwoDimensionalArrayOfLength(8);
      int[] expected = { 0, 1, 8, 16, 9, 2, 3, 10,
                        17, 24, 32, 25, 18, 11, 4, 5,
                        12, 19, 26, 33, 40, 48, 41, 34,
                        27, 20, 13, 6, 7, 14, 21, 28, 35,
                        42, 49, 56, 57, 50, 43, 36, 29,
                        22, 15, 23, 30, 37, 44, 51, 58,
                        59, 52, 45, 38, 31, 39, 46, 53,
                        60, 61, 54, 47, 55, 62, 63
      };

      int[] actual = ZickZack.SortBlock(block);
      actual.Should().BeEquivalentTo(expected);
    }

    [Test]
    public void Test_ZickZackSortChannel()
    {
      int[,] channel = ArrayHelper.GetTwoDimensionalArrayOfLength(16);
      List<int[]> expected = new List<int[]>();

      expected.Add(
        new[] { 0, 1, 16, 32, 17, 2, 3, 18,
                33, 48, 64, 49, 34, 19, 4, 5,
                20, 35, 50, 65, 80, 96, 81, 66,
                51, 36, 21, 6, 7, 22, 37, 52,
                67, 82, 97, 112, 113, 98, 83, 68,
                53, 38, 23, 39, 54, 69, 84, 99, 114,
                115, 100, 85, 70, 55, 71, 86, 101,
                116, 117, 102, 87, 103, 118, 119
        });
      expected.Add(
         new[] { 8, 9, 24, 40, 25, 10, 11, 26,
                 41, 56, 72, 57, 42, 27, 12, 13,
                 28, 43, 58, 73, 88, 104, 89, 74,
                 59, 44, 29, 14, 15, 30, 45, 60,
                 75, 90, 105, 120, 121, 106, 91, 76,
                 61, 46, 31, 47, 62, 77, 92, 107,
                 122, 123, 108, 93, 78, 63, 79, 94,
                 109, 124, 125, 110, 95, 111, 126, 127
         });
      expected.Add(
        new[] { 128, 129, 144, 160, 145, 130, 131, 146,
                161, 176, 192, 177, 162, 147, 132, 133,
                148, 163, 178, 193, 208, 224, 209, 194,
                179, 164, 149, 134, 135, 150, 165, 180,
                195, 210, 225, 240, 241, 226, 211, 196,
                181, 166, 151, 167, 182, 197, 212, 227,
                242, 243, 228, 213, 198, 183, 199, 214,
                229, 244, 245, 230, 215, 231, 246, 247
        });

      expected.Add(
        new[] { 136, 137, 152, 168, 153, 138, 139, 154,
                169, 184, 200, 185, 170, 155, 140, 141,
                156, 171, 186, 201, 216, 232, 217, 202,
                187, 172, 157, 142, 143, 158, 173, 188,
                203, 218, 233, 248, 249, 234, 219, 204,
                189, 174, 159, 175, 190, 205, 220, 235,
                250, 251, 236, 221, 206, 191, 207, 222,
                237, 252, 253, 238, 223, 239, 254, 255
        });

      List<int[]> actual = ZickZack.ZickZackSortChannel(channel);
      actual.Should().BeEquivalentTo(expected);
    }
  }
}
