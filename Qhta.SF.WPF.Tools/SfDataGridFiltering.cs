using System.Windows;
using Microsoft.VisualBasic;
using Qhta.SF.WPF.Tools.Resources;
using Syncfusion.Data;
using Syncfusion.UI.Xaml.Grid;
using Strings = Qhta.SF.WPF.Tools.Resources.Strings;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// Class containing tools for filtering data in Syncfusion DataGrid.
/// </summary>
public static class SfDataGridFiltering
{

  /// <summary>
  /// Handles the populating of filter items in a Syncfusion DataGrid. Sender must be a GridColumn or its descender.
  /// If the column is a GridComboBoxColumn, it sets up the filter items based on the provided ItemsSource
  /// adding "Empty" and "NonEmpty" options if applicable.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="e"></param>
  public static void OnFilterItemsPopulating(object? sender, GridFilterItemsPopulatingEventArgs e)
  {
    if (e.Column is GridComboBoxColumn comboBoxColumn)
    {
      if (comboBoxColumn.ItemsSource is not IEnumerable<ISelectableItem> selectableItems)
      {
        selectableItems = new List<ISelectableItem>();
        foreach (var item in comboBoxColumn.ItemsSource)
        {
          selectableItems = selectableItems.Append(new SelectableItem { DisplayName = item?.ToString() ?? Strings.EmptyValue, Value = item });
        }
      }
      SetSelectableItemsFilter(selectableItems);
    }
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
      if (items.First() is { } firstItem && !firstItem.IsEmpty)
      {
        // Add "Empty" item if not present.                        
        items.Insert(0, new SelectableItem { DisplayName = Strings.EmptyValue });
      }
      items.Insert(1, new SelectableItem { DisplayName = Strings.NonEmptyValue, Value = NonEmptyValue.Instance });

      var filters = items.Select(item => new FilterElement
      {
        ActualValue = item,
        FormattedString = (object obj) =>
        {
          if (obj is FilterElement filterElement && filterElement.ActualValue is ISelectableItem val)
            return val.DisplayName;
          return Strings.EmptyValue;
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
  public static void OnFilterChanging(object? sender, GridFilterEventArgs e)
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
          if (item.IsEmpty)
          {
            predicate.FilterType = FilterType.Equals;
            predicate.FilterValue = null;
          }
          if (item.IsNonEmpty)
          {
            predicate.FilterType = FilterType.NotEquals;
            predicate.FilterValue = null;
          }
        }
      }
  }


}