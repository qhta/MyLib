using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.EntityFrameworkCore.Update.Internal;
using Qhta.UnicodeBuild.Helpers;
using Qhta.UnicodeBuild.ViewModels;

using Syncfusion.UI.Xaml.Grid;

namespace Qhta.UnicodeBuild.Controls;

/// <summary>
/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
///
/// Step 1a) Using this custom control in a XAML file that exists in the current project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:Qhta.UnicodeBuild.Controls"
///
///
/// Step 1b) Using this custom control in a XAML file that exists in a different project.
/// Add this XmlNamespace attribute to the root element of the markup file where it is 
/// to be used:
///
///     xmlns:MyNamespace="clr-namespace:Qhta.UnicodeBuild.Controls;assembly=Qhta.UnicodeBuild.Controls"
///
/// You will also need to add a project reference from the project where the XAML file lives
/// to this project and Rebuild to avoid compilation errors:
///
///     Right click on the target project in the Solution Explorer and
///     "Add Reference"->"Projects"->[Browse to and select this project]
///
///
/// Step 2)
/// Go ahead and use your control in the XAML file.
///
///     <MyNamespace:RowResizer/>
///
/// </summary>
public class RowResizer : Thumb
{

  #region Constructors

  static RowResizer()
  {

    EventManager.RegisterClassHandler(typeof(RowResizer), Thumb.DragStartedEvent, new DragStartedEventHandler(RowResizer.OnDragStarted));
    EventManager.RegisterClassHandler(typeof(RowResizer), Thumb.DragDeltaEvent, new DragDeltaEventHandler(RowResizer.OnDragDelta));
    EventManager.RegisterClassHandler(typeof(RowResizer), Thumb.DragCompletedEvent, new DragCompletedEventHandler(RowResizer.OnDragCompleted));
    DefaultStyleKeyProperty.OverrideMetadata(typeof(RowResizer), new FrameworkPropertyMetadata(typeof(RowResizer)));



    FocusableProperty.OverrideMetadata(typeof(RowResizer), new FrameworkPropertyMetadata(false));
    FrameworkElement.VerticalAlignmentProperty.OverrideMetadata(typeof(RowResizer), new FrameworkPropertyMetadata(VerticalAlignment.Bottom));

    // Cursor depends on ResizeDirection, ActualWidth, and ActualHeight 
    CursorProperty.OverrideMetadata(typeof(RowResizer), new FrameworkPropertyMetadata(Cursors.SizeNS));
  }

  /// <summary>
  /// Instantiates a new instance of a RowResizer.
  /// </summary>
  public RowResizer()
  {
  }

  #endregion

  #region Properties

  private static void UpdateCursor(DependencyObject o, DependencyPropertyChangedEventArgs e)
  {
    o.CoerceValue(CursorProperty);
  }

  /// <summary>
  ///     The DependencyProperty for the ResizeBehavior property.
  ///     Default Value:      GridResizeBehavior.BasedOnAlignment
  /// </summary>
  public static readonly DependencyProperty ResizeBehaviorProperty
      = DependencyProperty.Register("ResizeBehavior",
                                      typeof(GridResizeBehavior),
                                      typeof(RowResizer),
                                      new FrameworkPropertyMetadata(GridResizeBehavior.BasedOnAlignment),
                                      new ValidateValueCallback(IsValidResizeBehavior));

  /// <summary>
  /// Indicates which Columns or Rows the Resizer resizes.
  /// </summary>
  public GridResizeBehavior ResizeBehavior
  {
    get { return (GridResizeBehavior)GetValue(ResizeBehaviorProperty); }

    set { SetValue(ResizeBehaviorProperty, value); }
  }

  private static bool IsValidResizeBehavior(object o)
  {
    GridResizeBehavior resizeBehavior = (GridResizeBehavior)o;
    return resizeBehavior == GridResizeBehavior.BasedOnAlignment ||
           resizeBehavior == GridResizeBehavior.CurrentAndNext ||
           resizeBehavior == GridResizeBehavior.PreviousAndCurrent ||
           resizeBehavior == GridResizeBehavior.PreviousAndNext;
  }


  /// <summary>
  ///     The DependencyProperty for the ShowsPreview property.
  ///     Default Value:      false
  /// </summary>
  public static readonly DependencyProperty ShowsPreviewProperty
      = DependencyProperty.Register("ShowsPreview",
                                    typeof(bool),
                                    typeof(RowResizer),
                                    new FrameworkPropertyMetadata(false));

