namespace Qhta.WPF.Utils;

/// <summary>
/// A class to define background behavior. 
/// It enables a control to observe target object "Waiting" boolean property and display a waiting cursor.
/// Target object must implement <see cref="INotifyPropertyChanged"/> interface.
/// There are two properties:
/// <list type="table">
///   <item><term>Target: object</term><description>Declares the observed object</description></item>
///   <item><term>EnableWaitingCursor: bool</term><description>Enables observing control to display waiting cursor.</description></item>
/// </list>
/// </summary>
public static class BackgroundBehavior
{
  #region Target property

  /// <summary>
  /// Getter for Target property.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static object GetTarget(DependencyObject obj)
  {
    return (INotifyPropertyChanged)obj.GetValue(TargetProperty);
  }

  /// <summary>
  /// Setter for Target property.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="value"></param>
  public static void SetTarget(DependencyObject obj, object value)
  {
    obj.SetValue(TargetProperty, value);
  }

  /// <summary>
  /// Dependency property to store Target property.
  /// </summary>
  public static readonly DependencyProperty TargetProperty =
      DependencyProperty.RegisterAttached("Target", typeof(object), typeof(BackgroundBehavior),
        new UIPropertyMetadata(false));
  #endregion

  #region EnableWaitingCursor property

  /// <summary>
  /// Getter for EnableWaitingCursor property.
  /// </summary>
  /// <param name="obj"></param>
  /// <returns></returns>
  public static bool GetEnableWaitingCursor(DependencyObject obj)
  {
    return (bool)obj.GetValue(EnableWaitingCursorProperty);
  }

  /// <summary>
  /// Setter for EnableWaitingCursor property.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="value"></param>
  public static void SetEnableWaitingCursor(DependencyObject obj, bool value)
  {
    obj.SetValue(EnableWaitingCursorProperty, value);
  }

  /// <summary>
  /// Dependency property to store EnableWaitingCursor property.
  /// </summary>
  public static readonly DependencyProperty EnableWaitingCursorProperty =
      DependencyProperty.RegisterAttached("EnableWaitingCursor", typeof(bool), typeof(CollectionViewBehavior),
        new UIPropertyMetadata(false, EnableWaitingCursorPropertyChangedCallback));

  /// <summary>
  /// Callback method invoked when EnableWaitingCursor property was changed.
  /// If it is true, then it adds a handler to observing control Loaded event.
  /// </summary>
  /// <param name="obj"></param>
  /// <param name="args"></param>
  private static void EnableWaitingCursorPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
  {
    //Debug.WriteLine($"EnableWaitingCursorPropertyChangedCallback({obj}, {args.NewValue})");
    if (obj is Control control)
    {
      if (args.NewValue is bool bv)
      {
        if (bv)
          control.Loaded += Control_Loaded;
        else
          control.Loaded -= Control_Loaded;
      }
    }
  }
  #endregion
  /// <summary>
  /// Handler for observing control Loaded event. 
  /// Invokes an EnableWaitingCursor method for the control with its EnabledWaitingCursor property as an argument.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  private static void Control_Loaded(object sender, RoutedEventArgs args)
  {
    //Debug.WriteLine($"ItemsControl_Loaded({sender})");
    if (sender is Control control)
    {
      var enable = GetEnableWaitingCursor(control);
      EnableWaitingCursor(control, enable);
    }
  }

  /// <summary>
  /// A method that adds or removes a control to internal Observers dictionary.
  /// </summary>
  /// <param name="control"></param>
  /// <param name="enable"></param>
  private static void EnableWaitingCursor(Control control, bool enable)
  {
    var target = GetTarget(control);
    if (target is INotifyPropertyChanged targetObject)
    {
      if (!observerMapping.TryGetValue(targetObject, out var observers))
      {
        observers = new Observers();
        observerMapping.Add(targetObject, observers);
      }
      if (enable)
      {
        if (!observers.Contains(control))
          observers.Add(control);
        else if (observers.Contains(control))
          observers.Remove(control);
      }
    }
  }

  /// <summary>
  /// A method to handle target object PropertyChanged event. It observes "Waiting" property of the target object.
  /// If it is set to true, then it sets each observing control cursor to Wait cursor.
  /// If it is set to false, then it sets each observing control cursor to Arrow cursor.
  /// </summary>
  /// <param name="sender"></param>
  /// <param name="args"></param>
  private static void TargetObject_PropertyChanged(object sender, PropertyChangedEventArgs args)
  {
    if (args.PropertyName == "Waiting")
    {
      if (observerMapping.TryGetValue(sender, out var observers))
      {
        var value = (bool?)sender.GetType()?.GetProperty("Waiting")?.GetValue(sender);
        if (value != null)
          foreach (var control in observers)
          {
            if (value == true)
            {
              control.Cursor = Cursors.Wait;
              control.ForceCursor = true;
            }
            else
            {
              control.Cursor = Cursors.Arrow;
              control.ForceCursor = true;
            }
          }
      }
    }
  }

  /// <summary>
  /// Helper definition for Observers hash set type.
  /// </summary>
  class Observers : HashSet<Control> { }

  /// <summary>
  /// Internal dictionary assigning a hash set of observing controls to each target object.
  /// </summary>
  private static Dictionary<object, Observers> observerMapping = new Dictionary<object, Observers>();

}
