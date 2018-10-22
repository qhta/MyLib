using System.Collections.Generic;

namespace Qhta.TextUtils
{
  public static class CSVUtils
  {
    public static string[] SplitCSV(this string str, char sep)
    {
      List<string> result = new List<string>();
      int i = 0;
      int k = 0;
      while (i>=0 && i<str.Length && k>=0)
      {
        if (str[i]=='\"')
        {
          if (i<str.Length-1)
          {
            k = str.IndexOf('\"', i+1);
            while (k>=0)
            {
              if (k==str.Length-1 || k<str.Length-1 && str[k+1]==sep)
              {
                string s = str.Substring(i+1, k-i-1);
                result.Add(s.Replace("\"\"", "\""));
                i=k+2;
                break;
              }
              else if (k<str.Length-2 && str[k+1]=='\"')
                k = str.IndexOf('\"', k+2);
              else
                k = str.IndexOf('\"', k+1);
            }
          }
          else
          {
            k=str.Length;
            string s = str.Substring(i, k-i);
            result.Add(s.Replace("\"\"", "\""));
            break;
          }
        }
        else
        {
          k = str.IndexOf(sep, i);
          if (k>=0)
          {
            string s = str.Substring(i, k-i);
            result.Add(s.Replace("\"\"", "\""));
            i=k+1;
          }
          else
          {
            k=str.Length;
            string s = str.Substring(i, k-i);
            if (s.Length>0)
              result.Add(s.Replace("\"\"", "\""));
            break;
          }
        }
      }
      return result.ToArray();
    }

  }
}
