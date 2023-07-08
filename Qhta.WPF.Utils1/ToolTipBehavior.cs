using System.Diagnostics;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;

namespace Qhta.WPF.Utils
{
  public class ToolTipBehavior
  {

    static ToolTipBehavior()
    {
      ToolTipPopup.MouseEnter += ToolTipPopup_MouseEnter;
      ToolTipPopup.MouseLeave += ToolTipPopup_MouseLeave;
      ToolTipPopup.Closed += ToolTipPopup_Closed;
      ToolTipTimer.Enabled = false;
      ToolTipTimer.Elapsed += ToolTipTimer_Elapsed;
    }

    #region ActiveToolTipEnabled property
    /// <summary>
    ///   ActiveToolTipEnabled behavior allows ToolTip Popup to contain active hyperlinks
    /// </summary>
    public static bool GetActiveToolTipEnabled(DependencyObject obj)
    {
      return (bool)obj.GetValue(ActiveToolTipEnabledProperty);
    }

    /// <summary>
    ///   ActiveToolTipEnabled behavior allows ToolTip Popup to contain active hyperlinks
    /// </summary>
    public static void SetActiveToolTipEnabled(DependencyObject obj, bool value)
    {
      obj.SetValue(ActiveToolTipEnabledProperty, value);
    }

    /// <summary>
    ///   Dependency property for ActiveToolTipEnabled
    /// </summary>
    public static readonly DependencyProperty ActiveToolTipEnabledProperty = DependencyProperty.RegisterAttached
      ("ActiveToolTipEnabled", typeof(bool), typeof(ListBoxBehavior),
          new UIPropertyMetadata(false, ActiveToolTipEnabledChanged));

    public static void ActiveToolTipEnabledChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
    {
      if (obj is DependencyObject element)
      {
        if ((bool)args.NewValue)
        {
          ToolTipService.AddToolTipOpeningHandler(element, ToolTip_Opening);
          //ToolTipService.AddToolTipClosingHandler(element, ToolTip_Closing);
        }
        else
        {
          ToolTipService.RemoveToolTipOpeningHandler(element, ToolTip_Opening);
          //ToolTipService.RemoveToolTipClosingHandler(element, ToolTip_Closing);
        }
      }
    }

    private static void ToolTip_Opening(object sender, ToolTipEventArgs args)
    {
      //Debug.WriteLine($"ToolTip_Opening({sender})");
      FrameworkElement toolTip = null;
      DependencyObject element = null;
      if (sender is FrameworkElement element1 /*&& element.ToolTip is FrameworkElement toolTip*/)
      {
        element = element1;
        toolTip = element1.ToolTip as FrameworkElement;
        ToolTipPopup.PlacementTarget = element1;
        ToolTipPopup.Placement = ToolTipService.GetPlacement(element1);
        element1.MouseLeave += TargetControl_MouseLeave;
      }
      else
      if (sender is FrameworkContentElement element2 /*&& element2.ToolTip is FrameworkElement toolTip2*/)
      {
        element = element2;
        toolTip = element2.ToolTip as FrameworkElement;
        ToolTipPopup.Placement = PlacementMode.MousePoint;
      }
      if (toolTip != null)
      {
        TargetControl = element;
        if (toolTip.Parent is Border oldBorder)
        {
          // release toolTip from previous parent;
          oldBorder.Child = null;
        }
        Border border = new Border
        {
          Background = SystemColors.ControlBrush,
          BorderBrush = SystemColors.ControlDarkBrush,
          BorderThickness = new Thickness(1),
          Padding = new Thickness(5)
        };
        border.Child = toolTip;
        ToolTipPopup.PlacementRectangle = ToolTipService.GetPlacementRectangle(element);
        ToolTipPopup.VerticalOffset = ToolTipService.GetVerticalOffset(element);
        ToolTipPopup.HorizontalOffset = ToolTipService.GetHorizontalOffset(element);
        ToolTipPopup.Child = border;
        //ToolTipPopup.Focusable = true;
        //ToolTipPopup.StaysOpen = false;
        ToolTipPopup.IsOpen = true;
        //Debug.WriteLine($"ToolTip_Opening handled");
        args.Handled = true;
        foreach (var hyperlink in VisualTreeHelperExt.FindAllDescendants<Hyperlink>(toolTip))
        {
          //Debug.WriteLine($"Hyperlink {hyperlink.NavigateUri}");
          hyperlink.PreviewMouseLeftButtonDown += Hyperlink_MouseLeftButtonDown;
        }
      }
    }

