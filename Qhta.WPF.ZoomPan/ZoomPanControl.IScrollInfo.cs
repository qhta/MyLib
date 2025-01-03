﻿// <copyright>
// This control is created by Ashley Davis and copyrighted under CPOL, and available as part of
// a CodeProject article at
//    http://www.codeproject.com/KB/WPF/zoomandpancontrol.aspx
// <date>This code is based on the article dated: 29 Jun 2010</date>
// </copyright>

using System;
using System.Linq;
using System.Text;
using System.Collections.Generic;

using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;

namespace Qhta.WPF.ZoomPan
{
  /// <summary>
  /// This is an extension to the ZoomPanControl class that implements
  /// the IScrollInfo interface properties and functions.
  /// 
  /// IScrollInfo is implemented to allow ZoomPanControl to be wrapped (in XAML)
  /// in a ScrollViewer.  IScrollInfo allows the ScrollViewer and ZoomPanControl to 
  /// communicate important information such as the horizontal and vertical scrollbar offsets.
  /// 
  /// There is a good series of articles showing how to implement IScrollInfo starting here:
  ///     http://blogs.msdn.com/bencon/archive/2006/01/05/509991.aspx
  ///     
  /// </summary>
  public partial class ZoomPanControl
  {

    #region IsMouseWheelScrollingEnabled property
    /// <summary>
    /// Set to 'true' to enable the mouse wheel to scroll the zoom and pan control.
    /// This is set to 'false' by default.
    /// </summary>
    public bool IsMouseWheelScrollingEnabled
    {
      get => (bool)GetValue(IsMouseWheelScrollingEnabledProperty);
      set => SetValue(IsMouseWheelScrollingEnabledProperty, value);
    }

    public static readonly DependencyProperty IsMouseWheelScrollingEnabledProperty =
            DependencyProperty.Register("IsMouseWheelScrollingEnabled", typeof(bool), typeof(ZoomPanControl),
                                        new FrameworkPropertyMetadata(false));
    #endregion

    /// <summary>
    /// Set to 'true' when the vertical scrollbar is enabled.
    /// </summary>
    public bool CanVerticallyScroll { get; set; }

    /// <summary>
    /// Set to 'true' when the vertical scrollbar is enabled.
    /// </summary>
    public bool CanHorizontallyScroll { get; set; }

    /// <summary>
    /// The width of the content (with 'ContentScale' applied).
    /// </summary>
    public double ExtentWidth => unScaledExtent.Width * ContentScale;

    /// <summary>
    /// The height of the content (with 'ContentScale' applied).
    /// </summary>
    public double ExtentHeight => unScaledExtent.Height * ContentScale;

    /// <summary>
    /// Get the width of the viewport onto the content.
    /// </summary>
    public double ViewportWidth => viewport.Width;

    /// <summary>
    /// Get the height of the viewport onto the content.
    /// </summary>
    public double ViewportHeight => viewport.Height;

    #region HorizontalOffset property & set method
    /// <summary>
    /// The offset of the horizontal scrollbar.
    /// </summary>
    public double HorizontalOffset => ContentOffsetX * ContentScale;
    /// <summary>
    /// Called when the offset of the horizontal scrollbar has been set.
    /// </summary>
    public void SetHorizontalOffset(double offset)
    {
      if (disableScrollOffsetSync)
        return;

      try
      {
        disableScrollOffsetSync = true;
        ContentOffsetX = offset / ContentScale;
      }
      finally
      {
        disableScrollOffsetSync = false;
      }
    }
    #endregion

    #region VerticalOffset property & set method
    /// <summary>
    /// The offset of the vertical scrollbar.
    /// </summary>
    public double VerticalOffset => ContentOffsetY * ContentScale;

    /// <summary>
    /// Called when the offset of the vertical scrollbar has been set.
    /// </summary>
    public void SetVerticalOffset(double offset)
    {
      if (disableScrollOffsetSync)
        return;

      try
      {
        disableScrollOffsetSync = true;

        ContentOffsetY = offset / ContentScale;
      }
      finally
      {
        disableScrollOffsetSync = false;
      }
    }
  #endregion

    #region ContentOffset moving methods

    /// <summary>
    /// Shift the content offset one line up.
    /// </summary>
    public void LineUp()
    {
      ContentOffsetY -= (ContentViewportHeight / 10);
    }

    /// <summary>
    /// Shift the content offset one line down.
    /// </summary>
    public void LineDown()
    {
      ContentOffsetY += (ContentViewportHeight / 10);
    }

    /// <summary>
    /// Shift the content offset one line left.
    /// </summary>
    public void LineLeft()
    {
      ContentOffsetX -= (ContentViewportWidth / 10);
    }

    /// <summary>
    /// Shift the content offset one line right.
    /// </summary>
    public void LineRight()
    {
      ContentOffsetX += (ContentViewportWidth / 10);
    }

    /// <summary>
    /// Shift the content offset one page up.
    /// </summary>
    public void PageUp()
    {
      ContentOffsetY -= ContentViewportHeight;
    }

    /// <summary>
    /// Shift the content offset one page down.
    /// </summary>
    public void PageDown()
    {
      ContentOffsetY += ContentViewportHeight;
    }

    /// <summary>
    /// Shift the content offset one page left.
    /// </summary>
    public void PageLeft()
    {
      ContentOffsetX -= ContentViewportWidth;
    }

    /// <summary>
    /// Shift the content offset one page right.
    /// </summary>
    public void PageRight()
    {
      ContentOffsetX += ContentViewportWidth;
    }
    #endregion

    #region Mouse wheel handling methods
    /// <summary>
    /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
    /// used for zooming in and out, not for manipulating the scrollbars.
    /// </summary>
    public void MouseWheelDown()
    {
      if (IsMouseWheelScrollingEnabled)
      {
        LineDown();
      }
    }

    /// <summary>
    /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
    /// used for zooming in and out, not for manipulating the scrollbars.
    /// </summary>
    public void MouseWheelLeft()
    {
      if (IsMouseWheelScrollingEnabled)
      {
        LineLeft();
      }
    }

    /// <summary>
    /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
    /// used for zooming in and out, not for manipulating the scrollbars.
    /// </summary>
    public void MouseWheelRight()
    {
      if (IsMouseWheelScrollingEnabled)
      {
        LineRight();
      }
    }

    /// <summary>
    /// Don't handle mouse wheel input from the ScrollViewer, the mouse wheel is
    /// used for zooming in and out, not for manipulating the scrollbars.
    /// </summary>
    public void MouseWheelUp()
    {
      if (IsMouseWheelScrollingEnabled)
      {
        LineUp();
      }
    }
    #endregion

    /// <summary>
    /// Bring the specified rectangle to view.
    /// </summary>
    public Rect MakeVisible(Visual visual, Rect rectangle)
    {
      if (content.IsAncestorOf(visual))
      {
        Rect transformedRect = visual.TransformToAncestor(content).TransformBounds(rectangle);
        if (!transformedRect.IntersectsWith(new Rect(ContentOffsetX, ContentOffsetY, ContentViewportWidth, ContentViewportHeight)))
        {
          AnimatedSnapTo(new Point(transformedRect.X + (transformedRect.Width / 2),
              transformedRect.Y + (transformedRect.Height / 2)));
        }
      }
      return rectangle;
    }

  }
}
