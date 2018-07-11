using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Runtime.Serialization;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Klasa danych do przekazywania informacji o dostępnych serwerach danych
  /// </summary>
  [DataContract]
  public class DataProvider
  {
    ///// <summary>
    ///// Konstruktor domyślny
    ///// </summary>
    //public DataProvider() {}

    ///// <summary>
    ///// Konstruktor z nazwą
    ///// </summary>
    //public DataProvider(string name) { Name = name; }
    
    ///// <summary>
    /////  Konstruktor tworzący instancję na podstawie informacji z konfiguracji
    ///// </summary>
    ///// <param name="info"></param>
    //public DataProvider(MyLib.DbUtils.DbProvider info)
    //{
    //  Kind = (ProviderKind)info.Kind;
    //  Name = info.Description;
    //  Provider = info.Name;
    //  Engine = (DbEngine)info.Engine;
    //  Version = info.Version;
    //  FileExtensions = info.FileExtensions;
    //}

    ///// <summary>
    /////  Konstruktor tworzący instancję na podstawie informacji o dostawcy danych z systemu
    ///// </summary>
    ///// <param name="info"></param>
    //public DataProvider(MyLib.DbUtils.DbProviderInfo info)
    //{
    //  Kind = (ProviderKind)info.Kind;
    //  Name = info.Description;
    //  Provider = info.Name;
    //  Engine = (DbEngine)info.Engine;
    //  Version = info.Version;
    //  FileExtensions = info.FileExtensions;
    //}

    ///// <summary>
    /////  Konwersja na element konfiguracji
    ///// </summary>
    ///// <param name="info"></param>
    //public static explicit operator MyLib.DbUtils.DbProvider(DataProvider info)
    //{
    //  return new MyLib.DbUtils.DbProvider
    //  {
    //    Kind = (MyLib.DbUtils.ProviderKind)info.Kind,
    //    Name = info.Description,
    //    Provider = info.Name,
    //    Engine = (MyLib.DbUtils.DbEngine)info.Engine,
    //    Version = info.Version,
    //    FileExtensions = info.FileExtensions,
    //  };
    //}

    /// <summary>
    /// Nazwa dostawcy (wewnątrz aplikacji)
    /// </summary>
    [DataMember]
    public string Name { get; set; }

    /// <summary>
    /// Opis dostawcy
    /// </summary>
    [DataMember]
    public string Description { get; set; }

    /// <summary>
    /// Typ (rodzaj) źródła danych
    /// </summary>
    [DataMember]
    public ProviderKind Kind { get; set; }

    /// <summary>
    /// Silnik źródła danych
    /// </summary>
    [DataMember]
    public DbEngine Engine { get; set; }

    /// <summary>
    /// Wersja źródła danych
    /// </summary>
    [DataMember]
    public string Version { get; set; }

    /// <summary>
    /// Źródło danych (nazwa\instancja serwera)
    /// </summary>
    [DataMember]
    public string DataSource { get; set; }

    /// <summary>
    /// Dostawca danych (OleDB)
    /// </summary>
    [DataMember]
    public string Provider { get; set; }

    /// <summary>
    /// Identyfikator klasy (z rejestru)
    /// </summary>
    [DataMember]
    public string ClsID { get; set; }

    /// <summary>
    /// rozszerzenia nazw plików (gdy kilka, to oddzielone przecinkami lub średnikami)
    /// </summary>
    [DataMember]
    public string FileExtensions { get; set; }

  }
}