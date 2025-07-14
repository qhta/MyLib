using System.Collections;
using System.Reflection;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;

/// <summary>
/// Information about a grid column, including its mapping name and value property.
/// It is needed if we want to operate on the data in the grid in the background thread,
/// as in this case operations on the grid (and on the grid column) are not possible.
/// </summary>
/// <param name="Column"></param>
/// <param name="MappingName"></param>
/// <param name="ValuePropertyInfo"></param>
public record GridColumnInfo(GridColumn Column, string MappingName, PropertyInfo ValuePropertyInfo)
{
  /// <summary>
  /// Identifies the column in the grid.
  /// </summary>
  public GridColumn Column { get; } = Column;

  /// <summary>
  /// Mapping name of the column, which corresponds to the property name in the data source.
  /// </summary>
  public string MappingName { get; } = MappingName;

  /// <summary>
  /// Gets the <see cref="PropertyInfo"/> object that represents the metadata of the value property.
  /// </summary>
  public PropertyInfo ValuePropertyInfo { get; } = ValuePropertyInfo;

  /// <summary>
  /// Gets or sets the <see cref="PropertyInfo"/> object that represents the property to be displayed.
  /// It is an optional property, used for example in <see cref="GridComboBoxColumn"/> to display the value from the items source.
  /// </summary>
  public PropertyInfo? DisplayPropertyInfo { get; set;  }

  /// <summary>
  /// Gets or sets the collection of items to be displayed.
  /// it is an optional property, used for example in <see cref="GridComboBoxColumn"/> to provide the items source for the combo box.
  /// </summary>
  public IEnumerable? ItemsSource { get; set; }
}