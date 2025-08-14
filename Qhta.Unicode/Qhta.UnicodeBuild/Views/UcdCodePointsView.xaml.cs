using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Qhta.SF.Tools;
using Qhta.UndoManager;
using Qhta.UnicodeBuild.Commands;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.Resources;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Shared;

namespace Qhta.UnicodeBuild.Views;

/// <summary>
/// View for displaying Unicode code points collection.
/// </summary>
public partial class UcdCodePointsView : UserControl, IRoutedCommandHandler
{
  /// <summary>
  /// Initializes a new instance of the <see cref="UcdCodePointsView"/> class.
  /// </summary>
  public UcdCodePointsView()
  {
    InitializeComponent();
  }

  private void DataGrid_OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
  {
    RowHeightProvider.OnQueryRowHeight(sender, e);
    if (e.Handled)
      return;
    LongTextColumn.OnQueryRowHeight(sender, e);
  }

  private void UpDown_OnValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
  {
    if (d is UpDown UpDown)
    {
      var dataGrid = UpDown.FindAscendant<SfDataGrid>();
      if (dataGrid == null)
        return;
      var rowData = UpDown.DataContext as UcdCodePointViewModel;
      var collection = dataGrid.ItemsSource as UcdCodePointsCollection;
      if (collection == null || rowData == null)
        return;
      var rowIndex = collection.IndexOf(rowData);
      if (rowIndex < 0)
        return;
      var column = dataGrid.Columns.FirstOrDefault(item => item.MappingName == "Glyph");
      if (column == null)
        return;
      var glyphSize = rowData?.GlyphSize ?? 12;
      var colWidth = glyphSize - 12 + 34;
      if (colWidth > column.Width) column.Width = colWidth;
      //Debug.WriteLine($"Column {column.MappingName} width = {colWidth}");
      dataGrid.View.Refresh();
    }
  }

  private void CodePointDataGrid_OnFilterItemsPopulating(object? sender, GridFilterItemsPopulatingEventArgs e) => SfDataGridFiltering.FilterItemsPopulating(sender, e);

  private void CodePointDataGrid_OnFilterChanging(object? sender, GridFilterEventArgs e) => SfDataGridFiltering.FilterChanging(sender, e);

  /// <summary>
  /// Implements the <see cref="IRoutedCommandHandler"/> interface to handle command execution and can-execute checks for routed commands.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public void OnCanExecute(object sender, CanExecuteRoutedEventArgs e) => _Commander.OnCanExecute(sender, e);

  /// <summary>
  /// Implements the <see cref="IRoutedCommandHandler"/> interface to handle command execution for routed commands.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public void OnExecuted(object sender, ExecutedRoutedEventArgs e) => _Commander.OnExecute(sender, e);
}