using Qhta.TestHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#nullable enable

namespace Qhta.Erratum
{
  public class Changes: Stack<Change>
  {
    public ITraceWriter? ChangeWriter { get; set; }

    public void Add(int pos, string? text, string? replacement)
    {
      Push (new Change (pos, text, replacement));
      if (ChangeWriter != null)
      {
        ChangeWriter.Write($"Source changed at pos {pos}: ");
        if (text!=null && replacement!=null)
          ChangeWriter.Write ($"\"{text.Replace("\n","\\n")}\" replaced with \"{replacement.Replace("\n", "\\n")}\".");
        else if (replacement!=null)
          ChangeWriter.WriteLine($"\"{replacement.Replace("\n", "\\n")}\" inserted.");
        else if (text != null)
          ChangeWriter.WriteLine($"\"{text.Replace("\n", "\\n")}\" removed.");
      }
    }
  }
}
