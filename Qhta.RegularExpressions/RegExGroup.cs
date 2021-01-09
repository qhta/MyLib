using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.RegularExpressions
{
  public class RegExGroup: RegExItem
  {
    public string Name { get; set; }

    public RegExItems Items { get; private set; } = new RegExItems();
  }
}
