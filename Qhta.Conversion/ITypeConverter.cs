﻿using System.Globalization;

namespace Qhta.Conversion;

public interface ITypeConverter
{
  public Type? ExpectedType { get; set;}
  public XsdSimpleType? XsdType { get; set; }
  public string? Format { get; set; }
  public CultureInfo? Culture { get; set; }
}