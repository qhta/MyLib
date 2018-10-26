using System;
using System.Data;

namespace Qhta.DbUtils
{
  /// <summary>
  /// Klasa reprezentująca informacje o kolumnie w tabeli w bazie danych
  /// </summary>
  public class DbColumnInfo
  {
    /// <summary>
    /// Nazwa kolumny
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Typ danych (dopuszczalne typy uzależnione od silnika danych)
    /// </summary>
    public SqlDbType Type { get; set; }

    /// <summary>
    /// Rozmiar pola danych (o ile to dopuszcza typ danych)
    /// </summary>
    public int? Size { get; set; }

    /// <summary>
    /// Precyzja pola danych (o ile to dopuszcza typ danych)
    /// </summary>
    public int? Precision { get; set; }

    /// <summary>
    /// Skala pola danych (o ile to dopuszcza typ danych)
    /// </summary>
    public int? Scale { get; set; }

    /// <summary>
    /// Czy pole dopuszcza wartość null
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    /// Czy pole jest automatycznie ustawiane przez bazę danych na unikatowy identyfikator rekordu
    /// </summary>
    public bool IsIdentity { get; set; }

    /// <summary>
    /// Czy pole jest kluczem głównym tabeli
    /// </summary>
    public bool IsPrimaryKey { get; set; }

  }
}
