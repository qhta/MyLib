using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.SqlServerCe;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyLib.DbUtils;

namespace MyLib.DbUtils.SqlCE
{
  [Description("SQL CE Engine")]
  public class SqlCEEngine: DbEngine
  {
    public override bool CanEnumerateServerInstances => false;

    /// <summary>
    /// Tworzenie połączenia do bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia połączenia</param>
    /// <returns></returns>
    public override DbConnection CreateConnection(DbInfo info)
    {
      SqlCeConnectionStringBuilder sb = new SqlCeConnectionStringBuilder(info.ConnectionRest);
      if (info.FileNames != null && info.FileNames.Length>0)
        sb["Data Source"] = info.FileNames[0];
      if (info.Password != null)
      {
        sb["Password"] = info.Password;
        if (info.Encrypt)
          sb["Encrypt"] = true;
      }
      info.Connection = new SqlCeConnection();
      info.Connection.ConnectionString = sb.ConnectionString;
      return info.Connection;
    }

    /// <summary>
    /// Tworzenie bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia bazy danych</param>
    public override void CreateDatabase(DbInfo info)
    {
      CreateConnection(info);
      SqlCeEngine engine = new SqlCeEngine(info.ConnectionString);
      engine.CreateDatabase();
    }

    public override void AttachDatabase(DbInfo info)
    {
      throw new InvalidOperationException("Cannot attach database to SqlCEEngine");
    }

    /// <summary>
    /// Sprawdzenie istnienia bazy danych
    /// </summary>
    /// <param name="info">informacje definiujące bazę danych</param>
    public override bool DatabaseExists(DbInfo info)
    {
      return ExistsDatabaseFiles(info);
    }

    /// <summary>
    /// Usuwanie bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do usunięcia bazy danych</param>
    public override void DeleteDatabase(DbInfo info)
    {
      DeleteDatabaseFiles(info);
    }

    public override void DetachDatabase(DbInfo info)
    {
      throw new InvalidOperationException("Cannot detach database from SqlCEEngine");
    }

    /// <summary>
    /// Zmiana nazwy bazy danych
    /// </summary>
    /// <param name="newDbName">nowa nazwa bazy danych</param>
    /// <param name="newFileNames">nowe nazwy plików</param>
    /// <param name="info">informacje potrzebne do wykonania operacji</param>
    public override void RenameDatabase(DbInfo info, string newDbName, params string[] newFileNames)
    {
      RenameDatabaseFiles(info, newFileNames);
    }

    /// <summary>
    /// Kopiowanie bazy danych
    /// </summary>
    /// <param name="newDbName">nowa nazwa bazy danych</param>
    /// <param name="newFileNames">nowe nazwy plików</param>
    /// <param name="info">informacje potrzebne do wykonania operacji</param>
    public override void CopyDatabase(DbInfo info, string newDbName, params string[] newFileNames)
    {
      CopyDatabaseFiles(info, newFileNames);
    }

  }
}
