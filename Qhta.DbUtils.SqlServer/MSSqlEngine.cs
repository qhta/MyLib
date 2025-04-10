﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Management.Smo;
using Microsoft.Win32;

namespace Qhta.DbUtils.SqlServer
{
  [EngineID("MSSQL")]
  [DisplayName("MSSQL Engine")]
  [Description("Microsoft SQL Server Client")]
  public class MSSqlEngine : DbEngine, IDbEngine
  {
    public override string ID => "MSSQL";

    public override bool CanEnumerateServerInstances(ServerType serverType)
    {
      return true;
    }

    /// <summary>
    /// Wyliczenie instancji serwera
    /// </summary>
    public override IEnumerable<DbServerInfo> EnumerateServers(ServerType serverType)
    {
      List<DbServerInfo> result = new List<DbServerInfo>();
      {
        DataTable aTable = null;
        string localServerName = Environment.MachineName;
        try
        {
          aTable = SmoApplication.EnumAvailableSqlServers(serverType.HasFlag(ServerType.Local));
        }
        catch (Exception ex)
        {
          HandleException(ex);
        }
        if (aTable == null && serverType.HasFlag(ServerType.Local))
        {
          var names = GetLocalInstanceNames();
          foreach (var name in names)
          {
            var ss = name.Split('\\');
            ss[0] = localServerName;
            var name1 = String.Join("\\", ss);
            var info = new DbServerInfo
            {
              ID = name1,
              ServerName = ss[0],
              InstanceName = ss.Length > 1 ? ss[1] : null,
              ServerType = ServerType.Local,
              Engine = this,
            };
            result.Add(info);
          }
        }
        if (aTable != null)
        {
          foreach (DataRow aRow in aTable.Rows)
          {
            string name = aRow["Name"] as string;
            string serverName = aRow["Server"] as string;
            string instanceName = aRow["Instance"] as string;
            string version = aRow["Version"] as string;
            var info = new DbServerInfo
            {
              ID = name,
              ServerName = serverName,
              InstanceName = instanceName,
              ServerType = serverName.Equals(localServerName) ? ServerType.Local : ServerType.Remote,
              Version = version,
              Engine = this,
            };
            result.Add(info);
          }
        }
      }
      return result;
    }

    public string[] GetLocalInstanceNames()
    {
      if (Environment.Is64BitOperatingSystem)
      {
        string[] list1;
        string[] list2;
        using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64))
        {
          list1 = GetLocalInstanceNames(hive);
        }

