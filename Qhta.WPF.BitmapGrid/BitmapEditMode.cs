using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Qhta.WPF.Controls
{
  public enum BitmapEditMode
  {
    Select,
    GetPoint,
    SetPoint,
    DrawLine,
    DrawRect,
    DrawEllipse,
    FloodFill,
    FillAll,
    MagicWand,

    Started = 128
  }
}
