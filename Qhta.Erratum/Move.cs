using System.Collections.Generic;

namespace Qhta.Erratum
{
  /// <summary>
  /// Line move correction operation
  /// </summary>
  public class Move: CorrOp
  {
    /// <summary>
    /// Distance to move line or lines.
    /// if it is less than 0 - move uUp, otherwise move down.
    /// </summary>
    public int Distance { get; set; }

    public override void ExecuteAt(List<string> lines, int index, int count = 1)
    {
      /// Index is counted from 1.
      index--;
      string find = FindText.Replace("\\n", "\n");
      string origText = lines[index] + '\n';
      if (count > 1)
      {
        for (int i = 1; i <= count; i++)
          if (index + i < lines.Count)
            origText += lines[index + i] + '\n';
      }
      if (origText.StartsWith(find))
      {
        int delta = - Distance;
        if (delta < 0)
        {
          if (delta <= index+1)
          {
            for (int i=0; i< delta; i++)
              lines.RemoveAt(index);
          }
        }
        else
        {
          for (int i = 0; i < delta; i++)
            lines.Insert(index, null);
        }
      }
    }

    public override void ExecuteFor(List<string> lines, int fromIndex, int? ToIndex = null)
    {
      throw new System.NotImplementedException();
    }
  }
}
