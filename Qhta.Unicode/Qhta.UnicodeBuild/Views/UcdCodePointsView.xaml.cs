using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Navigation;

using PropertyTools.Wpf;

using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.Resources;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.Data;
//using CollectionViewExtensions = Syncfusion.UI.Xaml.Grid.CollectionViewExtensions;
using Syncfusion.UI.Xaml.Grid;
using Syncfusion.Windows.Shared;

using CollectionViewExtensions = Syncfusion.Data.CollectionViewExtensions;

namespace Qhta.UnicodeBuild.Views;
/// <summary>
/// Interaction logic for CodePointsView.xaml
/// </summary>
public partial class UcdCodePointsView : UserControl
{
  public UcdCodePointsView()
  {
    InitializeComponent();
  }


  private void DataGrid_OnQueryRowHeight(object? sender, QueryRowHeightEventArgs e)
  {
    if (sender is SfDataGrid dataGrid && e.RowIndex > 0 && e.RowIndex <= dataGrid.View.Records.Count)
    {
      LongTextColumn.OnQueryRowHeight(sender, e);
      var rowIndex = e.RowIndex - 1;
      var rowData = dataGrid.View.Records[rowIndex].Data as UcdCodePointViewModel;
      var glyphSize = (rowData?.GlyphSize ?? 12);
      var rowHeight = (glyphSize * 200) / 100;
      if (rowHeight > e.Height)
      {
        e.Height = rowHeight;
        e.Handled = true;
        Debug.WriteLine($"Row {rowIndex} height = {rowHeight}");
      }
    }

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
      var glyphSize = (rowData?.GlyphSize ?? 12);
      var colWidth = glyphSize - 12 + 34;
      if (colWidth > column.Width)
      {
        column.Width = colWidth;
        //Debug.WriteLine($"Column {column.MappingName} width = {colWidth}");
      }
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
    else if (e.Column.MappingName == nameof(UcdCodePointViewModel.Artefact))
      SetWritingSystemFilter(_ViewModels.Instance.SelectableArtefacts);

    void SetCategoryFilter()
    {
      GridFilterControl filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Collapsed;
      filterControl.FilterMode = FilterMode.Both;
      filterControl.AllowBlankFilters = true;
      var selectableItems = _ViewModels.Instance.SelectableCategories.OrderBy(item => item?.Name ?? "").ToList();
      selectableItems.Insert(0, null); // Add a null item at the top
      selectableItems.Insert(1, new UnicodeCategoryViewModel()); // Add a blank item at the second position - blank item predicate will be used to filter items with non-empty category


      var UcdCategoryFilters = selectableItems.Select(item => new FilterElement
      {
        ActualValue = item,
        FormattedString = (object obj) =>
        {
          if (obj is FilterElement filterElement && filterElement.ActualValue is UnicodeCategoryViewModel val)
            return !String.IsNullOrEmpty(val.Name) ? val.Name : Strings.NonEmptyItem;
          return Strings.EmptyItem;
        },
      }).ToArray();
      e.ItemsSource = UcdCategoryFilters;
      e.Handled = true;
    }

    void SetBlockFilter()
    {
      GridFilterControl filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Collapsed;
      filterControl.FilterMode = FilterMode.Both;
      filterControl.AllowBlankFilters = true;
      var selectableItems = _ViewModels.Instance.SelectableBlocks.OrderBy(item => item?.Name ?? "").ToList();
      selectableItems.Insert(0, null); // Add a null item at the top
      selectableItems.Insert(1, new UcdBlockViewModel()); // Add a blank item at the second position - blank item predicate will be used to filter items with non-empty block
      var UcdBlockFilters = selectableItems.Select(item => new FilterElement
      {
        ActualValue = item,
        FormattedString = (object obj) =>
        {
          if (obj is FilterElement filterElement && filterElement.ActualValue is UcdBlockViewModel val)
            return !String.IsNullOrEmpty(val.Name) ? val.Name : Strings.NonEmptyItem;
          return Strings.EmptyItem;
        },
      }).ToArray();
      e.ItemsSource = UcdBlockFilters;
      e.Handled = true;
    }

    void SetWritingSystemFilter(IEnumerable<WritingSystemViewModel?> sourceCollection)
    {
      WritingSystemViewModel.LogEquals = true;
      GridFilterControl filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Collapsed;
      filterControl.FilterMode = FilterMode.Both;
      filterControl.AllowBlankFilters = true;
      var selectableItems = sourceCollection.OrderBy(item => item?.Name ?? "").ToList();
      selectableItems.Insert(0, null); // Add a null item at the top
      selectableItems.Insert(1, new WritingSystemViewModel()); // Add a blank item at the second position - blank item predicate will be used to filter items with non-empty writing system

      var WritingSystemFilters = selectableItems.Select(item => new FilterElement
      { 
        ActualValue = item,
        FormattedString = (object obj) =>
        {
          if (obj is FilterElement filterElement && filterElement.ActualValue is WritingSystemViewModel val)
            return !String.IsNullOrEmpty(val.Name) ? val.Name : Strings.NonEmptyItem;
          return Strings.EmptyItem;
        },
      }).ToArray();
      e.ItemsSource = WritingSystemFilters;
      e.Handled = true;
    }
  }

  private void CodePointDataGrid_OnFilterChanging(object? sender, GridFilterEventArgs e)
  {
    if (e.FilterPredicates==null)
      return;
    foreach (var predicate in e.FilterPredicates)
    {
      if (predicate is not null)
      {
        if (predicate.FilterValue is string)
        {
          predicate.FilterBehavior = FilterBehavior.StringTyped;
          predicate.FilterMode = ColumnFilter.DisplayText;
          continue; // Skip further processing for null values
        }
        else
        if (predicate.FilterValue is UnicodeCategoryViewModel ctg)
        {
          if (ctg.Name == null)
          {
            predicate.FilterBehavior = FilterBehavior.StringTyped;
            predicate.FilterType = FilterType.NotEquals;
            predicate.FilterValue = null;
          }
        }
        else
        if (predicate.FilterValue is UcdBlockViewModel bl)
        {
          if (bl.Name == null)
          {
            predicate.FilterBehavior = FilterBehavior.StringTyped;
            predicate.FilterType = FilterType.NotEquals;
            predicate.FilterValue = null;
          }
        }
        else
        if (predicate.FilterValue is WritingSystemViewModel wm)
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
}


