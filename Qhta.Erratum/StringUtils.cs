using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.Erratum
{
  internal static class StringUtils
  {
    public static  string TrimLastNL(this string str)
    {
      if (str.LastOrDefault() == '\n')
        str = str.Substring(0, str.Length - 1);
      return str;
    }
  }
}
