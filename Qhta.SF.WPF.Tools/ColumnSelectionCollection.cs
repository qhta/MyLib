using System.Collections.ObjectModel;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.WPF.Tools;

/// <summary>
/// A collection of ColumnSelectionItem objects, representing a list of selectable columns in a data grid.
/// </summary>
public class ColumnSelectionCollection : ObservableCollection<ColumnSelectionItem>
{
  /// <summary>
  /// The data grid associated with this collection.
  /// </summary>
  public SfDataGrid DataGrid { get; private set; }

  /// <summary>
  /// Initializes a new instance of the <see cref="ColumnSelectionCollection"/> class.
  /// </summary>
  /// <param name="dataGrid"></param>
  public ColumnSelectionCollection(SfDataGrid dataGrid)
  {
    DataGrid = dataGrid;
    GetDataGridColumns();
  }

  /// <summary>
  /// Gets the current columns from the data grid and populates the collection.
  /// </summary>
  public void GetDataGridColumns()
  {
    foreach (var column in DataGrid.Columns)
    {
      Add(new ColumnSelectionItem { Column = column, IsSelected = !column.IsHidden });
    }
  }

  /// <summary>
  /// Stores the current selection state of the columns back to the data grid.
  /// </summary>
  public void SetDataGridColumns()
  {
    if (NeedsColumnsOrderChange())
      ChangeColumnsOrder();
    foreach (var item in this)
    {
      item.Column.IsHidden = !item.IsSelected;
    }
  }

  private bool NeedsColumnsOrderChange()
  {
    if (this.Count!=DataGrid.Columns.Count)
      return true;
    for (int i=0; i<this.Count; i++)
      if (this[i].Column != DataGrid.Columns[i])
        return true;
    return false;
  }

  private void ChangeColumnsOrder()
  {
    DataGrid.Columns.Clear();
    foreach (var item in this)
      DataGrid.Columns.Add(item.Column);
  }
}