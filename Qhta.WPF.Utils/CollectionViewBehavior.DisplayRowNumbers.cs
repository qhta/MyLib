namespace Qhta.WPF.Utils;

public partial class CollectionViewBehavior
{

  #region DisplayRowNumberOffset

  /// <summary>
  /// Sets the starting value of the row header if enabled.
  /// </summary>
  public static DependencyProperty DisplayRowNumberOffsetProperty =
      DependencyProperty.RegisterAttached("DisplayRowNumberOffset",
                                          typeof(int),
                                          typeof(CollectionViewBehavior),
                                          new FrameworkPropertyMetadata(0, OnDisplayRowNumberOffsetChanged));

  /// <summary>
  /// Getter for DisplayRowNumberOffset property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static int GetDisplayRowNumberOffset(DependencyObject target)
  {
    return (int)target.GetValue(DisplayRowNumberOffsetProperty);
  }

  /// <summary>
  /// Setter for DisplayRowNumberOffset property.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetDisplayRowNumberOffset(DependencyObject target, int value)
  {
    target.SetValue(DisplayRowNumberOffsetProperty, value);
  }


  private static void OnDisplayRowNumberOffsetChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
  {
    DataGrid? dataGrid = target as DataGrid;
    int offset = (int)e.NewValue;

    if (dataGrid!=null && GetDisplayRowNumber(target))
    {
      VisualTreeHelperExt.GetVisualChildCollection<DataGridRow>(dataGrid).
              ForEach(d => d.Header = d.GetIndex() + offset);
    }
  }

  #endregion

  #region DisplayRowNumber
  /// <summary>
  /// Enable display of row header automatically.
  /// </summary>
  /// <remarks>
  /// Source: 
  /// </remarks>
  public static DependencyProperty DisplayRowNumberProperty =
      DependencyProperty.RegisterAttached("DisplayRowNumber",
                                          typeof(bool),
                                          typeof(CollectionViewBehavior),
                                          new FrameworkPropertyMetadata(false, OnDisplayRowNumberChanged));

  /// <summary>
  /// Getter for DisplayRowNumber property.
  /// </summary>
  /// <param name="target"></param>
  /// <returns></returns>
  public static bool GetDisplayRowNumber(DependencyObject target)
  {
    return (bool)target.GetValue(DisplayRowNumberProperty);
  }

  /// <summary>
  /// Setter for DisplayRowNumber property.
  /// </summary>
  /// <param name="target"></param>
  /// <param name="value"></param>
  public static void SetDisplayRowNumber(DependencyObject target, bool value)
  {
    target.SetValue(DisplayRowNumberProperty, value);
  }

  private static void OnDisplayRowNumberChanged(DependencyObject target, DependencyPropertyChangedEventArgs e)
  {
    DataGrid? dataGrid = target as DataGrid;
    if (dataGrid != null && (bool)e.NewValue == true)
    {
      int offset = GetDisplayRowNumberOffset(target);

      EventHandler<DataGridRowEventArgs> loadedRowHandler = null!;
      loadedRowHandler = (object? sender, DataGridRowEventArgs ea) =>
      {
        if (GetDisplayRowNumber(dataGrid) == false)
        {
          dataGrid.LoadingRow -= loadedRowHandler;
          return;
        }
        ea.Row.Header = ea.Row.GetIndex() + offset;
      };
      dataGrid.LoadingRow += loadedRowHandler;

      ItemsChangedEventHandler itemsChangedHandler = null!;
      itemsChangedHandler = (object sender, ItemsChangedEventArgs ea) =>
      {
        if (GetDisplayRowNumber(dataGrid) == false)
        {
          dataGrid.ItemContainerGenerator.ItemsChanged -= itemsChangedHandler;
          return;
        }
        VisualTreeHelperExt.GetVisualChildCollection<DataGridRow>(dataGrid).
            ForEach(d => d.Header = d.GetIndex() + offset);
      };
      dataGrid.ItemContainerGenerator.ItemsChanged += itemsChangedHandler;
    }
  }
  #endregion // DisplayRowNumber

}
