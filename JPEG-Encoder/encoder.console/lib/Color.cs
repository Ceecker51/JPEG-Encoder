namespace encoder.lib
{
  public class Color
  {
    public Color(float channel1, float channel2, float channel3)
    {
      Channel1 = channel1;
      Channel2 = channel2;
      Channel3 = channel3;
    }

    public float Channel1 { get; set; }
    public float Channel2 { get; set; }
    public float Channel3 { get; set; }
  }
}
