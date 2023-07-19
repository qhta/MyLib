namespace Qhta.Xml;

/// <summary>
/// Defines an attribute which can specify a type converter for a class, property or field.
/// </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property | AttributeTargets.Field)]
public class XmlConverterAttribute : Attribute
{
  /// <summary>
  /// Initializing constructor.
  /// </summary>
  /// <param name="converterType"></param>
  /// <param name="args"></param>
  public XmlConverterAttribute(Type converterType, params object[] args)
  {
    ConverterType = converterType;
    Args = args;
  }

  /// <summary>
  /// A type of the converter.
  /// </summary>
  public Type ConverterType { get; }

  /// <summary>
  /// Arguments to be passed to the type converter.
  /// </summary>
  public object[]? Args { get; }
}