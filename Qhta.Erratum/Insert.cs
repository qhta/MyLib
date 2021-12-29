using System.Collections.Generic;

namespace Qhta.Erratum
{
  /// <summary>
  /// Line insert correction operator
  /// </summary>
  public class Insert: CorrOp
  {
    /// <summary>
    /// A text to insert
    /// </summary>
    public string InsText { get; set; }

    public override void ExecuteAt(List<string> lines, int index, int count = 1)
    {
      throw new System.NotImplementedException();
    }

    public override void ExecuteFor(List<string> lines, int fromIndex, int? ToIndex = null)
    {
      throw new System.NotImplementedException();
    }
  }
}
