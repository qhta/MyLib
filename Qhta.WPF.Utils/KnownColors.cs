using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace Qhta.WPF.Utils
{
  public class KnownColors
  {
    List<KnownColor> _SelectableColors = null;

    public List<KnownColor> SelectableColors
    {
      get { return _SelectableColors; }
      set { _SelectableColors = value; }
    }

    public KnownColors()
    {
      _SelectableColors = new List<KnownColor>();
      Type ColorsType = typeof(Colors);
      PropertyInfo[] ColorsProperty = ColorsType.GetProperties();

      foreach (PropertyInfo property in ColorsProperty)
      {
        _SelectableColors.Add(new KnownColor
        {
          Name = property.Name,
          Color = (Color)ColorConverter.ConvertFromString(property.Name)
        });
      }
    }

  }

}
