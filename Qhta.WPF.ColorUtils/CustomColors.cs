using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Qhta.WPF
{
  /// <summary>
  /// Inherited class defined only for DataTemplates
  /// </summary>
  public class CustomColor: KnownColor
  {

  }

  public class CustomColors: ObservableCollection<CustomColor>
  {
  }
}
