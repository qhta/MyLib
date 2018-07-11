using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.Sql;
using System.Data.SqlTypes;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Data.SqlClient;
#if SQLCE
using System.Data.SqlServerCe;
#endif
using System.Collections.Specialized;
using System.Diagnostics;
using IO = System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.SqlServer;
using Microsoft.SqlServer.Server;
//using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;
using System.Runtime.Serialization;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Klasa oferująca metody pomocnicze dla połączeń baz danych
  /// </summary>
  public static class DbUtils
  {
#region wyliczenie informacji o fabrykach, źródłach danych i dostawcach danych
    /// <summary>
    /// Wyliczenie informacji o fabrykach typu <c>DbProviderFactory</c> z <c>DbProviderFactories</c>
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<FactoryInfo> EnumerateFactoryClasses()
    {
      List<FactoryInfo> result = new List<FactoryInfo>();
      DataTable aTable = DbProviderFactories.GetFactoryClasses();
      foreach (DataRow aRow in aTable.Rows)
        result.Add(new FactoryInfo
        {
          Name = aRow.Field<string>(aTable.Columns["Name"]),
          Description = aRow.Field<string>(aTable.Columns["Description"]),
          InvariantName = aRow.Field<string>(aTable.Columns["InvariantName"]),
          AssemblyQualifiedName = aRow.Field<string>(aTable.Columns["AssemblyQualifiedName"])
        });
      return result;
    }

    /// <summary>
    /// Wyszukanie źródeł danych dla określonej fabryki typu <c>DbProviderFactory</c>
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SqlDataSourceInfo> EnumerateDataSources(DbProviderFactory factory)
    {
      //Debug.WriteLine("Start Enumerate Data Sources" + DateTime.Now.TimeOfDay);

      List<SqlDataSourceInfo> result = new List<SqlDataSourceInfo>();
      DbDataSourceEnumerator enumerator = factory.CreateDataSourceEnumerator();
      if (enumerator != null)
      {
        DataTable aTable = enumerator.GetDataSources();
        foreach (DataRow aRow in aTable.Rows)
        {
          String serverName = aRow.Field<string>(aTable.Columns["ServerName"]);
          result.Add(new SqlDataSourceInfo
          {
            ServerName = serverName,
            InstanceName = aRow.Field<string>(aTable.Columns["InstanceName"]),
            IsClustered = aRow.Field<string>(aTable.Columns["IsClustered"]),
            Version = aRow.Field<string>(aTable.Columns["Version"]),
          });
        }
      }
      //Debug.WriteLine("End Enumerate Data Sources" + DateTime.Now.TimeOfDay);

      return result;
    }

    /// <summary>
    /// Wyszukanie źródeł danych dla klienta SQL Server
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SqlDataSourceInfo> EnumerateSqlDataSources()
    {
      SqlDataSources = EnumerateDataSources(SqlClientFactory.Instance);
      return SqlDataSources;
    }

    /// <summary>
    /// Przechowywany wynik funkcji EnumerateSqlDataSources
    /// </summary>
    public static IEnumerable<SqlDataSourceInfo> SqlDataSources;

    /// <summary>
    /// Pobranie źródeł danych dla klienta SQL Server wyszukanych poprzednio,
    /// a jeśli nie ma takiej listy, to wyszukanie na bieżąco.
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<SqlDataSourceInfo> GetSqlDataSources()
    {
      if (SqlDataSources != null)
        return SqlDataSources;
      return EnumerateSqlDataSources();
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
              ShortName = name,
              FullName = aRow.Field<string>(aTable.Columns["SOURCES_DESCRIPTION"]),
              Kind = ProviderKind.OleDb,
              ClsID = aRow.Field<string>(aTable.Columns["SOURCES_CLSID"]),
            });
            if (info.ShortName.Contains(".ACE."))
            {
              info.Engine = DbEngine.ACCESS;
              info.FileExtensions = "*.accdb, *.mdb";
            }
            else if (info.FullName.Contains(" Jet ") || info.FullName.Contains(" Access "))
            {
              info.Engine = DbEngine.ACCESS;
              info.FileExtensions = "*.mdb, *.accdb";
            }
            else if (info.FullName.Contains("SQL Server"))
            {
              info.Engine = DbEngine.MSSQL;
              info.FileExtensions = "*.mdf";
            }
            else if (info.ShortName.Contains("MSOLAP"))
              info.Engine = DbEngine.OLAP;
            else if (info.FullName.Contains("Oracle"))
              info.Engine = DbEngine.ORACLE;
            else if (info.FullName.Contains("Directory Services"))
              info.Engine = DbEngine.ADSI;
            else if (info.FullName.Contains("Simple Provider"))
              info.Engine = DbEngine.OSP;
            else if (info.FullName.Contains("Indexing Service"))
              info.Engine = DbEngine.IDXS;
            else if (info.FullName.EndsWith("Search"))
            {
              info.ShortName = "Search.CollatorDSO";
              info.Engine = DbEngine.WSDS;
            }
          }
      }
      return result.OrderBy(item => item.ShortName);
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
          DbProviderInfo info = new DbProviderInfo { Kind = ProviderKind.Odbc};
          description = description.ToUpper();
          if (description.Contains("DBASE"))
            info.Engine = DbEngine.DBASE;
          else if (description.Contains("ACCESS"))
            info.Engine = DbEngine.ACCESS;
          else if (description.Contains("EXCEL"))
            info.Engine = DbEngine.EXCEL;
          info.ShortName = aRow.Field<string>(aTable.Columns["SOURCES_NAME"]);
          info.FullName = aRow.Field<string>(aTable.Columns["SOURCES_DESCRIPTION"]);
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

    /// <summary>
    /// Wyliczenie zainstalowanych dostawców danych SQL CE
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<DbProviderInfo> EnumerateSqlProviders(SqlDataSourceInfo info)
    {
      List<DbProviderInfo> result = new List<DbProviderInfo>();
      string name = info.InstanceName;
      if (String.IsNullOrEmpty(name))
        name = "SQLSERVER";
      if (info.ServerName != null)
      {
        if (info.ServerName!="(local)")
          name += " on " + info.ServerName;
      }
      string description = "Microsoft SQL Server";
      string version = info.Version;
      if (version != null)
      {
        string[] ss = version.Split('.');
        if (ss.Length > 2)
          version = ss[0] + '.' + ss[1];
        description += " v." + version;
      }
      result.Add(new DbProviderInfo
      {
        ShortName = name,
        FullName = description,
        Version = info.Version,
        Engine = DbEngine.MSSQL,
        Kind = ProviderKind.Embedded,
        FileExtensions = "*.mdf"
      });
    return result;
    }
    /// <summary>
    /// Wyliczenie zainstalowanych dostawców danych SQL CE
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<DbProviderInfo> EnumerateSqlCeProviders()
    {
      List<DbProviderInfo> result = new List<DbProviderInfo>();
      // Klasa SSCE.EngineClass musi być użyta, aby jej asemblat był widoczny przez referencję.
      // Moduł SSCE musi mieć we właściwościach EmbedInteropTypes = false oraz Isolated = true.
      // SSCE.Engine engine = new SSCE.Engine();
      Assembly assembly = Assembly.GetExecutingAssembly();
      AssemblyName[] refAssemblies = assembly.GetReferencedAssemblies();
      foreach (AssemblyName aName in refAssemblies)
        if (aName.Name.Contains("SqlServerCe") || aName.Name.Contains("SSCE"))
          result.Add(new DbProviderInfo
          {
            ShortName = aName.Name,
            FullName = "Microsoft SQL Server Compact Edition v."+aName.Version.ToString(2),
            Version = aName.Version.ToString(4),
            Engine = DbEngine.SQLCE,
            Kind = ProviderKind.Embedded,
            FileExtensions = "*.sdf"
          });
      return result;
    }

    /// <summary>
    /// Wyliczenie zainstalowanych dostawców danych SQL CE
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<DbProviderInfo> EnumerateSQLiteProviders()
    {
      List<DbProviderInfo> result = new List<DbProviderInfo>();

      Assembly assembly = Assembly.GetExecutingAssembly();
      AssemblyName[] refAssemblies = assembly.GetReferencedAssemblies();
      foreach (AssemblyName aName in refAssemblies)
        if (aName.Name.Contains("SQLite"))
        {
          Assembly dll = Assembly.Load(aName);
          object[] attributes = 
              dll.GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
          string description = null;
          if (attributes.Length > 0)
          {
            AssemblyDescriptionAttribute descriptionAttribute =
                (AssemblyDescriptionAttribute)attributes[0];
            description = descriptionAttribute.Description;
          }
          /*
          string version = dll.ImageRuntimeVersion;
          /*
          if (version!=null)
          {
            string[] ss = version.Split('.');
            if (ss.Length>2)
              version = ss[0]+'.'+ss[1];
          }
           */ 
          result.Add(new DbProviderInfo
          {
            ShortName = aName.Name,
            FullName = description,//  String.Format("ADO.NET {0} Provider for SQLite v.{1}", version, aName.Version.ToString(2)),
            Version = aName.Version.ToString(4),
            Engine = DbEngine.SQLITE,
            Kind = ProviderKind.Embedded,
            FileExtensions = "*.db3"
          });
        }
      return result;
    }

    /// <summary>
    /// Wyliczenie zainstalowanych dostawców dla wybranego silnika bazy danych
    /// </summary>
    /// <param name="engine">wybrany silnik bazy danych</param>
    /// <returns></returns>
    public static IEnumerable<DbProvider> EnumerateDbProviders(DbEngine engine)
    {
      List<DbProvider> result = new List<DbProvider>();
      if (engine == DbEngine.MSSQL)
      {
        foreach (SqlDataSourceInfo ds in GetSqlDataSources())
          foreach (DbProviderInfo info in EnumerateSqlProviders(ds))
          {
            result.Add(new DbProvider(info, ds) );
          }
      }
      else if (engine == DbEngine.SQLCE)
      {
        foreach (DbProviderInfo info in EnumerateSqlCeProviders())
          result.Add(new DbProvider(info));
      }
      else if (engine == DbEngine.SQLITE)
      {
        foreach (DbProviderInfo info in EnumerateSQLiteProviders())
          result.Add(new DbProvider(info));
      }
      else
      {
        foreach (DbProviderInfo info in EnumerateOleDbProviders())
          if (info.Engine == engine)
            result.Add(new DbProvider (info));
      }
      return result;
    }
