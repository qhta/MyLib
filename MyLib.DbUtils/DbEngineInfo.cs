using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.DbUtils
{
  /// <summary>
  /// Informacja o silnikach danych <see cref="DbTools.EnumerateEngineClasses"/>
  /// </summary>
  public class DbEngineInfo
  {
    /// <summary>
    /// Przyjazna nazwa silnika
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// Opis silnika
    /// </summary>
    public string Description { get; set; }
    /// <summary>
    /// Opis niezmienny
    /// </summary>
    public string InvariantName { get; set; }
    /// <summary>
    /// Pełna nazwa silnika
    /// </summary>
    public string AssemblyQualifiedName { get; set; }
  }
}
