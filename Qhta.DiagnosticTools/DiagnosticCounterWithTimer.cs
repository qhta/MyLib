using System.Security.Cryptography.X509Certificates;

namespace Qhta.DiagnosticTools;

/// <summary>
/// This record represents a diagnostic count, which includes a total count and a dictionary of sub counters along with their associated timing information.  
/// </summary>
public record DiagnosticCounterWithTimer()
{
  public int Count => SubCounters.Sum(item => item.Value.Count);


  public double Time => SubCounters.Sum(item => item.Value.Time);

  public Dictionary<string, (int Count, double Time)> SubCounters { get; } = new ();
}