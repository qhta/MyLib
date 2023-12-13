namespace Qhta.WPF.Utils;
/// <summary>
/// An information of the single column.
/// </summary>
public class ColumnViewInfo : DependencyObject, INotifyPropertyChanged
{
  /// <summary>
  /// Default constructor
  /// </summary>
  public ColumnViewInfo() { }


  /// <summary>
  /// Duplicates the column definition.
  /// </summary>
  /// <returns></returns>
  public ColumnViewInfo Duplicate()
  {
    var result = new ColumnViewInfo();
    foreach (var propInfo in typeof(ColumnViewInfo).GetProperties())
    {
      if (propInfo.CanRead && propInfo.CanWrite)
        propInfo.SetValue(result, propInfo.GetValue(this));
    };
    return result;
  }

  /// <summary>
  /// Associated column.
  /// </summary>
  public object? Column { get; set; }

  /// <summary>
  /// Required binding to data.
  /// </summary>
  public BindingBase? Binding { get; set; } 

  /// <summary>
  /// Indicates whether the user can change the column display position by dragging the column header.
  /// Default is true.
  /// </summary>
  public bool CanUserReorder { get; set; } = true;

  /// <summary>
  /// Indicates whether the user can adjust the column width by using the mouse.
  /// Default is true.
  /// </summary>
  public bool CanUserResize { get; set; } = true;

  /// <summary>
  /// Indicates whether the user can sort the column by clicking the column header.
  /// Default is true.
  /// </summary>
  public bool CanUserSort { get; set; } = true;

  /// <summary>
  /// Indicates whether the user can search the column for a value provided in FindDialog.
  /// Default is true.
  /// </summary>
  public bool CanUserFind { get; set; } = true;

  /// <summary>
  /// Indicates whether Find button should be displayed in the column header.
  /// Default is true.
  /// </summary>
  public bool ShowFindButton { get; set; } = true;

  /// <summary>
  /// Indicates whether the user can filter column with a predicate provided in FilterDialog.
  /// Default is true.
  /// </summary>
  public bool CanUserFilter { get; set; } = true;

  /// <summary>
  /// Indicates whether Filter button should be displayed in the column header.
  /// Default is true.
  /// </summary>
  public bool ShowFilterButton { get; set; } = true;

  /// <summary>
  /// Specifies the binding path of properties to use when getting or setting cell content for the clipboard.
  /// Default is null.
  /// </summary>
  public string? ClipboardContentPath { get; set; }

  /// <summary>
  /// Gets or sets the display position of the column relative to the other columns in the DataGrid.
  /// Default is -1.
  /// </summary>
  public int DisplayIndex { get; set; } = -1;

  /// <summary>
  /// Gets or sets the column header.
  /// </summary>
  public object? Header { get; set; }

  /// <summary>
  /// Gets or sets the format pattern to apply to the content of the column header.
  /// </summary>
  public string? HeaderStringFormat { get; set; }

  /// <summary>
  /// Gets or sets the header template to apply to the content of the column header.
  /// </summary>
  public DataTemplate? HeaderTemplate { get; set; }

  /// <summary>
  /// Gets or sets the column hidden header.
  /// </summary>
  public string? HiddenHeader { get; set; }

  /// <summary>
  /// Gets or sets the column hidden header.
  /// </summary>
  public string? HeaderTooltip { get; set; }

  /// <summary>
  /// Gets a value that indicates whether cells in the column can be edited.
  /// Default is null.
  /// </summary>
  public bool IsReadOnly { get; set; } = false;

  /// <summary>
  /// Gets or sets the maximum width constraint of the column. Default is PositiveInfinity.
  /// </summary>
  public double MaxWidth { get; set; } = double.PositiveInfinity;

  /// <summary>
  /// Gets or sets the minimum width constraint of the column. Default is 20.
  /// </summary>
  public double MinWidth { get; set; } = 20;

  /// <summary>
  /// Gets or sets the sort direction (ascending or descending) of the column. Default is null;
  /// </summary>
  public ListSortDirection? SortDirection { get; set; }

