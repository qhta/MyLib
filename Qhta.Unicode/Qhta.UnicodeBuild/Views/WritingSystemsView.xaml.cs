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
using Syncfusion.UI.Xaml.TreeView;
using Syncfusion.UI.Xaml.TreeView.Engine;

using DropPosition = Syncfusion.UI.Xaml.TreeView.DropPosition;

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


    private void WritingSystemsTreeView_OnSelectionChanged(object? sender, ItemSelectionChangedEventArgs e)
    {
      if (isSyncFromDataGrid)
      {
        isSyncFromDataGrid = false;
        return;
      }
      isSyncFromTreeView = true;
      //Debug.WriteLine($"TreeViewSelectionChanged {e.NewValue}");
      WritingSystemsDataGrid.SelectedItem = e.AddedItems.FirstOrDefault();
      var rowIndex = WritingSystemsDataGrid.ResolveToRowIndex(WritingSystemsDataGrid.SelectedItem);
      var rowColumnIndex = new RowColumnIndex(rowIndex, 0);
      WritingSystemsDataGrid.ScrollInView(rowColumnIndex);
      isSyncFromTreeView = false;
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
          //WritingSystemsTreeView.SetSelectedItemInTreeView(selectedItem);
        }
      }
      isSyncFromDataGrid = false;
    }

    private void WritingSystemsTreeView_OnItemDropped(object? sender, TreeViewItemDroppedEventArgs e)
    {
      TreeViewNode draggingNode = e.DraggingNodes.First();
      TreeViewNode targetNode = e.TargetNode;
      if (draggingNode.Content is WritingSystemViewModel item && targetNode.Content is WritingSystemViewModel target && target!=item)
      {
        //Debug.WriteLine($"Item dropped {item} -({e.DropPosition})-> {target}");
        WritingSystemViewModel? parent = target;
        if (e.DropPosition is DropPosition.DropBelow or DropPosition.DropAbove)
          parent = target.Parent;
        if (parent != null)
        {
          //Debug.WriteLine($"{parent}.Children = {parent.ChildrenCount}");
          if (parent.Children != null && parent.Children.Contains(item))
          {
            //Debug.WriteLine($"Removing {item} from {parent}");
            parent.Children.Remove(item);
          }
          item.Parent = parent;
        }
        else
        {
          item.Parent = parent;
        }
      }
    }

    private void WritingSystemsDataGrid_OnAddNewRowInitiating(object? sender, AddNewRowInitiatingEventArgs e)
    {
      var data = new WritingSystem();
      data.Id = _ViewModels.Instance.GetNewWritingSystemId();
      var viewModel = new WritingSystemViewModel(data);
      //_ViewModels.Instance.AllWritingSystems.Add(viewModel);
      //_ViewModels.Instance.TopWritingSystems.Add(viewModel);
      e.NewObject = viewModel;
    }
  }
}
