using System.Runtime.Serialization;

namespace Qhta.DbUtils
{
  /// <summary>
  /// Typ dostawcy danych
  /// </summary>
  [DataContract]
  public enum ProviderKind
  {
    /// <summary>
    /// Nieznany - nieokreślony
    /// </summary>
    [EnumMember]
    Unknown,
    /// <summary>
    /// Dostawca wbudowany w aplikację
    /// </summary>
    [EnumMember]
    Embedded,
    /// <summary>
    /// Wykorzystywany mechanizm OleDb
    /// </summary>
    [EnumMember]
    OleDb,
    /// <summary>
    /// Wykorzystywany mechanizm ODBC
    /// </summary>
    [EnumMember]
    Odbc,
  }
}
