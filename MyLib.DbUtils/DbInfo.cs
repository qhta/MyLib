using System;
using System.Linq;
using System.Data.Common;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Klasa łącząca informacje o otwieranej bazie danych
  /// </summary>
  public class DbInfo
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
    /// Informacja o plikach bazy danych
    /// </summary>
    public DbFileInfo[] Files { get; set; }
    /// <summary>
    /// Nazwy plików fizycznych
    /// </summary>
    public string[] FileNames
    {
      get => Files?.Select(item => item.PhysicalName).ToArray() ?? _FileNames;
      set { _FileNames=value; }
    }
    private string[] _FileNames;
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
    /// Silnik, który obsługuje tę bazę danych
    /// </summary>
    public DbEngine Engine { get; set; }

    /// <summary>
    /// Serwer, na którym leży baza danych
    /// </summary>
    public DbServerInfo Server { get; set; }

    /// <summary>
    /// Klasa wyspecjalizowanej fabryki typu <c>DbProviderFactory</c> ustalana
    /// na podstawie rodzaju dostawcy (elementu <see cref="DataProvider"/>).
    /// Można też jawnie ustalić fabrykę, która będzie podawana.
    /// </summary>
    public SqlTableBuilder TableBuilder { get; set; }

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
    /// <summary>
    /// Data i czas utworzenia
    /// </summary>
    public DateTime? CreatedAt { get; set; }
    #endregion

    #region właściwości "przechowujące"
    /// <summary>
    /// Utworzone połączenie
    /// </summary>
    public DbConnection Connection { get; set; }


    #endregion
  }
}
