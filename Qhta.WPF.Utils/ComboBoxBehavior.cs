using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  public partial class ComboBoxBehavior
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
        DependencyProperty.RegisterAttached("IsNullable", typeof(bool), typeof(ComboBoxBehavior),
          new UIPropertyMetadata(false, IsNullablePropertyChangedCallback));

    public static void IsNullablePropertyChangedCallback(DependencyObject obj, DependencyPropertyChangedEventArgs args)
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
    public static void AddClearedHandler(DependencyObject obj, RoutedEventHandler handler)
    {
      if (obj is UIElement element)
        element.AddHandler(ComboBoxBehavior.ClearedEvent, handler);
    }

    public static void RemoveClearedHandler(DependencyObject obj, RoutedEventHandler handler)
    {
      if (obj is UIElement element)
        element.RemoveHandler(ComboBoxBehavior.ClearedEvent, handler);
    }

    public static readonly RoutedEvent ClearedEvent = EventManager.RegisterRoutedEvent
      ("Cleared",RoutingStrategy.Bubble,typeof(RoutedEventHandler), typeof(ComboBoxBehavior));

    private static void ComboBox_PreviewKeyDown(object sender, KeyEventArgs args)
    {
      if (sender is ComboBox comboBox)
      {
        if (args.Key == Key.Delete && Keyboard.Modifiers == ModifierKeys.Control)
        {
          if (comboBox != null && GetIsNullable(comboBox) && comboBox.IsEnabled)
          {
            comboBox.SelectedValue = null;
            comboBox.RaiseEvent(new RoutedEventArgs(ComboBoxBehavior.ClearedEvent, comboBox));
          }
        }
      }
    }

    private static void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs args)
    {
      //Debug.WriteLine($"ComboBox_SelectionChanged({sender}, addedItems={args.AddedItems}, removedItems={args.RemovedItems})");
    }

    public void ClearButton_Click(object sender, RoutedEventArgs args)
    {
      if (sender is Button button)
      {
        var comboBox = VisualTreeHelperExt.FindInVisualTreeUp<ComboBox>(button);
        if (comboBox != null && GetIsNullable(comboBox) && comboBox.IsEnabled)
        {
          comboBox.SelectedValue = null;
          comboBox.RaiseEvent(new RoutedEventArgs(ComboBoxBehavior.ClearedEvent, comboBox));
        }
      }
    }

    public void ComboBoxItem_Selected(object sender, RoutedEventArgs args)
    {
      //Debug.WriteLine($"ComboBoxItem_Selected ({sender})");
    }

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

}
