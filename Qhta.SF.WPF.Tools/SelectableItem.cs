using System.Windows.Data;
using Syncfusion.Windows.Forms.Tools;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// Class representing a selectable item with a display name and selection state - for UI purposes.
/// </summary>
public class SelectableItem: ISelectableItem
{
  /// <inheritdoc />
  public string DisplayName
  {
    get
    {
      if (!string.IsNullOrEmpty(_DisplayName))
        return _DisplayName;
      if (Value!=null)
      {
        if (Value is ISelectableItem selectableItem)
          return selectableItem.DisplayName;
        if (ValueConverter != null)
          return ValueConverter.Convert(Value, typeof(string), null, null) as string ?? string.Empty;
        return Value.ToString() ?? string.Empty;
      }
      return string.Empty;
    }
    set => _DisplayName = value;
  } 
  private string _DisplayName  = String.Empty;


  /// <inheritdoc />
  public string? ToolTip
  {
    get
    {
      if (!string.IsNullOrEmpty(_ToolTip))
        return _ToolTip;
      if (Value != null)
      {
        if (Value is ISelectableItem selectableItem)
          return selectableItem.ToolTip;
        if (TooltipConverter != null)
          return TooltipConverter.Convert(Value, typeof(string), null, null) as string ?? string.Empty;
        return Value.ToString() ?? string.Empty;
      }
      return null;
    }
    set => _ToolTip = value;
  }
  private string? _ToolTip = null;

  /// <inheritdoc />
  public bool IsSelected { get; set; }

  /// <summary>
  /// Implementation of the <see cref="ISelectableItem.ActualValue"/> property.
  /// </summary>
  public object? ActualValue => Value;

  /// <summary>
  /// Value associated with the selectable item, which can be of any type.
  /// </summary>
  public object? Value { get; set; }

  /// <summary>
  /// Value converter for converting the value to a display string.
  /// </summary>
  public IValueConverter? ValueConverter { get; set; }

  /// <summary>
  /// Value converter for converting the value to a tooltip string.
  /// </summary>
  public IValueConverter? TooltipConverter { get; set; }
}