#endregion

#region tworzenie połączenia do bazy danych
    /// <summary>
    /// Tworzenie połączenia do bazy danych
    /// </summary>
    /// <param name="context">kontekst zawierający informacje potrzebne do utworzenia połączenia</param>
    /// <returns></returns>
    public static DbConnection CreateConnection(DbContext context)
    {
      switch (context.DataProvider.Engine)
      { 
        case DbEngine.MSSQL:
          return CreateSqlServerConnection(context);
        case DbEngine.ACCESS:
          return CreateAccessConnection(context);
        case DbEngine.SQLCE:
          return CreateSqlCeConnection(context);
        case DbEngine.SQLITE:
          return CreateSQLiteConnection(context);
      }
      return null;
    }

    /// <summary>
    /// Tworzenie połączenia dla  bazy danych SQL serwera
    /// </summary>
    /// <param name="context">kontekst zawierający informacje potrzebne do utworzenia połączenia</param>
    /// <param name="serverConnection">połączenie tylko do serwera</param>
    /// <returns></returns>
    public static DbConnection CreateSqlServerConnection(DbContext context, bool serverConnection = false)
    {
      DbConnectionStringBuilder sb = new DbConnectionStringBuilder(context.DataProvider.Kind == ProviderKind.Odbc);
      if (context.DataProvider.Kind == ProviderKind.OleDb)
        sb["Provider"] = context.DataProvider.ShortName;
      sb["Data Source"] = context.DataProvider.DataSource;
      if (!serverConnection)
      {
        if (context.DbName != null)
          sb["Database"] = context.DbName;
      }
      if (context.UserID != null)
      {
        sb["User ID"] = context.UserID;
        if (context.Password != null)
          sb["Password"] = context.Password;
      }
      else
        sb["Trusted_Connection"] = "yes";
      context.Connection = context.Factory.CreateConnection();
      context.Connection.ConnectionString = sb.ConnectionString;
      return context.Connection;
    }

    /// <summary>
    /// Tworzenie połączenia dla  bazy danych MS Access
    /// </summary>
    /// <param name="context">kontekst zawierający informacje potrzebne do utworzenia połączenia</param>
    /// <returns></returns>
    public static DbConnection CreateAccessConnection(DbContext context)
    {
      DbConnectionStringBuilder sb = new DbConnectionStringBuilder(context.DataProvider.Kind == ProviderKind.Odbc);
      if (context.DataProvider.Kind == ProviderKind.OleDb)
      {
        sb["Provider"] = context.DataProvider.ShortName;
        if (context.FileName != null)
          sb["Data Source"] = context.FileName;
        if (context.UserID != null)
        {
          sb["User ID"] = context.UserID;
          if (context.Password != null)
            sb["Password"] = context.Password;
        }
      }
      else if (context.DataProvider.Kind == ProviderKind.Odbc)
      {
        sb["Driver"] = context.DataProvider.ShortName;
        if (context.FileName != null)
          sb["Dbq"] = context.FileName;
        if (context.UserID != null)
        {
          sb["UID"] = context.UserID;
          if (context.Password != null)
            sb["PWD"] = context.Password;
        }
      }
      context.Connection = context.Factory.CreateConnection();
      context.Connection.ConnectionString = sb.ConnectionString;
      return context.Connection;
    }

    /// <summary>
    /// Tworzenie połączenia dla  bazy danych SQL Compact Edition
    /// </summary>
    /// <param name="context">kontekst zawierający informacje potrzebne do utworzenia połączenia</param>
    /// <returns></returns>
    public static DbConnection CreateSqlCeConnection(DbContext context)
    {
#if SQLCE
      SqlCeConnectionStringBuilder sb = new SqlCeConnectionStringBuilder(context.ConnectionRest);
      if (context.FileName != null)
        sb["Data Source"] = context.FileName;
      if (context.Password != null)
      {
        sb["Password"] = context.Password;
        if (context.Encrypt)
          sb["Encrypt"] = true;
      }
      context.Connection = context.Factory.CreateConnection();
      context.Connection.ConnectionString = sb.ConnectionString;
      return context.Connection;
#else
      throw new NotImplementedException("SQLCE engine is not compatible with this version of DBUtils");
#endif
    }

    /// <summary>
    /// Tworzenie połączenia dla  bazy danych SQLite
    /// </summary>
    /// <param name="context">kontekst zawierający informacje potrzebne do utworzenia połączenia</param>
    /// <returns></returns>
    public static DbConnection CreateSQLiteConnection(DbContext context)
    {
#if SQLITE
      SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder(context.ConnectionRest);
      if (context.FileName != null)
        sb["Data Source"] = context.FileName;
      if (context.Password != null)
      {
        sb["Password"] = context.Password;
        if (context.Encrypt)
          sb["Encrypt"] = true;
      }
      context.Connection = context.Factory.CreateConnection();
      context.Connection.ConnectionString = sb.ConnectionString;
      return context.Connection;
#else
      throw new NotImplementedException("SQLite engine is not compatible with this version of DBUtils");
#endif

    }
