using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.TestHelper
{
  public static class TestUtils
  {
    public static string? MethodName([CallerMemberName] string? callerName = null)
    { 
      return callerName;
    }

    public static void Stop([CallerMemberName] string? callerName = null)
    { }
  }
}
