namespace Qhta.Xml;

/// <summary>
/// Specifies that the class instance must be serialized as full object
/// - not as a simple collection
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Class)]
public class XmlObjectAttribute : Attribute
{
}