#endregion

#region tworzenie bazy danych
    /// <summary>
    /// Tworzenie bazy danych
    /// </summary>
    /// <param name="context">kontekst zawierający informacje potrzebne do utworzenia bazy danych</param>
    public static void CreateDatabase(DbContext context)
    {
      switch (context.DataProvider.Engine)
      { 
        case DbEngine.MSSQL:
          CreateSqlServerDatabase(context);
          break;
        case DbEngine.ACCESS:
          CreateAccessDatabase(context);
          break;
        case DbEngine.SQLCE:
          CreateSqlCeDatabase(context);
          break;
        case DbEngine.SQLITE:
          CreateSQLiteDatabase(context);
          break;
      }
    }

    /// <summary>
    /// Tworzenie bazy danych przez SQL Serwer
    /// </summary>
    /// <param name="context">kontekst zawierający informacje potrzebne do utworzenia bazy danych</param>
    /// <param name="attachOnly">utworzyć wewnątrzną bazę danych przez podłączenie istniejącego pliku</param>
    public static void CreateSqlServerDatabase(DbContext context, bool attachOnly = false)
    {
      DbConnection connection = CreateSqlServerConnection(context, true);
      using (DbCommand command = context.Factory.CreateCommand())
      {
        command.CommandText = String.Format(
            "CREATE DATABASE [{0}]"
            + " ON (NAME='{0}' ,FILENAME='{1}')"
            + " LOG ON (NAME='{0}_LOG' ,FILENAME='{2}')",
            context.DbName, context.FileName,
            IO.Path.ChangeExtension(context.FileName, ".ldf")
           );
        if (attachOnly)
          command.CommandText += " FOR ATTACH";
        command.Connection = connection;
        bool connected = false;
        try
        {
          connection.Open();
          connected = true;
          command.ExecuteNonQuery();
        }
        finally
        {
          if (connected)
            connection.Close();
        }
      }
    }

    /// <summary>
    /// Tworzenie bazy danych Microsoft Access
    /// </summary>
    /// <param name="context">kontekst zawierający informacje potrzebne do utworzenia bazy danych</param>
    public static void CreateAccessDatabase(DbContext context)
    {
      CreateAccessConnection(context);
      ADOX.Catalog catalog = new ADOX.Catalog();
      Marshal.ReleaseComObject(catalog.Create(context.ConnectionString));
      Marshal.ReleaseComObject(catalog);
    }

    /// <summary>
    /// Tworzenie bazy danych SQL Compact Editon
    /// </summary>
    /// <param name="context">kontekst zawierający informacje potrzebne do utworzenia bazy danych</param>
    public static void CreateSqlCeDatabase(DbContext context)
    {
#if SQLCE
      CreateSqlCeConnection(context);
      SqlCeEngine engine = new SqlCeEngine(context.ConnectionString);
      engine.CreateDatabase();
#else
      throw new NotImplementedException("SQLCE engine is not compatible with this version of DBUtils");
#endif
    }

    /// <summary>
    /// Tworzenie bazy danych SQLite
    /// </summary>
    /// <param name="context">kontekst zawierający informacje potrzebne do utworzenia bazy danych</param>
    public static void CreateSQLiteDatabase(DbContext context)
    {
#if SQLITE
      CreateSQLiteConnection(context);
      string newConnectionString = context.ConnectionString + ";New=True";
      using (SQLiteConnection connection = new SQLiteConnection(newConnectionString))
      {
        bool opened = false;
        try
        {
          connection.Open();
          opened = true;
        }
        finally
        {
          if (opened)
            connection.Close();
        }
      }
#else
      throw new NotImplementedException("SQLite engine is not compatible with this version of DBUtils");
#endif
    }
