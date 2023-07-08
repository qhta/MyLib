﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  /// <summary>
  /// Class allows for reseting hard coded ListViewItem margins and paddings
  /// </summary>
  public class ListViewCustomizableCellPresenter : Decorator
  {
    protected override void OnVisualParentChanged(DependencyObject oldParent)
    {
      base.OnVisualParentChanged(oldParent);

      var cp = VisualTreeHelper.GetParent(this) as FrameworkElement;
      if (cp != null)
      {
        cp.Margin = Padding;
        cp.VerticalAlignment = VerticalAlignment;
        cp.HorizontalAlignment = HorizontalAlignment;
      }

      ResetGridViewRowPresenterMargin();
      ResetListViewItemPadding();
    }

    private T FindInVisualTreeUp<T>() where T : class
    {
      DependencyObject result = this;
      do
      {
        result = VisualTreeHelper.GetParent(result);
      }
      while (result != null && !(result is T));
      return result as T;
    }

    private void ResetGridViewRowPresenterMargin()
    {
      var gvrp = FindInVisualTreeUp<GridViewRowPresenter>();
      if (gvrp != null)
        gvrp.Margin = new Thickness(0);
    }

    private void ResetListViewItemPadding()
    {
      var lvi = FindInVisualTreeUp<ListViewItem>();
      if (lvi != null)
        lvi.Padding = new Thickness(0);
    }

    /// <summary>
    /// Padding dependency property registration
    /// </summary>
    public static readonly DependencyProperty PaddingProperty =
       DependencyProperty.Register("Padding", typeof(Thickness), typeof(ListViewCustomizableCellPresenter), new PropertyMetadata(default(Thickness)));

    /// <summary>
    /// Padding dependency property
    /// </summary>
    public Thickness Padding
    {
      get { return (Thickness)GetValue(PaddingProperty); }
      set { SetValue(PaddingProperty, value); }
    }
  }
}
