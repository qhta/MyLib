using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.Common;
using System.Data.Odbc;
using System.Data.OleDb;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.DbUtils.Access
{
  /// <summary>
  /// Klasa udostępniająca operacje na bazach danych MS Access
  /// </summary>
  [Description("MS Access Engine")]
  public class MSAccessEngine: DbEngine
  {
    public override bool CanEnumerateServerInstances => false;

    /// <summary>
    /// Tworzenie połączenia do bazy danych
    /// </summary>
    /// <param name="info">informacje potrzebne do utworzenia połączenia</param>
    /// <returns></returns>
    public override DbConnection CreateConnection(DbInfo info)
    {
      DbConnectionStringBuilder sb = new DbConnectionStringBuilder(info.DataProvider.Kind == ProviderKind.Odbc);
      sb["Driver"] = info.DataProvider.ShortName;
      if (info.FileNames != null && info.FileNames.Length>0)
        sb["Dbq"] = info.FileNames[0];
      if (info.UserID != null)
      {
        sb["UID"] = info.UserID;
        if (info.Password != null)
          sb["PWD"] = info.Password;
      }
      info.Connection = new OleDbConnection();
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
      ADOX.Catalog catalog = new ADOX.Catalog();
      Marshal.ReleaseComObject(catalog.Create(info.ConnectionString));
      Marshal.ReleaseComObject(catalog);
    }

    public override void AttachDatabase(DbInfo info)
    {
      throw new InvalidOperationException("Cannot attach database to MSAccessEngine");
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
      throw new InvalidOperationException("Cannot detach database from MSAccessEngine");
    }



  }
}
