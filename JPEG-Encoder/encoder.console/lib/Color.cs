namespace encoder.lib
{
  public class Color
  {
    public Color(double channel1, double channel2, double channel3)
    {
      Channel1 = channel1;
      Channel2 = channel2;
      Channel3 = channel3;
    }

    public double Channel1 { get; set; }
    public double Channel2 { get; set; }
    public double Channel3 { get; set; }
  }
}
