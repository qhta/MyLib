using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.OleDb;
using System.Data.Odbc;
#if SQLCE
using System.Data.SqlServerCe;
#endif

namespace MyLib.DbUtils
{
  /// <summary>
  /// Klasa łącząca informacje o otwieranej bazie danych
  /// </summary>
  public class DbContext
  {
#region właściwości "wejściowe"
    /// <summary>
    /// Dostawca danych. Identyfikuje mechanizm danych (np. OleDb), komponent dostawczy (np. Microsoft.Jet.OLEDB.4.0)
    /// i inne parametry
    /// </summary>
    public DbProvider DataProvider { get; set; }
    /// <summary>
    /// Źródło danych / adres sieciowy serwera 
    /// </summary>
    public string DataSource { get; set; }
    /// <summary>
    /// Nazwa bazy danych/katalogu na serwerze
    /// </summary>
    public string DbName { get; set; }
    /// <summary>
    /// Nazwa głównego pliku bazy danych
    /// </summary>
    public string FileName { get; set; }
    /// <summary>
    /// Identyfikator użytkownika (dla połączenia niezaufanego)
    /// </summary>
    public string UserID { get; set; }
    /// <summary>
    /// Hasło konieczne dla połączenia niezaufanego ew. do szyfrowania bazy danych
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// Szyfrowanie bazy danych
    /// </summary>
    public bool Encrypt { get; set; }
    /// <summary>
    /// pozostałe parametry dla łańcucha połączenia
    /// </summary>
    public string ConnectionRest { get; set; }
#endregion

#region właściwości "wyjściowe"
    /// <summary>
    /// Klasa wyspecjalizowanej fabryki typu <c>DbProviderFactory</c> ustalana
    /// na podstawie rodzaju dostawcy (elementu <see cref="DataProvider"/>).
    /// Można też jawnie ustalić fabrykę, która będzie podawana.
    /// </summary>
    public DbProviderFactory Factory 
    {
      get
      {
        if (_Factory != null)
          return _Factory;
        switch (DataProvider.Kind)
        {
          case ProviderKind.Embedded:
            switch (DataProvider.Engine)
            {
              case DbEngine.MSSQL:
                return SqlClientFactory.Instance;
              case DbEngine.SQLCE:
#if SQLCE
                return SqlCeProviderFactory.Instance;
#else
                throw new NotImplementedException("SQLCE engine is not compatible with this version of DBUtils");
#endif
              case DbEngine.SQLITE:
#if SQLITE
                return SQLiteFactory.Instance;
#else
                throw new NotImplementedException("SQLite engine is not compatible with this version of DBUtils");
#endif
            }
            break;
          case ProviderKind.OleDb:
            return OleDbFactory.Instance;
          case ProviderKind.Odbc:
            return OdbcFactory.Instance;
        }
        return null;
      }
      set { _Factory = value; } 
    }
    DbProviderFactory _Factory;

    /// <summary>
    /// Klasa wyspecjalizowanej fabryki typu <c>DbProviderFactory</c> ustalana
    /// na podstawie rodzaju dostawcy (elementu <see cref="DataProvider"/>).
    /// Można też jawnie ustalić fabrykę, która będzie podawana.
    /// </summary>
    public SqlTableBuilder TableBuilder
    {
      get
      {
        if (_TableBuilder != null)
          return _TableBuilder;
        switch (DataProvider.Engine)
        {
          case DbEngine.MSSQL:
            return new SqlTableBuilder();
          case DbEngine.SQLCE:
            return new SqlTableBuilder();
          case DbEngine.ACCESS:
            return new AccessTableBuilder();
          case DbEngine.SQLITE:
#if SQLITE
            return new SQLiteTableBuilder();
#else
            throw new NotImplementedException("SQLite engine is not compatible with this version of DBUtils");
#endif
        }
        return null;
      }
      set { _TableBuilder = value; }
    }
    SqlTableBuilder _TableBuilder;

    /// <summary>
    /// Domyślne rozszerzenie dla głównego pliku danych ustalane
    /// na podstawie informacji zapisanej w  dostawcy (elemencie <see cref="DataProvider"/>).
    /// Można też jawnie ustalić domyślne rozszerzenie, które będzie podawane.
    /// </summary>
    public string DefaultFileExt
    {
      get
      {
        if (_DefaultFileExt != null)
          return _DefaultFileExt;
        if (DataProvider.FileExtensions != null)
        {
          string[] ss = DataProvider.FileExtensions.Split(',',';');
          if (ss.Length > 0)
            return ss[0].Replace("*", "");
        }
        return null;
      }
      set { _DefaultFileExt = value; }
    }
    string _DefaultFileExt;
    /// <summary>
    /// Kompletny łańcuch parametrów podawany na podstawie utworzonego połączenia
    /// albo zapamiętany jawnie
    /// </summary>
    public string ConnectionString
    {
      get
      {
        if (Connection != null)
          return Connection.ConnectionString;
        return _ConnectionString;
      }
      set { _ConnectionString = value; }
    }
    string _ConnectionString;    
#endregion

#region właściwości "przechowujące"
    /// <summary>
    /// Utworzone połączenie
    /// </summary>
    public DbConnection Connection { get; set; }
    /// <summary>
    /// Utworzone połączenie do serwera
    /// </summary>
    public DbConnection ServerConnection { get; set; }
#endregion
  }
}