  /// <summary>
  /// Indicates whether to Preview the column resizing without updating layout.
  /// </summary>
  public bool ShowsPreview
  {
    get { return (bool)GetValue(ShowsPreviewProperty); }
    set { SetValue(ShowsPreviewProperty, value); }
  }

  /// <summary>
  ///     The DependencyProperty for the PreviewStyle property.
  ///     Default Value:      null
  /// </summary>
  public static readonly DependencyProperty PreviewStyleProperty =
              DependencyProperty.Register(
                          nameof(PreviewStyle),
                          typeof(Style),
                          typeof(RowResizer),
                          new FrameworkPropertyMetadata(null));

  /// <summary>
  ///     The Style used to render the Preview.
  /// </summary>
  public Style PreviewStyle
  {
    get => (Style)GetValue(PreviewStyleProperty);
    set => SetValue(PreviewStyleProperty, value);
  }


  /// <summary>
  ///     The DependencyProperty for the KeyboardIncrement property.
  ///     Default Value:      10.0
  /// </summary>
  public static readonly DependencyProperty KeyboardIncrementProperty =
              DependencyProperty.Register(
                          nameof(KeyboardIncrement),
                          typeof(double),
                          typeof(RowResizer),
                          new FrameworkPropertyMetadata(10.0),
                          new ValidateValueCallback(IsValidDelta));

  /// <summary>
  ///     The Distance to move the resizer when pressing the Keyboard arrow keys
  /// </summary>
  public double KeyboardIncrement
  {
    get => (double)GetValue(KeyboardIncrementProperty);
    set => SetValue(KeyboardIncrementProperty, value);
  }


  private static bool IsValidDelta(object o)
  {
    double delta = (double)o;
    return delta > 0.0 && !Double.IsPositiveInfinity(delta);
  }


  /// <summary>
  ///     The DependencyProperty for the DragIncrement property.
  ///     Default Value:      1.0
  /// </summary>
  public static readonly DependencyProperty DragIncrementProperty =
              DependencyProperty.Register(
                          "DragIncrement",
                          typeof(double),
                          typeof(RowResizer),
                          new FrameworkPropertyMetadata(1.0),
                          new ValidateValueCallback(IsValidDelta));

  /// <summary>
  ///     Restricts resizer to move a multiple of the specified units.
  /// </summary>
  public double DragIncrement
  {
    get { return (double)GetValue(DragIncrementProperty); }
    set { SetValue(DragIncrementProperty, value); }
  }

  #endregion


  #region Method Overrides

  ///// <summary>
  ///// Creates AutomationPeer (<see cref="UIElement.OnCreateAutomationPeer"/>)
  ///// </summary>
  //protected override AutomationPeer OnCreateAutomationPeer()
  //{
  //  return new RowResizerAutomationPeer(this);
  //}

  // Convert BasedOnAlignment to Next/Prev/Both depending on alignment and Direction
  private GridResizeBehavior GetEffectiveResizeBehavior(GridResizeDirection direction)
  {
    GridResizeBehavior resizeBehavior = ResizeBehavior;

    if (resizeBehavior == GridResizeBehavior.BasedOnAlignment)
    {
      if (direction == GridResizeDirection.Columns)
      {
        switch (HorizontalAlignment)
        {
          case HorizontalAlignment.Left:
            resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
            break;
          case HorizontalAlignment.Right:
            resizeBehavior = GridResizeBehavior.CurrentAndNext;
            break;
          default:
            resizeBehavior = GridResizeBehavior.PreviousAndNext;
            break;
        }
      }
      else
      {
        switch (VerticalAlignment)
        {
          case VerticalAlignment.Top:
            resizeBehavior = GridResizeBehavior.PreviousAndCurrent;
            break;
          case VerticalAlignment.Bottom:
            resizeBehavior = GridResizeBehavior.CurrentAndNext;
            break;
          default:
            resizeBehavior = GridResizeBehavior.PreviousAndNext;
            break;
        }
      }
    }
    return resizeBehavior;
  }

  /// <summary>
  /// Override for <seealso cref="UIElement.OnRenderSizeChanged"/>
  /// </summary>
  protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
  {
    base.OnRenderSizeChanged(sizeInfo);

    CoerceValue(CursorProperty);
  }

  #endregion

