using System;
using System.Runtime.CompilerServices;

namespace Qhta.TestHelper;

public class InternalException : Exception
{
  public InternalException(string message, Exception? innerException = null,
    [CallerMemberName] string? methodName = null) : base(ComposeMessage(message, methodName), innerException)
  {
    MethodName = methodName;
  }

  protected static string ComposeMessage(string message, string? methodName)
    => message + $" in {methodName}";

  public string? MethodName { get; init; }

}