#endregion

#region sprawdzenie istnienia bazy danych
    /// <summary>
    /// Sprawdzenie istnienia bazy danych
    /// </summary>
    /// <param name="context">kontekst zawierający informacje definiujące bazę danych</param>
    public static bool ExistsDatabase(DbContext context)
    {
      if (context.DataProvider.Engine == DbEngine.MSSQL)
        if (ExistsSqlServerDatabase(context))
          return true;
      return ExistsDatabaseFile(context);
    }

    /// <summary>
    /// Sprawdzenie istnienia bazy danych SQL Serwera
    /// </summary>
    /// <param name="context">kontekst zawierający informacje definiujące bazę danych</param>
    public static bool ExistsSqlServerDatabase(DbContext context)
    {
      DbConnection connection = CreateSqlServerConnection(context, true);
      using (DbCommand command = context.Factory.CreateCommand())
      {
        command.CommandText =
            String.Format("SELECT * FROM master.sys.databases WHERE Name='{0}'", context.DbName);
        command.Connection = connection;
        object result = null;
        bool opened = false;
        try
        {
          connection.Open();
          opened = true;
          result = command.ExecuteScalar();
        }
        finally
        {
          if (opened)
            connection.Close();
        }
        return result != null;
      }
    }

    /// <summary>
    /// Sprawdzenie istnienia głównego pliku bazy danych
    /// </summary>
    /// <param name="context">kontekst zawierający informacje definiujące bazę danych</param>
    public static bool ExistsDatabaseFile(DbContext context)
    {
      return IO.File.Exists(context.FileName);
    }
#endregion

#region usuwanie bazy danych
    /// <summary>
    /// Usuwanie bazy danych
    /// </summary>
    /// <param name="context">kontekst zawierający informacje potrzebne do usunięcia bazy danych</param>
    public static void DeleteDatabase(DbContext context)
    {
      if (context.DataProvider.Engine == DbEngine.MSSQL)
      {
        if (ExistsSqlServerDatabase(context))
          DeleteSqlServerDatabase(context);
      }
      if (context.DataProvider.Engine == DbEngine.SQLITE)
#if SQLITE
        SQLiteConnection.ClearPool(CreateSQLiteConnection(context) as SQLiteConnection); 
#else
      throw new NotImplementedException("SQLite engine is not compatible with this version of DBUtils");
#endif
      DeleteDatabaseFiles(context);
    }

    /// <summary>
    ///   Usunięcie bazy danych od SQL serwera
    /// </summary>
    /// <param name="context">kontekst zawierający informacje potrzebne do przeprowadzenia operacji</param>
    /// <param name="detachOnly">czy baza danych ma być tylko odłączona od serwera</param>
    public static void DeleteSqlServerDatabase(DbContext context, bool detachOnly=false)
    {
      SqlConnection.ClearPool(CreateSqlServerConnection(context) as SqlConnection);
      DbConnection connection = CreateSqlServerConnection(context, true);
      using (DbCommand command = context.Factory.CreateCommand())
      {
        command.CommandText =
          String.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", context.DbName);
        command.Connection = connection;
        bool opened = false;
        try
        {
          connection.Open();
          opened = true;
          command.ExecuteNonQuery();
          if (detachOnly)
            command.CommandText =
              String.Format("sp_detach_db @dbname= [{0}], @skipchecks= true", context.DbName);
          else
            command.CommandText =
              String.Format("DROP DATABASE [{0}]", context.DbName);
          command.ExecuteNonQuery();
        }
        finally
        {
          if (opened)
            connection.Close();
        }
      }
    }


    /// <summary>
    /// Usuwanie plików bazy danych
    /// </summary>
    /// <param name="context">kontekst zawierający informacje potrzebne do usunięcia bazy danych</param>
    public static void DeleteDatabaseFiles(DbContext context)
    {
      IEnumerable<string> files = PhysicalFilenames(context);
      if (files != null)
      {
        foreach (string file in files)
          if (file!=context.FileName)
            SafeDeleteFile(file);
      }
      SafeDeleteFile(context.FileName);
    }

    /// <summary>
    /// Nazwy wszystkich plików fizycznych skojarzonych z bazą danych.
    /// Niekoniecznie wszystkie z tych plików istnieją.
    /// </summary>
    /// <param name="context">kontekst identyfikujący bazę danych</param>
    /// <returns></returns>
    private static IEnumerable<string> PhysicalFilenames(DbContext context)
    {
      List<string> result = new List<string>();
      result.Add(context.FileName);
      if (context.DataProvider.Engine == DbEngine.MSSQL)
      {
        string logFile = IO.Path.ChangeExtension(context.FileName, ".ldf");
        result.Add(logFile);
        result.Add(logFile.Insert(logFile.Length - 4, "_log"));
      }
      if (context.DataProvider.Engine == DbEngine.ACCESS)
      {
        string ext = IO.Path.GetExtension(context.FileName);
        if (ext.ToUpper() == ".mdb")
          result.Add(IO.Path.ChangeExtension(context.FileName, ".ldf"));
        else
        {
          ext = ext.Insert(1, "l");
          result.Add(IO.Path.ChangeExtension(context.FileName, ext));
        }
      }
      return result;
    }

    /// <summary>
    /// Bezpieczne kasowanie pliku
    /// </summary>
    /// <param name="filename">nazwa pliku</param>
    /// <returns></returns>
    private static void SafeDeleteFile(string filename)
    {
      if (filename != null && IO.File.Exists(filename))
        IO.File.Delete(filename);
    }
