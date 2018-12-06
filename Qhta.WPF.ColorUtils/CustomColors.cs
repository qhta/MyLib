using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Qhta.WPF
{
  public class CustomColors: List<Color>
  {
    /// <summary>
    /// Protects List Capacity property from saving
    /// </summary>
    public new int Capacity { get; set; }
  }
}
