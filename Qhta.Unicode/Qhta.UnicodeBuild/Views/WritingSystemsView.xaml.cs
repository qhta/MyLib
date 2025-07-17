using System.Windows;
using System.Windows.Controls;
using Qhta.SF.Tools;
using Qhta.TextUtils;
using Qhta.Unicode.Models;
using Qhta.UnicodeBuild.ViewModels;
using Qhta.UnicodeBuild.Resources;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.UI.Xaml.ScrollAxis;
using Syncfusion.UI.Xaml.TreeView;
using Syncfusion.UI.Xaml.TreeView.Engine;
using DropPosition = Syncfusion.UI.Xaml.TreeView.DropPosition;
using WritingSystem = Qhta.Unicode.Models.WritingSystem;

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

  private void WritingSystemsDataGrid_OnAddNewRowInitiating(object? sender, AddNewRowInitiatingEventArgs e)
  {
    var data = new WritingSystem();
    data.Id = _ViewModels.Instance.GetNewWritingSystemId();
    var viewModel = new WritingSystemViewModel(data);
    //_ViewModels.Instance.AllWritingSystems.Add(viewModel);
    //_ViewModels.Instance.TopWritingSystems.Add(viewModel);
    e.NewObject = viewModel;
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
          var displayText = item.DisplayText.TitleCase();
          var newText = Qhta.UnicodeBuild.Resources.WritingSystemType.ResourceManager.GetString(displayText);
          if (newText != null) item.DisplayText = newText.ToLower();
        }
    }
    else if (e.Column != null && e.Column.MappingName == nameof(WritingSystemViewModel.Kind))
    {
      foreach (var item in e.ItemsSource)
        if (!item.DisplayText.StartsWith("("))
        {
          var displayText = item.DisplayText.TitleCase();
          var newText = Qhta.UnicodeBuild.Resources.WritingSystemKind.ResourceManager.GetString(displayText);
          if (newText != null) item.DisplayText = newText.ToLower();
        }
    }
  }

  private void WritingSystemsGrid_OnFilterItemsPopulating(object? sender, GridFilterItemsPopulatingEventArgs e)
  {
    //if (e.Column.MappingName == nameof(UcdCodePointViewModel.Category))
    //  SetCategoryFilter();
    //else if (e.Column.MappingName == nameof(UcdCodePointViewModel.UcdBlock))
    //  SetBlockFilter();
    //else if (e.Column.MappingName == nameof(UcdCodePointViewModel.Area))
    //  SetWritingSystemFilter(_ViewModels.Instance.SelectableAreas);
    //else if (e.Column.MappingName == nameof(UcdCodePointViewModel.Script))
    //  SetWritingSystemFilter(_ViewModels.Instance.SelectableScripts);
    //else if (e.Column.MappingName == nameof(UcdCodePointViewModel.Language))
    //  SetWritingSystemFilter(_ViewModels.Instance.SelectableLanguages);
    //else if (e.Column.MappingName == nameof(UcdCodePointViewModel.Notation))
    //  SetWritingSystemFilter(_ViewModels.Instance.SelectableNotations);
    //else if (e.Column.MappingName == nameof(UcdCodePointViewModel.SymbolSet))
    //  SetWritingSystemFilter(_ViewModels.Instance.SelectableSymbolSets);
    //else if (e.Column.MappingName == nameof(UcdCodePointViewModel.Subset))
    //  SetWritingSystemFilter(_ViewModels.Instance.SelectableSubsets);
    //else if (e.Column.MappingName == nameof(UcdCodePointViewModel.Artefact))
    //  SetWritingSystemFilter(_ViewModels.Instance.SelectableArtefacts);
    //else
    SetAdvancedFilter();

    void SetAdvancedFilter()
    {
      var filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Visible;
      filterControl.FilterMode = FilterMode.AdvancedFilter;
      filterControl.AllowBlankFilters = true;
    }

    void SetCategoryFilter()
    {
      var filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Visible;
      filterControl.FilterMode = FilterMode.Both;
      filterControl.AllowBlankFilters = true;
      var selectableItems = _ViewModels.Instance.SelectableCategories.OrderBy(item => item?.Name ?? "").ToList();
      selectableItems.Insert(0, null); // Add a null item at the top
      selectableItems.Insert(1,
        new UnicodeCategoryViewModel()); // Add a blank item at the second position - blank item predicate will be used to filter items with non-empty category

      var UcdCategoryFilters = selectableItems.Select(item => new FilterElement
      {
        ActualValue = item,
        FormattedString = (object obj) =>
        {
          if (obj is FilterElement filterElement && filterElement.ActualValue is UnicodeCategoryViewModel val)
            return !string.IsNullOrEmpty(val.Name) ? val.Name : Strings.NonEmptyItem;
          return Strings.EmptyItem;
        }
      }).ToArray();
      e.ItemsSource = UcdCategoryFilters;
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
}