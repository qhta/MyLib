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
      GridLongTextColumn.DataGrid_OnQueryRowHeight(sender, e);
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
          SetSelectedItemInTreeView(WritingSystemsTreeView, selectedItem);
        }
      }
      isSyncFromDataGrid = false;
    }

    private void SetSelectedItemInTreeView(TreeView treeView, object selectedItem)
    {
      var treeViewItem = FindTreeViewItem(treeView, selectedItem);
      if (treeViewItem != null)
      {
        treeViewItem.IsSelected = true;
        treeViewItem.BringIntoView();
      }
    }

    private TreeViewItem? FindTreeViewItem(ItemsControl? container, object item)
    {
      if (container == null)
        return null;

      if (container.DataContext.Equals(item))
        return container as TreeViewItem;

      for (int i = 0; i < container.Items.Count; i++)
      {
        if (container.ItemContainerGenerator.ContainerFromIndex(i) is TreeViewItem childContainer)
        {
          childContainer.IsExpanded = true;
          childContainer.UpdateLayout();

          var result = FindTreeViewItem(childContainer, item);
          if (result != null)
            return result;
        }
      }
      return null;
    }

  }
}
