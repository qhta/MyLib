using System;
using System.Data;
using System.Linq;

namespace Qhta.DbUtils
{
  /// <summary>
  /// Klasa tworząca komendę SQL do tworzenia tabeli w bazie danych
  /// na podstawie tablicy <c>DataTable</c>
  /// </summary>
  public abstract class SqlTableBuilder
  {
    /// <summary>
    /// Żądanie usuwania spacji z nazwy
    /// </summary>
    public bool RemoveSpacesFromName
    {
      get;
      set;
    }

    /// <summary>
    /// Procedura budująca łańcuch komendy SQL "CREATE TABLE"
    /// </summary>
    /// <param name="dataTable"></param>
    /// <returns></returns>
    public abstract string CreateTableString(DataTable dataTable);

    /// <summary>
    /// Dla podanej kolumny tworzy łańcuch definicji
    /// </summary>
    /// <param name="column">podana kolumna</param>
    /// <returns></returns>
    protected virtual string FormatColumnString(DataColumn column)
    {
      return string.Format("{0}{1}{2}{3}{4}",
        EncapsulateName(column.ColumnName),
        TypeDefString(column),
        NullConstraintString(column),
        IdentityString(column),
        ColumnConstraintString(column));
    }

    /// <summary>
    /// Dla podanej kolumny tworzy łańcuch definiujący typ
    /// </summary>
    /// <param name="column">podana kolumna</param>
    /// <returns></returns>
    protected virtual string TypeDefString(DataColumn column)
    {
      DbDataType sqlDataType = GetDataType(column);
      if (sqlDataType != null)
        return " " + DataTypeToString(sqlDataType);
      else
        throw new DataException(String.Format("Can't define type for column {0}", column.ColumnName));
    }

    /// <summary>
    /// Zwraca typ danych kolumny.
    /// </summary>
    /// <param name="column">podana kolumna</param>
    /// <returns></returns>
    protected virtual DbDataType GetDataType(DataColumn column)
    {
      if (column.DataType == typeof(System.Int32))
        return new DbDataType { SqlDbType=SqlDbType.Int };
      if (column.DataType == typeof(System.Int64))
        return new DbDataType { SqlDbType=SqlDbType.BigInt };
      if (column.DataType == typeof(System.Int16))
        return new DbDataType { SqlDbType=SqlDbType.SmallInt };
      if (column.DataType == typeof(System.String))
      {
        if (column.ExtendedProperties["Unlimited"] != null)
          return new DbDataType { SqlDbType=SqlDbType.NText };
        if (column.MaxLength == Int32.MaxValue)
          return new DbDataType { SqlDbType=SqlDbType.NText, ExtraInfo="Max" };
        if (column.MaxLength > 0)
          return new DbDataType { SqlDbType=SqlDbType.NText, ExtraInfo=column.MaxLength };
      }
      if (column.DataType == typeof(System.Single))
        return new DbDataType { SqlDbType=SqlDbType.Real };
      if (column.DataType == typeof(System.Double))
        return new DbDataType { SqlDbType=SqlDbType.Float };
      if (column.DataType == typeof(System.Decimal))
        return new DbDataType { SqlDbType=SqlDbType.Decimal };
      if (column.DataType == typeof(System.DateTime))
        return new DbDataType { SqlDbType=SqlDbType.DateTime };
      if (column.DataType == typeof(System.Guid))
        return new DbDataType { SqlDbType=SqlDbType.UniqueIdentifier };
      return null;
    }

    /// <summary>
    /// Podaje łańcuch dla określonego typu danych
    /// </summary>
    /// <param name="dataType">podany typ danych</param>
    /// <returns></returns>
    protected abstract string DataTypeToString(DbDataType dataType);

    /// <summary>
    /// Sprawdza, czy podana kolumna tworzy (samodzielnie) klucz główny
    /// </summary>
    /// <param name="column">sprawdzana kolumna</param>
    /// <returns></returns>
    protected virtual bool IsPrimaryKey(DataColumn column)
    {
      DataTable table = column.Table;
      return table.PrimaryKey != null 
        && table.PrimaryKey.Length==1
        && table.PrimaryKey.Contains(column);
    }

    /// <summary>
    /// Sprawdza, czy podana kolumna tworzy (samodzielnie) klucz obcy
    /// </summary>
    /// <param name="column">sprawdzana kolumna</param>
    /// <param name="constraint">zwracane zastrzeżenie klucza obcego</param>
    /// <returns></returns>
    protected virtual bool IsForeignKey(DataColumn column, out ForeignKeyConstraint constraint)
    {
      constraint = null;
      DataTable table = column.Table;
      foreach (Constraint _constraint in table.Constraints)
      {
        if (_constraint is ForeignKeyConstraint)
        {
          ForeignKeyConstraint ForeignKey = (ForeignKeyConstraint)_constraint;
          if (ForeignKey.Columns != null
            && ForeignKey.Columns.Length == 1
            && ForeignKey.Columns.Contains(column))
          {
            constraint = ForeignKey;
            return true;
          }
        }
      }
      return false;
    }

    /// <summary>
    /// Podaje łańcuch ograniczenia dla kolumny
    /// </summary>
    /// <param name="column">sprawdzana kolumna</param>
    /// <returns></returns>
    protected abstract string ColumnConstraintString(DataColumn column);

    /// <summary>
    /// Podaje łańcuch ograniczenia
    /// </summary>
    /// <param name="constraint">sprawdzana kolumna</param>
    /// <param name="isTableConstraint">czy zatrzeżenie dotyczy całej tabeli</param>
    /// <returns></returns>
    protected abstract string ConstraintString(Constraint constraint, bool isTableConstraint);

    /// <summary>
    /// Domyślna zasada reakcji na zmianę/usunięcie
    /// </summary>
    protected virtual Rule DefaultChangeRule
    {
      get { return Rule.Cascade; }
    }

    /// <summary>
    /// Podaje łańcuch dla zasady reakcji na zmianę/usunięcie
    /// </summary>
    /// <param name="rule"></param>
    /// <returns></returns>
    protected abstract string RuleString(Rule rule);

    /// <summary>
    /// Podaje odpowiedni łańcuch dla kolumny automatycznie inkrementowanej
    /// </summary>
    /// <param name="column">sprawdzana kolumna</param>
    /// <returns></returns>
    protected abstract string IdentityString(DataColumn column);

    /// <summary>
    /// Podaje odpowiedni łańcuch, gdy kolumna nie akceptuje pustych wartości
    /// </summary>
    /// <param name="column">sprawdzana kolumna</param>
    /// <returns></returns>
    protected abstract string NullConstraintString(DataColumn column);

    /// <summary>
    /// Odpowiednio formatuje nazwę, która zawiera spacje.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public abstract string EncapsulateName(string name);

  }
}
