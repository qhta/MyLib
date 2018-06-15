using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.TextUtils
{
  public static class StringAllowingDuplicateComparer
  {
    static DuplicateAllowingCompararer<string> instance = new DuplicateAllowingCompararer<string>();

    public static DuplicateAllowingCompararer<string> Instance
    {
      get
      {
        return instance;
      }
    }
  }
}
