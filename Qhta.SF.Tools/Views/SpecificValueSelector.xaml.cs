using System.Windows;
using System.Windows.Controls;

namespace Qhta.SF.Tools.Views;

/// <summary>
/// Part of the <see cref="SpecificValueWindow"/> for selecting a specific value.
/// </summary>
public partial class SpecificValueSelector : UserControl
{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  public SpecificValueSelector()
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
          ReplacementList.Visibility = System.Windows.Visibility.Visible;
          SelectionEditGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
        }
        else
        {
          ReplacementList.Visibility = System.Windows.Visibility.Collapsed;
          SelectionEditGrid.RowDefinitions.RemoveAt(SelectionEditGrid.RowDefinitions.Count - 1);
        }
      }
    }
  }

}