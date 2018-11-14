using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Qhta.WPF.ZoomPan
{
  // ZoomPanControl extension to handle mouse movement events
  public partial class ZoomPanControl
  {

    #region MouseMinDistance property
    /// <summary>
    /// Minimal distance of mouse movement to start mouse select mode.
    /// Default is 0.
    /// </summary>
    public int MouseMinDistance
    {
      get => (int)GetValue(MouseMinDistanceProperty);
      set => SetValue(MouseMinDistanceProperty, value);
    }

    public static readonly DependencyProperty MouseMinDistanceProperty = DependencyProperty.Register
      ("MouseMinDistance", typeof(int), typeof(ZoomPanControl),
         new FrameworkPropertyMetadata(0));

    #endregion

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
        }
        else
        {
          content.Cursor = Cursors.Arrow;
        }
      }
    }

    #endregion

    #region IsMouseSelectEnabled property
    /// <summary>
    /// Set to 'true' to enable content movement by mouse click and drag.
    /// This is set to 'false' by default.
    /// </summary>
    public bool IsMouseSelectEnabled
    {
      get => (bool)GetValue(IsMouseSelectEnabledProperty);
      set => SetValue(IsMouseSelectEnabledProperty, value);
    }

    public static readonly DependencyProperty IsMouseSelectEnabledProperty = DependencyProperty.Register
      ("IsMouseSelectEnabled", typeof(bool), typeof(ZoomPanControl),
         new FrameworkPropertyMetadata(false));

    #endregion

    //public Shape selectingShape = 
    //  new Rectangle
    //  { Stroke = Brushes.Blue, StrokeThickness=1, StrokeDashArray=new DoubleCollection(new double[] {2,2 }) };


    #region private fields
    /// <summary>
    /// Current state of the mouse handling logic
    /// </summary>
    private MouseHandlingMode mouseHandlingMode;

    /// <summary>
    /// The point that was clicked relative to the ZoomControl.
    /// </summary>
    private Point origZoomControlMouseDownPoint;

    /// <summary>
    /// The point that was clicked relative to the content that is contained within the ZoomAndPanControl.
    /// </summary>
    private Point origContentMouseDownPoint;

    /// <summary>
    /// Records if the mouse button is currently down
    /// </summary>
    private bool isMouseButtonDown;

    /// <summary>
    /// Records which mouse button clicked during mouse dragging.
    /// </summary>
    private MouseButton mouseButtonDown;

    private Canvas overlayCanvas;

    private Shape selectingShape;
    #endregion

    protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
    {
      var control = this.Content as FrameworkElement;
      if (control==null)
        return;
      control.Focus();
      Keyboard.Focus(control);

      mouseButtonDown = e.ChangedButton;
      isMouseButtonDown = true;
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
        else
        if (mouseHandlingMode == MouseHandlingMode.Selecting)
        {
          if (overlayCanvas!=null && selectingShape!=null)
          {
            if (Content is IAreaSelectionTarget selectionTarget)
            {
              var geometry = selectingShape.RenderedGeometry;
              var descale = 1.0/ContentScale;
              var selectionOffsetX = Canvas.GetLeft(selectingShape);
              var selectionOffsetY = Canvas.GetTop(selectingShape);
              var contentOffsetX = contentOffsetTransform.X;
              var contentOffsetY = contentOffsetTransform.Y;
              var translateX = (selectionOffsetX/ContentScale-contentOffsetX);
              var translateY = (selectionOffsetY/ContentScale-contentOffsetY);
              var transformGroup = new TransformGroup();
              transformGroup.Children.Add(new ScaleTransform(descale, descale));
              transformGroup.Children.Add(new TranslateTransform(translateX, translateY));
              geometry.Transform = transformGroup;
              selectionTarget.NotifyAreaSelection(this, geometry);
            }
            overlayCanvas.Children.Remove(selectingShape);
          }
        }

        this.ReleaseMouseCapture();
        mouseHandlingMode = MouseHandlingMode.None;
        e.Handled = true;
      }
      isMouseButtonDown = false;
    }

    protected override void OnPreviewMouseMove(MouseEventArgs e)
    {
      if (!isMouseButtonDown)
        return;
      var control = this.Content as FrameworkElement;
      if (control==null)
        return;
      Point curMousePoint = e.GetPosition(this);
      Vector moveOffset = curMousePoint - origZoomControlMouseDownPoint;
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
      if (moveOffset.X>=MouseMinDistance || moveOffset.Y>=MouseMinDistance)
      {
        // The minimal mouse distance should be small so there is no need to evaluate current distance with formula
        // Math.Sqrt (dragOffset.X*dragOffset.X + dragOffset.Y*dragOffset.Y)>=MouseMinDistance
        if (IsMouseSelectEnabled && mouseHandlingMode == MouseHandlingMode.None)
        {
          if (overlayCanvas!=null && selectingShape!=null)
          {
            var bounds = GetSelectingRectBounds(origZoomControlMouseDownPoint, moveOffset);
            selectingShape.Width=bounds.Width;
            selectingShape.Height=bounds.Height;
            overlayCanvas.Children.Add(selectingShape);
            Canvas.SetLeft(selectingShape, bounds.Left);
            Canvas.SetTop(selectingShape, bounds.Top);
            mouseHandlingMode = MouseHandlingMode.Selecting;
          }
        }
      }
      if (mouseHandlingMode == MouseHandlingMode.Selecting)
      {
        var bounds = GetSelectingRectBounds(origZoomControlMouseDownPoint, moveOffset);
        selectingShape.Width=bounds.Width;
        selectingShape.Height=bounds.Height;
        Canvas.SetLeft(selectingShape, bounds.Left);
        Canvas.SetTop(selectingShape, bounds.Top);
        e.Handled = true;
      }
      if (mouseHandlingMode != MouseHandlingMode.Selecting)
      {
        if (overlayCanvas!=null && selectingShape!=null)
        {
          if (selectingShape.Parent==overlayCanvas)
            overlayCanvas.Children.Remove(selectingShape);
        }
      }
    }

    private Rect GetSelectingRectBounds(Point startCoords, Vector distance)
    {
      var left = startCoords.X;
      var top = startCoords.Y;
      var width = distance.X;
      var height = distance.Y;
      if (width<0)
      {
        width = -width;
        left = left-width;
      }
      if (height<0)
      {
        height = -height;
        top = top-height;
      }
      return new Rect(left, top, width, height);
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
