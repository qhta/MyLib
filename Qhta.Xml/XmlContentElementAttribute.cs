namespace Qhta.Xml.Serialization;

/// <summary>
///   Defines an attribute which can specify that a property or field is serialized without preceding xml tag.
///   (as a content of parent object element).
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class XmlContentElementAttribute : Attribute
{

}