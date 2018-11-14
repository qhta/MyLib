// <copyright>
// This control is based on an article by Ashley Davis and copyrighted under CPOL, 
//  available as part of a CodeProject article at
//    http://www.codeproject.com/KB/WPF/zoomandpancontrol.aspx
// <date>This code is based on the article dated: 29 Jun 2010</date>
// </copyright>

using System;

using System.Windows.Controls;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using System.Diagnostics;
using System.Windows.Shapes;

namespace Qhta.WPF.ZoomPan
{
  /// <summary>
  /// A class that wraps up zooming and panning of it's content.
  /// </summary>
  public partial class ZoomPanControl : ContentControl, IScrollInfo
  {
    #region private fields
    /// <summary>
    /// Reference to the underlying content, which is named PART_Content in the template.
    /// </summary>
    private FrameworkElement content;

    /// <summary>
    /// The transform that is applied to the content to scale it by 'ContentScale'.
    /// </summary>
    private ScaleTransform contentScaleTransform;

    /// <summary>
    /// The transform that is applied to the content to offset it by 'ContentOffsetX' and 'ContentOffsetY'.
    /// </summary>
    private TranslateTransform contentOffsetTransform;

    /// <summary>
    /// Enable the update of the content offset as the content scale changes.
    /// This enabled for zooming about a point (Google-maps style zooming) and zooming to a rect.
    /// </summary>
    private bool enableContentOffsetUpdateFromScale;

    /// <summary>
    /// Enable subtraction of ContentOffsetX/Y from center position in UpdateTranslationX/Y
    /// </summary>
    private bool disableOffsetUsageWhenCentered;

    /// <summary>
    /// Used to disable synchronization between IScrollInfo interface and ContentOffsetX/ContentOffsetY.
    /// </summary>
    private bool disableScrollOffsetSync;

    /// <summary>
    /// Normally when content offsets changes the content focus is automatically updated.
    /// This synchronization is disabled when 'disableContentFocusSync' is set to 'true'.
    /// When we are zooming in or out we 'disableContentFocusSync' is set to 'true' because 
    /// we are zooming in or out relative to the content focus we don't want to update the focus.
    /// </summary>
    private bool disableContentFocusSync;

    /// <summary>
    /// The width of the viewport in content coordinates, clamped to the width of the content.
    /// </summary>
    private double constrainedContentViewportWidth;

    /// <summary>
    /// The height of the viewport in content coordinates, clamped to the height of the content.
    /// </summary>
    private double constrainedContentViewportHeight;
    
    /// <summary>
    /// Records the unscaled extent of the content.
    /// This is calculated during the measure and arrange.
    /// </summary>
    private Size unScaledExtent;

    /// <summary>
    /// Records the size of the viewport (in viewport coordinates) onto the content.
    /// This is calculated during the measure and arrange.
    /// </summary>
    private Size viewport;
    #endregion

    #region ContentScale property
    /// <summary>
    /// Get/set the current scale (or zoom factor) of the content.
    /// </summary>
    public double ContentScale
    {
      get => (double)GetValue(ContentScaleProperty);
      set => SetValue(ContentScaleProperty, value);
    }

    /// <summary>
    /// Event raised when the ContentScale property has changed.
    /// </summary>
    public event EventHandler ContentScaleChanged;

    public static readonly DependencyProperty ContentScaleProperty =
            DependencyProperty.Register("ContentScale", typeof(double), typeof(ZoomPanControl),
                                        new FrameworkPropertyMetadata(1.0, ContentScale_PropertyChanged, ContentScale_Coerce));

    private static void ContentScale_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      (o as ZoomPanControl).ContentScale_Changed();
    }

