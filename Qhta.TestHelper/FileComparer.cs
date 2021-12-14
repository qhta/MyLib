using System;
using System.Diagnostics;

namespace Qhta.TestHelper
{

  public abstract class FileComparer
  {
    protected FileCompareOptions Options { get; init; }
    protected TextWriterTraceListener Listener {get; init; }

    public FileComparer(FileCompareOptions options, TextWriterTraceListener listener = null)
    {
      Options = options;
      Listener = listener;
    }

    public abstract bool CompareFiles(string outFilename, string expFilename);

    protected bool AreEqual(string line1, string line2)
    {
      line1 = line1.Trim();
      line2 = line2.Trim();
      if (Options.IgnoreCase)
        return line1.Equals(line2, StringComparison.InvariantCultureIgnoreCase);
      else
        return line1.Equals(line2);
    }

    protected bool AreEqual(int? val1, int? val2)
    {
      if (val1 == null || val2 == null)
        return true;
      return (int)val1 == (int)val2;
    }


    protected bool AreEqual(int val1, int val2)
    {
      return val1 == val2;
    }

    protected void ShowLines(string[] lines, bool? isExpected = null)
    {
      foreach (var line in lines)
      {
        if (Listener != null)
          Listener.WriteLine(line);
      }
    }

    protected void ShowLine(string line, bool? isExp)
    {
      ShowLines(new[] { line }, isExp);
    }

    protected void ShowLine(string msg)
    {
      if (Listener != null)
        Listener.WriteLine(msg);
    }
  }
}
