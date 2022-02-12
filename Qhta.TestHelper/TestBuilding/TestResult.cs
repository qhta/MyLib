namespace Qhta.TestHelper
{
  public record TestResult
  {
    public TestResult(bool success, string message)
    {
     Success = success;
      Message = message;
    }

    public bool Success { get; init;}
    public string Message {get; init; }
  }
}
