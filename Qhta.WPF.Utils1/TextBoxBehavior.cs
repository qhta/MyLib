using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  public partial class TextBoxBehavior
  {
    public static bool GetIsNullable(DependencyObject obj)
    {
      return (bool)obj.GetValue(IsNullableProperty);
    }

    public static void SetIsNullable(DependencyObject obj, bool value)
    {
      obj.SetValue(IsNullableProperty, value);
    }

    public static readonly DependencyProperty IsNullableProperty =
        DependencyProperty.RegisterAttached("IsNullable", typeof(bool), typeof(TextBoxBehavior),
          new UIPropertyMetadata(false, IsNullablePropertyChangedCallback));

    public static void IsNullablePropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
    public static void AddClearedHandler(DependencyObject obj, RoutedEventHandler handler)
    {
      if (obj is UIElement element)
        element.AddHandler(TextBoxBehavior.ClearedEvent, handler);
    }

    public static void RemoveClearedHandler(DependencyObject obj, RoutedEventHandler handler)
    {
      if (obj is UIElement element)
        element.RemoveHandler(TextBoxBehavior.ClearedEvent, handler);
    }

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

}
