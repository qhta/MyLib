using System;
using System.Runtime.CompilerServices;

namespace Qhta.TestHelper;

#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
public class InvalidOperationException : Exception
{
  public InvalidOperationException(string message, Exception? innerException = null,
    [CallerMemberName] string? methodName = null) : base(ComposeMessage(message, methodName), innerException)
  {
    MethodName = methodName;
  }

  protected static string ComposeMessage(string message, string? methodName)
    => message + $" in {methodName}";

  public string? MethodName { get; private set; }

}