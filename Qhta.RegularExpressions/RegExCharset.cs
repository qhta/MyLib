using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.RegularExpressions
{
  public class RegExCharset: RegExItem
  {

    public bool IsNegative => Items.Count > 0 && Items[0].Tag == RegExTag.CharSetControlChar;

    public RegExItems Items { get; private set; } = new RegExItems();

    public override RegExItems SubItems => Items;

    public bool IsEmpty => Items.Count == 0 || Items.Count == 1 && Items[0].Tag == RegExTag.CharSetControlChar;

  }
}
