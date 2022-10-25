using Qhta.Conversion;

namespace ConversionTest;

public class ConverterTestData
{
  public object Value {get; set;} = null!;
  public XsdSimpleType XsdType { get; set; }
  public string Text { get; set; } = null!;

}