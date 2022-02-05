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

    public string Name { get; init; }

    public bool? Result { get; set; }

    public virtual bool Execute()
    {
      throw new InternalException("Not implemented execute function in test case");
    }
  }
}
