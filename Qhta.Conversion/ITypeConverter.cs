namespace Qhta.Conversion;

public interface ITypeConverter
{
  public string? Format { get; set;}
  public Type? ExpectedType { get; set;}
  public XsdSimpleType? XsdType { get; set; }
}