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

  private void CodePointDataGrid_OnFilterItemsPopulating(object? sender, GridFilterItemsPopulatingEventArgs e)
  {
    if (e.Column.MappingName == nameof(UcdCodePointViewModel.Category))
      SetCategoryFilter();
    else if (e.Column.MappingName == nameof(UcdCodePointViewModel.UcdBlock))
      SetBlockFilter();
    else if (e.Column.MappingName == nameof(UcdCodePointViewModel.Area))
      SetWritingSystemFilter(_ViewModels.Instance.SelectableAreas);
    else if (e.Column.MappingName == nameof(UcdCodePointViewModel.Script))
      SetWritingSystemFilter(_ViewModels.Instance.SelectableScripts);
    else if (e.Column.MappingName == nameof(UcdCodePointViewModel.Language))
      SetWritingSystemFilter(_ViewModels.Instance.SelectableLanguages);
    else if (e.Column.MappingName == nameof(UcdCodePointViewModel.Notation))
      SetWritingSystemFilter(_ViewModels.Instance.SelectableNotations);
    else if (e.Column.MappingName == nameof(UcdCodePointViewModel.SymbolSet))
      SetWritingSystemFilter(_ViewModels.Instance.SelectableSymbolSets);
    else if (e.Column.MappingName == nameof(UcdCodePointViewModel.Subset))
      SetWritingSystemFilter(_ViewModels.Instance.SelectableSubsets);
    else
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

      var ucdCategoryFilters = selectableItems.Select(item => new FilterElement
      {
        ActualValue = item,
        FormattedString = (object obj) =>
        {
          if (obj is FilterElement filterElement && filterElement.ActualValue is UnicodeCategoryViewModel val)
            return !string.IsNullOrEmpty(val.Name) ? val.Name : Strings.NonEmptyItem;
          return Strings.EmptyItem;
        }
      }).ToArray();
      e.ItemsSource = ucdCategoryFilters;
      e.Handled = true;
    }

    void SetBlockFilter()
    {
      var filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Visible;
      filterControl.FilterMode = FilterMode.Both;
      filterControl.AllowBlankFilters = true;
      var selectableItems = _ViewModels.Instance.SelectableBlocks.OrderBy(item => item?.Name ?? "").ToList();
      selectableItems.Insert(0, null); // Add a null item at the top
      selectableItems.Insert(1,
        new UcdBlockViewModel()); // Add a blank item at the second position - blank item predicate will be used to filter items with non-empty block
      var ucdBlockFilters = selectableItems.Select(item => new FilterElement
      {
        ActualValue = item,
        FormattedString = (object obj) =>
        {
          if (obj is FilterElement filterElement && filterElement.ActualValue is UcdBlockViewModel val)
            return !string.IsNullOrEmpty(val.Name) ? val.Name : Strings.NonEmptyItem;
          return Strings.EmptyItem;
        }
      }).ToArray();
      e.ItemsSource = ucdBlockFilters;
      e.Handled = true;
    }

    void SetWritingSystemFilter(IEnumerable<WritingSystemViewModel?> sourceCollection)
    {
      //WritingSystemViewModel.LogEquals = true;
      var filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Visible;
      filterControl.FilterMode = FilterMode.Both;
      filterControl.AllowBlankFilters = true;
      var selectableItems = sourceCollection.OrderBy(item => item?.Name ?? "").ToList();
      selectableItems.Insert(0, null); // Add a null item at the top
      selectableItems.Insert(1,
        new WritingSystemViewModel()); // Add a blank item at the second position - blank item predicate will be used to filter items with non-empty writing system

      var writingSystemFilters = selectableItems.Select(item => new FilterElement
      {
        ActualValue = item,
        FormattedString = (object obj) =>
        {
          if (obj is FilterElement filterElement && filterElement.ActualValue is WritingSystemViewModel val)
            return !string.IsNullOrEmpty(val.Name) ? val.Name : Strings.NonEmptyItem;
          return Strings.EmptyItem;
        }
      }).ToArray();
      e.ItemsSource = writingSystemFilters;
      e.Handled = true;
    }
  }

  private void CodePointDataGrid_OnFilterChanging(object? sender, GridFilterEventArgs e)
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

  /// <summary>
  /// Implements the <see cref="IRoutedCommandHandler"/> interface to handle command execution and can-execute checks for routed commands.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public void OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
  {
    _Commander.OnCanExecute(sender, e);
  }

  /// <summary>
  /// Implements the <see cref="IRoutedCommandHandler"/> interface to handle command execution for routed commands.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public void OnExecuted(object sender, ExecutedRoutedEventArgs e)
  {
    _Commander.OnExecute(sender, e);
  }


}