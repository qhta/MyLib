using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyLib.WpfTestUtils
{
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public class RunAfterAttribute: Attribute
  {
    public RunAfterAttribute(string precedingMethod)
    {
      Name=precedingMethod;
    }
    public string Name { get; private set; }
  }
}
