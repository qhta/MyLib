using System.Data;

namespace Qhta.DbUtils
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
