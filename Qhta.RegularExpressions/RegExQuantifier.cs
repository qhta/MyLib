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

    public int? LowLimit
    {
      get
      {
        var firstChar = Str.FirstOrDefault();
        switch (firstChar)
        {
          case '?':
            return 0;
          case '+':
            return 1;
          case '*':
            return 0;
          default:
            var lowLimitItem = LowLimitItem;
            if (lowLimitItem != null)
              if (int.TryParse(lowLimitItem.Str, out var n))
                return n;
            break;
        }
        return null;
      }
    }

    public int? HighLimit
    {
      get
      {
        var firstChar = Str.FirstOrDefault();
        switch (firstChar)
        {
          case '?':
            return 1;
          case '+':
            return null;
          case '*':
            return null;
          default:
            var highLimitItem = HighLimitItem;
            if (highLimitItem != null)
              if (int.TryParse(highLimitItem.Str, out var n))
                return n;
            break;
        }
        return null;
      }
    }

    public bool IsMultiplying
    {
      get
      {
        var firstChar = Str.FirstOrDefault();
        switch (firstChar)
        {
          case '?':
            return false;
          case '+':
            return true;
          case '*':
            return true;
          default:
            var lowLimit = LowLimit;
            if (lowLimit != null && lowLimit > 1)
              return true;
            var highLimit = HighLimit;
            if (highLimit != null && highLimit > 1)
              return true;
            break;
        }
        return false;
      }
    }
    public RegExItem LowLimitItem
    {
      get
      {
        var item = Items.FirstOrDefault();
        if (item.Tag == RegExTag.Number)
          return item;
        return null;
      }
    }

    public RegExItem HighLimitItem
    {
      get
      {
        var item = Items.LastOrDefault();
        if (item.Tag == RegExTag.Number)
          return item;
        return null;
      }
    }

  }
}
