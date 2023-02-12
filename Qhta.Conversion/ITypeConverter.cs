using System.Globalization;
using Qhta.Xml;

namespace Qhta.Conversion;

public interface ITypeConverter
{
  public Type? ExpectedType { get; set; }

  public Type[]? KnownTypes { get; set; }

  public XsdSimpleType? XsdType { get; set; }
  public string? Format { get; set; }
  public CultureInfo? Culture { get; set; }
}