#endregion

#region zmiana nazwy bazy danych
    /// <summary>
    /// Zmiana nazwy bazy danych
    /// </summary>
    /// <param name="newDbName">nowa nazwa bazy danych</param>
    /// <param name="newFileName">nowa nazwa pliku</param>
    /// <param name="context">kontekst zawierający informacje potrzebne do wykonania operacji</param>
    public static void RenameDatabase(DbContext context, string newDbName, string newFileName)
    {
      if (context.DataProvider.Engine == DbEngine.MSSQL)
      {
        if (ExistsSqlServerDatabase(context))
          RenameSqlServerDatabase(context, newDbName, newFileName);
        else
          throw new DataException("To rename a SQL database it must be connected to a SQL Server instance");
      }
      else
      {
        if (context.DataProvider.Engine == DbEngine.SQLITE)
          throw new NotImplementedException("SQLite engine is not compatible with this version of DBUtils");
//          SQLiteConnection.ClearPool(CreateSQLiteConnection(context) as SQLiteConnection);
        RenameDatabaseFiles(context, newFileName);
      }
    }

    /// <summary>
    ///   Zmiana nazwy bazy danych od SQL serwera
    /// </summary>
    /// <param name="newDbName">nowa nazwa bazy danych</param>
    /// <param name="newFileName">nowa nazwa pliku (bez rozszerzenia)</param>
    /// <param name="context">kontekst zawierający informacje potrzebne do przeprowadzenia operacji</param>
    public static void RenameSqlServerDatabase(DbContext context, string newDbName, string newFileName)
    {
      SqlConnection.ClearPool(CreateSqlServerConnection(context) as SqlConnection);
      DbConnection connection = CreateSqlServerConnection(context, true);
      context.ServerConnection = connection;
      using (DbCommand command = context.Factory.CreateCommand())
      {
        command.CommandText =
          String.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", context.DbName);
        command.Connection = connection;
        bool opened = false;
        try
        {
          connection.Open();
          opened = true;
          command.ExecuteNonQuery();
          if (newDbName != context.DbName)
          {
            command.CommandText =
              String.Format("ALTER DATABASE [{0}] MODIFY Name = [{1}]", context.DbName, newDbName);
            command.ExecuteNonQuery();
          }
          bool changePhysicalFile =
            (newFileName != null) && (newFileName != IO.Path.GetFileNameWithoutExtension(context.FileName));
          if (newDbName != context.DbName || changePhysicalFile)
          {

            // opcjonalne utworzenie potrzebnego katalogu
            string newPath = IO.Path.GetDirectoryName(newFileName);
            if (!IO.Directory.Exists(newPath))
              IO.Directory.CreateDirectory(newPath);

            DataTable filesTable = GetSqlDatabaseFiles(context, newDbName);

            // zmiana nazw plików
            foreach (DataRow dataRow in filesTable.Rows)
            {
              string alterSQL = "ALTER DATABASE [{0}]"
                    + " MODIFY FILE ( NAME = [{1}],"
                    + " NEWNAME = [{2}]";
              if (changePhysicalFile)
                alterSQL += ", FILENAME = N'{3}')";
              else
                alterSQL += ")";

              command.CommandText =
                String.Format(alterSQL,
                    newDbName,
                    dataRow["name"] as string,
                    (dataRow["name"] as String).Replace(context.DbName, newDbName),
                    ChangeFileName(dataRow["physical_name"] as String, newFileName));
              command.ExecuteNonQuery();
            }
          }
          // przesunięcie fizycznych plików
          if (changePhysicalFile)
          {
            command.CommandText =
                String.Format("ALTER DATABASE [{0}] SET OFFLINE", newDbName);
            command.ExecuteNonQuery();
            RenameDatabaseFiles(context, newFileName);
            command.CommandText =
                String.Format("ALTER DATABASE [{0}] SET ONLINE", newDbName);
            command.ExecuteNonQuery();
          }

          command.CommandText =
            String.Format("ALTER DATABASE [{0}] SET MULTI_USER WITH ROLLBACK IMMEDIATE", newDbName);
          command.ExecuteNonQuery();
        }
        finally
        {
          if (opened)
            connection.Close();
        }
      }
    }

    /// <summary>
    /// Pobranie nazw plików skojarzonych z bazą danych
    /// </summary>
    /// <param name="context">kontekst z utworzonym połączeniem do serwera</param>
    /// <param name="database">nazwa bazy danych</param>
    /// <returns>tabela z dwiema kolumnami: "name" i "physical_name"</returns>
    private static DataTable GetSqlDatabaseFiles(DbContext context, string database)
    {
      using (DbCommand command = context.Factory.CreateCommand())
      {
        command.Connection = context.ServerConnection;
        // pobranie nazw logicznych i fizycznych plików
        command.CommandText =
          String.Format("SELECT name, physical_name"
          + " FROM sys.master_files"
          + " WHERE database_id = DB_ID(N'{0}')",
          database);
        DbDataAdapter dataAdapter = context.Factory.CreateDataAdapter();
        dataAdapter.TableMappings.Add("Table", "Files");
        dataAdapter.SelectCommand = command;
        DataSet dataSet = new DataSet("Files");
        dataAdapter.Fill(dataSet);
        return dataSet.Tables["Files"];
      }
    }

    /// <summary>
    /// Zmiana nazwy plików bazy danych
    /// </summary>
    /// <param name="newFileName">nowa nazwa pliku (bez rozszerzenia)</param>
    /// <param name="context">kontekst zawierający informacje potrzebne do wykonania operacji</param>
    public static void RenameDatabaseFiles(DbContext context, string newFileName)
    {
      IEnumerable<string> files = PhysicalFilenames(context);
      if (files != null)
      {
        foreach (string file in files)
          if (file != context.FileName)
            SafeRenameFile(file, ChangeFileName(file, newFileName));
      }
      SafeRenameFile(context.FileName, ChangeFileName(context.FileName, newFileName));
    }

    /// <summary>
    /// Zmiana nazwa pliku bez zmiany rozszerzenia. 
    /// Jeśli nowa nazwa pliku nie zawiera ścieżki katalogów, 
    /// to kopiowana jest ścieżka ze starej nazwy.
    /// </summary>
    /// <param name="oldFileName">stara nazwa pliku</param>
    /// <param name="newFileName">nowa nazwa pliku (bez rozszerzenia)</param>
    /// <returns></returns>
    public static string ChangeFileName(string oldFileName, string newFileName)
    {
      string ext = IO.Path.GetExtension(oldFileName);
      if (String.IsNullOrEmpty(IO.Path.GetDirectoryName(newFileName)))
      {
        string oldPath = IO.Path.GetDirectoryName(oldFileName);
        newFileName = IO.Path.Combine(oldPath, newFileName);
      }
      return IO.Path.ChangeExtension(newFileName, ext);
    }

    /// <summary>
    /// Bezpieczna zmiana nazwy pliku
    /// </summary>
    /// <param name="oldFileName">stara nazwa pliku</param>
    /// <param name="newFileName">nowa nazwa pliku</param>
    private static void SafeRenameFile(string oldFileName, string newFileName)
    {
      if (oldFileName != null && IO.File.Exists(oldFileName))
      {
        string newPath = IO.Path.GetDirectoryName(newFileName);
        if (!IO.Directory.Exists(newPath))
          IO.Directory.CreateDirectory(newPath);
        IO.File.Move(oldFileName, newFileName);
      }
    }
