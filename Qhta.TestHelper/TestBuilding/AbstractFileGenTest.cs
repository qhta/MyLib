using IWshRuntimeLibrary;

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

using File = System.IO.File;

namespace Qhta.TestHelper;

/// <summary>
/// Abstract test with file generating. Contains output file comparison with expected content.
/// </summary>
public abstract class AbstractFileGenTest : AbstractTest
{
  protected static string testDir { get; set; }

  protected static XmlSerializer? serializer { get; set; }
  protected static FileCompareOptions writeCompareOptions { get; set; }
    = new FileCompareOptions { SyncLimit = 20, DiffLimit = 3, IgnoreAttributesOrder = true };

  static AbstractFileGenTest()
  {
    string filepath = Assembly.GetExecutingAssembly().Location;
    while (Path.GetFileName(filepath) != "bin")
      filepath = Path.GetDirectoryName(filepath) ?? "";
    filepath = Path.GetDirectoryName(filepath) ?? "";
    testDir = Path.Combine(filepath, "TestFiles");
  }


  protected override bool Init()
  {
    if (AcceptOnFail)
    {
      Console.WriteLine("Warning! AcceptOnFail option is set. All new results will override expected results (if different).");
      Console.Write("Are you sure you want to accept new results? (y/n)");
      var key = Console.ReadKey();
      if (Char.ToLower(key.KeyChar) != 'y')
        AcceptOnFail = false;
      Console.WriteLine();
    }
    return base.Init();
  }

  protected override bool Finalize(TestCase[]? plannedTests, int doneTestsCount, int failedTestsCount)
  {
    base.Finalize(plannedTests, doneTestsCount, failedTestsCount);
    if (doneTestsCount > 0)
    {
      if (failedTestsCount > 0)
      {
        if (AcceptOnFail)
        {
          TraceWriter?.WriteLine($"New results accepted for {failedTestsCount} tests.");
          TraceWriter?.Flush();
          return false;
        }
        else
        {
          TraceWriter?.WriteLine($"{failedTestsCount} tests failed.");
          TraceWriter?.Flush();
          return false;
        }
      }
      else
      {
        TraceWriter?.WriteLine($"All tests passed.");
        TraceWriter?.Flush();
        return true;
      }
    }
    return true;
  }

  protected bool CompareResultFiles(string filename, string expFilename, string outFilename, bool acceptOnFail)
  {
    if (File.Exists(expFilename))
    {
      if (!new XmlFileComparer(writeCompareOptions, TraceWriter).CompareFiles(outFilename, expFilename))
      {
        TraceWriter?.WriteLine($"Parse result for \"{filename}\" compare FAILED");
        if (acceptOnFail)
        {
          File.Copy(outFilename, expFilename, true);
          TraceWriter?.WriteLine($"New result for \"{filename}\" ACCEPTED");
        }
        return false;
      }
      else
      {
        TraceWriter?.WriteLine($"Parse result for \"{filename}\" compare PASSED");
        return true;
      }
    }
    else
    {
      TraceWriter?.WriteLine($"Parse result for \"{filename}\" compare NOT FOUND");
      if (acceptOnFail)
      {
        File.Copy(outFilename, expFilename);
        TraceWriter?.WriteLine($"New result for \"{filename}\" ACCEPTED");
      }
      return true;
    }
  }

  public static void CreateLink(string linkPath, [NotNull] string targetPath)
  {
    WshShell wsh = new WshShell();
    IWshShortcut shortcut = (IWshShortcut)wsh.CreateShortcut(linkPath);
    shortcut.TargetPath = targetPath;
    shortcut.Save();
  }

  public static string GetLnkTarget(string lnkPath)
  {
    WshShell wsh = new WshShell();
    IWshShortcut shortcut = (IWshShortcut)wsh.CreateShortcut(lnkPath);
    return shortcut.TargetPath;
  }

}