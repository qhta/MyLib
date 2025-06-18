using System.Windows;
using System.Windows.Input;

namespace Qhta.UnicodeBuild.Helpers;

public class MenuItemObject: DependencyObject
{
  public string? Header { get; set; }
  public object? Icon { get; set; }
  public string? ToolTip { get; set; }
  
  public static DependencyProperty CommandProperty = DependencyProperty.Register(
    nameof(Command),
    typeof(ICommand),
    typeof(MenuItemObject),
    new PropertyMetadata(null));

  public ICommand? Command { get => (ICommand)GetValue(CommandProperty); set => SetValue(CommandProperty, value); }
  public object? CommandParameter { get; set; }
}