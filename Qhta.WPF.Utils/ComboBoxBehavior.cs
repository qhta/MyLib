namespace Qhta.WPF.Utils;

/// <summary>
/// ComboBox behavior class that defines its IsNullable property and Cleared event.
/// </summary>
public partial class ComboBoxBehavior
{
  #region IsNullable property

  /// <summary>
  /// Getter for IsNullable property.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static bool GetIsNullable(DependencyObject obj)
  {
    return (bool)obj.GetValue(IsNullableProperty);
  }

  /// <summary>
  /// Setter for IsNullable property.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="value"></param>
  public static void SetIsNullable(DependencyObject obj, bool value)
  {
    obj.SetValue(IsNullableProperty, value);
  }

  /// <summary>
  /// Dependency property to store IsNullable property.
  /// </summary>
  public static readonly DependencyProperty IsNullableProperty =
      DependencyProperty.RegisterAttached("IsNullable", typeof(bool), typeof(ComboBoxBehavior),
        new UIPropertyMetadata(false, IsNullablePropertyChangedCallback));

  /// <summary>
  /// Callback method invoked when IsNullable property was changed.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="args"></param>
  private static void IsNullablePropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
  {
    //Debug.WriteLine($"IsNullablePropertyChangedCallback({obj}, {args.NewValue})");
    if (obj is ComboBox comboBox)
    {
      if ((bool)args.NewValue)
      {
        Validation.SetErrorTemplate(comboBox, null);
        comboBox.SelectionChanged += ComboBox_SelectionChanged;
        comboBox.PreviewKeyDown += ComboBox_PreviewKeyDown;
      }
    }
  }
  #endregion

  #region Cleared event

  /// <summary>
  /// Add accessor for Cleared event.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="handler"></param>
  public static void AddClearedHandler(DependencyObject obj, RoutedEventHandler handler)
  {
    if (obj is UIElement element)
      element.AddHandler(ComboBoxBehavior.ClearedEvent, handler);
  }

  /// <summary>
  /// Remove accessor for Cleared event.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="handler"></param>
  public static void RemoveClearedHandler(DependencyObject obj, RoutedEventHandler handler)
  {
    if (obj is UIElement element)
      element.RemoveHandler(ComboBoxBehavior.ClearedEvent, handler);
  }

  /// <summary>
  /// Routed event to store Cleared event handler.
  /// </summary>
  public static readonly RoutedEvent ClearedEvent = EventManager.RegisterRoutedEvent
    ("Cleared",RoutingStrategy.Bubble,typeof(RoutedEventHandler), typeof(ComboBoxBehavior));
  #endregion

  /// <summary>
  /// Handler method for PreviewKeyDown event of the ComboBox. Handles Delete key.
  /// Clears SelectedValue and SelectedItem properties and raises Cleared event.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  private static void ComboBox_PreviewKeyDown(object sender, KeyEventArgs args)
  {
    if (sender is ComboBox comboBox)
    {
      if (args.Key == Key.Delete && Keyboard.Modifiers == ModifierKeys.Control)
      {
        if (comboBox != null && GetIsNullable(comboBox) && comboBox.IsEnabled)
        {
          comboBox.SelectedValue = null;
          comboBox.SelectedItem = null;
          comboBox.RaiseEvent(new RoutedEventArgs(ComboBoxBehavior.ClearedEvent, comboBox));
        }
      }
    }
  }

  /// <summary>
  /// Handler for SelectionChanged event. Now does nothing.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  private static void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
  {
    //Debug.WriteLine($"ComboBox_SelectionChanged({sender}, addedItems={args.AddedItems}, removedItems={args.RemovedItems})");
  }

  /// <summary>
  /// Handler for Click event of ClearButton (defined in default ComboBox style).
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  public void ClearButton_Click(object sender, RoutedEventArgs args)
  {
    if (sender is Button button)
    {
      var comboBox = VisualTreeHelperExt.FindAncestor<ComboBox>(button);
      if (comboBox != null && GetIsNullable(comboBox) && comboBox.IsEnabled)
      {
        comboBox.SelectedValue = null;
        comboBox.RaiseEvent(new RoutedEventArgs(ComboBoxBehavior.ClearedEvent, comboBox));
      }
    }
  }

  /// <summary>
  /// Handler for ComboBox item Selected event. Now does nothing.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  public void ComboBoxItem_Selected(object sender, RoutedEventArgs args)
  {
    //Debug.WriteLine($"ComboBoxItem_Selected ({sender})");
  }

  /// <summary>
  /// Handler for PreviewMouseLeftButtonDown of the ComboBox item. Clears the parent ComboBox SelectedValue.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  public void ComboBoxItem_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
  {
    if (sender is ComboBoxItem comboBoxItem)
    {
      var dataItem = comboBoxItem.DataContext;
      if (dataItem==null)
      {
        var comboBox = VisualTreeHelperExt.FindRootParent<ComboBox>(comboBoxItem);
        if (comboBox != null && GetIsNullable(comboBox) && comboBox.IsEnabled)
        {
          comboBox.SelectedValue = null;
        }
      }
    }
  }
}
