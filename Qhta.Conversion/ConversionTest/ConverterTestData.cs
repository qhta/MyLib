using Qhta.Conversion;

namespace ConversionTest;

public class ConverterTestData
{
  public object Value {get; set;} = null!;
  public string Text { get; set; } = null!;
  public Type? ExpectedType { get; set; } = null!;
  public XsdSimpleType? XsdType { get; set; } = null!;

}