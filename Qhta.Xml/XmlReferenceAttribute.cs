namespace Qhta.Xml;

/// <summary>
/// Defines an attribute which specifies than a property (or field) represents a reference.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class XmlReferenceAttribute : Attribute
{
}