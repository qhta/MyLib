using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Qhta.SF.WPF.Tools;
using Qhta.SF.WPF.Tools.Resources;
using Qhta.TextUtils;
using Qhta.UndoManager;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Commands;
using Qhta.UnicodeBuild.Helpers;
using Strings = Qhta.UnicodeBuild.Resources.Strings;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.Data;
using Syncfusion.Data.Extensions;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.Grid.Helpers;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.TreeView;
using Syncfusion.UI.Xaml.TreeView.Engine;

using DropPosition = Syncfusion.UI.Xaml.TreeView.DropPosition;
using WritingSystem = Qhta.Unicode.Models.WritingSystem;
using WritingSystemKind = Qhta.Unicode.Models.WritingSystemKind;
using WritingSystemType = Qhta.Unicode.Models.WritingSystemType;
using DataStrings = Qhta.SF.WPF.Tools.Resources.Strings;

namespace Qhta.UnicodeBuild.Views;

/// <summary>
/// View for displaying and managing writing systems.
/// </summary>
public partial class WritingSystemsView : UserControl, IRoutedCommandHandler
{
  /// <summary>
  /// Initializes a new instance of the <see cref="WritingSystemsView"/> class.
  /// </summary>
  public WritingSystemsView()
  {
    InitializeComponent();
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
    if (WritingSystemsDataGrid.SelectedItem != null)
    {
      var rowIndex = WritingSystemsDataGrid.ResolveToRowIndex(WritingSystemsDataGrid.SelectedItem);
      if (rowIndex > 0)
      {
        var rowColumnIndex = new RowColumnIndex(rowIndex, 0);
        WritingSystemsDataGrid.ScrollInView(rowColumnIndex);
      }
    }
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
      if (gridRowInfo.RowData is WritingSystemViewModel selectedItem)
      {
        //Debug.WriteLine($"DataGridSelectionChanged {selectedItem.Name}");
        //WritingSystemsTreeView.SetSelectedItemInTreeView(selectedItem);
      }
    isSyncFromDataGrid = false;
  }

  private void WritingSystemsTreeView_OnItemDropped(object? sender, TreeViewItemDroppedEventArgs e)
  {
    var draggingNode = e.DraggingNodes.First();
    var targetNode = e.TargetNode;
    if (draggingNode.Content is WritingSystemViewModel item && targetNode.Content is WritingSystemViewModel target &&
        target != item)
    {
      //Debug.WriteLine($"Item dropped {item} -({e.DropPosition})-> {target}");
      var parent = target;
      if (e.DropPosition is DropPosition.DropBelow or DropPosition.DropAbove)
        parent = target.Parent;
      if (parent != null)
      {
        //Debug.WriteLine($"{parent}.Children = {parent.ChildrenCount}");
        if (parent.Children != null && parent.Children.Contains(item))
          //Debug.WriteLine($"Removing {item} from {parent}");
          parent.Children.Remove(item);
        item.Parent = parent;
      }
      else
      {
        item.Parent = parent;
      }
    }
  }

  private void NewWritingSystemButton_OnClick(object sender, RoutedEventArgs e)
  {
    var button = sender as Button;
    if (button?.ContextMenu != null)
    {
      button.ContextMenu.PlacementTarget = button;
      button.ContextMenu.IsOpen = true;
    }
  }

  /// <summary>
  /// Implements the <see cref="IRoutedCommandHandler"/> interface to handle command execution and can execute checks.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
  {
    Debug.WriteLine($"WritingsSystemsView.CommandBinding_OnCanExecute({(e.Command as RoutedUICommand)?.Text ?? e.Command.ToString()})");
    _Commander.OnCanExecute(sender, e);
    Debug.WriteLine($"WritingsSystemsView.CommandBinding_OnCanExecute({(e.Command as RoutedUICommand)?.Text ?? e.Command.ToString()})={e.CanExecute}");
  }

  /// <summary>
  /// Implements the <see cref="IRoutedCommandHandler"/> interface to handle command execution.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public void OnExecuted(object sender, ExecutedRoutedEventArgs e)
  {
    _Commander.OnExecute(sender, e);
  }

  private void WritingSystemsDataGrid_OnRecordDeleted(object? sender, RecordDeletedEventArgs e)
  {
    foreach (var item in e.Items)
    {
      if (item is WritingSystemViewModel writingSystem)
      {
        Debug.WriteLine($"WritingSystemsDataGrid_OnRecordDeleted {writingSystem.Name}");
        _ViewModels.Instance.WritingSystems.Remove(writingSystem);
      }
    }
  }
}