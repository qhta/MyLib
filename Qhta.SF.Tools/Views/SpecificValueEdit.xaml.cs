using System.Windows;
using System.Windows.Controls;

namespace Qhta.SF.Tools.Views;

/// <summary>
/// Part of the <see cref="SpecificValueWindow"/> for edit a specific value in a text box.
/// </summary>
public partial class SpecificValueEdit : UserControl
{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public SpecificValueEdit()
  {
    InitializeComponent();
  }

  /// <summary>
  /// Handler for property changes. Needed for view reconfiguration on <see cref="SpecificValueWindow.Replace"/> change.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public void OnNotifyPropertyChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
  {
    if (sender is SpecificValueWindow window)
    {
      if (e.PropertyName == nameof(SpecificValueWindow.Replace))
      {
        // Reconfigure the view based on the Replace property.
        if (window.Replace)
        {
          ReplaceTextBox.Visibility = System.Windows.Visibility.Visible;
          TextEditGrid.RowDefinitions.Add(new RowDefinition{Height = new GridLength(1, GridUnitType.Star)});
        }
        else
        {
          ReplaceTextBox.Visibility = System.Windows.Visibility.Collapsed;
          TextEditGrid.RowDefinitions.RemoveAt(TextEditGrid.RowDefinitions.Count-1);
        }
      }
    }
  }
}