  /// <summary>
  /// Gets or sets a property name, or a period-delimited hierarchy of property names, 
  /// that indicates the member to sort by.
  /// Default is null.
  /// </summary>
  public string? SortMemberPath { get; set; }

  /// <summary>
  /// Gets or sets the visibility of the column. Default is Visible
  /// </summary>
  public Visibility Visibility { get; set; } = Visibility.Visible;

  ///// <summary>
  ///// Gets or sets the column width or automatic sizing mode.
  ///// Default is NaN.
  ///// </summary>
  //public double Width { get; set; } = double.NaN;

  /// <summary>
  /// Context menu for a column.
  /// </summary>
  public ContextMenu? HeaderContextMenu { get; set; }

  /// <summary>
  /// Property name to bind values.
  /// </summary>
  public string? PropertyName { get; set; }

  /// <summary>
  /// Property info to bind values.
  /// </summary>
  [XmlIgnore]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public PropertyInfo? PropertyInfo { get; set; }

  /// <summary>
  /// Data type of the property.
  /// </summary>
  public Type? DataType { get; set; }

  /// <summary>
  /// Sequence of PropertyInfo from DataContext object
  /// to the filtered property.
  /// </summary>
  public PropPath? PropPath { get; set; }

  /// <summary>
  /// Property to change column visibility.
  /// A converter to Control.Visibility may be needed to bind to column Visibility.
  /// If IsVisibile is changed then ActualWidth is also changed.
  /// </summary>
  [DefaultValue(true)]
  public bool IsVisible
  {
    get => _isVisible;
    set
    {
      if (_isVisible != value)
      {
        _isVisible = value;
        if (IsVisible)
          ActualWidth = Width;
        else
          ActualWidth = 0;
        NotifyPropertyChanged(nameof(IsVisible));
      }
    }
  }
  private bool _isVisible = true;

  /// <summary>
  /// Stored Width of the column. 
  /// </summary>
  [DefaultValue(double.NaN)]
  public double Width
  {
    get => _Width;
    set
    {
      if (_Width != value)
      {
        _Width = value;
        NotifyPropertyChanged(nameof(Width));
      }
      if (IsVisible) 
        ActualWidth = value;
    }
  }
  private double _Width = double.NaN;

  /// <summary>
  /// Actual width of the column. Is changed between 0 and HiddenWidth
  /// when IsVisible is changed.
  /// </summary>
  [XmlIgnore]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  [DefaultValue(double.NaN)]
  public double ActualWidth
  {
    get => _actualWidth;
    set
    {
      if (_actualWidth != value)
      {
        _actualWidth = value;
        NotifyPropertyChanged(nameof(ActualWidth));
      }
    }
  }
  private double _actualWidth = double.NaN;

  /// <summary>
  /// Gets a header text of the column. 
  /// Header text can be get from Header or HiddenHeader, or from Column.GetHeader.
  /// </summary>
  public string? GetHeaderText()
  {
    var result = Header as string;
    if (result == null)
      result = HiddenHeader;
    if (result == null && Column is DataGridColumn dataGridColumn)
      result = dataGridColumn.GetHeaderText();
    return result;
  }

  /// <summary>
  /// Name of the column to display.
  /// </summary>

  public string? ColumnName
  {
    get { return _ColumnName ?? GetHeaderText(); }
    set
    {
      if (_ColumnName != value)
      {
        _ColumnName = value;
        NotifyPropertyChanged(nameof(ColumnName));
      }
    }
  }
  private string? _ColumnName;


  /// <summary>
  /// Event used to implement <see cref="INotifyPropertyChanged"/> interface.
  /// </summary>
  public event PropertyChangedEventHandler? PropertyChanged;

  private void NotifyPropertyChanged(string propertyName)
  {
    if (PropertyChanged != null)
      PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
  }


  /// <summary>
  /// Overriden to help debugging.
  /// </summary>
  /// <returns></returns>
  public override string? ToString()
  {
    var result = base.ToString();
    if (Header != null)
      result += ": " + Header;
    return result;
  }
}

