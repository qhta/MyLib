namespace Qhta.WPF.Utils;

/// <summary>
/// Observable collection of <see cref="ColumnViewInfo"/>.
/// </summary>
public class ColumnsViewInfo : ObservableCollection<ColumnViewInfo>
{
  /// <summary>
  /// Default constructor.
  /// </summary>
  public ColumnsViewInfo() { }

  /// <summary>
  /// Checks if all the items are selected.
  /// </summary>
  [XmlIgnore]
  [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  public bool? AllSelected
  {
    get
    {
      bool areAllSelected = true;
      bool areAllUnselected = true;
      foreach (var item in this)
      {
        if (item.IsVisible)
          areAllUnselected = false;
        else
          areAllSelected = false;
      }
      if (areAllSelected && !areAllUnselected)
        return true;
      if (!areAllSelected && areAllUnselected)
        return false;
      return null;
    }
    set
    {
      if (value == true)
      {
        foreach (var item in this)
          item.IsVisible = true;
      }
      else
      if (value == false)
      {
        foreach (var item in this)
          item.IsVisible = false;
      }

    }
  }
}

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
    return new ColumnViewInfo
    {
      Header = this.Header,
      Width = this.Width,
      IsVisible = this.IsVisible,
      PropertyName = this.PropertyName,
    };
  }

  /// <summary>
  /// Header text to be displayed.
  /// </summary>
  public string? Header { get; set; }

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

  ////[XmlIgnore]
  ////[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
  ////[DefaultValue(double.NaN)]
  ///// <summary>
  ///// Hidden width of the column. Should be initialized 
  ///// if ActualWidth is to be changed when IsVisible changes.
  //public double HiddenWidth
  //{
  //  get => _hiddenWidth;
  //  set
  //  {
  //    if (_hiddenWidth != value)
  //    {
  //      _hiddenWidth = value;
  //      NotifyPropertyChanged(nameof(HiddenWidth));
  //      if (IsVisible)
  //        ActualWidth = value;
  //    }
  //  }

  //}
  //private double _hiddenWidth = double.NaN;

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
