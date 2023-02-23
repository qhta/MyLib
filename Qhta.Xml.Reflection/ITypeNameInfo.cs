namespace Qhta.Xml.Reflection;

/// <summary>
/// Extension for INamedElement that provides a Type
/// </summary>
/// <seealso cref="Qhta.Xml.Reflection.INamedElement" />
public interface ITypeNameInfo : INamedElement
{
  /// <summary>
  /// Gets the original type.
  /// </summary>
  Type Type { get; }
}