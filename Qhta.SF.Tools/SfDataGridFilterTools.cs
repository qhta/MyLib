using System.Windows;
using Microsoft.VisualBasic;
using Qhta.SF.Tools.Resources;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

/// <summary>
/// Class containing tools for filtering data in Syncfusion DataGrid.
/// </summary>
public static class SfDataGridFilterTools
{

  /// <summary>
  /// Handles the populating of filter items in a Syncfusion DataGrid. Sender must be a GridColumn or its descender.
  /// If the column is a GridComboBoxColumn, it sets up the filter items based on the provided ItemsSource
  /// adding "Empty" and "NonEmpty" options if applicable.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public static void FilterItemsPopulating(object? sender, GridFilterItemsPopulatingEventArgs e)
  {
    if (e.Column is GridComboBoxColumn comboBoxColumn && comboBoxColumn.ItemsSource is IEnumerable<ISelectableItem> selectableItems)
      SetSelectableItemsFilter(selectableItems);
    else
      SetAdvancedFilter();

    void SetAdvancedFilter()
    {
      var filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Visible;
      filterControl.FilterMode = FilterMode.AdvancedFilter;
      filterControl.AllowBlankFilters = true;
    }

    void SetSelectableItemsFilter(IEnumerable<ISelectableItem?> sourceCollection)
    {
      //WritingSystemViewModel.LogEquals = true;
      var filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Visible;
      filterControl.FilterMode = FilterMode.Both;
      filterControl.AllowBlankFilters = true;
      var items = sourceCollection.ToList();
      if (items.First() is { } firstItem && firstItem.DisplayName != DataStrings.EmptyValue)
      {
        // Add "Empty" item if not present.                        
        items.Insert(0, new SelectableItemStub { DisplayName = DataStrings.EmptyValue });
      }
      items.Insert(1, new SelectableItemStub { DisplayName = DataStrings.NonEmptyValue });

      var filters = items.Select(item => new FilterElement
      {
        ActualValue = item,
        FormattedString = (object obj) =>
        {
          if (obj is FilterElement filterElement && filterElement.ActualValue is ISelectableItem val)
            return val.DisplayName;
          return DataStrings.EmptyValue;
        }
      }).ToArray();
      e.ItemsSource = filters;
      e.Handled = true;
    }
  }

  /// <summary>
  /// Handles the changing of filters in a Syncfusion DataGrid for proper ISelectableItem handling.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public static void FilterChanging(object? sender, GridFilterEventArgs e)
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
        }
        else if (predicate.FilterValue is ISelectableItem item)
        {
          if (item.DisplayName == DataStrings.EmptyValue)
          {
            predicate.FilterType = FilterType.Equals;
            predicate.FilterValue = null;
          }
          if (item.DisplayName == DataStrings.NonEmptyValue)
          {
            predicate.FilterType = FilterType.NotEquals;
            predicate.FilterValue = null;
          }
        }
      }
  }


}