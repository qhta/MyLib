using System.Collections.Generic;

namespace Qhta.Erratum
{

  /// <summary>
  /// Abstract correction operation
  /// </summary>
  public abstract class CorrOp
  {
    /// <summary>
    /// A text to find. 
    /// All errata entries must have a text to find to avoid duplicate operation
    /// in the file that was just corrected.
    /// </summary>
    public string FindText { get; init; }

    public abstract void ExecuteAt(List<string> lines, int index, int count=1);

    public abstract void ExecuteFor(List<string> lines, int fromIndex, int? ToIndex = null);
  }
}
