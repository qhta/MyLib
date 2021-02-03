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
    public RegExGroup()
    {
      Items = new RegExItems();
    }


    [DefaultValue(null)]
    public string Name { get; set; }

    public override bool Equals(object obj)
    {
      var result = base.Equals(obj);
      if (!result)
        return false;

      if (obj is RegExGroup other)
      {
        if (this.Name != other.Name)
        {
          AddInequality(new Inequality { Property = "Name", Obtained = this.Name, Expected = other.Name });
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
    public override string ToString()
    {
      var result = $"{Tag} \'{Name}\' ({Start}, {Length}) {Status}: \"{Str}\"";
      if (Inequalities != null)
        result += $" expected {Inequalities.ToString()}";
      return result;
    }
  }
}
