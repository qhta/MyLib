namespace Qhta.DbUtils
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
    public string Name { get; set; }
    /// <summary>
    /// Opis dostawcy
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Rodzaj dostawcy
    /// </summary>
    public ProviderKind Kind { get; set; }
    /// <summary>
    /// ID Silnika bazy danych
    /// </summary>
    public string Type { get; set; }
    /// <summary>
    /// Wersja dostawcy
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
