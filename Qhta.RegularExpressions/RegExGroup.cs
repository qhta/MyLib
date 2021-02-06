using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Qhta.RegularExpressions
{
  public class RegExGroup: RegExNamedItem
  {
    public RegExGroup()
    {
      Items = new RegExItems();
    }

    public int? GroupNumber { get; set; }

    public RegExOptions UsingOptions
    {
      get
      {
        if (Items.Count > 1 && Items[1] is RegExOptions result)
          return result;
        return null;
      }
    }

    public RegExOptions CancelOptions
    {
      get
      {
        if (Items.Count > 2 && Items[1].Tag == RegExTag.GroupControlChar && Items[2] is RegExOptions result)
          return result;
        if (Items.Count > 3 && Items[2].Tag == RegExTag.GroupControlChar && Items[2] is RegExOptions result2)
          return result2;
        return null;
      }
    }

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
