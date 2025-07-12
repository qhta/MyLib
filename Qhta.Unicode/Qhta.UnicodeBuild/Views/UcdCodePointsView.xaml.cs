using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Navigation;
using Qhta.SF.Tools;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.Resources;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.Data;
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
    CodePointDataGrid.GridCopyContent += CodePointDataGrid_OnGridCopyContent;
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
    else
      SetAdvancedFilter();

    void SetAdvancedFilter()
    {
      GridFilterControl filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Visible;
      filterControl.FilterMode = FilterMode.AdvancedFilter;
      filterControl.AllowBlankFilters = true;
    }

    void SetCategoryFilter()
    {
      GridFilterControl filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Visible;
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
      filterControl.SortOptionVisibility = Visibility.Visible;
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
      //WritingSystemViewModel.LogEquals = true;
      GridFilterControl filterControl = e.FilterControl;
      filterControl.SortOptionVisibility = Visibility.Visible;
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

  private void CommandBinding_OnCanExecute(object sender, CanExecuteRoutedEventArgs e)
  {
    if (e.Command is RoutedCommand command)
    {
      if (command == ApplicationCommands.Save)
        e.CanExecute = _ViewModels.Instance.DbContext?.ThereAreUnsavedChanges ?? false;
      else if (command == ApplicationCommands.Copy)
        e.CanExecute = ((_ViewModels.Instance.UcdCodePoints)?.IsLoaded ?? false) && Controller.CanCopyData(CodePointDataGrid);
      else if (command == ApplicationCommands.Cut)
        e.CanExecute = ((_ViewModels.Instance.UcdCodePoints)?.IsLoaded ?? false) && Controller.CanCutData(CodePointDataGrid);
      else if (command == ApplicationCommands.Paste)
        e.CanExecute = ((_ViewModels.Instance.UcdCodePoints)?.IsLoaded ?? false) && Controller.CanPasteData(CodePointDataGrid);
      else if (command == ApplicationCommands.Delete)
        e.CanExecute = ((_ViewModels.Instance.UcdCodePoints)?.IsLoaded ?? false) && Controller.CanDeleteData(CodePointDataGrid);
      else
        e.CanExecute = true; // Default to true for other commands
    }
    //Debug.WriteLine($"CommandBinding_OnCanExecute({sender}, {(e.Command as RoutedUICommand)?.Text})={e.CanExecute}");
  }

  private void CommandBinding_OnExecuted(object sender, ExecutedRoutedEventArgs e)
  {
    Debug.WriteLine($"CommandBinding_OnExecuted({sender}, {(e.Command as RoutedUICommand)?.Text})");
    if (e.Command is RoutedUICommand command)
    {
      if (command == ApplicationCommands.Save)
      {
        _ViewModels.Instance.DbContext?.SaveChanges();
        Debug.WriteLine("Data changes saved");
      }
      else if (command == ApplicationCommands.Copy)
        Controller.CopyData(CodePointDataGrid);
      else if (command == ApplicationCommands.Cut)
        Controller.CutData(CodePointDataGrid);
      else if (command == ApplicationCommands.Paste)
        Controller.PasteData(CodePointDataGrid);
      else if (command == ApplicationCommands.Delete)
        Controller.DeleteData(CodePointDataGrid);
      else
      {
        Debug.WriteLine($"Command {command.Text} not executed");
      }
    }
  }

  private void CodePointDataGrid_OnGridCopyContent(object? sender, GridCopyPasteEventArgs e)
  {
    if (sender is not SfDataGrid grid)
      return;
    Controller.CopyData(grid);
    e.Handled = true;
  }

}


