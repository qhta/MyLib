namespace Qhta.CodeMetrics
{
  public static class ArrayUtils
  {
    public static string GetString(this byte[] sign)
    {
      string result = null;
      foreach (var b in sign)
      {
        if (b>=32 && b<127 && b!='\\')
          result += (char)b;
        else
        {
          result+= $"\\{b:X2}";
        }
      }
      return result;
    }

    public static int EqualsCount(this byte[] a, byte[] b)
    {
      if (b==null)
        return 0;
      //var s1 = new String(Encoding.UTF8.GetChars(a));
      //var s2 = new String(Encoding.UTF8.GetChars(b));
      for (int i = 0; i<a.Length && i<b.Length; i++)
      {
        if (a[i]!=b[i])
          return i;
      }
      return a.Length;
    }
  }
}
