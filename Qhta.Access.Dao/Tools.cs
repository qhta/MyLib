using System.Diagnostics;
using System.Runtime.InteropServices;

using Microsoft.Office.Interop.Access.Dao;

using DAO = Microsoft.Office.Interop.Access.Dao;

namespace Qhta.Access.Dao;

public static class Tools
{
  public static void KillMsAccess()
  {
    foreach (var process in Process.GetProcessesByName("MSACCESS"))
    {
      process.Kill();
    }
  }

  public static void CreateEnumLookupTable(Database database, string tableName, Type enumType)
  {
    if (!enumType.IsEnum)
    {
      throw new ArgumentException("Type must be an enum", nameof(enumType));
    }

    TableDef tableDef = null!;
    Recordset recordset = null!;

    try
    {
      tableDef = CreateLookupTable(database, tableName);
      foreach (var value in Enum.GetValues(enumType))
      {
        recordset = database.OpenRecordset(tableName);
        recordset.AddNew();
        byte n = (byte)Convert.ChangeType(value, typeof(byte));
        recordset.Fields["Id"].Value = n;
        recordset.Fields["Name"].Value = value.ToString();
        recordset.Update();
        recordset.Close();
      }
    }
    catch (COMException comEx)
    {
      // Handle the COM exception, e.g., log it or show a message to the user.
      Debug.WriteLine($"COMException occurred: {comEx.Message}");
    }
    finally
    {
      // Ensure COM objects are released even if an exception occurs.
      if (recordset != null) Marshal.ReleaseComObject(recordset);
      /*if (tableDef != null)*/
      Marshal.ReleaseComObject(tableDef);

      // For good measure, force a garbage collection.
      GC.Collect();
      GC.WaitForPendingFinalizers();
    }
  }

  public static TableDef CreateLookupTable(Database database, string tableName)
  {
    TableDef tableDef = null!;
    Field idField = null!;
    Field nameField = null!;
    DAO.Index pkIndex = null!;
    try
    {
      tableDef = database.CreateTableDef(tableName);
      idField = tableDef.CreateField("Id", DataTypeEnum.dbByte);
      tableDef.Fields.Append(idField);
      nameField = tableDef.CreateField("Name", DataTypeEnum.dbText, 25);
      tableDef.Fields.Append(nameField);
      database.TableDefs.Append(tableDef);
      // Create a primary key index
      pkIndex = tableDef.CreateIndex("PrimaryKey");
      pkIndex.Primary = true; // Set as primary key
      pkIndex.Unique = true; // Ensure unique values

      // Add the ID field to the index
      Field indexField = pkIndex.CreateField("Id");
      (pkIndex.Fields as IndexFields)?.Append(indexField);

      // Append the Index to the TableDef
      tableDef.Indexes.Append(pkIndex);
    }
    catch (COMException comEx)
    {
      // Handle the COM exception, e.g., log it or show a message to the user.
      Debug.WriteLine($"COMException occurred: {comEx.Message}");
    }
    finally
    {
      // Ensure COM objects are released even if an exception occurs.
      if (pkIndex != null) Marshal.ReleaseComObject(pkIndex);
      if (nameField != null) Marshal.ReleaseComObject(nameField);
      if (idField != null) Marshal.ReleaseComObject(idField);
      if (tableDef != null) Marshal.ReleaseComObject(tableDef);

      // For good measure, force a garbage collection.
      GC.Collect();
      GC.WaitForPendingFinalizers();
    }
    return database.TableDefs[tableName];
  }

  public static void SetQuery(Database database, string queryName, string sqlText)
  {
    QueryDef query = null!;
    try
    {
      query = database.CreateQueryDef(queryName, sqlText);
    }
    catch (COMException comEx)
    {
      // Handle the COM exception, e.g., log it or show a message to the user.
      Debug.WriteLine($"COMException occurred: {comEx.Message}");
    }
    finally
    {
      // Ensure COM objects are released even if an exception occurs.
      if (query != null) Marshal.ReleaseComObject(query);

      // For good measure, force a garbage collection.
      GC.Collect();
      GC.WaitForPendingFinalizers();
    }
  }

  public static void SetLookup(Database database, string tableName, string fieldName, string lookupTableName)
  {
    var field = database.TableDefs[tableName].Fields[fieldName];
    SetProperty(field, "DisplayControl", DataTypeEnum.dbInteger, 111); // acComboBox
    SetProperty(field, "RowSourceType", DataTypeEnum.dbText, "Table/Query");
    SetProperty(field, "RowSource", DataTypeEnum.dbText, lookupTableName);
    SetProperty(field, "ColumnCount", DataTypeEnum.dbInteger, 2);
    SetProperty(field, "ColumnWidths", DataTypeEnum.dbText, "0");
  }

  public static void SetProperty(Field field, string propertyName, DataTypeEnum dataType, object value)
  {
    Property prop = null!;

    try
    {
      // Try to access the property if it already exists
      prop = field.Properties[propertyName];
      prop.Value = value;
    }
    catch (System.Runtime.InteropServices.COMException)
    {
      Property newProp = null!;
      try
      {
        // If the property does not exist, create and append it
        newProp = field.CreateProperty(propertyName, dataType, value);
        field.Properties.Append(newProp);
      }
      catch (COMException comEx)
      {
        // Handle the COM exception, e.g., log it or show a message to the user.
        Debug.WriteLine($"COMException occurred: {comEx.Message}");
      }
      finally
      {
        // Ensure COM objects are released even if an exception occurs.
        if (newProp != null) Marshal.ReleaseComObject(newProp);
      }

    }
    finally
    {
      // Ensure COM objects are released even if an exception occurs.
      if (prop != null) Marshal.ReleaseComObject(prop);

      // For good measure, force a garbage collection.
      GC.Collect();
      GC.WaitForPendingFinalizers();
    }
  }
}
