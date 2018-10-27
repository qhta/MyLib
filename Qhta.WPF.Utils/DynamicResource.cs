using System;
using System.Windows;
using System.Windows.Markup;

namespace Qhta.WPF.Utils
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
