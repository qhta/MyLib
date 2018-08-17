using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Informacja o plikach bazy danych
  /// </summary>
  public struct DbFileInfo
  {
    /// <summary>
    /// Typ pliku (zależny od silnika)
    /// </summary>
    public string Type;
    /// <summary>
    /// Nazwa logiczna pliku (zależny od silnika)
    /// </summary>
    public string LogicalName;
    /// <summary>
    /// Nazwa fizyczna pliku
    /// </summary>
    public string PhysicalName;
  }
}
