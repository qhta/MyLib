using System.Collections.Generic;

namespace Qhta.WPF.Converters
{
  /// <summary>
  /// Dictionary of Colors indexed by string names.
  /// Used in <see cref="ValidityBrushConverter"/>.
  /// </summary>
  public class ColorDictionary: Dictionary<string, System.Windows.Media.Color>
  {
  }
}