  #region PreviewAdorner

  // This adorner draws the preview for the RowResizer
  // It also positions the adorner
  // Note:- This class is sealed because it calls OnVisualChildrenChanged virtual in the 
  //              constructor, but it does not override it, but derived classes could.        
  private sealed class PreviewAdorner : Adorner
  {
    public PreviewAdorner(RowResizer RowResizer, Style previewStyle)
        : base(RowResizer)
    {
      // Create a preview control to overlay on top of the RowResizer
      Control previewControl = new Control();
      previewControl.Style = previewStyle;
      previewControl.IsEnabled = false;

      // Add a decorator to perform translations
      Translation = new TranslateTransform();
      _decorator = new Decorator();
      _decorator.Child = previewControl;
      _decorator.RenderTransform = Translation;

      this.AddVisualChild(_decorator);
    }

    /// <summary>
    ///   Derived class must implement to support Visual children. The method must return
    ///    the child at the specified index. Index must be between 0 and GetVisualChildrenCount-1.
    ///
    ///    By default, a Visual does not have any children.
    ///
    ///  Remark: 
    ///       During this virtual call it is not valid to modify the Visual tree. 
    /// </summary>
    protected override Visual GetVisualChild(int index)
    {
      // it is initialized in the constructor
      Debug.Assert(_decorator != null);
      if (index != 0)
      {
        throw new ArgumentOutOfRangeException("index", index, "Argument {0} out of range");
      }

      return _decorator;
    }

    /// <summary>
    ///  Derived classes override this property to enable the Visual code to enumerate 
    ///  the Visual children. Derived classes need to return the number of children
    ///  from this method.
    ///
    ///    By default, a Visual does not have any children.
    ///
    ///  Remark: During this virtual method the Visual tree must not be modified.
    /// </summary>        
    protected override int VisualChildrenCount
    {
      get
      {
        // it is initialized in the constructor
        Debug.Assert(_decorator != null);
        return 1;
      }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
      _decorator.Arrange(new Rect(new Point(), finalSize));
      return finalSize;
    }

    // The Preview's Offset in the X direction from the RowResizer
    public double OffsetX
    {
      get => Translation.X;
      set => Translation.X = value;
    }

    // The Preview's Offset in the Y direction from the RowResizer
    public double OffsetY
    {
      get => Translation.Y;
      set => Translation.Y = value;
    }

    private readonly TranslateTransform Translation;
    private readonly Decorator _decorator;
  }

  // Removes the Preview Adorner
  private void RemovePreviewAdorner()
  {
    if (_resizeData == null)
      return;
    // Remove the preview grid from the adorner
    if (_resizeData.Adorner != null)
    {
      AdornerLayer? layer = VisualTreeHelper.GetParent(_resizeData.Adorner) as AdornerLayer;
      layer?.Remove(_resizeData.Adorner);
    }
  }

  #endregion

  #region Resizer Setup

  // Initialize the data needed for resizing
  private void InitializeData(bool showsPreview)
  {
    var grid = this.FindParent<SfDataGrid>();
    var dataContext = this.DataContext;

    var cell = this.FindParent<GridRowHeaderCell>();
    var rowIndex = cell?.RowIndex-1 ?? 0;

    // If not in a grid or can't resize, do nothing
    if (grid != null)
    {
      // Setup data used for resizing
      _resizeData = new ResizeData();
      _resizeData.Grid = grid;
      _resizeData.ShowsPreview = showsPreview;
      _resizeData.RowIndex = rowIndex;
      _resizeData.ResizerLength = Math.Min(ActualWidth, ActualHeight);

      // Store the rows and columns to resize on drag events
      if (!SetupDefinitionsToResize())
      {
        // Unable to resize, clear data
        _resizeData = null;
        return;
      }

      // Set up the preview in the adorner if ShowsPreview is true
      SetupPreview();
    }
  }

  // Returns true if RowResizer can resize rows/columns
  private bool SetupDefinitionsToResize()
  {
    if (_resizeData == null)
      return false;

    var rowIndex = _resizeData.RowIndex;

    // Get # of rows/columns in the resize direction
    int count = _resizeData.Grid.View.Records.Count;

    if (rowIndex >= 0 && rowIndex < count)
    {
      _resizeData.RowIndex = rowIndex;
      _resizeData.OriginalRowHeight = GetRowHeight(_resizeData.Grid, rowIndex);
      return true;
    }
    return false;
  }

