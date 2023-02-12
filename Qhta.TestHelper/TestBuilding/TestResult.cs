namespace Qhta.TestHelper;
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

public record TestResult
{
  public TestResult(bool? success, string message)
  {
    Success = success;
    Message = message;
  }

  public bool? Success { get; init;}
  public string Message {get; init; }
}