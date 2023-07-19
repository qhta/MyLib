namespace Qhta.Xml;

/// <summary>
/// Defines an attribute which specifies than a property (or class) represents collection of references.
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Class)]
public class XmlReferencesAttribute : Attribute
{
}