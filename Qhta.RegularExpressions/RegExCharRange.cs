using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.RegularExpressions
{
  public class RegExCharRange: RegExItem
  {
    public RegExItem FirstChar => Items.FirstOrDefault();
    public RegExItem LastChar => Items.LastOrDefault();

    public RegExItems Items { get; private set; } = new RegExItems();

    public override RegExItems SubItems => Items;

    public bool IsEmpty => Items.Count == 0 || Items.Count == 1 && Items[0].Tag == RegExTag.CharSetControlChar;

  }
}
