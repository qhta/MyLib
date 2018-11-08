using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.WPF.IconDefinition
{
  public class Rectangle : Shape
  {
    protected override Qhta.Drawing.Shape DrawingShape { get; set; } = new Qhta.Drawing.Rectangle();

  }
}
