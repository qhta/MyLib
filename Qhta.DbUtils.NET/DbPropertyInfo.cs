using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.DbUtils
{
  public class DbPropertyInfo
  {
    /// <summary>
    /// Grupa właściwości
    /// </summary>
    public string Group { get; set; }

    /// <summary>
    /// Nazwa właściwości
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Wartość właściwości
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// Czy właściwość może być zmieniona
    /// </summary>
    public bool Readonly { get; set; }

    /// <summary>
    /// Typ właściwości - określony wówczas, gdy właściwość może być zmieniona.
    /// </summary>
    public Type Type { get; set; }
  }
}
