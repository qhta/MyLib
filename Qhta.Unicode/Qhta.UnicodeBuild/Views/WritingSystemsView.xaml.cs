using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.EntityFrameworkCore;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;

namespace Qhta.UnicodeBuild.Views
{
  /// <summary>
  /// Interaction logic for WritingSystemsView.xaml
  /// </summary>
  public partial class WritingSystemsView : UserControl
  {
    public WritingSystemsView()
    {
      InitializeComponent();
    }

    private void DataGrid_OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
    {
      LongTextColumn.DataGrid_OnQueryRowHeight(sender, e);
    }

    private void WritingSystemsTreeView_OnSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
    {
      if (isSyncFromDataGrid) 
      {
        isSyncFromDataGrid = false;
        return;
      }
      isSyncFromTreeView = true;
      //Debug.WriteLine($"TreeViewSelectionChanged {e.NewValue}");
      WritingSystemsDataGrid.SelectedItem = e.NewValue;
      var rowIndex = WritingSystemsDataGrid.ResolveToRowIndex(WritingSystemsDataGrid.SelectedItem);
      var rowColumnIndex = new RowColumnIndex(rowIndex, 0);
      WritingSystemsDataGrid.ScrollInView(rowColumnIndex);
      isSyncFromTreeView = false;
    }

    private bool isSyncFromTreeView;
    private bool isSyncFromDataGrid;

    private void WritingSystemsDataGrid_OnSelectionChanged(object? sender, GridSelectionChangedEventArgs e)
    {
      if (isSyncFromTreeView)
      {
        isSyncFromTreeView = false;
        return;
      }
      isSyncFromDataGrid = true;
      var newValue = e.AddedItems.FirstOrDefault();
      if (newValue is GridRowInfo gridRowInfo)
      {
        if (gridRowInfo.RowData is WritingSystemViewModel selectedItem)
        {
          //Debug.WriteLine($"DataGridSelectionChanged {selectedItem.Name}");
          WritingSystemsTreeView.SetSelectedItemInTreeView(selectedItem);
        }
      }
      isSyncFromDataGrid = false;
    }

  }
}
