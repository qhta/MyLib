using System.Collections.Generic;

namespace Qhta.Erratum
{
  /// <summary>
  /// Line remove correction operation
  /// </summary>
  public class Remove: CorrOp
  {
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
