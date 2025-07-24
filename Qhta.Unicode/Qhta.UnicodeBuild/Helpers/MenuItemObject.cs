using System.Diagnostics;
using System.Windows;
using System.Windows.Input;

namespace Qhta.UnicodeBuild.Helpers;

/// <summary>
/// MenuItemObject is a class that represents a menu item with properties for header, icon, tooltip, command, and command parameter.
/// </summary>
public class MenuItemObject: DependencyObject
{
  /// <summary>
  /// Header of the menu item, which can be a string or null.
  /// </summary>
  public string? Header { [DebuggerStepThrough] get; set; }
  /// <summary>
  /// An icon for the menu item, which can be an object (e.g., an image or a font icon) or null.
  /// </summary>
  public object? Icon { [DebuggerStepThrough] get; set; }
  /// <summary>
  /// ToolTip for the menu item, which can be a string or null.
  /// </summary>
  public string? ToolTip { [DebuggerStepThrough] get; set; }

  /// <summary>
  /// DependencyProperty for the Command property.
  /// </summary>
  public static DependencyProperty CommandProperty = DependencyProperty.Register(
    nameof(Command),
    typeof(ICommand),
    typeof(MenuItemObject),
    new PropertyMetadata(null));

  /// <summary>
  /// Command associated with the menu item, which can be an ICommand or null.
  /// </summary>
  public ICommand? Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }

  /// <summary>
  /// CommandParameter for the Command, which can be any object or null.
  /// </summary>
  public object? CommandParameter { [DebuggerStepThrough] get; set; }
}