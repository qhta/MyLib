using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Qhta.SF.Tools;
using Qhta.TextUtils;
using Qhta.UndoManager;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.Resources;
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
using WritingSystemType = Qhta.Unicode.Models.WritingSystemType;
using WritingSystemKind = Qhta.Unicode.Models.WritingSystemKind;


namespace Qhta.UnicodeBuild.Views;

/// <summary>
/// View for displaying and managing writing systems.
/// </summary>
public partial class WritingSystemsView : UserControl
{
  /// <summary>
  /// Initializes a new instance of the <see cref="WritingSystemsView"/> class.
  /// </summary>
  public WritingSystemsView()
  {
    InitializeComponent();
  }

  private void DataGrid_OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
  {
    RowHeightProvider.OnQueryRowHeight(sender, e);
    if (!e.Handled)
      LongTextColumn.OnQueryRowHeight(sender, e);
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

  private void WritingSystemsDataGrid_OnFilterItemsPopulated(object? sender, GridFilterItemsPopulatedEventArgs e)
  {
    if (e.Column != null && e.Column.MappingName == nameof(WritingSystemViewModel.Type))
    {
      foreach (var item in e.ItemsSource)
        if (!item.DisplayText.StartsWith("("))
        {
          if (item.ActualValue is WritingSystemType writingSystemType && writingSystemType == WritingSystemType.SymbolSet)
            Debug.Assert(true);
          var displayText = item.DisplayText;
          var newText = Qhta.UnicodeBuild.Resources.WritingSystemTypeStrings.ResourceManager.GetString(displayText);
          if (newText != null) item.DisplayText = newText;
        }
    }
    else if (e.Column != null && e.Column.MappingName == nameof(WritingSystemViewModel.Kind))
    {
      foreach (var item in e.ItemsSource)
        if (!item.DisplayText.StartsWith("("))
        {
          if (item.ActualValue is WritingSystemKind writingSystemKind && writingSystemKind == WritingSystemKind.SemiSyllabary)
            Debug.Assert(true);
          var displayText = item.DisplayText;
          var newText = Qhta.UnicodeBuild.Resources.WritingSystemKindStrings.ResourceManager.GetString(displayText);
          if (newText != null) item.DisplayText = newText;
        }
    }
  }

  private void WritingSystemsGrid_OnFilterItemsPopulating(object? sender, GridFilterItemsPopulatingEventArgs e)
  {
    if (e.Column != null && e.Column.MappingName == nameof(WritingSystemViewModel.Type))
      SetTypeFilter();
    else if (e.Column != null && e.Column.MappingName == nameof(WritingSystemViewModel.Kind))
      SetKindFilter();
    else
      SetAdvancedFilter();

    void SetAdvancedFilter()
    {
      var filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Visible;
      filterControl.FilterMode = FilterMode.AdvancedFilter;
      filterControl.AllowBlankFilters = true;
    }

    void SetTypeFilter()
    {
      var filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Visible;
      filterControl.FilterMode = FilterMode.CheckboxFilter;
      filterControl.AllowBlankFilters = true;
      var selectableItems = _ViewModels.Instance.SelectableWritingSystemTypes.ToList();
      selectableItems.Insert(0, null); // Add a null item at the top

      var UcdWritingSystemTypesFilter = selectableItems.Select(item => new FilterElement
      {
        ActualValue = item,
        FormattedString = (object obj) =>
        {
          if (obj is FilterElement filterElement && filterElement.ActualValue is WritingSystemType val)
            return val.ToString();
          return Strings.EmptyItem;
        }
      }).ToArray();
      e.ItemsSource = UcdWritingSystemTypesFilter;
      e.Handled = true;
    }

    void SetKindFilter()
    {
      var filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Visible;
      filterControl.FilterMode = FilterMode.CheckboxFilter;
      filterControl.AllowBlankFilters = true;
      var selectableItems = _ViewModels.Instance.SelectableWritingSystemKinds.ToList();
      selectableItems.Insert(0, null); // Add a null item at the top

      var UcdWritingSystemKindFilter = selectableItems.Select(item => new FilterElement
      {
        ActualValue = item,
        FormattedString = (object obj) =>
        {
          if (obj is FilterElement filterElement && filterElement.ActualValue is WritingSystemKind val)
            return val.ToString();
          return Strings.EmptyItem;
        }
      }).ToArray();
      e.ItemsSource = UcdWritingSystemKindFilter;
      e.Handled = true;
    }

  }

  private void WritingSystemsGrid_OnFilterChanging(object? sender, GridFilterEventArgs e)
  {
    if (e.FilterPredicates == null)
      return;
    foreach (var predicate in e.FilterPredicates)
      if (predicate is not null)
      {
        if (predicate.FilterValue is string)
        {
          predicate.FilterBehavior = FilterBehavior.StringTyped;
          predicate.FilterMode = ColumnFilter.DisplayText;
          continue; // Skip further processing for null values
        }
        else if (predicate.FilterValue is UnicodeCategoryViewModel ctg)
        {
          if (ctg.Name == null)
          {
            predicate.FilterBehavior = FilterBehavior.StringTyped;
            predicate.FilterType = FilterType.NotEquals;
            predicate.FilterValue = null;
          }
        }
        else if (predicate.FilterValue is UcdBlockViewModel bl)
        {
          if (bl.Name == null)
          {
            predicate.FilterBehavior = FilterBehavior.StringTyped;
            predicate.FilterType = FilterType.NotEquals;
            predicate.FilterValue = null;
          }
        }
        else if (predicate.FilterValue is WritingSystemViewModel wm)
        {
          if (wm.Name == null)
          {
            predicate.FilterType = FilterType.NotEquals;
            predicate.FilterValue = null;
          }
        }
      }
  }


  private void CommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
  {
    if (e.Command is RoutedCommand command)
    {
      if (command == ApplicationCommands.Save)
        e.CanExecute = _ViewModels.Instance.DbContext?.ThereAreUnsavedChanges ?? false;
      else if (command == ApplicationCommands.Copy)
        e.CanExecute = (_ViewModels.Instance.WritingSystems?.IsLoaded ?? false) &&
                       Controller.CanCopyData(WritingSystemsDataGrid);
      else if (command == ApplicationCommands.Cut)
        e.CanExecute = (_ViewModels.Instance.WritingSystems?.IsLoaded ?? false) &&
                       Controller.CanCutData(WritingSystemsDataGrid);
      else if (command == ApplicationCommands.Paste)
        e.CanExecute = (_ViewModels.Instance.WritingSystems?.IsLoaded ?? false) &&
                       Controller.CanPasteData(WritingSystemsDataGrid);
      else if (command == ApplicationCommands.Delete)
        e.CanExecute = (_ViewModels.Instance.WritingSystems?.IsLoaded ?? false) &&
                       Controller.CanDeleteData(WritingSystemsDataGrid);
      else if (command == ApplicationCommands.Undo)
        e.CanExecute = UndoMgr.IsUndoAvailable;
      else if (command == ApplicationCommands.Redo)
        e.CanExecute = UndoMgr.IsRedoAvailable;
      else
        e.CanExecute = true; // Default to true for other commands
    }
    //Debug.WriteLine($"CommandBinding_OnCanExecute({sender}, {(e.Command as RoutedUICommand)?.Text})={e.CanExecute}");
  }

  private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
  {
    //Debug.WriteLine($"CommandBinding_OnExecuted({sender}, {(e.Command as RoutedUICommand)?.Text})");
    if (e.Command is RoutedUICommand command)
    {
      if (command == ApplicationCommands.Save)
      {
        Debug.WriteLine("Data changes save started");
        _ViewModels.Instance.DbContext?.SaveChangesAsync();
      }
      else if (command == ApplicationCommands.Copy)
      {
        Controller.CopyData(WritingSystemsDataGrid);
      }
      else if (command == ApplicationCommands.Cut)
      {
        Controller.CutData(WritingSystemsDataGrid);
      }
      else if (command == ApplicationCommands.Paste)
      {
        Controller.PasteData(WritingSystemsDataGrid);
      }
      else if (command == ApplicationCommands.Delete)
      {
        Controller.DeleteData(WritingSystemsDataGrid);
      }
      else if (command == ApplicationCommands.Undo)
      {
        UndoManager.UndoMgr.Undo();
        WritingSystemsDataGrid.UpdateLayout();
      }
      else if (command == ApplicationCommands.Redo)
      {
        UndoManager.UndoMgr.Redo();
        WritingSystemsDataGrid.UpdateLayout();
      }
      else
      {
        Debug.WriteLine($"Command {command.Text} not executed");
      }
    }
  }

  private void WritingSystemsDataGrid_OnGridCopyContent(object? sender, GridCopyPasteEventArgs e)
  {
    if (sender is not SfDataGrid grid)
      return;
    Controller.CopyData(grid);
    e.Handled = true;
  }

  private void WritingSystemsDataGrid_KeyDown(object sender, KeyEventArgs e)
  {
    //Debug.WriteLine($"WritingSystemsDataGrid_KeyDown: {e.Key} {Keyboard.Modifiers}");
    if (sender is not SfDataGrid grid)
      return;
    switch (e.Key)
    {
      case Key.C when (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control:
        {
          if (Controller.CanCopyData(grid))
            Controller.CopyData(grid);
          e.Handled = true;
          return;
        }
      case Key.X when (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control:
        {
          if (Controller.CanCutData(grid))
            Controller.CutData(grid);
          e.Handled = true;
          return;
        }
      case Key.V when (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control:
        {
          if (Controller.CanPasteData(grid))
            Controller.PasteData(grid);
          e.Handled = true;
          return;
        }
      case Key.Delete:
        {
          if (Controller.CanDeleteData(grid))
            Controller.DeleteData(grid);
          e.Handled = true;
          return;
        }
    }
  }
}