  // Create the Preview adorner and add it to the adorner layer
  private void SetupPreview()
  {
    if (_resizeData == null)
      return;
    if (_resizeData.ShowsPreview)
    {
      // Get the adorner layer and add an adorner to it
      AdornerLayer? adornerLayer = AdornerLayer.GetAdornerLayer(_resizeData.Grid);

      // Can't display preview
      if (adornerLayer == null)
      {
        return;
      }

      _resizeData.Adorner = new PreviewAdorner(this, PreviewStyle);
      adornerLayer.Add(_resizeData.Adorner);

      // Get constraints on preview's translation
      GetDeltaConstraints(out _resizeData.MinChange, out _resizeData.MaxChange);
    }
  }

  #endregion

  #region Event Handlers

  private static void OnDragStarted(object sender, DragStartedEventArgs e)
  {
    RowResizer? resizer = sender as RowResizer;
    resizer?.OnDragStarted(e);
  }

  // Thumb Mouse Down
  private void OnDragStarted(DragStartedEventArgs e)
  {
    //Debug.WriteLine($"OnDragStarted({_resizeData != null})");
    InitializeData(ShowsPreview);
  }

  private static void OnDragDelta(object sender, DragDeltaEventArgs e)
  {
    RowResizer? resizer = sender as RowResizer;
    resizer?.OnDragDelta(e);
  }

  // Thumb dragged
  private void OnDragDelta(DragDeltaEventArgs e)
  {
    //Debug.WriteLine($"OnDragDelta({_resizeData != null})");
    if (_resizeData != null)
    {
      double horizontalChange = e.HorizontalChange;
      double verticalChange = e.VerticalChange;

      // Round change to nearest multiple of DragIncrement
      double dragIncrement = DragIncrement;
      horizontalChange = Math.Round(horizontalChange / dragIncrement) * dragIncrement;
      verticalChange = Math.Round(verticalChange / dragIncrement) * dragIncrement;

      if (_resizeData.ShowsPreview && _resizeData.Adorner!=null)
      {
          _resizeData.Adorner.OffsetY = Math.Min(Math.Max(verticalChange, _resizeData.MinChange), _resizeData.MaxChange);
      }
      else
      {
        // Directly update the grid
        MoveResizer(horizontalChange, verticalChange);
      }
    }
  }

  private static void OnDragCompleted(object sender, DragCompletedEventArgs e)
  {
    var resizer = sender as RowResizer;
    resizer?.OnDragCompleted(e);
  }

  // Thumb dragging finished
  private void OnDragCompleted(DragCompletedEventArgs e)
  {
    //Debug.WriteLine($"OnDragCompleted({_resizeData != null})");
    if (_resizeData != null)
    {
      if (_resizeData.ShowsPreview && _resizeData.Adorner != null)
      {
        // Update the grid
        MoveResizer(0, _resizeData.Adorner.OffsetY);
        RemovePreviewAdorner();
      }

      _resizeData = null;
    }
  }

  #endregion

  #region Helper Methods

  #region Row Height get/set methods

  // Gets the grid row height (if defined) or the default row height.
  private static double GetRowHeight(SfDataGrid grid, int index)
  {
    var height = grid.RowHeight;
    if (grid.View.Records[index].Data is IRowHeightProvider data)
    {
      if (!double.IsNaN(data.RowHeight))
        height = data.RowHeight;
    }
    return height;
  }

  // Set the grid row height
  private static void SetRowHeight(SfDataGrid grid, int index, double height)
  {
    if (grid.View.Records[index].Data is IRowHeightProvider data)
    {
      data.RowHeight = height;
      grid.View.Refresh();
    }
  }

  #endregion

  // Get the minimum and maximum Delta can be given definition constraints (MinWidth/MaxWidth)
  private void GetDeltaConstraints(out double minDelta, out double maxDelta)
  {
    var grid = _resizeData?.Grid;
    if (_resizeData == null || grid == null)
    {
      minDelta = 0;
      maxDelta = 0;
      return;
    }
    double definition1Len = GetRowHeight(grid, _resizeData.RowIndex);
    double definition1Min = 24;//_resizeData.Definition1.UserMinSizeValueCache;
    double definition1Max = 120;//_resizeData.Definition1.UserMaxSizeValueCache;

    minDelta = definition1Min - definition1Len;
    maxDelta = definition1Max - definition1Len;
  }

