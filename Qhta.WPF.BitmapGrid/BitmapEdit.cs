using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Qhta.WPF.Controls
{
  public class BitmapEdit: Grid
  {
    BitmapRaster Raster= new BitmapRaster();
    protected override void OnInitialized(EventArgs e)
    {
      Children.Add(Raster);
    }
  }
}
