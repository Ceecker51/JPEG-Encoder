namespace encoder.lib
{
  public class Color
  {
    public Color(int channel1, int channel2, int channel3)
    {
      Channel1 = channel1;
      Channel2 = channel2;
      Channel3 = channel3;
    }

    public int Channel1 { get; set; }
    public int Channel2 { get; set; }
    public int Channel3 { get; set; }
  }
}
