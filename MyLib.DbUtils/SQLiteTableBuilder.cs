using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using Microsoft.SqlServer.Management.Smo;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Klasa tworząca komendę SQL do tworzenia tabeli w bazie danych
  /// SQLite na podstawie tablicy <c>DataTable</c>
  /// </summary>  
  public class SQLiteTableBuilder: SqlTableBuilder
  {
    /// <summary>
    /// Dla podanej kolumny tworzy łańcuch definicji
    /// </summary>
    /// <param name="column">podana kolumna</param>
    /// <returns></returns>
    protected override string FormatColumnString(DataColumn column)
    {
      return string.Format("{0}{1}{2}{3}",
        EncapsulateName(column.ColumnName),
        TypeDefString(column),
        NullConstraintString(column),
        ColumnConstraintString(column));
    }

    /// <summary>
    /// Podaje łańcuch dla typu danych SQL serwera
    /// </summary>
    /// <param name="dataType">podany typ danych</param>
    /// <returns></returns>    
    protected override string DataTypeToString(DataType dataType)
    {
      if (dataType.Name == "int")
        return "INTEGER";
      if (dataType.Name == "uniqueidentifier")
        return "CHAR (36)";
      return base.DataTypeToString(dataType);
    }

    /// <summary>
    /// Podaje łańcuch " NOT NULL", gdy podana kolumna nie akceptuje pustych wartości
    /// </summary>
    /// <param name="column">sprawdzana kolumna</param>
    /// <returns></returns>
    protected override string NullConstraintString(DataColumn column)
    {
      if (!column.AllowDBNull)
        return " NOT NULL";
      return null;
    }

    /// <summary>
    /// Podaje łańcuch " PRIMARY KEY", gdy podana kolumna tworzy klucz główny,
    /// a " UNIQUE", gdy jest to klucz unikatowy
    /// </summary>
    /// <param name="column">sprawdzana kolumna</param>
    /// <returns></returns>
    protected override string ColumnConstraintString(DataColumn column)
    {
      string result = null;
      if (IsPrimaryKey(column))
      {
        result = " PRIMARY KEY";
        if (column.AutoIncrement)
          result += " AUTOINCREMENT";
      }
      else if (column.Unique)
        result = " UNIQUE";
      ForeignKeyConstraint FK = null;
      if (IsForeignKey(column, out FK))
        result += BuildConstraintString(FK, false);
      return result;
    }
  }
}
