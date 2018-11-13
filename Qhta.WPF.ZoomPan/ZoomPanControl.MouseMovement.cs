using System.Windows;
using System.Windows.Input;

namespace Qhta.WPF.ZoomPan
{
  // ZoomPanControl extension to handle mouse movement events
  public partial class ZoomPanControl
  {

    #region IsMousePanEnabled property
    /// <summary>
    /// Set to 'true' to enable content movement by mouse click and drag.
    /// This is set to 'false' by default.
    /// </summary>
    public bool IsMousePanEnabled 
    {
      get => (bool)GetValue(IsMousePanEnabledProperty);
      set => SetValue(IsMousePanEnabledProperty, value);
    }

    public static readonly DependencyProperty IsMousePanEnabledProperty = DependencyProperty.Register
      ("IsMousePanEnabled", typeof(bool), typeof(ZoomPanControl),
         new FrameworkPropertyMetadata(false, IsMousePanEnabledPropertyChanged
      ));

    private static void IsMousePanEnabledPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
      (sender as ZoomPanControl).IsMousePanEnabledChanged();
    }

    private void IsMousePanEnabledChanged()
    {
      if (Content is FrameworkElement content)
      {
        if (IsMousePanEnabled)
        {
          content.Cursor = Cursors.SizeAll;
          //content.IsEnabled=false;
        }
        else
        {
          content.Cursor = Cursors.Arrow;
          //content.IsEnabled=true;
        }
      }
    }

    #endregion

    /// <summary>
    /// Current state of the mouse handling logic
    /// </summary>
    private MouseHandlingMode mouseHandlingMode { get; set; }

    /// <summary>
    /// The point that was clicked relative to the ZoomControl.
    /// </summary>
    private Point origZoomControlMouseDownPoint { get; set; }

    /// <summary>
    /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
    /// </summary>
    private Point origContentMouseDownPoint { get; set; }

    /// <summary>
    /// Records which mouse button clicked during mouse dragging.
    /// </summary>
    private MouseButton mouseButtonDown { get; set; }

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
      var control = this.Content as FrameworkElement;
      if (control==null)
        return;
      control.Focus();
      Keyboard.Focus(control);

      mouseButtonDown = e.ChangedButton;
      origZoomControlMouseDownPoint = e.GetPosition(this);
      origContentMouseDownPoint = e.GetPosition(control);

      if ((Keyboard.Modifiers & ModifierKeys.Shift) != 0 &&
          (e.ChangedButton == MouseButton.Left ||
           e.ChangedButton == MouseButton.Right))
      {
        // Shift + left- or right-down initiates zooming mode.
        mouseHandlingMode = MouseHandlingMode.Zooming;
      }
      else if (mouseButtonDown == MouseButton.Left)
      {
        if (IsMousePanEnabled)
          mouseHandlingMode = MouseHandlingMode.Panning;
      }

      if (mouseHandlingMode != MouseHandlingMode.None)
      {
        // Capture the mouse so that we eventually receive the mouse up event.
        this.CaptureMouse();
        e.Handled = true;
      }
    }

    protected override void OnPreviewMouseUp(MouseButtonEventArgs e)
    {
      if (mouseHandlingMode != MouseHandlingMode.None)
      {
        if (mouseHandlingMode == MouseHandlingMode.Zooming)
        {
          if (mouseButtonDown == MouseButton.Left)
          {
            // Shift + left-click zooms in on the content.
            ZoomIn();
          }
          else if (mouseButtonDown == MouseButton.Right)
          {
            // Shift + left-click zooms out from the content.
            ZoomOut();
          }
        }

        this.ReleaseMouseCapture();
        mouseHandlingMode = MouseHandlingMode.None;
        e.Handled = true;
      }
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
      var control = this.Content as FrameworkElement;
      if (control==null)
        return;
      if (mouseHandlingMode == MouseHandlingMode.Panning)
      {
        //
        // The user is left-dragging the mouse.
        // Pan the viewport by the appropriate amount.
        //
        Point curContentMousePoint = e.GetPosition(control);
        Vector dragOffset = curContentMousePoint - origContentMouseDownPoint;

        if (dragOffset.X!=0 || dragOffset.Y!=0)
        {
          this.ContentOffsetX -= dragOffset.X;
          this.ContentOffsetY -= dragOffset.Y;
        }
        e.Handled = true;
      }
    }

    protected override void OnMouseWheel(MouseWheelEventArgs e)
    {
      e.Handled = true;

      if (e.Delta > 0)
      {
        ZoomIn();
      }
      else if (e.Delta < 0)
      {
        ZoomOut();
      }
    }

  }
}
