using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.TestHelper
{
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

    public DataType Data { get; init; }

    public new Func<DataType?, TestResult>? ExecFunction { get; set; }

    public override TestResult Execute()
    {
      if (ExecFunction == null)
        throw new InternalException($"ExecFunction nust be specified to run these test case");
      Result = ExecFunction(Data);
      return Result;
    }
  }
}
