using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Qhta.RegularExpressions
{
  //[XmlType("RegExGroup")]
  public class RegExGroup: RegExItem
  {
    [DefaultValue(null)]
    public string Name { get; set; }

    public RegExItems Items { get; private set; } = new RegExItems();

    public override bool Equals(object obj)
    {
      if (obj is RegExGroup other)
      {
        if (this.Name != other.Name)
          return false;
        if (this.Items != null && !this.Items.Equals(other.Items))
          return false;
        return base.Equals(obj);
      }
      return base.Equals(obj);
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }

  }
}
