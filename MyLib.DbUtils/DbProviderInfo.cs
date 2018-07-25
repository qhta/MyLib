using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.DbUtils
{
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
    public DbEngineKind Engine { get; set; }
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
}
