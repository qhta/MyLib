using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Informacja o źródłach danych dla klienta Sql Serwera 
  /// zwracana przez metodę <see cref="DbTools.EnumerateServerInstances"/>
  /// </summary>
  public class DbServerInfo
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
    public bool IsClustered { get; set; }
    /// <summary>
    /// Wersja serwera
    /// </summary>
    public string Version { get; set; }
    /// <summary>
    /// Nazwa typy silnika
    /// </summary>
    public string TypeName { get; set; }
    /// <summary>
    /// Silnik, który podał tę informację
    /// </summary>
    public DbEngine Engine { get; set; }
    /// <summary>
    /// Identyfikator użytkownika (dla połączenia niezaufanego)
    /// </summary>
    public string UserID { get; set; }
    /// <summary>
    /// Hasło konieczne dla połączenia niezaufanego ew. do szyfrowania bazy danych
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// Połączenie do serwera
    /// </summary>
    public DbConnection Connection { get; set; }
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
}
