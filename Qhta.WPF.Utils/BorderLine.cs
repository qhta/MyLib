namespace Qhta.WPF.Utils;

/// <summary>
/// A helper class that draws a border line adorner for an UI element.
/// </summary>
public class BorderLine : Adorner
{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="adornedElement"></param>
  public BorderLine(UIElement adornedElement) : base(adornedElement)
  {
  }

  /// <summary>
  /// Defines a side of the element at which the border line is drawn.
  /// </summary>
  public Dock Side
  {
    get => (Dock)GetValue(SideProperty);
    set => SetValue(SideProperty, value);
  }

  /// <summary>
  /// Dependency property to store Side property. Note: it is declared with "DrawAt" name.
  /// </summary>
  public static DependencyProperty SideProperty = DependencyProperty.Register
    ("DrawAt", typeof(Dock), typeof(BorderLine),
    new FrameworkPropertyMetadata(Dock.Top, FrameworkPropertyMetadataOptions.AffectsRender));

  /// <summary>
  /// Render event handling method that draws a line at the declared side of the element.
  /// </summary>
  /// <param name="drawingContext"></param>
  protected override void OnRender(DrawingContext drawingContext)
  {
    if (this.AdornedElement != null)
    {
      Rect targetRect = new Rect(this.AdornedElement.DesiredSize);

      Pen renderPen = new Pen(SystemColors.ControlTextBrush, 1.5);
      switch (Side)
      {
        case Dock.Top:
          drawingContext.DrawLine(renderPen, targetRect.TopLeft, targetRect.TopRight);
          break;
        case Dock.Bottom:
          drawingContext.DrawLine(renderPen, targetRect.BottomLeft, targetRect.BottomRight);
          break;
        case Dock.Left:
          drawingContext.DrawLine(renderPen, targetRect.TopLeft, targetRect.BottomLeft);
          break;
        case Dock.Right:
          drawingContext.DrawLine(renderPen, targetRect.TopRight, targetRect.BottomRight);
          break;
      }
    }
  }
}
