using System.Collections;
using System.Reflection;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.SF.Tools;


public record GridColumnInfo(GridColumn Column, string MappingName, PropertyInfo ValuePropertyInfo)
{
  public GridColumn Column { get; } = Column;
  public string MappingName { get; } = MappingName;
  public PropertyInfo ValuePropertyInfo { get; } = ValuePropertyInfo;
  public PropertyInfo? DisplayPropertyInfo { get; set;  }
  public IEnumerable? ItemsSource { get; set; }
}