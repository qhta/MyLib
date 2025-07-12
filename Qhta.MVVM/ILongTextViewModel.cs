namespace Qhta.MVVM;

/// <summary>
/// View model interface for handling long text properties.
/// </summary>
public interface ILongTextViewModel
{
  /// <summary>
  /// Long text property value that can be expanded or collapsed.
  /// </summary>
  public string? LongText { get; set; }

  /// <summary>
  /// Gets a value indicating whether long text can be expanded.
  /// </summary>
  public bool CanExpandLongText { get; }

  /// <summary>
  /// Gets or sets a value indicating whether the long text is expanded.
  /// </summary>
  public bool IsLongTextExpanded { get; set; }
}