#endregion

#region kopiowanie bazy danych
    /// <summary>
    /// Kopiowanie bazy danych
    /// </summary>
    /// <param name="newDbName">nowa nazwa bazy danych</param>
    /// <param name="newFileName">nowa nazwa pliku</param>
    /// <param name="context">kontekst zawierający informacje potrzebne do wykonania operacji</param>
    public static void CopyDatabase(DbContext context, string newDbName, string newFileName)
    {
      if (context.DataProvider.Engine == DbEngine.MSSQL)
      {
        if (ExistsSqlServerDatabase(context))
          CopySqlServerDatabase(context, newDbName, newFileName);
        else
          throw new DataException("To copy a SQL database it must be connected to a SQL Server instance");
      }
      else
        CopyDatabaseFiles(context, newFileName);
    }

    /// <summary>
    ///   Kopiowanie bazy danych SQL serwera
    /// </summary>
    /// <param name="newDbName">nowa nazwa bazy danych</param>
    /// <param name="newFileName">nowa nazwa pliku (bez rozszerzenia)</param>
    /// <param name="context">kontekst zawierający informacje potrzebne do przeprowadzenia operacji</param>
    public static void CopySqlServerDatabase(DbContext context, string newDbName, string newFileName)
    {
      DbConnection connection = CreateSqlServerConnection(context, true);
      context.ServerConnection = connection;
      using (DbCommand command = context.Factory.CreateCommand())
      {
        command.CommandText =
          String.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", context.DbName);
        command.Connection = connection;
        bool opened = false;
        try
        {
          connection.Open();
          opened = true;
          command.ExecuteNonQuery();

          string tempFileName = IO.Path.Combine(IO.Path.GetDirectoryName(context.FileName),
            IO.Path.GetFileNameWithoutExtension(context.FileName) + "_TEMPCOPY");

          // skopiowanie fizycznych plików do plików tymczasowych
          DataTable fileTable = GetSqlDatabaseFiles(context, context.DbName);
          command.CommandText =
              String.Format("ALTER DATABASE [{0}] SET OFFLINE", context.DbName);
          command.ExecuteNonQuery();
          foreach (DataRow dataRow in fileTable.Rows)
          {
            string oldFileName = dataRow["physical_name"] as string;
            string copyFileName = ChangeFileName(oldFileName, tempFileName);
            if (IO.File.Exists(copyFileName))
              IO.File.Delete(copyFileName);
            IO.File.Copy(oldFileName, copyFileName);
          }
          command.CommandText =
              String.Format("ALTER DATABASE [{0}] SET ONLINE", context.DbName);
          command.ExecuteNonQuery();

          // przywrócenie oryginalnej bazy danych do trybu wieloużytkowego
          command.CommandText =
            String.Format("ALTER DATABASE [{0}] SET MULTI_USER WITH ROLLBACK IMMEDIATE", context.DbName);
          command.ExecuteNonQuery();

          // zmiana nazwy/przesunięcie oryginalnej bazy danych
          RenameSqlServerDatabase(context, newDbName, newFileName);

          // zmiana nazw poprzednio utworzonych tymczasowych kopii plików na poprzednie
          foreach (DataRow dataRow in fileTable.Rows)
          {
            string oldFileName = dataRow["physical_name"] as string;
            string copyFileName = ChangeFileName(oldFileName, tempFileName);
            if (IO.File.Exists(copyFileName))
              IO.File.Move(copyFileName, oldFileName);
          }

          // ponowne utworzenie pierwotnej bazy danych
          // na podstawie zachowanych kopii plików
          command.CommandText =
              String.Format(
                    "CREATE DATABASE [{0}]"
                  + " ON (NAME='{0}' ,FILENAME='{1}')"
                  + " LOG ON (NAME='{0}_LOG' ,FILENAME='{2}')"
                  + " FOR ATTACH",
                  context.DbName, context.FileName,
                  IO.Path.ChangeExtension(context.FileName, ".ldf"));
          command.ExecuteNonQuery();
        }
        finally
        {
          if (opened)
            connection.Close();
        }
      }
    }


    /// <summary>
    /// Kopiowanie plików bazy danych
    /// </summary>
    /// <param name="newFileName">nowa nazwa pliku (bez rozszerzenia)</param>
    /// <param name="context">kontekst zawierający informacje potrzebne do wykonania operacji</param>
    public static void CopyDatabaseFiles(DbContext context, string newFileName)
    {
      IEnumerable<string> files = PhysicalFilenames(context);
      if (files != null)
      {
        foreach (string file in files)
          if (file != context.FileName)
            SafeCopyFile(file, ChangeFileName(file, newFileName));
      }
      SafeCopyFile(context.FileName, ChangeFileName(context.FileName, newFileName));
    }

    /// <summary>
    /// Bezpieczne kopiowanie pliku
    /// </summary>
    /// <param name="fromFileName">nazwa pliku oryginalnego</param>
    /// <param name="toFileName">nazwa kopii pliku</param>
    /// <returns></returns>
    private static void SafeCopyFile(string fromFileName, string toFileName)
    {
      if (fromFileName != null && IO.File.Exists(fromFileName))
      {
        string newPath = IO.Path.GetDirectoryName(toFileName);
        if (!IO.Directory.Exists(newPath))
          IO.Directory.CreateDirectory(newPath);
        IO.File.Copy(fromFileName, toFileName);
      }
    }
