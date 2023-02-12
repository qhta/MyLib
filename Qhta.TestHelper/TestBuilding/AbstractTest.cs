
using System;
using System.Linq;

namespace Qhta.TestHelper;

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
  public bool AcceptOnFail { get; set; }

  /// <summary>
  /// If failed test case breaks the whole test
  /// </summary>
  public bool ShowTestRunTime { get; set; }

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

  public TraceTextWriter? TraceWriter { get; set; }

  public string TestCasesStr { get; set; } = "test cases";

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
    if (testResult && plannedTest != null && plannedTest.Count() != 0)
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
  /// Abstract test preparation. Output parameter <paramref name="plannedTests"/> must be filled.
  /// If not - return false
  /// </summary>
  /// <param name="plannedTests">Result collection of planned test cases</param>
  /// <returns>if result collection prepared</returns>
  protected abstract bool Prepare(out TestCase[]? plannedTests);

  /// <summary>
  /// Abstract test execution. Planned test cases are given as input parameter.
  /// Result is number of test cases that were run and number of test cases failed.
  /// </summary>
  /// <param name="plannedTests">Collection od planned test cases</param>
  /// <param name="doneTestsCount">Number of planned test cases that were run</param>
  /// <param name="failedTestsCount">Number of failed test cases</param>
  /// <returns>If all the test cases were run succesfully</returns>
  protected abstract bool Execute(TestCase[] plannedTests, out int doneTestsCount, out int failedTestsCount);

  /// <summary>
  /// Virtual test finalization. Input parameters are: collection of planned test cases,
  /// and number of test cases that were run and number of test cases failed.
  /// Appriopriate messages are written to <see cref="TraceWriter"/>
  /// </summary>
  /// <param name="plannedTests">Collection od planned test cases</param>
  /// <param name="doneTestsCount">Number of planned test cases that were run</param>
  /// <param name="failedTestsCount">Number of failed test cases</param>
  /// <returns>False only if not tests were planned</returns>
  protected virtual bool Finalize(TestCase[]? plannedTests, int doneTestsCount, int failedTestsCount)
  {
    TraceWriter?.WriteLine();
    var plannedTestsCount = plannedTests?.Length ?? 0;
    if (plannedTestsCount == 0)
    {
      TraceWriter?.WriteLine($"No {TestCasesStr} planned");
      return false;
    }
    if (plannedTestsCount != doneTestsCount)
      TraceWriter?.WriteLine($"{plannedTestsCount} {TestCasesStr} planned but {doneTestsCount} run.");
    else
      TraceWriter?.WriteLine($"{doneTestsCount} {TestCasesStr} run.");

    if (failedTestsCount == 0)
      TraceWriter?.WriteLine($"All {TestCasesStr} passed.");
    else
    if (failedTestsCount == doneTestsCount)
      TraceWriter?.WriteLine($"All {TestCasesStr} failed.");
    else
      TraceWriter?.WriteLine($"{failedTestsCount} {TestCasesStr} failed.");

    return true;
  }

  #region IDisposed implementation
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
  #endregion

  /// <summary>
  /// Virtual method called on dispose
  /// </summary>
  protected virtual void OnDispose()
  {
    TraceWriter?.Dispose();
  }
}