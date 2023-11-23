namespace Qhta.WPF.Utils.ViewModels;

/// <summary>
/// ViewModel for filter of DataGrid
/// </summary>
public class DataGridFilterViewModel: ViewModel
{                                            

  /// <summary>
  /// Information about filterable columns in data grid. 
  /// </summary>
  public DataGridFilteredColumns? Columns { get; set; }

  ///// <summary>
  ///// Property of the column on which filter button was pressed.
  ///// </summary>
  //public string? PropName{ get; set; }

  /// <summary>
  /// Column on which filter button was pressed.
  /// </summary>
  public DataGridColumn? Column { get; set; }

  /// <summary>
  /// Filter to edit.
  /// </summary>
  public ColumnFilterViewModel? Filter { get; set; }

  /// <summary>
  /// Result of the FilterDialog
  /// </summary>
  public FilterOperation DialogResult { get; set; } 

  /// <summary>
  /// Names of columns to select.
  /// </summary>
  public IEnumerable<string>? ColumnNames => Columns?.Select(item=>item.ColumnName);

}
