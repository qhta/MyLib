using System;
using System.Runtime.CompilerServices;

namespace Qhta.TestHelper
{
  public class InternalException : Exception
  {
    public InternalException(string message, Exception? innerException = null,
      [CallerMemberName] string? methodName = null) : base(message + $" in {methodName}", innerException)
    {
      MethodName = methodName;
    }

    public string? MethodName { get; init; }

  }
}
