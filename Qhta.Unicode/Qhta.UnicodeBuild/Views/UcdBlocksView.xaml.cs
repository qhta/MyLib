using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using Microsoft.EntityFrameworkCore;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.ViewModels;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Views
{
  /// <summary>
  /// Interaction logic for UcdBlocksView.xaml
  /// </summary>
  public partial class UcdBlocksView : UserControl
  {
    public UcdBlocksView()
    {
      InitializeComponent();
    }

    private void DataGrid_OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
    {
      Debug.WriteLine($"DataGrid_OnQueryRowHeight invoked for row {e.RowIndex}");
      if (e.RowIndex > 0 && sender is SfDataGrid dataGrid)
      {
        if (dataGrid.View.Records[e.RowIndex - 1].Data is UcdBlockViewModel record)
        {
          if (record.Comment == null)
            return;
          if (!record.IsWrapped)
            return;
          var wrapColumn = dataGrid.Columns.FirstOrDefault(col => col.MappingName == "Comment");
          if (wrapColumn == null)
            return;
          var maxWidth = wrapColumn.ActualWidth - 6;
          var formattedText = new FormattedText(
              record.Comment,
              System.Globalization.CultureInfo.CurrentCulture,
              System.Windows.FlowDirection.LeftToRight, // Fixed the error by qualifying with type name
              new Typeface("Segoe UI"),
              12,
              Brushes.Black,
              new NumberSubstitution(),
              1);
          formattedText.MaxTextWidth = maxWidth;
          e.Height = formattedText.Height + 6; // Add some padding
          e.Handled = true;
        }
      }
    }

    private T? FindParent<T>(DependencyObject child) where T : DependencyObject
    {
      DependencyObject? parentObject = VisualTreeHelper.GetParent(child);
      if (parentObject == null) return null;

      if (parentObject is T parent)
      {
        return parent;
      }
      else
      {
        return FindParent<T>(parentObject);
      }
    }

    private void WrapButton_CheckedChanged(object sender, RoutedEventArgs e)
    {
      if (sender is ToggleButton button)
        if (button.DataContext is ILongTextViewModel viewModel)
        {
          // Find the parent SfDataGrid
          var dataGrid = FindParent<SfDataGrid>(button);
          if (dataGrid != null)
          {
            // Resolve the row index of the view model
            var rowIndex = dataGrid.ResolveToRowIndex(viewModel);
            Debug.WriteLine($"Resolved row index: {rowIndex}");
            // Invalidate the row height
            dataGrid.InvalidateRowHeight(rowIndex);
            Debug.WriteLine($"dataGrid.InvalidateRowHeight({rowIndex}) invoked");
            // Force the SfDataGrid to update its layout and refresh the visible rows
            dataGrid.UpdateLayout();
            // Force the SfDataGrid to re-evaluate the row heights by updating the data source
            dataGrid.View.Refresh();
          }
        }
    }
  }
}

