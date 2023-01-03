namespace Qhta.Xml.Serialization;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
public class XmlDataFormatAttribute : Attribute
{
  public string? Format { get; set; }

  public CultureInfo? Culture { get; set; }
}