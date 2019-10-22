using System;
using System.IO;

namespace encoder.lib
{
    class PPMWriter
    {
        public static void WritePictureToPPM (string file, Picture picture)
        {
            // write header to stream
            StreamWriter writer = new StreamWriter(file);
            writer.WriteLine("P3");
            writer.WriteLine("# Created by Encoder");
            writer.WriteLine($"{picture.Width} {picture.Height}");
            writer.WriteLine(picture.MaxColorValue);

            // write the data
            int colorCounter = 0;
            for (int y = 0; y < picture.Height; y++)
            {
                for (int x = 0; x < picture.Width; x++)
                {
                    Color color = picture.GetPixel(x, y);
                    writer.Write($"{(int) color.Channel1} ");
                    writer.Write($"{(int) color.Channel2} ");
                    writer.Write($"{(int) color.Channel3} ");

                    colorCounter++;

                    if (colorCounter == 5)
                    {
                        writer.Write('\n');
                        colorCounter = 0;
                    }
                }
            }

            writer.Close();
        }
    }
}
