namespace Qhta.WPF.Utils;

/// <summary>
/// Markup extension class that provides a type instance.
/// </summary>
public class DynamicResourceProvider : MarkupExtension
{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="typeName"></param>
  public DynamicResourceProvider(string typeName)
  {
    TypeName = typeName;
  }

  /// <summary>
  /// Name of the type.
  /// </summary>
  public string TypeName { get; set; }

  /// <summary>
  /// Provides value using IXamlTypeResolver interface.
  /// </summary>
  /// <param name="serviceProvider"></param>
  /// <returns></returns>
  /// <exception cref="ArgumentNullException"></exception>
  /// <exception cref="ArgumentException"></exception>
  public override object? ProvideValue(IServiceProvider serviceProvider)
  {
    if (serviceProvider == null)
    {
      throw new ArgumentNullException("serviceProvider");
    }
    IXamlTypeResolver? xamlTypeResolver = serviceProvider.GetService(typeof(IXamlTypeResolver)) as IXamlTypeResolver;
    if (xamlTypeResolver == null)
    {
      throw new ArgumentException("IXamlTypeResolver");
    }
    Type type = xamlTypeResolver.Resolve(this.TypeName);
    return type.GetConstructor(new Type[] { })?.Invoke(new object[] { });
  }

}
