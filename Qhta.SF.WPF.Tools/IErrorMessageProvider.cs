using System.Diagnostics;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// Interface for providing error message.
/// </summary>
public interface IErrorMessageProvider
{
  /// <summary>
  /// Gets or sets an error message.
  /// </summary>
  public string? ErrorMessage { [DebuggerStepThrough] get; set; }
}