#endregion

#region tworzenie tabeli
    /// <summary>
    /// Tworzenie tabeli
    /// </summary>
    /// <param name="context">kontekst zawierający informacje o bazie danych</param>
    /// <param name="table">definicja tabeli</param>
    public static void CreateTable(DbContext context, DataTable table)
    {
      using (DbCommand command = context.Factory.CreateCommand())
      {
        command.Connection = context.Connection;
        command.CommandText = context.TableBuilder.BuildCreateTable(table);
        Console.WriteLine(command.CommandText);
        command.ExecuteNonQuery();
      }
    }

#endregion

#region pobieranie tabeli
    /// <summary>
    /// Pobieranie tabeli
    /// </summary>
    /// <param name="context">kontekst zawierający informacje o bazie danych</param>
    /// <param name="tableName">nazwa tabeli</param>
    public static DataTable GetTable(DbContext context, string tableName)
    {
      DataTable table = new DataTable(tableName);
      using (DbCommand command = context.Factory.CreateCommand())
      {
        command.Connection = context.Connection;
        command.CommandText =
          String.Format(
          "SELECT * FROM [{0}]",
          table.TableName);
        DbDataAdapter adapter = context.Factory.CreateDataAdapter();
        adapter.TableMappings.Add(table.TableName, table.TableName);
        adapter.SelectCommand = command;
        adapter.Fill(table);
        Console.WriteLine("Columns:");
        foreach (DataColumn aCol in table.Columns)
          Console.WriteLine(aCol.ColumnName + ": "+aCol.DataType);
        Console.WriteLine("PrimaryKey:");
        foreach (DataColumn aCol in table.PrimaryKey)
          Console.Write(aCol.ColumnName + " ");
        Console.WriteLine("");
        Console.WriteLine("Loaded rows:");
        foreach (DataRow aRow in table.Rows)
        {
          foreach (DataColumn aCol in table.Columns)
            Console.Write(aCol.ColumnName + ": " + aRow.Field<object>(aCol.ColumnName)+" ");
          Console.WriteLine("");
        }
        return table;
      }
    }
#endregion

