namespace Qhta.DbUtils
{
  /// <summary>
  /// Informacja o silnikach danych <see cref="DbUtilities.EnumerateEngineClasses"/>
  /// </summary>
  public class DbEngineInfo
  {
    /// <summary>
    /// Niezmienny identyfikator
    /// </summary>
    public string ID { get; set; }
    /// <summary>
    /// Przyjazna nazwa silnika
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Opis silnika
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Pełna nazwa silnika
    /// </summary>
    public string AssemblyQualifiedName { get; set; }

  }
}