  // Move the resizer by the given Delta's in the horizontal and vertical directions
  private void MoveResizer(double horizontalChange, double verticalChange)
  {
    if (_resizeData == null)
      return;
    var grid = _resizeData.Grid;
    var index = _resizeData.RowIndex;

    Debug.Assert(_resizeData != null, "_resizeData should not be null when calling MoveResizer");

    double delta;
    DpiScale dpi = VisualTreeHelper.GetDpi(this);

    // Calculate the offset to adjust the resizer.  If layout rounding is enabled, we
    // need to round to an integer physical pixel value to avoid round-ups of children that
    // expand the bounds of the Grid. In practice this only happens in high dpi because
    // horizontal/vertical offsets here are never fractional (they correspond to mouse movement
    // across logical pixels).  Rounding error only creeps in when converting to a physical
    // display with something other than the logical 96 dpi.
    delta = verticalChange;
    if (this.UseLayoutRounding) delta = RoundLayoutValue(delta, dpi.DpiScaleY);

    double actualRowHeight = GetRowHeight(grid, index);

    GetDeltaConstraints(out var min, out var max);

    // Flip when the resizer's flow direction isn't the same as the grid's
    if (FlowDirection != _resizeData.Grid.FlowDirection)
      delta = -delta;

    // Constrain Delta to Min/MaxWidth of columns
    delta = Math.Min(Math.Max(delta, min), max);

    double newHeight = actualRowHeight + delta;


    SetRowHeight(grid, index, newHeight);
  }

  #endregion

  #region Data


  // Only store resize data if we are resizing
  private class ResizeData
  {
    public bool ShowsPreview;
    public PreviewAdorner? Adorner;

    // The constraints to keep the Preview within valid ranges
    public double MinChange;
    public double MaxChange;

    // The grid to Resize
    public SfDataGrid Grid = null!;

    // The index of the resizer
    public int RowIndex;

    public double OriginalRowHeight;

    // The minimum of Height of Resizer.  Used to ensure resizer 
    //isn't hidden by resizing a row smaller than the resizer
    public double ResizerLength;
  }

  // Data used for resizing
  private ResizeData? _resizeData;

  #endregion

  /// <summary>
  /// Rounds a size to integer values for layout purposes, compensating for high DPI screen coordinates.
  /// </summary>
  /// <param name="size">Input size.</param>
  /// <param name="dpiScaleX">DPI along x-dimension.</param>
  /// <param name="dpiScaleY">DPI along y-dimension.</param>
  /// <returns>Value of size that will be rounded under screen DPI.</returns>
  /// <remarks>This is a layout helper method. It takes DPI into account and also does not return
  /// the rounded value if it is unacceptable for layout, e.g. Infinity or NaN. It's a helper associated with
  /// UseLayoutRounding  property and should not be used as a general rounding utility.</remarks>
  internal static Size RoundLayoutSize(Size size, double dpiScaleX, double dpiScaleY)
  {
    return new Size(RoundLayoutValue(size.Width, dpiScaleX), RoundLayoutValue(size.Height, dpiScaleY));
  }

  /// <summary>
  /// Calculates the value to be used for layout rounding at high DPI.
  /// </summary>
  /// <param name="value">Input value to be rounded.</param>
  /// <param name="dpiScale">Ratio of screen's DPI to layout DPI</param>
  /// <returns>Adjusted value that will produce layout rounding on screen at high dpi.</returns>
  /// <remarks>This is a layout helper method. It takes DPI into account and also does not return
  /// the rounded value if it is unacceptable for layout, e.g. Infinity or NaN. It's a helper associated with
  /// UseLayoutRounding  property and should not be used as a general rounding utility.</remarks>
  internal static double RoundLayoutValue(double value, double dpiScale)
  {
    double newValue;

    // If DPI == 1, don't use DPI-aware rounding.
    if (!DoubleUtil.AreClose(dpiScale, 1.0))
    {
      newValue = Math.Round(value * dpiScale) / dpiScale;
      // If rounding produces a value unacceptable to layout (NaN, Infinity or MaxValue), use the original value.
      if (double.IsNaN(newValue) ||
          Double.IsInfinity(newValue) ||
          DoubleUtil.AreClose(newValue, Double.MaxValue))
      {
        newValue = value;
      }
    }
    else
    {
      newValue = Math.Round(value);
    }

    return newValue;
  }
}