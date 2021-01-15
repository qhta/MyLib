using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.RegularExpressions
{
  public class RegExCharRange: RegExItem
  {
    public Char FirstChar => Str.FirstOrDefault();
    public Char LastChar => Str.LastOrDefault();
  }
}
