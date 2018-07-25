using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SMO = Microsoft.SqlServer.Management.Smo;

namespace MyLib.DbUtils.SqlServer
{
  /// <summary>
  /// Klasa tworząca komendę SQL do tworzenia tabeli w bazie danych
  /// SQL Serwera na podstawie tablicy <c>DataTable</c>
  /// </summary>
  public class SqlTableBuilder: DbUtils.SqlTableBuilder
  {
    /// <summary>
    /// Procedura budująca łańcuch komendy SQL "CREATE TABLE"
    /// </summary>
    /// <param name="dataTable"></param>
    /// <returns></returns>
    public override string CreateTableString(DataTable dataTable)
    {
      string result = null;
      foreach (DataColumn column in dataTable.Columns)
      {
        if (result != null)
          result += ", ";
        result += FormatColumnString(column);
      }
      foreach (Constraint constraint in dataTable.Constraints)
      {
        string str = ConstraintString(constraint, true);
        if (str != null)
        {
          if (result != null)
            result += ", ";
          result += str;
        }
      }
      return String.Format(
        "CREATE TABLE {0} ({1})",
        EncapsulateName(dataTable.TableName), result);
    }

    /// <summary>
    /// Podaje łańcuch dla typu danych SQL serwera
    /// </summary>
    /// <param name="dataType">podany typ danych</param>
    /// <returns></returns>
    protected override string DataTypeToString(DbDataType dataType)
    {
      string result = dataType.SqlDbType.ToString();
      string[] ss = result.Split('.');
      if (ss.Length >= 2)
        result = ss[1];
      if (dataType.ExtraInfo is int extInt)
        result += String.Format(" ({0})", extInt);
      else if (dataType.ExtraInfo is string extStr)
        result += extStr;
      return result;
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
        result = " PRIMARY KEY";
      else if (column.Unique)
        result = " UNIQUE";
      ForeignKeyConstraint FK = null;
      if (IsForeignKey(column, out FK))
        result += ConstraintString(FK, false);
      return result;
    }

    /// <summary>
    /// Podaje łańcuch " PRIMARY KEY", gdy podana kolumna tworzy klucz główny,
    /// a " UNIQUE", gdy jest to klucz unikatowy
    /// </summary>
    /// <param name="constraint">sprawdzana kolumna</param>
    /// <param name="isTableConstraint">czy zatrzeżenie dotyczy całej tabeli</param>
    /// <returns></returns>
    protected override string ConstraintString(Constraint constraint, bool isTableConstraint)
    {
      string result = null;
      if (constraint is UniqueConstraint)
      {
        UniqueConstraint ucs = (UniqueConstraint)constraint;
        if (ucs.Columns.Count() > 1)
        {
          if (ucs.IsPrimaryKey)
            result += " PRIMARY KEY";
          else
            result += " UNIQUE";
          string cstr = String.Join(", ", ucs.Columns.Select(item => item.ColumnName));
          if (cstr != null)
            result += " (" + cstr + ")";
        }
      }
      else if (constraint is ForeignKeyConstraint)
      {
        ForeignKeyConstraint fcs = (ForeignKeyConstraint)constraint;
        if (fcs.Columns.Count() > 1)
        {
          if (isTableConstraint)
          {
            result += " FOREIGN KEY";
            string cstr = String.Join(", ", fcs.Columns.Select(item => EncapsulateName(item.ColumnName)));
            if (cstr != null)
              result += " (" + cstr + ")";
          }
          result += " REFERENCES " + EncapsulateName(fcs.RelatedTable.TableName);
          string rstr = String.Join(", ", fcs.RelatedColumns.Select(item => EncapsulateName(item.ColumnName)));
          if (rstr != null)
            result += " (" + rstr + ")";
          if (fcs.DeleteRule != DefaultChangeRule)
            result += " ON DELETE" + RuleString(fcs.DeleteRule);
          if (fcs.UpdateRule != DefaultChangeRule)
            result += " ON UPDATE" + RuleString(fcs.UpdateRule);
        }
      }
      return result;
    }

    /// <summary>
    /// Zmiana podanej zasady reakcji na zmianę/usunięcie na łańcuch
    /// </summary>
    /// <param name="rule"></param>
    /// <returns></returns>
    protected override string RuleString(Rule rule)
    {
      switch (rule)
      {
        case System.Data.Rule.Cascade:
          return " CASCADE";
        case System.Data.Rule.SetNull:
          return " SET NULL";
        case System.Data.Rule.SetDefault:
          return " SET DEFAULT";
      }
      return " NO ACTION";
    }

    /// <summary>
    /// Gdy kolumna jest autoinkrementowana, to podaje łańcuch " IDENTITY"
    /// albo " IDENTITY (seed, step), gdzie <c>seed</c> jest wartością początkową autoinkrementacji
    /// a <c>step</c> jest krokiem autoinkrementacji
    /// </summary>
    /// <param name="column">sprawdzana kolumna</param>
    /// <returns></returns>
    protected override string IdentityString(DataColumn column)
    {
      string result = null;
      if (column.AutoIncrement)
      {
        result = " IDENTITY";
        if (column.AutoIncrementSeed != 0)
          result += String.Format(" ({0},{1})", column.AutoIncrementSeed, column.AutoIncrementStep);

      }
      if (column.ExtendedProperties["RowID"] != null)
        result += " ROWGUIDCOL";
      return result;
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
    /// Jeśli nazwa zawiera spację, to jest ujmowana w nawiasy kwadratowe
    /// albo, jeśli ustawiona jest właściwość <see cref="RemoveSpaces"/>, 
    /// to spacje z niej są usuwane
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public override string EncapsulateName(string name)
    {
      if (name.Contains(" "))
      {
        if (RemoveSpacesFromName)
          name = name.Replace(" ", "");
        else
          name = "[" + name + "]";
      }
      return name;
    }

  }
}
