namespace Qhta.WPF.Utilss
{
  public class ColumnsViewInfo : ObservableCollection<ColumnViewInfo>
  {
    public ColumnsViewInfo() { }

    //// Do not save Capacity
    //public new int Capacity { get; set; }

    [XmlIgnore]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DefaultValue(double.NaN)]
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

  public class ColumnViewInfo : DependencyObject, INotifyPropertyChanged
  {
    public ColumnViewInfo() { }

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

    public string Header { get; set; }
    public string PropertyName { get; set; }

    [XmlIgnore]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public PropertyInfo PropertyInfo { get; set; }

    [DefaultValue(true)]
    /// <summary>
    /// Property to change column visibility.
    /// A converter to Control.Visibility may be needed to bind to column Visibility.
    /// If IsVisibile is changed then ActualWidth is also changed.
    /// </summary>
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

    [DefaultValue(double.NaN)]
    /// <summary>
    /// Stored Width of the column. 
    /// </summary>
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

    [XmlIgnore]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [DefaultValue(double.NaN)]
    /// <summary>
    /// Actual width of the column. Is changed between 0 and HiddenWidth
    /// when IsVisible is changed.
    /// </summary>
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

    public event PropertyChangedEventHandler PropertyChanged;

    private void NotifyPropertyChanged(string propertyName)
    {
      if (PropertyChanged != null)
        PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    public override string ToString()
    {
      var result = base.ToString();
      if (Header != null)
        result += ": " + Header;
      return result;
    }
  }

}
