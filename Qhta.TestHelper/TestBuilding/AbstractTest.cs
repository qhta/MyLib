using IWshRuntimeLibrary;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using File = System.IO.File;

using System.Diagnostics.CodeAnalysis;
using System.Xml.Serialization;

namespace Qhta.TestHelper
{
  /// <summary>
  /// Basic abstract test class
  /// </summary>
  public abstract class AbstractTest : IDisposable
  {
    /// <summary>
    /// If output test results to trace
    /// </summary>
    public bool TraceOutputEnabled { get; set; }

    /// <summary>
    /// Filename to output test results
    /// </summary>
    public string? OutputToFile { get; set; }

    /// <summary>
    /// If failed test case breaks the whole test
    /// </summary>
    public bool BreakOnFail { get; set; }

    /// <summary>
    /// If failed test case breaks the whole test
    /// </summary>
    public bool ShowTestRunTime { get; set; }

    public ITraceTextWriter? TraceWriter { get; set; }

    /// <summary>
    /// Main method to run a test. Creates a TraceTextWriter to monitor a test and show results.
    /// First invokes <see cref="Init"/> method. 
    /// Then <see cref="Prepare"/>, <see cref="Execute"/> and <see cref="Finalize"/>.
    /// Additionally time of run is measured.
    /// </summary>
    /// <returns>True if test passes, false if failed</returns>
    public virtual bool Run()
    {
        if (!Init())
          return false;
        var testResult = Prepare(out var plannedTest);
        int doneTestCount = 0;
        int failTestCount = 0;
        var t1 = DateTime.Now;
        if (testResult && plannedTest!=null && plannedTest.Count() != 0)
        {
          testResult = Execute(plannedTest, out doneTestCount, out failTestCount);
        }
        if (!Finalize(plannedTest, doneTestCount, failTestCount))
          testResult = false;
        var t2 = DateTime.Now;
        TraceWriter?.WriteLine($"Total test execution time = {(t2 - t1).TotalSeconds} seconds");
        return testResult;
    }

    /// <summary>
    /// Creates trace text writer for test results.
    /// Console output is enabled by default.
    /// Other test output is controlled by properties.
    /// </summary>
    /// <returns></returns>
    protected virtual bool Init()
    {
      TraceWriter = new TraceTextWriter(OutputToFile, true, TraceOutputEnabled);
      return true;
    }

    /// <summary>
    /// Abstract test preparation 
    /// </summary>
    /// <param name="plannedTests"></param>
    /// <returns></returns>
    protected abstract bool Prepare(out TestCase[]? plannedTests);

    protected abstract bool Execute(TestCase[] plannedTests, out int doneTestsCount, out int failedTestsCount);

    protected virtual bool Finalize(TestCase[]? plannedTests, int doneTestsCount, int failedTestsCount)
    {
      TraceWriter?.WriteLine();
      var plannedTestsCount = plannedTests?.Length ?? 0;
      if (plannedTestsCount == 0)
      {
        TraceWriter?.WriteLine("No test planned");
        return false;
      }
      if (plannedTestsCount != doneTestsCount)
        TraceWriter?.WriteLine($"{plannedTestsCount} tests planned but {doneTestsCount} run.");
      else
        TraceWriter?.WriteLine($"{doneTestsCount} tests run.");
      return true;
    }

    private bool disposedValue1;

    protected virtual void Dispose(bool disposing)
    {
      if (!disposedValue1)
      {
        if (disposing)
        {
          OnDispose();
        }

        disposedValue1 = true;
      }
    }

    public void Dispose()
    {
      Dispose(disposing: true);
      GC.SuppressFinalize(this);
    }

    protected virtual void OnDispose()
    {
      TraceWriter?.Dispose();
    }
  }
}
