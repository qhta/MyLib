using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Qhta.RegularExpressions
{
  public class RegExOptions: RegExItem
  {
    public RegExOptions()
    {
      Items = new RegExItems();
    }

  }
}
