using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Qhta.RegularExpressions
{
  public class RegExQuantifier: RegExItem
  {
    public RegExQuantifier()
    {
      Items = new RegExItems();
    }

    public RegExItem LowLimit
    {
      get
      {
        var item = Items.FirstOrDefault();
        if (item.Tag == RegExTag.Number)
          return item;
        return null;
      }
    }

    public RegExItem HighLimit
    {
      get
      {
        var item = Items.LastOrDefault();
        if (item.Tag == RegExTag.Number)
          return item;
        return null;
      }
    }

    public bool IsEmpty => Items.Count == 0 || Items.Count == 1 && Items[0].Tag == RegExTag.CharSetControlChar;

  }
}
