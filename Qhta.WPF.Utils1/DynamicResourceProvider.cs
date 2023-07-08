using System;
using System.Windows.Markup;

namespace Qhta.WPF.Utils
{
  public class DynamicResourceProvider : MarkupExtension
  {
    public DynamicResourceProvider(string typeName)
    {
      TypeName = typeName;
    }
    public string TypeName { get; set; }

    public override object ProvideValue(IServiceProvider serviceProvider)
    {
      if (serviceProvider == null)
      {
        throw new ArgumentNullException("serviceProvider");
      }
      IXamlTypeResolver xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
      if (xamlTypeResolver == null)
      {
        throw new ArgumentException("IXamlTypeResolver");
      }
      Type type = xamlTypeResolver.Resolve(this.TypeName);
      return type.GetConstructor(new Type[] { }).Invoke(new object[] { });
    }

  }
}
