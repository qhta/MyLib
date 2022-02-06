using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.TestHelper
{
  public class TestCase
  {
    public TestCase(string name)
    {
      Name = name;
    }
    public TestCase(string name, Func<bool> execFunc)
    {
      Name = name;
      ExecFunction = execFunc;
    }

    public string Name { get; init; }

    public bool? Result { get; set; }


    public Func<bool>? ExecFunction { get; set; }

    public virtual bool Execute()
    {
      if (ExecFunction == null)
        throw new InternalException($"ExecFunction nust be specified to run these test case");
      Result = ExecFunction();
      return (bool)Result;
    }
  }
}