    private static void Hyperlink_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
      //Debug.WriteLine($"Hyperlink_MouseLeftButtonDown({sender})");
      if (sender is Hyperlink hyperlink)
      {
        //hyperlink.DoClick();
        Hyperlink_Click(sender, e);
        e.Handled = true;
      }
    }

    private static void Hyperlink_Click(object sender, RoutedEventArgs e)
    {
      //Debug.WriteLine($"Hyperlink_Click({sender})");
      Process.Start(((Hyperlink)sender).NavigateUri.ToString());
      e.Handled = true;
    }

    private static void TargetControl_MouseLeave(object sender, MouseEventArgs args)
    {
      //Debug.WriteLine($"TargetControl_MouseLeave({sender})");
      RequestToolTipPopupClose(100);
    }

    private static void RequestToolTipPopupClose(int after)
    {
      if (ToolTipTimer != null)
      {
        if (ToolTipPopup.IsOpen)
        {
          ToolTipTimer.Enabled = false;
          ToolTipTimer.Interval = 200;
          ShouldOpen = false;
          ShouldClose = true;
          ToolTipTimer.Enabled = true;
          //Debug.WriteLine($"ToolTipTimer.Enabled({after})");
        }
        else
          ToolTipTimer.Enabled = false;
      }
    }

    //private static void ToolTip_Closing(object sender, ToolTipEventArgs args)
    //{
    //  //Debug.WriteLine($"ToolTip_Closing({sender})");
    //  if (ToolTipTimer != null)
    //  {
    //    if (ToolTipPopup.IsOpen)
    //    {
    //      ToolTipTimer.Enabled = false;
    //      ToolTipTimer.Interval = 100;
    //      ShouldOpen = false;
    //      ShouldClose = true;
    //      ToolTipTimer.Enabled = true;
    //    }
    //    else
    //      ToolTipTimer.Enabled = false;
    //    //Debug.WriteLine($"ToolTip_Closing handled");
    //    args.Handled = true;
    //  }
    //}

    public static Popup ToolTipPopup = new Popup { /*Placement = PlacementMode.Bottom*/ };

    private static void ToolTipPopup_Closed(object sender, System.EventArgs e)
    {
      //Debug.WriteLine($"ToolTip_Closed({sender})");
      var border = ToolTipPopup.Child as Border;
      border.Child = null;
    }

    #endregion

    private static void ToolTipPopup_MouseEnter(object sender, MouseEventArgs e)
    {
      //Debug.WriteLine($"ToolTipPopup_MouseEnter({sender})");
      if (ToolTipTimer != null)
        ToolTipTimer.Enabled = false;
      ToolTipPopup.IsOpen = true;
    }

    private static void ToolTipPopup_MouseLeave(object sender, MouseEventArgs e)
    {
      //Debug.WriteLine($"ToolTipPopup_MouseLeave({sender})");
      RequestToolTipPopupClose(1000);
    }


    private static System.Timers.Timer ToolTipTimer = new System.Timers.Timer();
    private static bool ShouldOpen;
    private static bool ShouldClose;
    private static DependencyObject TargetControl;

    private static void ToolTipTimer_Elapsed(object sender, ElapsedEventArgs e)
    {
      //Debug.WriteLine($"ToolTipTimer_Elapsed({sender}, ShouldOpen:{ShouldOpen}, ShouldClose:{ShouldClose})");
      if (ToolTipTimer != null && TargetControl != null)
      {
        ToolTipTimer.Enabled = false;
        if (ShouldOpen)
        {
          ShouldClose = false;
          ShouldOpen = false;
          TargetControl.Dispatcher.Invoke(() => { ToolTipPopup.IsOpen = true; });
        }
        else
        if (ShouldClose)
        {
          ShouldOpen = false;
          ShouldClose = false;
          TargetControl.Dispatcher.Invoke(() => { ToolTipPopup.IsOpen = false; });
        }
      }
    }


  }
}
