using System.Security.Cryptography.X509Certificates;

namespace Qhta.DiagnosticTools;

/// <summary>
/// This record represents a diagnostic count, which includes a total count and a dictionary of sub counters.  
/// </summary>
public record DiagnosticCounter()
{
  public int Count => SubCounters.Sum(item => item.Value);
  public Dictionary<string, int> SubCounters { get; } = new Dictionary<string, int>();
}