#region aktualizacja tabeli w bazie danych
    /// <summary>
    /// Aktualizacja tabeli w bazie danych
    /// </summary>
    /// <param name="context">kontekst zawierający informacje o bazie danych</param>
    /// <param name="table">tabela</param>
    public static void UpdateTable(DbContext context, DataTable table)
    {
      using (DbDataAdapter adapter = context.Factory.CreateDataAdapter())
      using (DbCommand selectCommand = context.Factory.CreateCommand())
      {
        selectCommand.Connection = context.Connection;
        selectCommand.CommandText =
          String.Format(
          "SELECT * FROM [{0}]",
          table.TableName);
        adapter.SelectCommand = selectCommand;
        DbCommandBuilder cb = context.Factory.CreateCommandBuilder();
        cb.DataAdapter = adapter;
        using (adapter.UpdateCommand = cb.GetUpdateCommand(true))
          using (adapter.InsertCommand = cb.GetInsertCommand(true))
            using (adapter.DeleteCommand = cb.GetDeleteCommand(true))
            {
              Console.WriteLine(adapter.UpdateCommand.CommandText);
              Console.WriteLine();
              Console.WriteLine(adapter.InsertCommand.CommandText);
              Console.WriteLine();
              Console.WriteLine(adapter.DeleteCommand.CommandText);
              Console.WriteLine(); 
              adapter.Update(table);
            }
      }
    }

#endregion
  
  }

  /// <summary>
  /// Informacja o fabrykach podawana przez metodę <see cref="DbUtils.EnumerateFactoryClasses"/>
  /// </summary>
  public class FactoryInfo
  {
    /// <summary>
    /// Przyjazna nazwa fabryki
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Opis fabryki
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Nazwa używana do podawania klasy fabryki
    /// </summary>
    public string InvariantName { get; set; }
    /// <summary>
    /// Pełna nazwa klasy fabryki
    /// </summary>
    public string AssemblyQualifiedName { get; set; }
  }

  /// <summary>
  /// Informacja o źródłach danych dla klienta Sql Serwera 
  /// zwracana przez metodę <see cref="DbUtils.EnumerateDataSources"/>
  /// </summary>
  public class SqlDataSourceInfo
  {
    /// <summary>
    /// Nazwa serwera (maszyny, na której serwer jest postawiony)
    /// </summary>
    public string ServerName { get; set; }
    /// <summary>
    /// Nazwa instancji serwera (domyślna instancja ma nazwę pustą)
    /// </summary>
    public string InstanceName { get; set; }
    /// <summary>
    /// Czy serwer należy do klastra?
    /// </summary>
    public string IsClustered { get; set; }
    /// <summary>
    /// Wersja serwera
    /// </summary>
    public string Version { get; set; }
    /// <summary>
    /// Adres sieciowy tworzony z połączenia nazwy serwera i nazwy instancji
    /// </summary>
    /// <returns></returns>
    public override string ToString()
    {
      if (String.IsNullOrEmpty(InstanceName))
        return ServerName;
      else
        return ServerName+"\\"+InstanceName;
    }
  }

  /// <summary>
  /// Informacja o dostawcach baz danych 
  /// zwracana przez metody wyliczające dostawców
  /// </summary>
  public class DbProviderInfo
  {
    /// <summary>
    /// Nazwa dostawcy (używana do identyfikacji)
    /// </summary>
    public string ShortName { get; set; }
    /// <summary>
    /// Opis dostawcy
    /// </summary>
    public string FullName { get; set; }
    /// <summary>
    /// Rodzaj dostawcy
    /// </summary>
    public ProviderKind Kind { get; set; }
    /// <summary>
    /// Silnik bazy danych
    /// </summary>
    public DbEngine Engine { get; set; }
    /// <summary>
    /// Wersja silnika
    /// </summary>
    public string Version { get; set; }
    /// <summary>
    /// Znane rozszerzenia plików danych
    /// </summary>
    public string FileExtensions { get; set; }
    /// <summary>
    /// Identyfikator klasy (GUID)
    /// </summary>
    public string ClsID { get; set; }
  }

  /// <summary>
  /// Silnik (typ) bazy danych
  /// </summary>
  [DataContract]
  public enum DbEngine: int
  {
    /// <summary>
    /// Nieokreślony
    /// </summary>
    [EnumMember]
    Unknown,
    /// <summary>
    /// Microsoft SQL Server
    /// </summary>
    [EnumMember]
    MSSQL,
    /// <summary>
    /// Microsoft Access
    /// </summary>
    [EnumMember]
    ACCESS,
    /// <summary>
    /// Microsoft Excel
    /// </summary>
    [EnumMember]
    EXCEL,
    /// <summary>
    /// DBASE
    /// </summary>
    [EnumMember]
    DBASE,
    /// <summary>
    /// SQL Compact Edition
    /// </summary>
    [EnumMember]
    SQLCE,
    /// <summary>
    /// Online Analytical Processing
    /// </summary>
    [EnumMember]
    OLAP,
    /// <summary>
    /// Active Directory Service Interface
    /// </summary>
    [EnumMember]
    ADSI,
    /// <summary>
    /// baza danych Oracle
    /// </summary>
    [EnumMember]
    ORACLE,
    /// <summary>
    /// OleDb Simple Provider
    /// </summary>
    [EnumMember]
    OSP,
    /// <summary>
    /// Indexing Services
    /// </summary>
    [EnumMember]
    IDXS,
    /// <summary>
    /// Windows Search Data Services 
    /// </summary>
    [EnumMember]
    WSDS,
    /// <summary>
    /// Baza danych oparta o pliki XML
    /// </summary>
    [EnumMember]
    XMLDB,
    /// <summary>
    /// Wbudowana baza danych SQLite
    /// </summary>
    [EnumMember]
    SQLITE,
  }

  /// <summary>
  /// Typ dostawcy danych
  /// </summary>
  [DataContract]
  public enum ProviderKind
  {
    /// <summary>
    /// Nieznany - nieokreślony
    /// </summary>
    [EnumMember]
    Unknown,
    /// <summary>
    /// Dostawca wbudowany w aplikację
    /// </summary>
    [EnumMember]
    Embedded,
    /// <summary>
    /// Wykorzystywany mechanizm OleDb
    /// </summary>
    [EnumMember]
    OleDb,
    /// <summary>
    /// Wykorzystywany mechanizm ODBC
    /// </summary>
    [EnumMember]
    Odbc,
  }

}
