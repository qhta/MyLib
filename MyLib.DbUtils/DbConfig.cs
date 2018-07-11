using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.ComponentModel;
using System.Xml;
using System.Text;
using System.Security.Cryptography;
using MyLib.GuidUtils;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Sekcja konfiguracji, w której zapisywane są komponenty dostarczające dane
  /// </summary>
  public class DataProvidersSection : ConfigurationSection
  {
    /// <summary>
    /// Nazwa, pod którą sekcja jest widoczna
    /// </summary>
    public const string Name = "DataProviders";

    /// <summary>
    /// Konstruktor domyślny
    /// </summary>
    public DataProvidersSection() { }

    /// <summary>
    /// Dostęp do elementu reprezentującego kolekcję dostawców danych
    /// </summary>
    [ConfigurationProperty("", IsRequired = false, IsDefaultCollection = true)]
    [ConfigurationCollectionAttribute(typeof(DbProvidersCollection),
        AddItemName = "DataProvider")]
    public DbProvidersCollection Providers
    {
      get { return (DbProvidersCollection)base[""]; }
      set { base[""] = value; }
    }

    /// <summary>
    /// Nazwa domyślnego dostawcy danych
    /// </summary>
    [ConfigurationProperty("defaultProvider", IsRequired = false)]
    public string DefaultProvider 
    {
      get { return (string)this["defaultProvider"]; }
      set { this["defaultProvider"] = value; }
    }

  }

  /// <summary>
  /// Kolekcja elementów reprezentujących dostawców danych
  /// </summary>
  public class DbProvidersCollection : ConfigurationElementCollection, IEnumerable<DbProvider>
  {
    /// <summary>
    /// Konstruktor domyślny
    /// </summary>
    public DbProvidersCollection() 
    {
    }

    /// <summary>
    /// Nieemitowany tag <c>clear</c>
    /// </summary>
    protected override bool SerializeElement(XmlWriter writer, bool serializeCollectionKey)
    {
      EmitClear = false;
      return base.SerializeElement(writer, serializeCollectionKey);
    }

    /// <summary>
    /// Tworzenie nowego elementu składowego w kolekcji
    /// </summary>
    /// <returns></returns>
    protected override ConfigurationElement CreateNewElement()
    {
      return new DbProvider();
    }

    /// <summary>
    /// Dostęp do nazwy elementu
    /// </summary>
    protected override Object GetElementKey(ConfigurationElement element)
    {
      return ((DbProvider)element).FullName;
    }

    /// <summary>
    /// Dostęp do elementu przez indeks
    /// </summary>
    public DbProvider this[int index]
    {
      get { return (DbProvider)BaseGet(index); }
      set
      {
        if (BaseGet(index) != null)
        {
          BaseRemoveAt(index);
        }
        BaseAdd(index, value);
      }
    }

    /// <summary>
    /// Dostęp do elementu przez nazwę
    /// </summary>
    new public DbProvider this[string Name]
    {
      get { return (DbProvider)BaseGet(Name);  }
    }

    /// <summary>
    /// Indeks elementu
    /// </summary>
    public int IndexOf(DbProvider item)
    {
      return BaseIndexOf(item);
    }

    /// <summary>
    /// Nadpisanie metody <c>Clear</c> powoduje usunięcie elementu <c>clear</c>
    /// </summary>
    public void Clear()
    {
      BaseClear();
      EmitClear = false;
    }

    /// <summary>
    /// Dodanie elementu
    /// </summary>
    public void Add(DbProvider item)
    {
      BaseAdd(item);
    }

    /// <summary>
    /// Dodanie elementu - przykrycie metody bazowej
    /// </summary>
    protected override void BaseAdd(ConfigurationElement item)
    {
      BaseAdd(item, false);
    }

    /// <summary>
    /// Usunięcie elementu
    /// </summary>
    public void Remove(DbProvider item)
    {
      if (BaseIndexOf(item) >= 0)
        BaseRemove(item.FullName);
    }

    /// <summary>
    /// Usunięcie elementu o podanym indeksie
    /// </summary>
    public void RemoveAt(int index)
    {
      BaseRemoveAt(index);
    }

    /// <summary>
    /// Usunięcie elementu o podanej nazwie
    /// </summary>
    public void Remove(string name)
    {
      BaseRemove(name);
    }

    IEnumerator<DbProvider> IEnumerable<DbProvider>.GetEnumerator()
    {
      foreach (object item in this)
        yield return item as DbProvider;
    }

  }

  /// <summary>
  /// Element konfiguracji reprezentujący dostawcę danych
  /// </summary>
  public class DbProvider : ConfigurationElement
  {
    private string NullWhenEmpty(string value)
    {
      if (value == "")
        return null;
      return value;
    }

    /// <summary>
    /// Konstruktor domyślny
    /// </summary>
    public DbProvider() {}

    /// <summary>
    ///  Konstruktor tworzący instancję na podstawie informacji o dostawcy danych z systemu
    /// </summary>
    /// <param name="info"></param>
    public DbProvider(DbProviderInfo info)
    {
      Kind = info.Kind;
      FullName = info.FullName;
      ShortName = info.ShortName;
      Engine = info.Engine;
      Version = info.Version;
      FileExtensions = info.FileExtensions;
      ID = CreateHash();
    }

    /// <summary>
    ///  Konstruktor tworzący instancję na podstawie informacji o dostawcy danych z systemu
    ///  i źródła danych SQL
    /// </summary>
    /// <param name="info">informacja o dostawcy danych z systemu</param>
    /// <param name="ds">źródło danych SQL</param>
    public DbProvider(DbProviderInfo info, SqlDataSourceInfo ds)
    {
      Kind = info.Kind;
      FullName = info.FullName;
      ShortName = info.ShortName;
      Engine = info.Engine;
      Version = info.Version;
      FileExtensions = info.FileExtensions;
      DataSource = ds.ToString();
      ID = CreateHash();
    }

    private string CreateHash()
    {
      StringBuilder sb = new StringBuilder();
      sb.Append(Engine.ToString());
      sb.Append(Kind.ToString());
      sb.Append(ShortName);
      sb.Append(FullName);
      sb.Append(Version);
      sb.Append(FileExtensions);
      sb.Append(DataSource);
      sb.Append(ClsID);
      string input = sb.ToString();
      Guid result = GuidTools.HashGuid(input);
      return result.ToString();
    }

    /// <summary>
    /// Nazwa dostawcy (wewnątrz aplikacji)
    /// </summary>
    [ConfigurationProperty("ID", IsRequired = true, IsKey = true)]
    public string ID
    {
      get { return (string)this["ID"]; }
      set { this["ID"] = value; }
    }

    /// <summary>
    /// Pełna nazwa dostawcy
    /// </summary>
    [ConfigurationProperty("fullName", IsRequired = true)]
    public string FullName
    {
      get { return (string)this["fullName"]; }
      set { this["fullName"] = value; }
    }


    /// <summary>
    /// Typ (rodzaj) źródła danych
    /// </summary>
    [ConfigurationProperty("kind", IsRequired = true)]
    public ProviderKind Kind
    {
      get 
      {
        if (this["kind"] is ProviderKind)
          return (ProviderKind)this["kind"];
        else
          return (ProviderKind)Enum.Parse(typeof(ProviderKind), (string)this["kind"]);
      }
      set { this["kind"] = value.ToString(); }
    }

    /// <summary>
    /// Silnik źródła danych
    /// </summary>
    [ConfigurationProperty("engine", IsRequired = true, DefaultValue=DbEngine.Unknown)]
    [DefaultValue (false)]
    public DbEngine Engine
    {
      get 
      {
        if (this["engine"] is DbEngine)
          return (DbEngine)this["engine"];
        else
          return (DbEngine)Enum.Parse(typeof(DbEngine), (string)this["engine"]);
      }
      set { this["engine"] = value; }
    }

    /// <summary>
    /// Wersja źródła danych
    /// </summary>
    [ConfigurationProperty("version")]
    public string Version
    {
      get { return (string)this["version"]; }
      set { this["version"] = value; }
    }

    /// <summary>
    /// Źródło danych (nazwa\instancja serwera)
    /// </summary>
    [ConfigurationProperty("dataSource")]
    public string DataSource
    {
      get { return (string)this["dataSource"]; }
      set { this["dataSource"] = value; }
    }

    /// <summary>
    /// Dostawca danych (OleDB)
    /// </summary>
    [ConfigurationProperty("shortName")]
    public string ShortName
    {
      get { return NullWhenEmpty((string)this["shortName"]); }
      set { this["shortName"] = value; }
    }

    /// <summary>
    /// Identyfikator klasy (z rejestru)
    /// </summary>
    [ConfigurationProperty("clsID")]
    public string ClsID
    {
      get { return NullWhenEmpty((string)this["clsID"]); }
      set { this["clsID"] = value; }
    }

    /// <summary>
    /// rozszerzenia nazw plików (gdy kilka, to oddzielone przecinkami lub średnikami)
    /// </summary>
    [ConfigurationProperty("fileExtensions")]
    public string FileExtensions
    {
      get { return  NullWhenEmpty((string)this["fileExtensions"]); }
      set { this["fileExtensions"] = value; }
    }

    /// <summary>
    /// Podaje przyjazną nazwę
    /// </summary>
    public override string ToString()
    {
      string result = FullName;
      if (!String.IsNullOrEmpty(ShortName))
        result += " [" + ShortName + "]";
      else if (Kind == ProviderKind.Embedded && !String.IsNullOrEmpty(DataSource))
      {
        string s = DataSource;
        s = s.Replace(".", "localhost");
        result += " [" + s + "]";
      }
      if (!String.IsNullOrEmpty(FileExtensions))
        result += " (" + FileExtensions + ")";
      return result;
    }
  }


}