namespace encoder.lib
{
    public class Pixel
    {
        public RGBColor Color { get; set; }
        public override string ToString()
        {
            return string.Format("{0},{1},{2}", Color.Red, Color.Green, Color.Blue);
        }
    }
}