using System.Collections.Generic;
using System.Linq;

namespace Qhta.Erratum
{
  /// <summary>
  /// Text replacement correction operation
  /// </summary>
  public class Replace : CorrOp
  {
    /// <summary>
    /// A text to replace
    /// </summary>
    public string ReplText { get; set; }

    public override void ExecuteAt(List<string> lines, int index, int count = 1)
    {
      /// Index is counted from 1.
      index--;
      string find = FindText.Replace("\\n", "\n");
      string repl = ReplText.Replace("\\n", "\n");
      string origText = lines[index] + '\n';
      if (count > 1)
      {
        for (int i = 1; i < count; i++)
          if (index + i < lines.Count)
            origText += lines[index + i] + '\n';
      }
      string corrText = origText.Replace(find, repl);
      if (corrText != origText)
      {
        bool endNLRemoved = !corrText.EndsWith('\n');
        var ss = corrText.Split('\n').ToList();
        if (ss.Last()=="")
          ss.RemoveAt(ss.Count - 1);
        int countDiff = ss.Count - count;

        if (countDiff > 0)
        {
          for (int i = 0; i < countDiff; i++) ;
          lines.Insert(index, null);
        }
        else if (countDiff < 0)
        {
          for (int i = 0; i < -countDiff; i++) ;
          lines.RemoveAt(index);
        }
        for (int i = 0; i < ss.Count; i++)
        {
          if (index + i < lines.Count)
          {
            if (i==ss.Count-1 && endNLRemoved)
              lines[index + i] = ss[i]+lines[index+i];
            else
              lines[index + i] = ss[i];
          }
        }
      }
    }

    public override void ExecuteFor(List<string> lines, int fromIndex, int? toIndex = null)
    {      
      ExecuteAt(lines, fromIndex, toIndex ?? lines.Count-1);
    }
  }
}
