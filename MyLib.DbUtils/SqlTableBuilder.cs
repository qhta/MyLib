using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using SMO = Microsoft.SqlServer.Management.Smo;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Klasa tworząca komendę SQL do tworzenia tabeli w bazie danych
  /// SQL Serwera na podstawie tablicy <c>DataTable</c>
  /// </summary>
  public class SqlTableBuilder
  {
    /// <summary>
    /// Wejściowa tablica danych
    /// </summary>
    public DataTable DataTable 
    { 
      get; 
      private set; 
    }

    /// <summary>
    /// Procedura budująca łańcuch komendy SQL "CREATE TABLE"
    /// </summary>
    /// <param name="dataTable"></param>
    /// <returns></returns>
    public string BuildCreateTable(DataTable dataTable)
    {
      DataTable = dataTable;
      return BuildCreateTable();
    }


    /// <summary>
    /// Właściwa procedura budująca łańcuch komendy SQL "CREATE TABLE"
    /// </summary>
    /// <returns></returns>
    private string BuildCreateTable()
    {
      string result = null;
      foreach (DataColumn column in DataTable.Columns)
      {
        if (result != null)
          result += ", ";
        result += FormatColumnString(column);
      }
      foreach (Constraint constraint in DataTable.Constraints)
      {
        string str = BuildConstraintString(constraint, true);
        if (str != null)
        {
          if (result != null)
            result += ", ";
          result += str;
        }
      }
      return String.Format(
        "CREATE TABLE {0} ({1})",
        EncapsulateName(DataTable.TableName), result);
    }

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
      SMO.DataType sqlDataType = GetDataType(column);
      if (sqlDataType != null)
        return " " + DataTypeToString(sqlDataType);
      else
        throw new DataException(String.Format("Can't define type for column {0}", column.ColumnName));
    }

    /// <summary>
    /// Zwraca typ danych SQL z podanej kolumny danych w określonym kontekście
    /// </summary>
    /// <param name="column">podana kolumna</param>
    /// <returns></returns>
    protected virtual SMO.DataType GetDataType(DataColumn column)
    {
      if (column.DataType == typeof(System.Int32))
        return SMO.DataType.Int;
      if (column.DataType == typeof(System.Int64))
        return SMO.DataType.BigInt;
      if (column.DataType == typeof(System.Int16))
        return SMO.DataType.SmallInt;
      if (column.DataType == typeof(System.String))
      {
        if (column.ExtendedProperties["Unlimited"] != null)
          return SMO.DataType.NText;
        if (column.MaxLength == Int32.MaxValue)
          return SMO.DataType.NVarChar(MaxStringLength);
        if (column.MaxLength > 0)
          return SMO.DataType.NVarChar(column.MaxLength);
      }
      if (column.DataType == typeof(System.Single))
        return SMO.DataType.Real;
      if (column.DataType == typeof(System.Double))
        return SMO.DataType.Float;
      if (column.DataType == typeof(System.Decimal))
        return SMO.DataType.Decimal(2, 18);
      if (column.DataType == typeof(System.DateTime))
        return SMO.DataType.DateTime;
      if (column.DataType == typeof(System.Guid))
        return SMO.DataType.UniqueIdentifier;
      return null;
    }

    /// <summary>
    /// Podaje maksymalną długość łańcucha w określonym kontekście
    /// </summary>
    /// <returns></returns>
    protected virtual int MaxStringLength
    {
      get { return 4000;}
    }

    /// <summary>
    /// Podaje łańcuch dla typu danych SQL serwera
    /// </summary>
    /// <param name="dataType">podany typ danych</param>
    /// <returns></returns>
    protected virtual string DataTypeToString(SMO.DataType dataType)
    {

      string result = dataType.Name;
      string[] ss = result.Split('.');
      if (ss.Length >= 2)
        result = ss[1];
      if (dataType.MaximumLength > 0)
        result += String.Format(" ({0})", dataType.MaximumLength);
      return result;
    }


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
    /// Podaje łańcuch " PRIMARY KEY", gdy podana kolumna tworzy klucz główny,
    /// a " UNIQUE", gdy jest to klucz unikatowy
    /// </summary>
    /// <param name="column">sprawdzana kolumna</param>
    /// <returns></returns>
    protected virtual string ColumnConstraintString(DataColumn column)
    {
      string result = null;
      if (IsPrimaryKey(column))
        result = " PRIMARY KEY";
      else if (column.Unique)
        result = " UNIQUE";
      ForeignKeyConstraint FK = null;
      if (IsForeignKey(column, out FK))
        result += BuildConstraintString(FK, false);
      return result;
    }

    /// <summary>
    /// Podaje łańcuch " PRIMARY KEY", gdy podana kolumna tworzy klucz główny,
    /// a " UNIQUE", gdy jest to klucz unikatowy
    /// </summary>
    /// <param name="constraint">sprawdzana kolumna</param>
    /// <param name="isTableConstraint">czy zatrzeżenie dotyczy całej tabeli</param>
    /// <returns></returns>
    protected virtual string BuildConstraintString(Constraint constraint, bool isTableConstraint)
    {
      string result = null;
      //if (constraint.ConstraintName != null)
      //  result += " CONSTRAINT " + constraint.ConstraintName;
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
            result += " ON DELETE" + ChangeRuleToString(fcs.DeleteRule);
          if (fcs.UpdateRule != DefaultChangeRule)
            result += " ON UPDATE" + ChangeRuleToString(fcs.UpdateRule);
        }
      }
      return result;
    }

    /// <summary>
    /// Domyślna zasada reakcji na zmianę/usunięcie
    /// </summary>
    protected virtual Rule DefaultChangeRule
    {
      get { return Rule.Cascade; }
    }

    /// <summary>
    /// Zmiana podanej zasady reakcji na zmianę/usunięcie na łańcuch
    /// </summary>
    /// <param name="rule"></param>
    /// <returns></returns>
    protected virtual string ChangeRuleToString(Rule rule)
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
    protected virtual string IdentityString(DataColumn column)
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
    protected virtual string NullConstraintString(DataColumn column)
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
    public virtual string EncapsulateName(string name)
    {
      if (name.Contains(" "))
      {
        if (RemoveSpaces)
          name = name.Replace(" ", "");
        else
          name = "[" + name + "]";
      }
      return name;
    }

    /// <summary>
    /// Żądanie
    /// </summary>
    public bool RemoveSpaces
    {
      get;
      set;
    }
  }
}
