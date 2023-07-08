using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  public class BorderLine: Adorner
  {
    public BorderLine(UIElement adornedElement): base(adornedElement)
    {
    }

    public static DependencyProperty SideProperty = DependencyProperty.Register
      ("DrawAt", typeof(Dock), typeof(BorderLine), 
      new FrameworkPropertyMetadata(Dock.Top, FrameworkPropertyMetadataOptions.AffectsRender));

    public Dock Side { get => (Dock)GetValue(SideProperty); set => SetValue(SideProperty, value); }

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
}
