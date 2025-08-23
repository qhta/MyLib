using System.Windows;
using Qhta.MVVM;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// Command for managing column layout`
/// </summary>
public class ColumnManagementCommand: Command
{
  /// <summary>
  /// Command can execute only if parameter is SfDataGrid and its AllowColumnManagement is true.
  /// </summary>
  /// <param name="parameter"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentException"></exception>
  public override bool CanExecute(object? parameter)
  {
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Parameter must be of type SfDataGrid.", nameof(parameter));
    }
    return SfDataGridBehavior.GetAllowColumnManagement(dataGrid);
  }

  /// <summary>
  /// Command is executed by displaying ColumnManagementWindow for the given SfDataGrid.
  /// </summary>
  /// <param name="parameter"></param>
  /// <exception cref="ArgumentException"></exception>
  public override void Execute(object? parameter)
  {
    if (parameter is not SfDataGrid dataGrid)
    {
      throw new ArgumentException("Parameter must be of type SfDataGrid.", nameof(parameter));
    }
    var window = new Views.ColumnManagementWindow
    {
      Owner = Application.Current?.MainWindow,
      DataContext = new ColumnSelectionCollection(dataGrid)
    };  
    window.ShowDialog();
  }
}