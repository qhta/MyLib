using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.TestHelper
{
  public class FuncTestCase<DataType> : TestCase<DataType>
  {

    public FuncTestCase(string name, DataType data, Func<DataType?, bool> execFunc) : base(name, data)
    {
      ExecFunc = execFunc;
    }

    public Func<DataType?, bool> ExecFunc { get; set; }

    public override bool Execute()
    {
      Result = ExecFunc(Data);
      return (bool)Result;
    }
  }
}
