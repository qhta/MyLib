namespace Qhta.Xml.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class XmlDataFormatAttribute: Attribute
{
  public string? Format { get; set; }

  public CultureInfo? Culture { get; set;}
}