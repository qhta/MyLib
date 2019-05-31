using System;

namespace Qhta.DbUtils
{
  /// <summary>
  /// Klasa reprezentująca informacje o tabeli w bazie danych
  /// </summary>
  public class DbTableInfo
  {
    /// <summary>
    /// Nazwa tabeli
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Data/czas ostatniej modyfikacji
    /// </summary>
    public DateTime? LastModifiedAt { get; set; }

    /// <summary>
    /// Całkowita liczba rekordów w tabeli
    /// </summary>
    public Int64? RowsCount { get; set; }

    /// <summary>
    /// Informacje o bazie danych
    /// </summary>
    public DbInfo Database { get; set; }

  }
}