        using (var hive = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32))
        {
          list2 = GetLocalInstanceNames(hive);
        }
        var result = list1.Concat(list2).ToArray();
        return result;
      }
      else
      {
        return GetLocalInstanceNames(Registry.LocalMachine);
      }
    }

    private string[] GetLocalInstanceNames(RegistryKey hive)
    {
      const string keyName = @"Software\Microsoft\Microsoft SQL Server";
      const string valueName = "InstalledInstances";
      const string defaultName = "MSSQLSERVER";

      using (var key = hive.OpenSubKey(keyName, false))
      {
        if (key == null)
          return Array.Empty<string>();

        var value = key.GetValue(valueName) as string[];
        if (value == null)
          return Array.Empty<string>();

        for (int index = 0; index < value.Length; index++)
        {
          if (string.Equals(value[index], defaultName, StringComparison.OrdinalIgnoreCase))
          {
            value[index] = ".";
          }
          else
          {
            value[index] = @".\" + value[index];
          }
        }

        return value;
      }
    }

    /// <summary>
    /// Tworzenie połączenia do serwera bazy danych
    /// </summary>
    /// <param name="server">informacje potrzebne do utworzenia połączenia</param>
    /// <param name="toServer">czy to ma być połączenie do serwera, czy do bazy danych</param>
    /// <returns></returns>
    public SqlConnection CreateConnection(DbServerInfo server)
    {
      SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
      sb["Data Source"] = server.NetName;
      if (server.UserID != null)
      {
        sb["User ID"] = server.UserID;
        if (server.Password != null)
          sb["Password"] = server.Password;
      }
      else
        sb["Trusted_Connection"] = "yes";
      string connectionString = sb.ConnectionString;
      return new SqlConnection(connectionString);
    }

    /// <summary>
    /// Tworzenie połączenia do bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia połączenia</param>
    /// <returns></returns>
    public SqlConnection CreateConnection(DbInfo info)
    {
      SqlConnectionStringBuilder sb = new SqlConnectionStringBuilder();
      sb["Data Source"] = info.Server.NetName;
      if (info.DbName != null)
        sb["Database"] = info.DbName;
      if (info.UserID != null)
      {
        sb["User ID"] = info.UserID;
        if (info.Password != null)
          sb["Password"] = info.Password;
      }
      else
        sb["Trusted_Connection"] = "yes";
      string connectionString = sb.ConnectionString;
      return new SqlConnection(connectionString);
    }


    /// <summary>
    /// Wyliczenie baz danych na serwerze
    /// </summary>
    /// <param name="info">informacje o serwerze</param>
    public override IEnumerable<DbInfo> EnumerateDatabases(DbServerInfo server)
    {
      SqlConnection connection = null;
      bool opened = true;
      try
      {
        connection = (SqlConnection)CreateConnection(server);
        using (var command = new SqlCommand())
        {
          command.CommandText =
            "SELECT [name]"            //0
            + ",[database_id]"         //1
            + ",[source_database_id]"  //2
            + ",[owner_sid]"           //3
            + ",[create_date]"         //4
            + ",[compatibility_level]" //5
            + " FROM sys.databases WHERE not name in ('master','model','msdb','tempdb')";
          command.Connection = connection;
          opened = connection.State == ConnectionState.Open;
          if (!opened)
            connection.Open();

          List<DbInfo> result = new List<DbInfo>();
          using (var dataReader = command.ExecuteReader())
          {
            while (dataReader.Read())
            {
              var dbInfo = new DbInfo
              {
                DbName = dataReader[0].ToString(),
                DefaultFileExt = "mdf",
                CreatedAt = (DateTime)dataReader.GetValue(4),
                Server = server,
              };
              result.Add(dbInfo);
            }
          }
          foreach (var dbInfo in result)
          {
            dbInfo.Engine = this;
            dbInfo.Server = server;
            dbInfo.Files = GetSqlDatabaseFiles(dbInfo);
          }
          return result;
        }
      }
      catch (Exception ex1)
      {
        HandleException(ex1);
        return new DbInfo[0];
      }
      finally
      {
        if (!opened)
          connection?.Close();
      }
      
    }


    /// <summary>
    /// Wyliczenie potencjalnych baz danych, których pliki są dostępne na serwerze
    /// </summary>
    /// <param name="serverInfo">informacje o serwerze</param>
    public override IEnumerable<DbInfo> EnumeratePotentialDatabases(DbServerInfo serverInfo)
    {
      var result = new List<DbInfo>();
      var locations = GetSqlServerDefaultLocations(serverInfo);
      if (System.IO.Directory.Exists(locations[0]))
      {
        var mainFiles = System.IO.Directory.EnumerateFiles(locations[0], "*.mdf");
        var logFiles = System.IO.Directory.EnumerateFiles(locations[0], "*.ldf");
        var logFiles2 = logFiles;
        if (locations[1] != locations[0] && System.IO.Directory.Exists(locations[1]))
          logFiles2 = System.IO.Directory.EnumerateFiles(locations[1], "*.ldf");
        foreach (var item in mainFiles)
        {
          var name1 = System.IO.Path.GetFileNameWithoutExtension(item);
          var dbInfo = new DbInfo { Server = serverInfo, DbName = name1 };
          var fileNames = new List<string>();
          fileNames.Add(item);
          var logFileName = logFiles.FirstOrDefault(item2 => System.IO.Path.GetFileNameWithoutExtension(item2).Contains(name1))
            ?? logFiles2.FirstOrDefault(item2 => System.IO.Path.GetFileNameWithoutExtension(item2).Contains(name1));
          if (logFileName != null)
            fileNames.Add(logFileName);
          dbInfo.FileNames = fileNames.ToArray();
          result.Add(dbInfo);
        }
      }
      var existedDatabases = EnumerateDatabases(serverInfo);
      foreach (var db in existedDatabases)
        result.RemoveAll(item => item.DbName == db.DbName);
      return result;
    }

    /// <summary>
    /// Tworzenie bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia bazy danych</param>
    public override void CreateDatabase(DbInfo info)
    {
      CreateSqlDatabase(info, false);
      info.Files = GetSqlDatabaseFiles(info);
    }

    /// <summary>
    /// Dołączenie istniejącej bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia bazy danych</param>
    public override void AttachDatabase(DbInfo info)
    {
      CreateSqlDatabase(info, true);
    }

    /// <summary>
    /// Dołączenie istniejącej bazy danych do SQL Serwera
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia bazy danych</param>
    private void AttachSqlServerDatabase(DbInfo info)
    {
      CreateSqlDatabase(info, true);
    }

    /// <summary>
    /// Tworzenie bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia bazy danych</param>
    /// <param name="attachOnly">utworzyć wewnętrzną bazę danych przez podłączenie istniejącego pliku</param>
    private void CreateSqlDatabase(DbInfo info, bool attachOnly)
    {
      SqlConnection connection = CreateConnection(info.Server);
      using (var command = new SqlCommand())
      {
        if (info.FileNames != null)
        {
          var files = info.FileNames;
          if (files.Count() == 2 && !String.IsNullOrEmpty(files[1]))
            command.CommandText = String.Format(
                "CREATE DATABASE [{0}]"
                + " ON (NAME='{0}' ,FILENAME='{1}')"
                + " LOG ON (NAME='{0}_LOG' ,FILENAME='{2}')",
                info.DbName, files[0], files[1]
                );
          else
            command.CommandText = String.Format(
                "CREATE DATABASE [{0}]"
                + " ON (NAME='{0}' ,FILENAME='{1}')",
                info.DbName, files[0]
                );
        }
        else
          command.CommandText = String.Format(
              "CREATE DATABASE [{0}]",
              info.DbName);
        if (attachOnly)
          command.CommandText += " FOR ATTACH";
        command.Connection = connection;
        bool opened = connection.State == ConnectionState.Open;
        try
        {
          if (!opened)
            connection.Open();
          command.ExecuteNonQuery();
        }
        finally
        {
          if (!opened)
            connection.Close();
        }
      }
    }

    /// <summary>
    /// Sprawdzenie istnienia bazy danych
    /// </summary>
    /// <param name="info">informacje definiujące bazę danych</param>
    public override bool DatabaseExists(DbInfo info)
    {
      var ok = ExistsSqlServerDatabase(info);
      return ok;
    }

    /// <summary>
    /// Sprawdzenie istnienia bazy danych SQL Serwera
    /// </summary>
    /// <param name="info">informacje definiujące bazę danych</param>
    private bool ExistsSqlServerDatabase(DbInfo info)
    {
      SqlConnection connection = CreateConnection(info.Server);
      using (var command = new SqlCommand())
      {
        command.CommandText =
            String.Format("SELECT * FROM master.sys.databases WHERE Name='{0}'", info.DbName);
        command.Connection = connection;
        bool opened = connection.State == ConnectionState.Open;
        try
        {
          if (!opened)
            connection.Open();
          object result = command.ExecuteScalar();
          var ok = result != null;
          if (ok)
          {
            info.Engine = this;
            info.Files = GetSqlDatabaseFiles(info);
          }
          return ok;
        }
        catch (Exception ex)
        {
          return false;
        }
        finally
        {
          if (!opened)
            connection.Close();
        }
      }
    }

    /// <summary>
    /// Usuwanie bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do usunięcia bazy danych</param>
    public override void DeleteDatabase(DbInfo info)
    {
      if (ExistsSqlServerDatabase(info))
        DeleteSqlServerDatabase(info);
      DeleteDatabaseFiles(info);
    }

    /// <summary>
    /// Odłączenie bazy danych od serwera
    /// </summary>
    /// <param name="info">informacje identyfikujące bazę danych</param>
    public override void DetachDatabase(DbInfo info)
    {
      if (ExistsSqlServerDatabase(info))
        DeleteSqlServerDatabase(info, true);
    }

    /// <summary>
    /// Odłączenie bazy danych od serwera SQL
    /// </summary>
    /// <param name="info">informacje identyfikujące bazę danych</param>
    private void DetachSqlServerDatabase(DbInfo info)
    {
      DeleteSqlServerDatabase(info, true);
    }

    /// <summary>
    ///   Usunięcie bazy danych od SQL serwera
    /// </summary>
    /// <param name="info">informacje potrzebne do przeprowadzenia operacji</param>
    /// <param name="detachOnly">czy baza danych ma być tylko odłączona od serwera</param>
    private void DeleteSqlServerDatabase(DbInfo info, bool detachOnly = false)
    {
      SqlConnection connection = CreateConnection(info);
      if (connection != null && connection.State == ConnectionState.Open)
        connection.Close();
      connection = CreateConnection(info.Server);
      SqlConnection.ClearPool(connection);
      using (var command = new SqlCommand())
      {
        command.CommandText =
            String.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", info.DbName);
        command.Connection = connection;
        bool opened = connection.State == ConnectionState.Open;
        try
        {
          if (!opened)
            connection.Open();
          command.ExecuteNonQuery();
          if (detachOnly)
            command.CommandText =
              String.Format("sp_detach_db @dbname= [{0}], @skipchecks= true", info.DbName);
          else
            command.CommandText =
              String.Format("DROP DATABASE [{0}]", info.DbName);
          command.ExecuteNonQuery();
        }
        finally
        {
          if (!opened)
            connection.Close();
        }
      }
    }

    /// <summary>
    /// Domyślne rozszerzenie nazwy pliku głównego
    /// </summary>
    public override string DefaultFileExt => ".mdf";


    /// <summary>
    /// Nazwy plików fizycznych skojarzonych z bazą danych.
    /// Niekoniecznie wszystkie te pliki istnieją.
    /// </summary>
    /// <param name="info">kontekst identyfikujący bazę danych</param>
    /// <returns></returns>
    public override string[] PhysicalFilenames(DbInfo info)
    {
      List<string> result = new List<string>();
      if (info.FileNames != null)
      {
        result.AddRange(info.FileNames);
        if (result.Count == 1)
          result.Add(SqlLogFileName(result[0]));
      }
      else if (DatabaseExists(info))
      {
        var files = GetSqlDatabaseFiles(info);
        foreach (var item in files)
          result.Add(item.PhysicalName);
      }
      else
      {
        result.AddRange(GetDefaultFileNames(info));
      }
      return result.ToArray();
    }

    /// <summary>
    /// Sprawdzenie istnienia plików bazy danych
    /// </summary>
    /// <param name="info">informacje definiujące bazę danych</param>
    public override bool ExistsDatabaseFiles(DbInfo info)
    {
      if (info.Files == null)
      {
        if (info.FileNames == null)
          info.FileNames = GetDefaultFileNames(info);
        for (int i = 0; i < info.FileNames.Length; i++)
          if (!System.IO.File.Exists(info.FileNames[i]))
            return false;
      }
      else
      {
        for (int i = 0; i < info.Files.Length; i++)
          if (!System.IO.File.Exists(info.Files[i].PhysicalName))
            return false;
      }
      return true;
    }

    private string[] GetDefaultFileNames(DbInfo info)
    {
      var result = GetSqlServerDefaultLocations(info.Server);
      result[0] = System.IO.Path.Combine(result[0], info.DbName + ".mdf");
      result[1] = SqlLogFileName(System.IO.Path.Combine(result[1], info.DbName + ".ldf"));
      return result;
    }

    /// <summary>
    /// Podanie domyślnej lokalizacji plików
    /// </summary>
    /// <param name="info">informacje definiujące bazę danych</param>
    private string[] GetSqlServerDefaultLocations(DbServerInfo info)
    {
      SqlConnection connection = CreateConnection(info);
      using (var command = new SqlCommand())
      {
        command.CommandText =
             "SELECT SERVERPROPERTY('InstanceDefaultDataPath') AS DataFiles, SERVERPROPERTY('InstanceDefaultLogPath') AS LogFiles";
        command.Connection = connection;
        bool opened = connection.State == ConnectionState.Open;
        try
        {
          if (!opened)
            connection.Open();
          var result = new List<string>();
          using (var dataReader = command.ExecuteReader())
          {
            while (dataReader.Read())
            {
              result.Add(dataReader[0].ToString());
              result.Add(dataReader[1].ToString());
            }
          }
          return result.ToArray();
        }
        finally
        {
          if (!opened)
            connection.Close();
        }
      }
    }

    private string SqlLogFileName(string rowsFileName)
    {
      var path = System.IO.Path.GetDirectoryName(rowsFileName);
      var filename = System.IO.Path.GetFileNameWithoutExtension(rowsFileName);
      filename += "_log.ldf";
      return System.IO.Path.Combine(path, filename);
    }
    /// <summary>
    /// Zmiana nazwy bazy danych
    /// </summary>
    /// <param name="newDbName">nowa nazwa bazy danych</param>
    /// <param name="newFileNames">nowe nazwy plików danych</param>
    /// <param name="info">informacje potrzebne do wykonania operacji</param>
    public override void RenameDatabase(DbInfo info, string newDbName, params string[] newFileNames)
    {
      if (ExistsSqlServerDatabase(info))
      {
        RenameSqlServerDatabase(info, newDbName, newFileNames);
        info.DbName = newDbName;
        info.Files = GetSqlDatabaseFiles(info);
      }
      else
        throw new DataException("To rename a SQL database it must be connected to a SQL Server instance");
    }

    /// <summary>
    ///   Zmiana nazwy bazy danych od SQL serwera
    /// </summary>
    /// <param name="info">informacje o "starej" bazie danych</param>
    /// <param name="newDbName">nowa nazwa bazy danych</param>
    /// <param name="newFileName">nowa nazwa pliku (bez rozszerzenia)</param>
    private void RenameSqlServerDatabase(DbInfo info, string newDbName, params string[] newFileNames)
    {
      SqlConnection connection = CreateConnection(info.Server);
      SqlConnection.ClearPool(connection);

      using (var command = new SqlCommand())
      {
        var oldDbName = info.DbName;
        var oldFiles = GetSqlDatabaseFiles(info);
        command.CommandText =
          String.Format("ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE", info.DbName);
        command.Connection = connection;

        bool opened = connection.State == ConnectionState.Open;
        try
        {
          if (!opened)
            connection.Open();
          command.ExecuteNonQuery();
          if (newDbName != info.DbName)
          {
            command.CommandText =
              String.Format("ALTER DATABASE [{0}] MODIFY Name = [{1}]", oldDbName, newDbName);
            command.ExecuteNonQuery();
          }
          bool changePhysicalFile = newFileNames != null && newFileNames.Length > 0;
          if (changePhysicalFile)
          {
            var newFiles = NewFileInfos(info, newDbName, newFileNames);

            // zmiana nazw plików
            for (int i = 0; i < newFiles.Count() && i < newFileNames.Count(); i++)
            {
              var oldFileName = oldFiles[i].LogicalName;

              string alterSQL = "ALTER DATABASE [{0}]"
                    + " MODIFY FILE ( NAME = [{1}],"
                    + " NEWNAME = [{2}]";
              if (oldFiles[i].PhysicalName != newFiles[i].PhysicalName)
                alterSQL += ", FILENAME = N'{3}')";
              else
                alterSQL += ")";
              command.CommandText =
                String.Format(alterSQL,
                    newDbName,
                    oldFiles[i].LogicalName, newFiles[i].LogicalName,
                    newFiles[i].PhysicalName);
              command.ExecuteNonQuery();
            }
            info.DbName = newDbName;
            DetachSqlServerDatabase(info);
            var oldFileNames = oldFiles.Select(item => item.PhysicalName).Take(newFileNames.Length).ToArray();
            RenameDatabaseFiles(oldFileNames, newFileNames);
            for (int i = newFileNames.Length; i < newFiles.Length; i++)
              newFiles[i] = oldFiles[i];
            info.Files = newFiles;
            AttachSqlServerDatabase(info);
          }
          else
            info.DbName = newDbName;

          command.CommandText =
            String.Format("ALTER DATABASE [{0}] SET MULTI_USER WITH ROLLBACK IMMEDIATE", newDbName);
          command.ExecuteNonQuery();
        }
        finally
        {
          if (!opened)
            connection.Close();
        }
      }
    }

    //private List<DbFileInfo> NewDbFileInfos(string newDbName, string[] newFileNames)
    //{
    //  var newFiles = new List<DbFileInfo>();
    //  var defaultFiles = GetDefaultFileNames();
    //  newFiles.Add(new DbFileInfo {})
    //  for (int i = 0; i<oldFiles.Count; i++)
    //    if (i<newFileNames.Length)
    //      newFiles.Add(new DbFileInfo
    //      {
    //        LogicalName = oldFiles[i].LogicalName.Replace(oldDbName, newDbName),
    //        PhysicalName = ChangeFileName(oldFiles[i].PhysicalName, newFileNames[i])
    //      });
    //    else
    //      newFiles.Add(new DbFileInfo
    //      {
    //        LogicalName = oldFiles[i].LogicalName.Replace(oldDbName, newDbName),
    //        PhysicalName = oldFiles[i].PhysicalName
    //      });
    //  return newFiles;
    //}

    private DbFileInfo[] NewFileInfos(DbInfo info, string newDbName, string[] newFileNames)
    {
      var oldDbName = info.DbName;
      var oldFiles = info.Files;
      var newFiles = new List<DbFileInfo>();
      for (int i = 0; i < oldFiles.Count(); i++)
        if (newFileNames != null && i < newFileNames.Length)
          newFiles.Add(new DbFileInfo
          {
            Type = oldFiles[i].Type,
            LogicalName = oldFiles[i].LogicalName.Replace(oldDbName, newDbName),
            PhysicalName = ChangeFileName(oldFiles[i].PhysicalName, newFileNames[i]),
          });
        else
          newFiles.Add(new DbFileInfo
          {
            Type = oldFiles[i].Type,
            LogicalName = oldFiles[i].LogicalName.Replace(oldDbName, newDbName),
            PhysicalName = oldFiles[i].PhysicalName.Replace(oldDbName, newDbName),
          });
      return newFiles.ToArray();
    }

    /// <summary>
    /// Pobranie nazw plików skojarzonych z bazą danych
    /// </summary>
    /// <param name="info">informacje o bazie danych</param>
    /// <returns>lista informacji o plikach</returns>
    private DbFileInfo[] GetSqlDatabaseFiles(DbInfo info)
    {
      SqlConnection connection = CreateConnection(info.Server);

      using (var command = new SqlCommand())
      {
        // pobranie nazw logicznych i fizycznych plików
        command.CommandText =
          String.Format("SELECT type_desc, name, physical_name"
          + " FROM sys.master_files"
          + " WHERE database_id = DB_ID(N'{0}')"
          + " ORDER BY [type]",
          info.DbName);
        command.Connection = connection;
        bool opened = connection.State == ConnectionState.Open;
        try
        {
          if (!opened)
            connection.Open();

          var result = new List<DbFileInfo>();
          using (var dataReader = command.ExecuteReader())
          {
            while (dataReader.Read())
            {
              result.Add(
                new DbFileInfo
                {
                  Type = dataReader[0].ToString(),
                  LogicalName = dataReader[1].ToString(),
                  PhysicalName = dataReader[2].ToString(),
                });
            }
          }
          return result.ToArray();
        }
        finally
        {
          if (!opened)
            connection.Close();
        }
      }
    }

    /// <summary>
    /// Kopiowanie bazy danych
    /// </summary>
    /// <param name="dbInfo">informacje o istniejącej bazie danych</param>
    /// <param name="newDbInfo">informacje o nowej bazie danych</param>
    public override void CopyDatabase(DbInfo dbInfo, DbInfo newDbInfo)
    {
      if (ExistsSqlServerDatabase(dbInfo))
      {
        if (newDbInfo.Server != dbInfo.Server)
          throw new DataException("You can copy a database only within the same SQL Server instance");
        CopySqlServerDatabase(dbInfo, newDbInfo.DbName, newDbInfo.FileNames);
      }
      else
        throw new DataException("To copy a SQL database it must be connected to a SQL Server instance");
    }

    /// <summary>
    ///   Kopiowanie bazy danych SQL serwera
    /// </summary>
    /// <param name="oldDbInfo">informacje o "starej" bazie danych</param>
    /// <param name="newDbName">nowa nazwa bazy danych</param>
    /// <param name="newFileNames">nowe nazwy plików (bez rozszerzenia)</param>
    private void CopySqlServerDatabase(DbInfo oldDbInfo, string newDbName, params string[] newFileNames)
    {
      var oldDbName = oldDbInfo.DbName;
      var oldFiles = GetSqlDatabaseFiles(oldDbInfo);
      var newFiles = NewFileInfos(oldDbInfo, newDbName, newFileNames);
      var newDbInfo = new DbInfo { Server = oldDbInfo.Server, DbName = newDbName, Files = newFiles };
      CopySqlServerDatabase(oldDbInfo, newDbInfo);
    }

    /// <summary>
    ///   Kopiowanie bazy danych SQL serwera
    /// </summary>
    /// <param name="oldDbInfo">informacje o "starej" bazie danych</param>
    /// <param name="newDbInfo">informacje o "nowej" bazie danych</param>
    private void CopySqlServerDatabase(DbInfo oldDbInfo, DbInfo newDbInfo)
    {
      SqlConnection connection = CreateConnection(oldDbInfo.Server);
      using (var command = new SqlCommand())
      {
        var oldDbName = oldDbInfo.DbName;
        var oldFiles = GetSqlDatabaseFiles(oldDbInfo);
        command.Connection = connection;
        bool opened = connection.State == ConnectionState.Open;
        try
        {
          if (!opened)
            connection.Open();
          DetachSqlServerDatabase(oldDbInfo);
          var newFiles = newDbInfo.Files;
          // skopiowanie plików danych
          for (int i = 0; i < oldFiles.Count() && i < newFiles.Count(); i++)
          {
            if (!SafeCopyFile(oldFiles[i].PhysicalName, newFiles[i].PhysicalName))
              newFiles[i].PhysicalName = null;
          }
          AttachSqlServerDatabase(oldDbInfo);
          AttachSqlServerDatabase(newDbInfo);
        }
        finally
        {
          if (!opened)
            connection.Close();
        }
      }
    }

    /// <summary>
    /// Tworzenie komendy SQL
    /// </summary>
    /// <param name="cmdText">text komendy (SQL)</param>
    /// <param name="connection">połączenie do serwera</param>
    public override DbCommand CreateCommand(string cmdText, DbConnection connection)
    {
      return new SqlCommand(cmdText, (SqlConnection)connection);
    }


    /// <summary>
    /// Wyliczenie tabel w bazie danych
    /// </summary>
    /// <param name="info">informacje o bazie danych</param>
    public override IEnumerable<DbTableInfo> EnumerateTables(DbInfo info)
    {
      SqlConnection connection = CreateConnection(info);

      using (var command = new SqlCommand())
      {
        command.CommandText =
          "SELECT SCHEMA_NAME(schema_id) AS [SchemaName]," +
          " [Tables].name AS [TableName], [Tables].modify_date AS [LastModified]," +
          " SUM([Partitions].[rows]) AS [TotalRowCount]" +
          " FROM sys.tables AS [Tables]" +
          " JOIN sys.partitions AS [Partitions] ON [Tables].[object_id] = [Partitions].[object_id]" +
          " AND [Partitions].index_id IN ( 0, 1 )" +
          " GROUP BY SCHEMA_NAME(schema_id), [Tables].name, [Tables].modify_date" +
          " ORDER BY [Tables].name;";
        command.Connection = connection;
        bool opened = connection.State == ConnectionState.Open;
        try
        {
          if (!opened)
            connection.Open();

          var result = new List<DbTableInfo>();
          using (var dataReader = command.ExecuteReader())
          {
            while (dataReader.Read())
            {
              string schemaName = dataReader.GetString(0);
              if (schemaName == "dbo")
                schemaName = "";
              else
                schemaName += ".";
              result.Add(
                new DbTableInfo
                {
                  Name = schemaName+dataReader.GetString(1),
                  LastModifiedAt = dataReader.GetDateTime(2),
                  RowsCount = dataReader.GetInt64(3),
                  Database = info,
                });
            }
          }
          return result.ToArray();
        }
        finally
        {
          if (!opened)
            connection.Close();
        }
      }
    }

    /// <summary>
    /// Wyliczenie kolumn w tabeli
    /// </summary>
    /// <param name="info">informacje o tabeli danych</param>
    public override IEnumerable<DbColumnInfo> EnumerateColumns(DbTableInfo info)
    {
      SqlConnection connection = CreateConnection(info.Database);

      using (var command = new SqlCommand())
      {
        command.CommandText = String.Format(
         "SELECT c.column_id" +
         "   ,c.name AS 'name'" +
         "   ,t.Name AS 'type'" +
         "   ,c.max_length AS 'length'" +
         "   ,c.precision" +
         "   ,c.scale" +
         "   ,c.is_nullable" +
         "   ,c.is_identity" +
         "   ,ISNULL(ic.is_primary_key, 0) 'is_primary_key'" +
         " FROM sys.columns AS c" +
         " INNER JOIN  sys.types t" +
         "   ON c.user_type_id = t.user_type_id LEFT" +
         " OUTER JOIN" +
         " (" +
         "   SELECT" +
         "     ic.object_id" +
         "    ,ic.column_id" +
         "    ,i.is_primary_key" +
         "   FROM sys.index_columns AS ic" +
         "   JOIN sys.indexes AS i" +
         "     ON i.object_id = ic.object_id AND i.index_id=ic.index_id AND i.is_primary_key=1" +
         "  ) AS ic" +
         "    ON ic.object_id = c.object_id AND ic.column_id = c.column_id" +
         "  WHERE c.object_id = OBJECT_ID('{0}')" +
         ";", info.Name);
        command.Connection = connection;
        bool opened = connection.State == ConnectionState.Open;
        try
        {
          if (!opened)
            connection.Open();

          var result = new List<DbColumnInfo>();
          using (var dataReader = command.ExecuteReader())
          {
            while (dataReader.Read())
            {
              result.Add(
                new DbColumnInfo
                {
                  Name = dataReader.GetString(1),
                  Type = dataReader.GetString(2),
                  Size = dataReader.GetInt16(3),
                  Precision = dataReader.GetByte(4),
                  Scale = dataReader.GetByte(5),
                  IsNullable = dataReader.GetBoolean(6),
                  IsIdentity = dataReader.GetBoolean(7),
                  IsPrimaryKey = dataReader.GetBoolean(8),
                });
            }
          }
          var arr = result.ToArray();
          return arr;
        }
        finally
        {
          if (!opened)
            connection.Close();
        }
      }
    }

    /// <summary>
    /// Pobranie wszystkich danych z tabeli
    /// </summary>
    /// <param name="table">informacje o tabeli danych</param>
    public override DataTable GetDataTable(DbTableInfo table)
    {
      SqlConnection connection = CreateConnection(table.Database);
      {
        bool opened = connection.State == ConnectionState.Open;
        try
        {
          if (!opened)
            connection.Open();

          DataTable dt = new DataTable();
          using (var da = new SqlDataAdapter(String.Format("SELECT * FROM {0}", table.Name), connection))
          {
            da.Fill(dt);
          }
          return dt;
        }
        finally
        {
          if (!opened)
            connection.Close();
        }
      }
    }

    #region helper methods
    private static IEnumerable<string> SplitSqlStatements(string sqlScript)
    {
      // Split by "GO" statements
      var statements = Regex.Split(
              sqlScript,
              @"^[\t ]*GO[\t ]*\d*[\t ]*(?:--.*)?$",
              RegexOptions.Multiline |
              RegexOptions.IgnorePatternWhitespace |
              RegexOptions.IgnoreCase);

      // Remove empties, trim, and return
      return statements
          .Where(x => !string.IsNullOrWhiteSpace(x))
          .Select(x => x.Trim(' ', '\r', '\n'));
    }

    private static void HandleException(Exception ex)
    {
      Debug.IndentSize = 2;
      Debug.Indent();
      Debug.WriteLine($"{ex.GetType().Name}: {ex.Message}");
      if (ex.InnerException != null)
        HandleException(ex.InnerException);
      Debug.Unindent();
    }
    #endregion
  }
}
