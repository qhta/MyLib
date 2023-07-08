using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  public static class BackgroundBehavior
  {

    public static object GetTarget(DependencyObject obj)
    {
      return (INotifyPropertyChanged)obj.GetValue(TargetProperty);
    }

    public static void SetTarget(DependencyObject obj, object value)
    {
      obj.SetValue(TargetProperty, value);
    }

    public static readonly DependencyProperty TargetProperty =
        DependencyProperty.RegisterAttached("Target", typeof(object), typeof(BackgroundBehavior),
          new UIPropertyMetadata(false));

    public static bool GetEnableWaitingCursor(DependencyObject obj)
    {
      return (bool)obj.GetValue(EnableWaitingCursorProperty);
    }

    public static void SetEnableWaitingCursor(DependencyObject obj, bool value)
    {
      obj.SetValue(EnableWaitingCursorProperty, value);
    }

    public static readonly DependencyProperty EnableWaitingCursorProperty =
        DependencyProperty.RegisterAttached("EnableWaitingCursor", typeof(bool), typeof(CollectionViewBehavior),
          new UIPropertyMetadata(false, EnableWaitingCursorPropertyChangedCallback));

    public static void EnableWaitingCursorPropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      //Debug.WriteLine($"EnableWaitingCursorPropertyChangedCallback({obj}, {args.NewValue})");
      if (obj is Control control)
      {
        control.Loaded += Control_Loaded;
      }
    }

    private static void Control_Loaded(object sender, RoutedEventArgs e)
    {
      //Debug.WriteLine($"ItemsControl_Loaded({sender})");
      if (sender is Control control)
      {
        var enable = GetEnableWaitingCursor(control);
        EnableWaitingCursor(control, enable);
      }
    }

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

    private static void TargetObject_PropertyChanged(object sender, PropertyChangedEventArgs args)
    {
      if (args.PropertyName=="Waiting")
      {
        if (observerMapping.TryGetValue(sender, out var observers))
        {
          var value = (bool)sender.GetType().GetProperty("Waiting").GetValue(sender);
          foreach (var control in observers)
          {
            if (value)
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

    class Observers : HashSet<Control> { }

    private static Dictionary<object, Observers> observerMapping = new Dictionary<object, Observers>();

  }

}
