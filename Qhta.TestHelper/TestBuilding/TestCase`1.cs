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

    public DataType Data { get; init; }

  }
}
