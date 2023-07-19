using System;

namespace Qhta.TestHelper;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class TestCase<DataType> : TestCase
{

  public TestCase(string name, DataType data) : base(name)
  {
    Data = data;
  }

  public TestCase(string name, DataType data, Func<DataType?, TestResult> execFunc): base(name)
  {
    Data = data;
    ExecFunction = execFunc;
  }

  public DataType Data { get; private set; }

  public new Func<DataType?, TestResult>? ExecFunction { get; set; }

  public override TestResult Execute()
  {
    if (ExecFunction == null)
      throw new InvalidOperationException($"ExecFunction nust be specified to run these test case");
    Result = ExecFunction(Data);
    return Result;
  }
}