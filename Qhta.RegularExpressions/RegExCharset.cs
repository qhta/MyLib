using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.RegularExpressions
{
  public class RegExCharSet: RegExItem
  {
    public RegExCharSet()
    {
      Items = new RegExItems();
    }

    public bool IsNegative => Items.Count > 0 && Items[0].Tag == RegExTag.CharSetControlChar;


    public bool IsEmpty => Items.Count == 0 || Items.Count == 1 && Items[0].Tag == RegExTag.CharSetControlChar;

  }
}
