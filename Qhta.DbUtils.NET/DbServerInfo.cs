﻿using System;
using System.Data.Common;

namespace Qhta.DbUtils
{
  /// <summary>
  /// Informacja o serwerach danych
  /// zwracana przez metodę <see cref="DbEngine.EnumerateServers"/>
  /// </summary>
  public class DbServerInfo
  {
    /// <summary>
    /// Nazwa widoczna
    /// </summary>
    public string ID { get; set; }
    /// <summary>
    /// Nazwa serwera (maszyny, na której serwer jest postawiony)
    /// </summary>
    public string ServerName { get; set; }
    /// <summary>
    /// Nazwa instancji serwera (domyślna instancja ma nazwę pustą)
    /// </summary>
    public string InstanceName { get; set; }
    /// <summary>
    /// Typ serwera: lokalny/zdalny
    /// </summary>
    public ServerType ServerType { get; set; }
    /// <summary>
    /// Wersja serwera
    /// </summary>
    public string Version { get; set; }
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
    /// Adres sieciowy tworzony z połączenia nazwy serwera i nazwy instancji
    /// </summary>
    /// <returns></returns>
    public string NetName
    {
      get
      {
        if (String.IsNullOrEmpty(InstanceName))
          return ServerName;
        else
          return ServerName + "\\" + InstanceName;
      }
    }

    /// <summary>
    /// Ścieżka dostępu do serwera
    /// </summary>
    public string Path => Engine.ID + ":" + NetName;
  }
}
