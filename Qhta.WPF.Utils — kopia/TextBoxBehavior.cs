namespace Qhta.WPF.Utils;

/// <summary>
/// Behavior class that defines TextBox properties.
/// </summary>
public partial class TextBoxBehavior
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
      DependencyProperty.RegisterAttached("IsNullable", typeof(bool), typeof(TextBoxBehavior),
        new UIPropertyMetadata(false, IsNullablePropertyChangedCallback));

  private static void IsNullablePropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
  {
    //Debug.WriteLine($"IsNullablePropertyChangedCallback({obj}, {args.NewValue})");
    if (obj is TextBox textBox)
    {
      if ((bool)args.NewValue)
      {
        Validation.SetErrorTemplate(textBox, null);
        textBox.PreviewKeyDown += TextBox_PreviewKeyDown;
      }
    }
  }
  #endregion

  #region Cleared event

  /// <summary>
  /// Add method for Cleared event handler.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="handler"></param>
  public static void AddClearedHandler(DependencyObject obj, RoutedEventHandler handler)
  {
    if (obj is UIElement element)
      element.AddHandler(TextBoxBehavior.ClearedEvent, handler);
  }

  /// <summary>
  /// Remove method for Cleared event handler.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="handler"></param>
  public static void RemoveClearedHandler(DependencyObject obj, RoutedEventHandler handler)
  {
    if (obj is UIElement element)
      element.RemoveHandler(TextBoxBehavior.ClearedEvent, handler);
  }

  /// <summary>
  /// Routed event to store Cleared event handler.
  /// </summary>
  public static readonly RoutedEvent ClearedEvent = EventManager.RegisterRoutedEvent
    ("Cleared",RoutingStrategy.Bubble,typeof(RoutedEventHandler), typeof(TextBoxBehavior));

  private static void TextBox_PreviewKeyDown(object sender, KeyEventArgs args)
  {
    if (sender is TextBox textBox)
    {
      if (args.Key == Key.Delete && Keyboard.Modifiers==ModifierKeys.Control)
      {
        if (textBox != null && GetIsNullable(textBox) && textBox.IsEnabled)
        {
          textBox.Text = null;
          textBox.RaiseEvent(new RoutedEventArgs(TextBoxBehavior.ClearedEvent, textBox));
        }
      }
    }
  }
  #endregion

  /// <summary>
  /// A method to handle ClearedButton Click event.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  public void ClearButton_Click(object sender, RoutedEventArgs args)
  {
    if (sender is Button button)
    {
      var comboBox = VisualTreeHelperExt.FindAncestor<TextBox>(button);
      if (comboBox != null && GetIsNullable(comboBox) && comboBox.IsEnabled)
      {
        comboBox.Text = null;
        comboBox.RaiseEvent(new RoutedEventArgs(TextBoxBehavior.ClearedEvent, comboBox));
      }
    }
  }

}
