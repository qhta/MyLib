using System.Security.Cryptography.X509Certificates;

namespace Qhta.DiagnosticTools;

/// <summary>
/// This record represents a diagnostic count, which includes a total count and a dictionary of caller counts.  
/// </summary>
public record DiagnosticCounter()
{
  public int Count => Callers.Sum(item => item.Value);
  public Dictionary<string, int> Callers { get; } = new Dictionary<string, int>();
}