    private void ContentScale_Changed()
    {
      if (contentScaleTransform != null)
      {
        contentScaleTransform.ScaleX = ContentScale;
        contentScaleTransform.ScaleY = ContentScale;
      }

      UpdateContentViewportSize();

      if (enableContentOffsetUpdateFromScale)
      {
        try
        {
          // 
          // Disable content focus synchronization.  We are about to update content offset whilst zooming
          // to ensure that the viewport is focused on our desired content focus point.  Setting this
          // to 'true' stops the automatic update of the content focus when content offset changes.
          //
          disableContentFocusSync = true;

          //
          // Whilst zooming in or out keep the content offset up-to-date so that the viewport is always
          // focused on the content focus point (and also so that the content focus is locked to the 
          // viewport focus point - this is how the Google maps style zooming works).
          //
          double viewportOffsetX = ViewportZoomFocusX - (ViewportWidth / 2);
          double viewportOffsetY = ViewportZoomFocusY - (ViewportHeight / 2);
          double contentOffsetX = viewportOffsetX / ContentScale;
          double contentOffsetY = viewportOffsetY / ContentScale;
          ContentOffsetX = CoerceContentOffsetX(ContentZoomFocusX - (ContentViewportWidth / 2) - contentOffsetX);
          ContentOffsetY = CoerceContentOffsetY(ContentZoomFocusY - (ContentViewportHeight / 2) - contentOffsetY);
        }
        finally
        {
          disableContentFocusSync = false;
        }
      }

      if (ContentScaleChanged != null)
        ContentScaleChanged(this, EventArgs.Empty);

      if (ScrollOwner != null)
        ScrollOwner.InvalidateScrollInfo();
    }

    private static object ContentScale_Coerce(DependencyObject d, object baseValue)
    {
      ZoomPanControl c = (ZoomPanControl)d;
      double value = (double)baseValue;
      value = Math.Min(Math.Max(value, c.MinContentScale), c.MaxContentScale);
      return value;
    }

    #endregion

    #region Min/MaxContentScale properties
    /// <summary>
    /// Get/set the minimum value for 'ContentScale'.
    /// </summary>
    public double MinContentScale
    {
      get => (double)GetValue(MinContentScaleProperty);
      set => SetValue(MinContentScaleProperty, value);
    }

    public static readonly DependencyProperty MinContentScaleProperty =
            DependencyProperty.Register("MinContentScale", typeof(double), typeof(ZoomPanControl),
                                        new FrameworkPropertyMetadata(0.01, MinOrMaxContentScale_PropertyChanged));

    /// <summary>
    /// Get/set the maximum value for 'ContentScale'.
    /// </summary>
    public double MaxContentScale
    {
      get => (double)GetValue(MaxContentScaleProperty);
      set => SetValue(MaxContentScaleProperty, value);
    }

    public static readonly DependencyProperty MaxContentScaleProperty =
            DependencyProperty.Register("MaxContentScale", typeof(double), typeof(ZoomPanControl),
                                        new FrameworkPropertyMetadata(10.0, MinOrMaxContentScale_PropertyChanged));

