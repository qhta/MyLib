using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Typ danych w bazie danych
  /// </summary>
  public class DbDataType
  {
    /// <summary>
    /// Typ danych SQL
    /// </summary>
    public SqlDbType SqlDbType { get; set; }
    /// <summary>
    /// Dodatkowa informacja
    /// </summary>
    public object ExtraInfo { get; set; }
  }


}
