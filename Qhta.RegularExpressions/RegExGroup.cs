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

    public override RegExItems SubItems => Items;

    public override bool Equals(object obj)
    {
      var result = base.Equals(obj);
      if (!result)
        return false;

      if (obj is RegExGroup other)
      {
        if (this.Name != other.Name)
        {
          Inequality = new Inequality { Property = "Name", Obtained = this.Name, Expected = other.Name };
          return false;
        }
        return true;
      }
      return false;
    }

    public override int GetHashCode()
    {
      return base.GetHashCode();
    }
  }
}
