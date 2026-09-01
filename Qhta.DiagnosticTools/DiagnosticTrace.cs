namespace Qhta.DiagnosticTools;

using System.Diagnostics;
using System.Runtime.CompilerServices;


/// <summary>
/// Represents a context, including the member name and a reference to the previous context in the chain.
/// </summary>
public record Context(Context? Previous, Object? Data)
{

  /// <summary>
  /// Gets the previous context in the chain, or null if this is the first context.
  /// </summary>
  public Context? Previous { get; } = Previous;

  /// <summary>
  /// Gets the message associated with this context.
  /// </summary>
  public Object? Data { get; } = Data;

  /// <summary>
  /// Converts the diagnostic trace to a string representation, showing the chain of member names.
  /// </summary>
  /// <returns>A string representation of the diagnostic trace.</returns>
  public override string? ToString()
  {
    if (Previous!=null)
    {
      if (Data!=null)
        return $"{Previous} -> {Data}";
      return Previous.ToString();
    }
    return Data?.ToString();
  }


}