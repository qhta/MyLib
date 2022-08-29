using System;

namespace Qhta.TestHelper;

public class TestCase
{
  public TestCase(string name)
  {
    Name = name;
  }
  public TestCase(string name, Func<TestResult> execFunc)
  {
    Name = name;
    ExecFunction = execFunc;
  }

  public string Name { get; init; }

  public TestResult? Result { get; set; }

  public Func<TestResult>? ExecFunction { get; set; }

  public virtual TestResult Execute()
  {
    if (ExecFunction == null)
      throw new InternalException($"ExecFunction nust be specified to run these test case");
    Result = ExecFunction();
    return Result;
  }
}