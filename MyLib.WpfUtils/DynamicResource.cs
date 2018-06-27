using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Markup;

namespace MyLib.WpfUtils
{
  public class DynamicResource : MarkupExtension
  {
    public DynamicResource(string resourceName)
    {
      ResourceName = resourceName;
    }
    public string ResourceName { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      try
      {
        object result = Application.Current.Resources[ResourceName];
        return result;
      }
      catch
      {

      }
      return null;
    }

  }
}
