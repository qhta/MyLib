namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// View model for boolean operation selection.
/// </summary>
public class BooleanOperationViewModel
{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="displayName"></param>
  /// <param name="operation"></param>
  public BooleanOperationViewModel(string displayName, BooleanOperation operation)
  {
    DisplayName = displayName;
    Operation = operation;
  }

  /// <summary>
  /// Display name.
  /// </summary>
  public string DisplayName { get; set; }

  /// <summary>
  /// Operation to select.
  /// </summary>
  public BooleanOperation Operation { get; set;}
}
