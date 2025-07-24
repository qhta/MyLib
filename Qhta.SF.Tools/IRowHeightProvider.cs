using System.Diagnostics;

namespace Qhta.SF.Tools;

/// <summary>
/// Interface for providing row height in a data grid.
/// </summary>
public interface IRowHeightProvider
{
  /// <summary>
  /// Gets or sets the height of a row in the grid.
  /// </summary>
  public double RowHeight { [DebuggerStepThrough] get; set; }
}