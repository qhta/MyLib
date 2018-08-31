using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Common;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Klasa oferująca metody pomocnicze dla baz danych
  /// </summary>
  public static class DbUtilities
  {
    #region wyliczenie informacji
    /// <summary>
    /// Wyliczenie informacji o silnikach baz danych
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<DbEngineClass> EnumerateEngineClasses()
    {
      List<DbEngineClass> result = new List<DbEngineClass>();
      Assembly assembly = Assembly.GetCallingAssembly();
      AssemblyName[] refAssemblies = assembly.GetReferencedAssemblies();
      foreach (AssemblyName aName in refAssemblies)
      {
        var refAssembly = Assembly.Load(aName);
        foreach (Type aType in refAssembly.ExportedTypes)
          if (aType.BaseType==typeof(DbEngine))
          {
            string id = aType.Name.Replace("Engine", "").ToUpper();
            string name = aType.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? aType.Name;
            string description = aType.GetCustomAttribute<DescriptionAttribute>()?.Description;
            result.Add(new DbEngineClass
            {
              ID = id,
              Name = name,
              Description = description,
              AssemblyQualifiedName = aType.AssemblyQualifiedName,
              Type = aType,
            });
          }
      }
      return result;
    }

    private static Dictionary<Type, DbEngine> engines = new Dictionary<Type, DbEngine>();
    /// <summary>
    /// Podanie silnika danych
    /// </summary>
    /// <returns></returns>
    public static DbEngine GetEngine(DbEngineClass aClass)
    {
      DbEngine result;
      if (!engines.TryGetValue(aClass.Type, out result))
      {
        result = (DbEngine)aClass.Type.GetConstructor(new Type[0]).Invoke(new object[0]);
        engines.Add(aClass.Type, result);
      }
      return result;
    }

    /// <summary>
    /// Konwersja łańcucha na boolean
    /// </summary>
    /// <param name="str"></param>
    /// <returns></returns>
    public static bool StringToBool(this string str)
    {
      switch (str.ToLower())
      {
        case "true":
        case "yes":
          return true;
        case "false":
        case "no":
          return false;
      }
      throw new InvalidCastException($"Cannot convert \"{str}\" to boolean value");
    }

    /// <summary>
    /// Wyszukanie dostawców OleDb
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<DbProviderInfo> EnumerateOleDbProviders()
    {
      List<DbProviderInfo> result = new List<DbProviderInfo>();
      OleDbEnumerator enumerator = new OleDbEnumerator();
      if (enumerator != null)
      {
        DataTable aTable = enumerator.GetElements();
        foreach (DataRow aRow in aTable.Rows)
          if (aRow.Field<int>(aTable.Columns["SOURCES_TYPE"]) == 1)
          {
            string name = aRow.Field<string>(aTable.Columns["SOURCES_NAME"]);
            DbProviderInfo info;
            result.Add(info = new DbProviderInfo
            {
              Name = name,
              Description = aRow.Field<string>(aTable.Columns["SOURCES_DESCRIPTION"]),
              Kind = ProviderKind.OleDb,
              ClsID = aRow.Field<string>(aTable.Columns["SOURCES_CLSID"]),
            });
            if (info.Name.Contains(".ACE."))
            {
              info.Type = "ACCESS";
              info.FileExtensions = "*.accdb, *.mdb";
            }
            else if (info.Description.Contains(" Jet ") || info.Description.Contains(" Access "))
            {
              info.Type = "ACCESS";
              info.FileExtensions = "*.mdb, *.accdb";
            }
            else if (info.Description.Contains("SQL Server"))
            {
              info.Type = "MSSQL";
              info.FileExtensions = "*.mdf";
            }
          }
      }
      return result.OrderBy(item => item.Name);
    }

    /// <summary>
    /// Wyszukanie sterowników Odbc
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<DbProviderInfo> EnumerateOdbcDrivers()
    {
      List<DbProviderInfo> result = new List<DbProviderInfo>();
      // MSDAENUM = Microsoft Data Access - OLE DB Root Enumerator Stub
      // MSDASQL Enumerator = Microsoft OLE DB Enumerator for ODBC Drivers
      // SQLNCLI Enumerator = Microsoft SQL Server Native Client Enumerator
      // SQLOLEDB Enumerator = Microsoft OLE DB Provider for SQL Server 
      OleDbDataReader reader =
        OleDbEnumerator.GetEnumerator(Type.GetTypeFromProgID("MSDASQL Enumerator"));
      DataTable aTable = GetOleDbProviderData(reader);
      foreach (DataRow aRow in aTable.Rows)
      {
        if (aRow.Field<int>(aTable.Columns["SOURCES_TYPE"]) == 1)
        {
          string description = aRow.Field<string>(aTable.Columns["SOURCES_DESCRIPTION"]);
          string extensions = null;
          int k = description.IndexOf('(');
          if (k > 0)
          {
            extensions = description.Substring(k).Trim();
            extensions = extensions.Substring(1, extensions.Length - 2).Trim();
            description = description.Substring(0, k - 1).Trim();
          }
          DbProviderInfo info = new DbProviderInfo { Kind = ProviderKind.Odbc };
          description = description.ToUpper();
          if (description.Contains("DBASE"))
            info.Type = "DBASE";
          else if (description.Contains("ACCESS"))
            info.Type = "ACCESS";
          else if (description.Contains("EXCEL"))
            info.Type = "EXCEL";
          info.Name = aRow.Field<string>(aTable.Columns["SOURCES_NAME"]);
          info.Description = aRow.Field<string>(aTable.Columns["SOURCES_DESCRIPTION"]);
          info.FileExtensions = extensions;
          result.Add(info);
        }
      }
      return result;
    }

    /// <summary>
    /// Odczytanie danych z OleDb do tabeli
    /// </summary>
    /// <param name="reader"></param>
    /// <returns></returns>
    static DataTable GetOleDbProviderData(OleDbDataReader reader)
    {
      DataTable result = new DataTable();
      for (int i = 0; i < reader.FieldCount; i++)
        result.Columns.Add(new DataColumn(reader.GetName(i), reader.GetFieldType(i)));
      while (reader.Read())
      {
        object[] row = new object[reader.FieldCount];
        for (int i = 0; i < reader.FieldCount; i++)
          row[i] = reader.GetValue(i);
        result.LoadDataRow(row, true);
      }
      return result;
    }

    /// <summary>
    /// Sprawdzenie, czy źródło danych reprezentuje serwer lokalny
    /// </summary>
    /// <param name="dataSource"></param>
    /// <returns></returns>
    public static bool IsLocalDataSource(this string dataSource)
    {
      return dataSource.StartsWith("(local)") || dataSource.StartsWith(".") || dataSource.StartsWith(Environment.MachineName);
    }

    ///// <summary>
    ///// Wyliczenie zainstalowanych dostawców danych SQL CE
    ///// </summary>
    ///// <returns></returns>
    //public static IEnumerable<DbProviderInfo> EnumerateSqlProviders(DbServerInfo info)
    //{
    //  List<DbProviderInfo> result = new List<DbProviderInfo>();
    //  string name = info.InstanceName;
    //  if (String.IsNullOrEmpty(name))
    //    name = "SQLSERVER";
    //  if (info.ServerName != null)
    //  {
    //    if (info.ServerName!="(local)")
    //      name += " on " + info.ServerName;
    //  }
    //  string description = "Microsoft SQL Server";
    //  string version = info.Version;
    //  if (version != null)
    //  {
    //    string[] ss = version.Split('.');
    //    if (ss.Length > 2)
    //      version = ss[0] + '.' + ss[1];
    //    description += " v." + version;
    //  }
    //  result.Add(new DbProviderInfo
    //  {
    //    ShortName = name,
    //    FullName = description,
    //    Version = info.Version,
    //    Engine = DbEngineKind.MSSQL,
    //    Kind = ProviderKind.Embedded,
    //    FileExtensions = "*.mdf"
    //  });
    //  return result;
    //}
    ///// <summary>
    ///// Wyliczenie zainstalowanych dostawców danych SQL CE
    ///// </summary>
    ///// <returns></returns>
    //public static IEnumerable<DbProviderInfo> EnumerateSqlCeProviders()
    //{
    //  List<DbProviderInfo> result = new List<DbProviderInfo>();
    //  // Klasa SSCE.EngineClass musi być użyta, aby jej asemblat był widoczny przez referencję.
    //  // Moduł SSCE musi mieć we właściwościach EmbedInteropTypes = false oraz Isolated = true.
    //  // SSCE.Engine engine = new SSCE.Engine();
    //  Assembly assembly = Assembly.GetExecutingAssembly();
    //  AssemblyName[] refAssemblies = assembly.GetReferencedAssemblies();
    //  foreach (AssemblyName aName in refAssemblies)
    //    if (aName.Name.Contains("SqlServerCe") || aName.Name.Contains("SSCE"))
    //      result.Add(new DbProviderInfo
    //      {
    //        ShortName = aName.Name,
    //        FullName = "Microsoft SQL Server Compact Edition v."+aName.Version.ToString(2),
    //        Version = aName.Version.ToString(4),
    //        Engine = DbEngineKind.SQLCE,
    //        Kind = ProviderKind.Embedded,
    //        FileExtensions = "*.sdf"
    //      });
    //  return result;
    //}

    ///// <summary>
    ///// Wyliczenie zainstalowanych dostawców danych SQL CE
    ///// </summary>
    ///// <returns></returns>
    //public static IEnumerable<DbProviderInfo> EnumerateSQLiteProviders()
    //{
    //  List<DbProviderInfo> result = new List<DbProviderInfo>();

    //  Assembly assembly = Assembly.GetExecutingAssembly();
    //  AssemblyName[] refAssemblies = assembly.GetReferencedAssemblies();
    //  foreach (AssemblyName aName in refAssemblies)
    //    if (aName.Name.Contains("SQLite"))
    //    {
    //      Assembly dll = Assembly.Load(aName);
    //      object[] attributes =
    //          dll.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
    //      string description = null;
    //      if (attributes.Length > 0)
    //      {
    //        AssemblyDescriptionAttribute descriptionAttribute =
    //            (AssemblyDescriptionAttribute)attributes[0];
    //        description = descriptionAttribute.Description;
    //      }
    //      /*
    //      string version = dll.ImageRuntimeVersion;
    //      /*
    //      if (version!=null)
    //      {
    //        string[] ss = version.Split('.');
    //        if (ss.Length>2)
    //          version = ss[0]+'.'+ss[1];
    //      }
    //       */
    //      result.Add(new DbProviderInfo
    //      {
    //        ShortName = aName.Name,
    //        FullName = description,//  String.Format("ADO.NET {0} Provider for SQLite v.{1}", version, aName.Version.ToString(2)),
    //        Version = aName.Version.ToString(4),
    //        Engine = DbEngineKind.SQLITE,
    //        Kind = ProviderKind.Embedded,
    //        FileExtensions = "*.db3"
    //      });
    //    }
    //  return result;
    //}

    ///// <summary>
    ///// Wyliczenie zainstalowanych dostawców dla wybranego silnika bazy danych
    ///// </summary>
    ///// <param name="engine">wybrany silnik bazy danych</param>
    ///// <returns></returns>
    //public static IEnumerable<DbProvider> EnumerateDbProviders(DbEngineKind engine)
    //{
    //  List<DbProvider> result = new List<DbProvider>();
    //  if (engine == DbEngineKind.MSSQL)
    //  {
    //    foreach (DbServerInfo ds in GetSqlDataSources(true))
    //      foreach (DbProviderInfo info in EnumerateSqlProviders(ds))
    //      {
    //        result.Add(new DbProvider(info, ds));
    //      }
    //  }
    //  else if (engine == DbEngineKind.SQLCE)
    //  {
    //    foreach (DbProviderInfo info in EnumerateSqlCeProviders())
    //      result.Add(new DbProvider(info));
    //  }
    //  else if (engine == DbEngineKind.SQLITE)
    //  {
    //    foreach (DbProviderInfo info in EnumerateSQLiteProviders())
    //      result.Add(new DbProvider(info));
    //  }
    //  else
    //  {
    //    foreach (DbProviderInfo info in EnumerateOleDbProviders())
    //      if (info.Engine == engine)
    //        result.Add(new DbProvider(info));
    //  }
    //  return result;
    //}
    #endregion

    //#region tworzenie tabeli
    ///// <summary>
    ///// Tworzenie tabeli
    ///// </summary>
    ///// <param name="info">informacje o bazie danych</param>
    ///// <param name="table">definicja tabeli</param>
    //public static void CreateTable(DbInfo info, DataTable table)
    //{
    //  using (DbCommand command = info.ProviderFactory.CreateCommand())
    //  {
    //    command.Connection = info.Connection;
    //    command.CommandText = info.TableBuilder.CreateTableString(table);
    //    Console.WriteLine(command.CommandText);
    //    command.ExecuteNonQuery();
    //  }
    //}

    //#endregion

    //#region pobieranie tabeli
    ///// <summary>
    ///// Pobieranie tabeli
    ///// </summary>
    ///// <param name="info">informacje o bazie danych</param>
    ///// <param name="tableName">nazwa tabeli</param>
    //public static DataTable GetTable(DbInfo info, string tableName)
    //{
    //  DataTable table = new DataTable(tableName);
    //  using (DbCommand command = info.ProviderFactory.CreateCommand())
    //  {
    //    command.Connection = info.Connection;
    //    command.CommandText =
    //      String.Format(
    //      "SELECT * FROM [{0}]",
    //      table.TableName);
    //    DbDataAdapter adapter = info.ProviderFactory.CreateDataAdapter();
    //    adapter.TableMappings.Add(table.TableName, table.TableName);
    //    adapter.SelectCommand = command;
    //    adapter.Fill(table);
    //    Console.WriteLine("Columns:");
    //    foreach (DataColumn aCol in table.Columns)
    //      Console.WriteLine(aCol.ColumnName + ": "+aCol.DataType);
    //    Console.WriteLine("PrimaryKey:");
    //    foreach (DataColumn aCol in table.PrimaryKey)
    //      Console.Write(aCol.ColumnName + " ");
    //    Console.WriteLine("");
    //    Console.WriteLine("Loaded rows:");
    //    foreach (DataRow aRow in table.Rows)
    //    {
    //      foreach (DataColumn aCol in table.Columns)
    //        Console.Write(aCol.ColumnName + ": " + aRow.Field<object>(aCol.ColumnName)+" ");
    //      Console.WriteLine("");
    //    }
    //    return table;
    //  }
    //}
    //#endregion

    //#region aktualizacja tabeli w bazie danych
    ///// <summary>
    ///// Aktualizacja tabeli w bazie danych
    ///// </summary>
    ///// <param name="info">informacje o bazie danych</param>
    ///// <param name="table">tabela</param>
    //public static void UpdateTable(DbInfo info, DataTable table)
    //{
    //  using (DbDataAdapter adapter = info.ProviderFactory.CreateDataAdapter())
    //  using (DbCommand selectCommand = info.ProviderFactory.CreateCommand())
    //  {
    //    selectCommand.Connection = info.Connection;
    //    selectCommand.CommandText =
    //      String.Format(
    //      "SELECT * FROM [{0}]",
    //      table.TableName);
    //    adapter.SelectCommand = selectCommand;
    //    DbCommandBuilder cb = info.ProviderFactory.CreateCommandBuilder();
    //    cb.DataAdapter = adapter;
    //    using (adapter.UpdateCommand = cb.GetUpdateCommand(true))
    //    using (adapter.InsertCommand = cb.GetInsertCommand(true))
    //    using (adapter.DeleteCommand = cb.GetDeleteCommand(true))
    //    {
    //      Console.WriteLine(adapter.UpdateCommand.CommandText);
    //      Console.WriteLine();
    //      Console.WriteLine(adapter.InsertCommand.CommandText);
    //      Console.WriteLine();
    //      Console.WriteLine(adapter.DeleteCommand.CommandText);
    //      Console.WriteLine();
    //      adapter.Update(table);
    //    }
    //  }
    //}

    //#endregion

  }

}



