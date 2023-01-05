using System.ComponentModel;
using System.Globalization;
using System.Xml;
using Qhta.TypeUtils;

namespace Qhta.Conversion;

public class ValueTypeConverter : TypeConverter, ITypeConverter
{
  public static readonly Dictionary<XsdSimpleType, Type[]> XsdSimpleTypeAcceptedTypes = new()
  {
    { XsdSimpleType.AnyUri, new[] { typeof(Uri), typeof(string) } },
    { XsdSimpleType.Base64Binary, new[] { typeof(byte[]) } },
    { XsdSimpleType.Boolean, new[] { typeof(bool) } },
    {
      XsdSimpleType.Byte, new[] { typeof(sbyte), typeof(int), typeof(byte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) }
    },
    { XsdSimpleType.Date, new[] { typeof(DateTime), typeof(DateTimeOffset), typeof(DateOnly) } },
    { XsdSimpleType.DateTime, new[] { typeof(DateTime), typeof(DateTimeOffset) } },
    {
      XsdSimpleType.Decimal,
      new[] { typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) }
    },
    { XsdSimpleType.Double, new[] { typeof(double), typeof(float) } },
    { XsdSimpleType.Duration, new[] { typeof(TimeSpan), typeof(string) } },
    { XsdSimpleType.Entities, new[] { typeof(string[]) } },
    { XsdSimpleType.Entity, new[] { typeof(string) } },
    { XsdSimpleType.Float, new[] { typeof(float), typeof(double) } },
    { XsdSimpleType.GDay, new[] { typeof(GDate) } },
    { XsdSimpleType.GMonth, new[] { typeof(GDate) } },
    { XsdSimpleType.GMonthDay, new[] { typeof(GDate) } },
    { XsdSimpleType.GYear, new[] { typeof(GDate) } },
    { XsdSimpleType.GYearMonth, new[] { typeof(GDate) } },
    { XsdSimpleType.HexBinary, new[] { typeof(byte[]) } },
    { XsdSimpleType.Id, new[] { typeof(string) } },
    { XsdSimpleType.IdRef, new[] { typeof(string) } },
    { XsdSimpleType.IdRefs, new[] { typeof(string[]) } },
    {
      XsdSimpleType.Int, new[] { typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) }
    },
    {
      XsdSimpleType.Integer,
      new[]
      {
        typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong),
        typeof(string)
      }
    },
    { XsdSimpleType.Language, new[] { typeof(string) } },
    {
      XsdSimpleType.Long, new[] { typeof(long), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort) }
    },
    { XsdSimpleType.Name, new[] { typeof(string) } },
    { XsdSimpleType.NcName, new[] { typeof(string) } },
    {
      XsdSimpleType.NegativeInteger,
      new[]
      {
        typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong),
        typeof(string)
      }
    },
    { XsdSimpleType.NmToken, new[] { typeof(string) } },
    { XsdSimpleType.NmTokens, new[] { typeof(string[]) } },
    {
      XsdSimpleType.NonNegativeInteger,
      new[]
      {
        typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong),
        typeof(string)
      }
    },
    {
      XsdSimpleType.NonPositiveInteger,
      new[]
      {
        typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong),
        typeof(string)
      }
    },
    { XsdSimpleType.NormalizedString, new[] { typeof(string) } },
    { XsdSimpleType.Notation, new[] { typeof(string) } },
    {
      XsdSimpleType.PositiveInteger,
      new[]
      {
        typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong),
        typeof(string)
      }
    },
    { XsdSimpleType.QName, new[] { typeof(XmlQualifiedName), typeof(string) } },
    {
      XsdSimpleType.Short,
      new[] { typeof(short), typeof(ushort), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(long) }
    },
    { XsdSimpleType.String, new[] { typeof(string) } },
    { XsdSimpleType.Time, new[] { typeof(DateTime), typeof(DateTimeOffset), typeof(TimeOnly) } },
    { XsdSimpleType.Token, new[] { typeof(string) } },
    {
      XsdSimpleType.UnsignedByte,
      new[] { typeof(byte), typeof(sbyte), typeof(int), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) }
    },
    {
      XsdSimpleType.UnsignedInt,
      new[] { typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort), typeof(long), typeof(ulong) }
    },
    {
      XsdSimpleType.UnsignedLong,
      new[] { typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort), typeof(long) }
    },
    {
      XsdSimpleType.UnsignedShort,
      new[] { typeof(ushort), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(long) }
    }
  };

  public static readonly Dictionary<Type, TypeConverter> KnownTypeConverters = new()
  {
    //{ typeof(Object), new ObjectTypeConverter() },
    { typeof(Array), new ArrayTypeConverter() },
    { typeof(bool), new BooleanTypeConverter() },
    { typeof(byte), new NumericTypeConverter { XsdType = XsdSimpleType.UnsignedByte } },
    { typeof(byte[]), new ArrayTypeConverter { XsdType = XsdSimpleType.Base64Binary } },
    { typeof(DateOnly), new DateTimeTypeConverter { XsdType = XsdSimpleType.Date } },
    { typeof(DateTime), new DateTimeTypeConverter { XsdType = XsdSimpleType.DateTime } },
    { typeof(DateTimeOffset), new DateTimeTypeConverter { XsdType = XsdSimpleType.DateTime } },
    { typeof(decimal), new NumericTypeConverter { XsdType = XsdSimpleType.Decimal } },
    { typeof(double), new NumericTypeConverter { XsdType = XsdSimpleType.Double } },
    { typeof(float), new NumericTypeConverter { XsdType = XsdSimpleType.Float } },
    { typeof(GDate), new GDateTypeConverter() },
    { typeof(Guid), new GuidConverter() },
    { typeof(int), new NumericTypeConverter { XsdType = XsdSimpleType.Int } },
    { typeof(long), new NumericTypeConverter { XsdType = XsdSimpleType.Long } },
    { typeof(sbyte), new NumericTypeConverter { XsdType = XsdSimpleType.Byte } },
    { typeof(short), new NumericTypeConverter { XsdType = XsdSimpleType.Short } },
    { typeof(char), new StringTypeConverter() },
    { typeof(string), new StringTypeConverter() },
    { typeof(string[]), new ArrayTypeConverter() },
    { typeof(TimeOnly), new DateTimeTypeConverter { XsdType = XsdSimpleType.Time } },
    { typeof(TimeSpan), new TimeSpanTypeConverter() },
    { typeof(uint), new NumericTypeConverter { XsdType = XsdSimpleType.UnsignedInt } },
    { typeof(ulong), new NumericTypeConverter { XsdType = XsdSimpleType.UnsignedLong } },
    { typeof(Uri), new UriTypeConverter() },
    { typeof(ushort), new NumericTypeConverter { XsdType = XsdSimpleType.UnsignedShort } },
    { typeof(XmlQualifiedName), new XmlQualifiedNameTypeConverter() }
  };

  public ValueTypeConverter()
  {
  }

  public ValueTypeConverter(Type? expectedType, XsdSimpleType? xsdType, string? format, CultureInfo? culture = null,
    ConversionOptions? options = null)
  {
    Init(expectedType, xsdType, format, culture, options);
  }

  public TypeConverter? InternalTypeConverter { get; set; }

  public ConversionOptions? Options { get; set; }

  public string? Format { get; set; }

  public Type? ExpectedType { get; set; }

  public XsdSimpleType? XsdType { get; set; }

  public CultureInfo? Culture { get; set; }

  public void Init()
  {
    Init(ExpectedType, XsdType, Format, Culture, Options);
  }

  public void Init(Type? expectedType, XsdSimpleType? xsdType, string? format, CultureInfo? culture = null, ConversionOptions? options = null)
  {
    if (expectedType?.IsNullable(out var baseType) == true)
      expectedType = baseType;
    ExpectedType = expectedType;

    XsdType = xsdType;
    Format = format;
    if (XsdType != null && XsdType != 0)
    {
      if (!XsdSimpleTypeAcceptedTypes.TryGetValue((XsdSimpleType)XsdType, out var allowedTypes))
        throw new InvalidOperationException($"Unrecognized XmlDataType \"{XsdType}\"");
      if (expectedType == null)
        expectedType = allowedTypes.First();
      ExpectedType = expectedType;
      switch (XsdType)
      {
        case XsdSimpleType.DateTime:
        case XsdSimpleType.Date:
        case XsdSimpleType.Time:
          InternalTypeConverter = CreateDateTimeTypeConverter((XsdSimpleType)XsdType, format, culture, options);
          return;
        case XsdSimpleType.Boolean:
          InternalTypeConverter = CreateBooleanTypeConverter(ExpectedType, XsdType, format, culture, options);
          if (expectedType != typeof(bool))
            InternalTypeConverter = new NumericTypeConverter { ExpectedType = expectedType, Format = format };
          return;
        case XsdSimpleType.Integer:
        case XsdSimpleType.NegativeInteger:
        case XsdSimpleType.NonNegativeInteger:
        case XsdSimpleType.NonPositiveInteger:
        case XsdSimpleType.PositiveInteger:
          InternalTypeConverter = new NumericTypeConverter { ExpectedType = expectedType };
          if (expectedType == typeof(bool)) InternalTypeConverter = CreateBooleanTypeConverter(ExpectedType, XsdType, format, culture, options);
          return;

        case XsdSimpleType.Int:
        case XsdSimpleType.Byte:
        case XsdSimpleType.UnsignedInt:
        case XsdSimpleType.UnsignedByte:
        case XsdSimpleType.Short:
        case XsdSimpleType.UnsignedShort:
        case XsdSimpleType.Long:
        case XsdSimpleType.UnsignedLong:
          InternalTypeConverter = new NumericTypeConverter { ExpectedType = expectedType, Format = format };
          if (expectedType == typeof(bool))
            InternalTypeConverter = CreateBooleanTypeConverter(expectedType, XsdType, format, culture, options);
          ExpectedType = expectedType;
          return;
        case XsdSimpleType.Decimal:
          InternalTypeConverter = new NumericTypeConverter { ExpectedType = expectedType, Format = format };
          if (expectedType == typeof(bool))
            InternalTypeConverter = CreateBooleanTypeConverter(expectedType, XsdType, format, culture, options);
          return;
        case XsdSimpleType.Float:
        case XsdSimpleType.Double:
          InternalTypeConverter = new NumericTypeConverter { ExpectedType = expectedType, Format = format };
          return;
        case XsdSimpleType.GYear:
        case XsdSimpleType.GYearMonth:
        case XsdSimpleType.GMonth:
        case XsdSimpleType.GMonthDay:
        case XsdSimpleType.GDay:
          InternalTypeConverter = new GDateTypeConverter { XsdType = xsdType };
          return;
      }
    }
    if (expectedType != null)
    {
      if (expectedType.IsEnum)
      {
        InternalTypeConverter = new EnumConverter(expectedType);
      }
      else
      if (KnownTypeConverters.TryGetValue(expectedType, out var converter))
      {
        var converterType = converter.GetType();
        InternalTypeConverter = (TypeConverter?)converterType.GetConstructor(new Type[0])?.Invoke(new object[0]);
        if (converter is ITypeConverter iTypeConverter0 && iTypeConverter0.XsdType != null && xsdType == null)
          xsdType = iTypeConverter0.XsdType;
        if (InternalTypeConverter is ITypeConverter iTypeConverter)
        {
          iTypeConverter.ExpectedType = expectedType;
          iTypeConverter.XsdType = xsdType;
          iTypeConverter.Format = format;
        }
      }
    }

    if (InternalTypeConverter == null)
    {
      if (expectedType != null && expectedType != typeof(object))
        throw new InvalidOperationException($"TypeConverter for {expectedType?.Name} type not found");
      if (xsdType != null)
        throw new InvalidOperationException($"TypeConverter for xsdType={xsdType} not found");
      InternalTypeConverter = new StringTypeConverter();
    }
  }

  private StringTypeConverter CreateStringTypeConverter(XsdSimpleType? xsdType, string? format, CultureInfo? culture,
    ConversionOptions? options)
  {
    var result = new StringTypeConverter { XsdType = xsdType, Format = format };
    return result;
  }

  private DateTimeTypeConverter CreateDateTimeTypeConverter(XsdSimpleType xsdType, string? format, CultureInfo? culture,
    ConversionOptions? options)
  {
    var result = new DateTimeTypeConverter
    {
      XsdType = xsdType,
      Format = format
      //Culture = culture,
    };
    if (options != null)
    {
      result.DateTimeSeparator = options.DateTimeSeparator;
      result.ShowFullTime = options.ShowFullTime;
      result.ShowTimeZone = options.ShowTimeZone;
    }
    return result;
  }

  private TypeConverter CreateBooleanTypeConverter(Type? expectedType, XsdSimpleType? xsdType, string? format, CultureInfo? culture,
    ConversionOptions? options = null)
  {
    var result = new BooleanTypeConverter { ExpectedType = expectedType, XsdType = xsdType };
    if (options?.BooleanStrings != null)
      result.BooleanStrings = options.BooleanStrings;
    return result;
  }

  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? Culture, object? value, Type destinationType)
  {
    if (InternalTypeConverter == null)
      Init(ExpectedType, XsdType, Format, Culture);
    if (InternalTypeConverter != null)
      return InternalTypeConverter.ConvertTo(context, Culture, value, destinationType);
    return base.ConvertTo(context, Culture, value, destinationType);
  }

  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (InternalTypeConverter == null)
      Init(ExpectedType, XsdType, Format, Culture);
    if (InternalTypeConverter != null)
      return InternalTypeConverter.ConvertFrom(context, Culture, value);
    return base.ConvertFrom(context, Culture, value);
  }
}