    /// <summary>
    /// Event raised 'MinContentScale' or 'MaxContentScale' has changed.
    /// </summary>
    private static void MinOrMaxContentScale_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      ZoomPanControl c = (ZoomPanControl)o;
      c.ContentScale = Math.Min(Math.Max(c.ContentScale, c.MinContentScale), c.MaxContentScale);
    }
    #endregion

    #region ContentScaleFactor property
    /// <summary>
    /// Scale (or zoom) factor used to multiply or divide scale in ZoomIn/ZoomOut operations.
    /// The default value is 2.0
    /// </summary>
    public double ContentScaleFactor
    {
      get => (double)GetValue(ContentScaleFactorProperty);
      set => SetValue(ContentScaleFactorProperty, value);
    }
    
    public static readonly DependencyProperty ContentScaleFactorProperty =
            DependencyProperty.Register("ContentScaleFactor", typeof(double), typeof(ZoomPanControl),
                                        new FrameworkPropertyMetadata(2.0));
    
    #endregion


    #region ContentOffsetX property
    /// <summary>
    /// Get/set the X offset (in content coordinates) of the view on the content.
    /// </summary>
    public double ContentOffsetX
    {
      get => (double)GetValue(ContentOffsetXProperty);
      set => SetValue(ContentOffsetXProperty, value);
    }

    /// <summary>
    /// Event raised when the ContentOffsetX property has changed.
    /// </summary>
    public event EventHandler ContentOffsetXChanged;

    public static readonly DependencyProperty ContentOffsetXProperty =
            DependencyProperty.Register("ContentOffsetX", typeof(double), typeof(ZoomPanControl),
               new FrameworkPropertyMetadata(0.0, ContentOffsetX_PropertyChanged/*, ContentOffsetX_Coerce*/));

    private static void ContentOffsetX_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      (o as ZoomPanControl).ContentOffsetX_Changed();
    }

    private void ContentOffsetX_Changed()
    {
      UpdateTranslationX();
      if (!disableContentFocusSync)
        UpdateContentZoomFocusX();
      ContentOffsetXChanged?.Invoke(this, EventArgs.Empty);
      if (!disableScrollOffsetSync && ScrollOwner != null)
        ScrollOwner.InvalidateScrollInfo();
    }

    private static object ContentOffsetX_Coerce(DependencyObject d, object baseValue)
    {
      return (d as ZoomPanControl).CoerceContentOffsetX((double)baseValue);
    }

    private double CoerceContentOffsetX(double value)
    {
      double minOffsetX = 0.0;
      double maxOffsetX = Math.Max(0.0, unScaledExtent.Width - constrainedContentViewportWidth);
      value = Math.Min(Math.Max(value, minOffsetX), maxOffsetX);
      return value;
    }
    #endregion

    #region ContentOffsetY property
    /// <summary>
    /// Y offset (in content coordinates) of the view on the content.
    /// </summary>
    public double ContentOffsetY
    {
      get => (double)GetValue(ContentOffsetYProperty);
      set => SetValue(ContentOffsetYProperty, value);
    }

    /// <summary>
    /// Event raised when the ContentOffsetY property has changed.
    /// </summary>
    public event EventHandler ContentOffsetYChanged;

    public static readonly DependencyProperty ContentOffsetYProperty =
            DependencyProperty.Register("ContentOffsetY", typeof(double), typeof(ZoomPanControl),
              new FrameworkPropertyMetadata(0.0, ContentOffsetY_PropertyChanged/*, ContentOffsetY_Coerce*/));

    private static void ContentOffsetY_PropertyChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
    {
      (o as ZoomPanControl).ContentOffsetY_Changed();
    }

    private void ContentOffsetY_Changed()
    {
      UpdateTranslationY();
      if (!disableContentFocusSync)
        UpdateContentZoomFocusY();
      ContentOffsetYChanged?.Invoke(this, EventArgs.Empty);
      if (!disableScrollOffsetSync && ScrollOwner != null)
        ScrollOwner.InvalidateScrollInfo();
    }

    private static object ContentOffsetY_Coerce(DependencyObject d, object baseValue)
    {
      return (d as ZoomPanControl).CoerceContentOffsetY((double)baseValue);
    }

    private double CoerceContentOffsetY(double value)
    {
      double minOffsetY = 0.0;
      double maxOffsetY = Math.Max(0.0, unScaledExtent.Height - constrainedContentViewportHeight);
      value = Math.Min(Math.Max(value, minOffsetY), maxOffsetY);
      return value;
    }

    #endregion

    #region ConventViewportWidth property
    /// <summary>
    /// Get the viewport width, in content coordinates.
    /// </summary>
    public double ContentViewportWidth
    {
      get => (double)GetValue(ContentViewportWidthProperty);
      set => SetValue(ContentViewportWidthProperty, value);
    }

    public static readonly DependencyProperty ContentViewportWidthProperty =
            DependencyProperty.Register("ContentViewportWidth", typeof(double), typeof(ZoomPanControl),
                                        new FrameworkPropertyMetadata(0.0));
    #endregion

    #region ContentViewportHeight property
    /// <summary>
    /// Get the viewport height, in content coordinates.
    /// </summary>
    public double ContentViewportHeight
    {
      get => (double)GetValue(ContentViewportHeightProperty);
      set => SetValue(ContentViewportHeightProperty, value);
    }

    public static readonly DependencyProperty ContentViewportHeightProperty =
            DependencyProperty.Register("ContentViewportHeight", typeof(double), typeof(ZoomPanControl),
                                        new FrameworkPropertyMetadata(0.0));
    #endregion

    #region ContentZoomFocusX property
    /// <summary>
    /// The X coordinate of the content focus, this is the point that we are focusing on when zooming.
    /// </summary>
    public double ContentZoomFocusX
    {
      get => (double)GetValue(ContentZoomFocusXProperty);
      set => SetValue(ContentZoomFocusXProperty, value);
    }

    public static readonly DependencyProperty ContentZoomFocusXProperty =
            DependencyProperty.Register("ContentZoomFocusX", typeof(double), typeof(ZoomPanControl),
                                        new FrameworkPropertyMetadata(0.0));
    #endregion

    #region ContentZoomFocusY property
    /// <summary>
    /// The Y coordinate of the content focus, this is the point that we are focusing on when zooming.
    /// </summary>
    public double ContentZoomFocusY
    {
      get => (double)GetValue(ContentZoomFocusYProperty);
      set => SetValue(ContentZoomFocusYProperty, value);
    }

    public static readonly DependencyProperty ContentZoomFocusYProperty =
            DependencyProperty.Register("ContentZoomFocusY", typeof(double), typeof(ZoomPanControl),
                                        new FrameworkPropertyMetadata(0.0));
    #endregion

    #region ViewportZoomFocusX property
    /// <summary>
    /// The X coordinate of the viewport focus, this is the point in the viewport (in viewport coordinates) 
    /// that the content focus point is locked to while zooming in.
    /// </summary>
    public double ViewportZoomFocusX
    {
      get => (double)GetValue(ViewportZoomFocusXProperty);
      set => SetValue(ViewportZoomFocusXProperty, value);
    }

    public static readonly DependencyProperty ViewportZoomFocusXProperty =
            DependencyProperty.Register("ViewportZoomFocusX", typeof(double), typeof(ZoomPanControl),
                                        new FrameworkPropertyMetadata(0.0));
    #endregion

    #region ViewportZoomFocusY property
    /// <summary>
    /// The Y coordinate of the viewport focus, this is the point in the viewport (in viewport coordinates) 
    /// that the content focus point is locked to while zooming in.
    /// </summary>
    public double ViewportZoomFocusY
    {
      get => (double)GetValue(ViewportZoomFocusYProperty);
      set => SetValue(ViewportZoomFocusYProperty, value);
    }

    public static readonly DependencyProperty ViewportZoomFocusYProperty =
            DependencyProperty.Register("ViewportZoomFocusY", typeof(double), typeof(ZoomPanControl),
                                        new FrameworkPropertyMetadata(0.0));
    #endregion

    #region ScrollOwnerProperty
    /// <summary>
    /// Reference to the ScrollViewer that is wrapped (in XAML) around the ZoomPanControl.
    /// Or set to null if there is no ScrollViewer.
    /// </summary>
    public ScrollViewer ScrollOwner { get; set; }
    //{
    //  get => (ScrollViewer)GetValue(ScrollOwnerProperty);
    //  set => SetValue(ScrollOwnerProperty, value);
    //}

    //public static readonly DependencyProperty ScrollOwnerProperty =
    //        DependencyProperty.Register("ScrollOwner", typeof(ScrollViewer), typeof(ZoomPanControl),
    //                                    new FrameworkPropertyMetadata(null));
    #endregion

    public ZoomPanControl()
    {
      unScaledExtent = new Size(0, 0);
      viewport = new Size(0, 0);
    }

    /// <summary>
    /// Static constructor to define metadata for the control (and link it to the style in Generic.xaml).
    /// </summary>
    static ZoomPanControl()
    {
      DefaultStyleKeyProperty.OverrideMetadata(typeof(ZoomPanControl),
          new FrameworkPropertyMetadata(typeof(ZoomPanControl)));
    }

    /// <summary>
    /// Instantly zoom to the specified rectangle (in content coordinates).
    /// </summary>
    public void ZoomTo(Rect contentRect)
    {
      double scaleX = this.ContentViewportWidth / contentRect.Width;
      double scaleY = this.ContentViewportHeight / contentRect.Height;
      double newScale = this.ContentScale * Math.Min(scaleX, scaleY);

      ZoomPointToViewportCenter(newScale, new Point(contentRect.X + (contentRect.Width / 2), contentRect.Y + (contentRect.Height / 2)));
    }

    public void ZoomReset()
    {
      ContentScale = 1;
      ContentOffsetX = 0;
      ContentOffsetY = 0;
    }

    public void ZoomIn()
    {
      this.ContentScale *= ContentScaleFactor;
    }

    /// <summary>
    public void ZoomOut()
    {
      this.ContentScale /= ContentScaleFactor;
    }

    public void ZoomFit()
    {
      var fitControl = this.Content as FrameworkElement;
      if (fitControl!=null)
      {
        var width = fitControl.Width;
        var height = fitControl.Height;
        Rect bounds = new Rect(0, 0, width, height);
        this.AnimatedZoomTo(bounds);
      }
    }
    /// <summary>
    /// Instantly center the view on the specified point (in content coordinates).
    /// </summary>
    public void SnapTo(Point contentPoint)
    {
      ZoomPanAnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
      ZoomPanAnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

      this.ContentOffsetX = contentPoint.X - (this.ContentViewportWidth / 2);
      this.ContentOffsetY = contentPoint.Y - (this.ContentViewportHeight / 2);
    }

    /// <summary>
    /// Zoom in/out centered on the specified point (in content coordinates).
    /// The focus point is kept locked to it's on screen position (ala google maps).
    /// </summary>
    public void ZoomAboutPoint(double newContentScale, Point contentZoomFocus)
    {
      newContentScale = Math.Min(Math.Max(newContentScale, MinContentScale), MaxContentScale);

      double screenSpaceZoomOffsetX = (contentZoomFocus.X - ContentOffsetX) * ContentScale;
      double screenSpaceZoomOffsetY = (contentZoomFocus.Y - ContentOffsetY) * ContentScale;
      double contentSpaceZoomOffsetX = screenSpaceZoomOffsetX / newContentScale;
      double contentSpaceZoomOffsetY = screenSpaceZoomOffsetY / newContentScale;
      double newContentOffsetX = contentZoomFocus.X - contentSpaceZoomOffsetX;
      double newContentOffsetY = contentZoomFocus.Y - contentSpaceZoomOffsetY;

      ZoomPanAnimationHelper.CancelAnimation(this, ContentScaleProperty);
      ZoomPanAnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
      ZoomPanAnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

      disableOffsetUsageWhenCentered = true;
      this.ContentScale = newContentScale;
      this.ContentOffsetX = newContentOffsetX;
      this.ContentOffsetY = newContentOffsetY;
      disableOffsetUsageWhenCentered = false;
    }

    /// <summary>
    /// Zoom in/out centered on the viewport center.
    /// </summary>
    public void ZoomTo(double contentScale)
    {
      Point zoomCenter = new Point(ContentOffsetX + (ContentViewportWidth / 2), ContentOffsetY + (ContentViewportHeight / 2));
      ZoomAboutPoint(contentScale, zoomCenter);
    }

    /// <summary>
    /// Instantly scale the content so that it fits completely in the control.
    /// </summary>
    public void ScaleToFit()
    {
      if (content == null)
      {
        throw new ApplicationException("PART_Content was not found in the ZoomPanControl visual template!");
      }
      ZoomTo(new Rect(0, 0, content.ActualWidth, content.ActualHeight));
    }

    /// <summary>
    /// Called when a template has been applied to the control.
    /// </summary>
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();


      content = this.Template.FindName("PART_Content", this) as FrameworkElement;
      overlayCanvas = this.Template.FindName("OverlayCanvas", this) as Canvas;
      selectingShape = content.TryFindResource("SelectingShape") as Shape;
      if (content != null)
      {
        //
        // Setup the transform on the content so that we can scale it by 'ContentScale'.
        //
        this.contentScaleTransform = new ScaleTransform(this.ContentScale, this.ContentScale);

        //
        // Setup the transform on the content so that we can translate it by 'ContentOffsetX' and 'ContentOffsetY'.
        //
        this.contentOffsetTransform = new TranslateTransform();
        UpdateTranslationX();
        UpdateTranslationY();

        //
        // Setup a transform group to contain the translation and scale transforms, and then
        // assign this to the content's 'RenderTransform'.
        //
        TransformGroup transformGroup = new TransformGroup();
        transformGroup.Children.Add(this.contentOffsetTransform);
        transformGroup.Children.Add(this.contentScaleTransform);
        content.RenderTransform = transformGroup;
      }
    }

    /// <summary>
    /// Measure the control and it's children.
    /// </summary>
    protected override Size MeasureOverride(Size constraint)
    {
      Size infiniteSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
      Size childSize = base.MeasureOverride(infiniteSize);

      if (childSize != unScaledExtent)
      {
        //
        // Use the size of the child as the un-scaled extent content.
        //
        unScaledExtent = childSize;

        if (ScrollOwner != null)
        {
          ScrollOwner.InvalidateScrollInfo();
        }
      }

      //
      // Update the size of the viewport onto the content based on the passed in 'constraint'.
      //
      UpdateViewportSize(constraint);

      double width = constraint.Width;
      double height = constraint.Height;

      if (double.IsInfinity(width))
      {
        //
        // Make sure we don't return infinity!
        //
        width = childSize.Width;
      }

      if (double.IsInfinity(height))
      {
        //
        // Make sure we don't return infinity!
        //
        height = childSize.Height;
      }

      return new Size(width, height);
    }

    /// <summary>
    /// Arrange the control and it's children.
    /// </summary>
    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      Size size = base.ArrangeOverride(this.DesiredSize);
      if (content!=null && content.DesiredSize != unScaledExtent)
      {
        //
        // Use the size of the child as the un-scaled extent content.
        //
        unScaledExtent = content.DesiredSize;

        if (ScrollOwner != null)
        {
          ScrollOwner.InvalidateScrollInfo();
        }
      }

      //
      // Update the size of the viewport onto the content based on the passed in 'arrangeBounds'.
      //
      UpdateViewportSize(arrangeBounds);

      return size;
    }

    /// <summary>
    /// Zoom to the specified scale and move the specified focus point to the center of the viewport.
    /// </summary>
    private void ZoomPointToViewportCenter(double newContentScale, Point contentZoomFocus)
    {
      newContentScale = Math.Min(Math.Max(newContentScale, MinContentScale), MaxContentScale);

      ZoomPanAnimationHelper.CancelAnimation(this, ContentScaleProperty);
      ZoomPanAnimationHelper.CancelAnimation(this, ContentOffsetXProperty);
      ZoomPanAnimationHelper.CancelAnimation(this, ContentOffsetYProperty);

      this.ContentScale = newContentScale;
      this.ContentOffsetX = contentZoomFocus.X - (ContentViewportWidth / 2);
      this.ContentOffsetY = contentZoomFocus.Y - (ContentViewportHeight / 2);
    }

    /// <summary>
    /// Reset the viewport zoom focus to the center of the viewport.
    /// </summary>
    private void ResetViewportZoomFocus()
    {
      ViewportZoomFocusX = ViewportWidth / 2;
      ViewportZoomFocusY = ViewportHeight / 2;
    }

    /// <summary>
    /// Update the viewport size from the specified size.
    /// </summary>
    private void UpdateViewportSize(Size newSize)
    {
      if (viewport == newSize)
      {
        //
        // The viewport is already the specified size.
        //
        return;
      }

      viewport = newSize;

      //
      // Update the viewport size in content coordiates.
      //
      UpdateContentViewportSize();

      //
      // Initialise the content zoom focus point.
      //
      UpdateContentZoomFocusX();
      UpdateContentZoomFocusY();

      //
      // Reset the viewport zoom focus to the center of the viewport.
      //
      ResetViewportZoomFocus();

      //
      // Update content offset from itself when the size of the viewport changes.
      // This ensures that the content offset remains properly clamped to its valid range.
      //
      this.ContentOffsetX = this.ContentOffsetX;
      this.ContentOffsetY = this.ContentOffsetY;

      if (ScrollOwner != null)
      {
        //
        // Tell that owning ScrollViewer that scrollbar data has changed.
        //
        ScrollOwner.InvalidateScrollInfo();
      }
    }

    /// <summary>
    /// Update the size of the viewport in content coordinates after the viewport size or 'ContentScale' has changed.
    /// </summary>
    private void UpdateContentViewportSize()
    {
      ContentViewportWidth  = ViewportWidth / ContentScale;
      ContentViewportHeight = ViewportHeight / ContentScale;

      constrainedContentViewportWidth  = Math.Min(ContentViewportWidth, unScaledExtent.Width);
      constrainedContentViewportHeight = Math.Min(ContentViewportHeight, unScaledExtent.Height);

      UpdateTranslationX();
      UpdateTranslationY();
    }

    /// <summary>
    /// Update the X coordinate of the translation transformation.
    /// </summary>
    private void UpdateTranslationX()
    {
      if (this.contentOffsetTransform != null)
      {
        double scaledContentWidth = this.unScaledExtent.Width * this.ContentScale;
        if (scaledContentWidth < this.ViewportWidth)
        {
          //
          // When the content can fit entirely within the viewport, try to center it.
          //
          var x = (this.ContentViewportWidth - this.unScaledExtent.Width) / 2;
          if (!disableOffsetUsageWhenCentered)
            x -= this.ContentOffsetX;
          this.contentOffsetTransform.X = x;
        }
        else
        {
          this.contentOffsetTransform.X = -this.ContentOffsetX;
        }
      }
    }

    /// <summary>
    /// Update the Y coordinate of the translation transformation.
    /// </summary>
    private void UpdateTranslationY()
    {
      if (this.contentOffsetTransform != null)
      {
        double scaledContentHeight = this.unScaledExtent.Height * this.ContentScale;
        if (scaledContentHeight < this.ViewportHeight)
        {
          //
          // When the content can fit entirely within the viewport, try to center it.
          //
          var y = (this.ContentViewportHeight - this.unScaledExtent.Height) / 2;
          if (!disableOffsetUsageWhenCentered)
            y -= this.ContentOffsetY;
          this.contentOffsetTransform.Y = y;
        }
        else
        {
          this.contentOffsetTransform.Y = -this.ContentOffsetY;
        }
      }
    }

    /// <summary>
    /// Update the X coordinate of the zoom focus point in content coordinates.
    /// </summary>
    private void UpdateContentZoomFocusX()
    {
      ContentZoomFocusX = ContentOffsetX + (constrainedContentViewportWidth / 2);
    }

    /// <summary>
    /// Update the Y coordinate of the zoom focus point in content coordinates.
    /// </summary>
    private void UpdateContentZoomFocusY()
    {
      ContentZoomFocusY = ContentOffsetY + (constrainedContentViewportHeight / 2);
    }

  }
}
