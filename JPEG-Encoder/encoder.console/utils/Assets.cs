using encoder.utils;

namespace encoder.utils
{
  public static class Assets
  {
    public static string GetFilePath(string filename)
    {
      return OperatingSystem.IsWindows() ? @"../../../../assets/" + filename : @"../assets/" + filename;
    }
  }
}
