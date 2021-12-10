using System;
using System.Diagnostics;

namespace Qhta.TestHelper
{

  public abstract class FileComparer
  {
    protected FileCompareOptions Options { get; init; }

    public FileComparer(FileCompareOptions options)
    {
      Options = options;
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
      if (Options.WriteToConsole)
      {
        if (isExpected != null)
        {
          if ((bool)isExpected)
            Console.ForegroundColor = ConsoleColor.Green;
          else
            Console.ForegroundColor = ConsoleColor.Red;
        }
        else
          Console.ResetColor();
        foreach (var line in lines)
        {
          if (Options.WriteToConsole)
            Console.WriteLine(line);
          if (Options.WriteToDebug)
            Debug.WriteLine(line);
        }
        if (Options.WriteToConsole)
          Console.ResetColor();
      }
    }

    protected void ShowLine(string line, bool? isExp)
    {
      ShowLines(new[] { line }, isExp);
    }

    protected void ShowLine(string msg)
    {
      if (Options.WriteToConsole)
      {
        Console.ResetColor();
        Console.WriteLine(msg);
      }
      if (Options.WriteToDebug)
      {
        Debug.WriteLine(msg);
      }
    }
  }
}
