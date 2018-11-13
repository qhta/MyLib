using System;
using System.Diagnostics;
using System.Windows;

namespace Qhta.WPF.ZoomPan
{
  public partial class ZoomPanControl
  {
    // Animation handling logic
    #region AnimationDuration property
    /// <summary>
    /// The duration of the animations (in seconds) started by calling AnimatedZoomTo and the other animation methods.
    /// </summary>
    public double AnimationDuration
    {
      get => (double)GetValue(AnimationDurationProperty);
      set => SetValue(AnimationDurationProperty, value);
    }

    public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register("AnimationDuration", typeof(double), typeof(ZoomPanControl),
                                        new FrameworkPropertyMetadata(0.4));
    #endregion

    /// <summary>
    /// Use animation to center the view on the specified point (in content coordinates).
    /// </summary>
    public void AnimatedSnapTo(Point contentPoint)
    {
      double newX = contentPoint.X - (this.ContentViewportWidth / 2);
      double newY = contentPoint.Y - (this.ContentViewportHeight / 2);

      ZoomPanAnimationHelper.StartAnimation(this, ContentOffsetXProperty, newX, AnimationDuration);
      ZoomPanAnimationHelper.StartAnimation(this, ContentOffsetYProperty, newY, AnimationDuration);
    }

    /// <summary>
    /// Zoom in/out centered on the specified point (in content coordinates).
    /// The focus point is kept locked to it's on screen position (ala google maps).
    /// </summary>
    public void AnimatedZoomAboutPoint(double newContentScale, Point contentZoomFocus)
    {
      newContentScale = Math.Min(Math.Max(newContentScale, MinContentScale), MaxContentScale);

      ZoomPanAnimationHelper.CancelAnimation(this, ContentZoomFocusXProperty);
      ZoomPanAnimationHelper.CancelAnimation(this, ContentZoomFocusYProperty);
      ZoomPanAnimationHelper.CancelAnimation(this, ViewportZoomFocusXProperty);
      ZoomPanAnimationHelper.CancelAnimation(this, ViewportZoomFocusYProperty);

      ContentZoomFocusX = contentZoomFocus.X;
      ContentZoomFocusY = contentZoomFocus.Y;
      ViewportZoomFocusX = (ContentZoomFocusX - ContentOffsetX) * ContentScale;
      ViewportZoomFocusY = (ContentZoomFocusY - ContentOffsetY) * ContentScale;

      //
      // When zooming about a point make updates to ContentScale also update content offset.
      //
      enableContentOffsetUpdateFromScale = true;

      ZoomPanAnimationHelper.StartAnimation(this, ContentScaleProperty, newContentScale, AnimationDuration,
          delegate (object sender, EventArgs e)
          {
            enableContentOffsetUpdateFromScale = false;

            ResetViewportZoomFocus();
          });
    }

    /// <summary>
    /// Zoom to the specified scale and move the specified focus point to the center of the viewport.
    /// </summary>
    private void AnimatedZoomPointToViewportCenter(double newContentScale, Point contentZoomFocus, EventHandler callback)
    {
      newContentScale = Math.Min(Math.Max(newContentScale, MinContentScale), MaxContentScale);

      ZoomPanAnimationHelper.CancelAnimation(this, ContentZoomFocusXProperty);
      ZoomPanAnimationHelper.CancelAnimation(this, ContentZoomFocusYProperty);
      ZoomPanAnimationHelper.CancelAnimation(this, ViewportZoomFocusXProperty);
      ZoomPanAnimationHelper.CancelAnimation(this, ViewportZoomFocusYProperty);

      ContentZoomFocusX = contentZoomFocus.X;
      ContentZoomFocusY = contentZoomFocus.Y;
      ViewportZoomFocusX = (ContentZoomFocusX - ContentOffsetX) * ContentScale;
      ViewportZoomFocusY = (ContentZoomFocusY - ContentOffsetY) * ContentScale;

      //
      // When zooming about a point make updates to ContentScale also update content offset.
      //
      enableContentOffsetUpdateFromScale = true;
      disableOffsetUsageWhenCentered = true;

      ZoomPanAnimationHelper.StartAnimation(this, ContentScaleProperty, newContentScale, AnimationDuration,
          delegate (object sender, EventArgs e)
          {
            enableContentOffsetUpdateFromScale = false;
            disableOffsetUsageWhenCentered = false;
            if (callback != null)
            {
              callback(this, EventArgs.Empty);
            }
          });

      ZoomPanAnimationHelper.StartAnimation(this, ViewportZoomFocusXProperty, ViewportWidth / 2, AnimationDuration);
      ZoomPanAnimationHelper.StartAnimation(this, ViewportZoomFocusYProperty, ViewportHeight / 2, AnimationDuration);
    }


    /// <summary>
    /// Do an animated zoom to view a specific scale and rectangle (in content coordinates).
    /// </summary>
    public void AnimatedZoomTo(double newScale, Rect contentRect)
    {
      AnimatedZoomPointToViewportCenter(newScale, new Point(contentRect.X + (contentRect.Width / 2), contentRect.Y + (contentRect.Height / 2)),
          delegate (object sender, EventArgs e)
          {
            //
            // At the end of the animation, ensure that we are snapped to the specified content offset.
            // Due to zooming in on the content focus point and rounding errors, the content offset may
            // be slightly off what we want at the end of the animation and this bit of code corrects it.
            //
            this.ContentOffsetX = CoerceContentOffsetX(contentRect.X);
            this.ContentOffsetY = CoerceContentOffsetY(contentRect.Y);
          });
    }

    /// <summary>
    /// Do an animated zoom to the specified rectangle (in content coordinates).
    /// </summary>
    public void AnimatedZoomTo(Rect contentRect)
    {
      double scaleX = this.ContentViewportWidth / contentRect.Width;
      double scaleY = this.ContentViewportHeight / contentRect.Height;
      double newScale = this.ContentScale * Math.Min(scaleX, scaleY);

      AnimatedZoomPointToViewportCenter(newScale, new Point(contentRect.X + (contentRect.Width / 2), contentRect.Y + (contentRect.Height / 2)), null);
    }

    /// <summary>
    /// Zoom in/out centered on the viewport center.
    /// </summary>
    public void AnimatedZoomTo(double contentScale)
    {
      Point zoomCenter = new Point(ContentOffsetX + (ContentViewportWidth / 2), ContentOffsetY + (ContentViewportHeight / 2));
      AnimatedZoomAboutPoint(contentScale, zoomCenter);
    }

    /// <summary>
    /// Do animation that scales the content so that it fits completely in the control.
    /// </summary>
    public void AnimatedScaleToFit()
    {
      if (content == null)
      {
        throw new ApplicationException("PART_Content was not found in the ZoomPanControl visual template!");
      }

      AnimatedZoomTo(new Rect(0, 0, content.ActualWidth, content.ActualHeight));
    }
  }
}
