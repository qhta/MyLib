using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

using Qhta.TypeUtils;

namespace Qhta.Conversion
{
  public class ValueTypeConverter : TypeConverter, ITypeConverter
  {
    public TypeConverter? InternalTypeConverter { get; set; }

    public string? Format { get; set; }

    public Type? ExpectedType { get; set; }

    public XsdSimpleType? XsdType { get; set; }

    public CultureInfo? Culture { get; set; }

    public ConversionOptions? Options { get; set; }

    public static readonly Dictionary<XsdSimpleType, Type[]> XsdSimpleTypeAcceptedTypes = new()
    {
      { XsdSimpleType.AnyUri, new Type[]{ typeof(Uri), typeof(string) } },
      { XsdSimpleType.Base64Binary, new Type[] { typeof(byte[]) } },
      { XsdSimpleType.Boolean, new Type[] { typeof(bool) } },
      { XsdSimpleType.Byte, new Type[] { typeof(sbyte), typeof(int), typeof(byte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), } },
      { XsdSimpleType.Date, new Type[] { typeof(DateTime), typeof(DateTimeOffset), typeof(DateOnly) } },
      { XsdSimpleType.DateTime, new Type[] { typeof(DateTime), typeof(DateTimeOffset) } },
      { XsdSimpleType.Decimal, new Type[] { typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), } },
      { XsdSimpleType.Double, new Type[] { typeof(double), typeof(float) } },
      { XsdSimpleType.Duration, new Type[] { typeof(TimeSpan), typeof(string) } },
      { XsdSimpleType.Entities, new Type[] { typeof(string[]) } },
      { XsdSimpleType.Entity, new Type[] { typeof(string) } },
      { XsdSimpleType.Float, new Type[] { typeof(float), typeof(double) } },
      { XsdSimpleType.GDay, new Type[] { typeof(GDate) } },
      { XsdSimpleType.GMonth, new Type[] { typeof(GDate) } },
      { XsdSimpleType.GMonthDay, new Type[] { typeof(GDate) } },
      { XsdSimpleType.GYear, new Type[] { typeof(GDate) } },
      { XsdSimpleType.GYearMonth, new Type[] { typeof(GDate) } },
      { XsdSimpleType.HexBinary, new Type[] { typeof(byte[]) } },
      { XsdSimpleType.Id, new Type[] { typeof(string) } },
      { XsdSimpleType.IdRef, new Type[] { typeof(string) } },
      { XsdSimpleType.IdRefs, new Type[] { typeof(string[]) } },
      { XsdSimpleType.Int, new Type[] { typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), } },
      { XsdSimpleType.Integer, new Type[] { typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(string) } },
      { XsdSimpleType.Language, new Type[] { typeof(string) } },
      { XsdSimpleType.Long, new Type[] { typeof(long), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort), } },
      { XsdSimpleType.Name, new Type[] { typeof(string) } },
      { XsdSimpleType.NcName, new Type[] { typeof(string) } },
      { XsdSimpleType.NegativeInteger, new Type[] { typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(string) } },
      { XsdSimpleType.NmToken, new Type[] { typeof(string) } },
      { XsdSimpleType.NmTokens, new Type[] { typeof(string[]) } },
      { XsdSimpleType.NonNegativeInteger, new Type[] { typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(string) } },
      { XsdSimpleType.NonPositiveInteger, new Type[] { typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(string) } },
      { XsdSimpleType.NormalizedString, new Type[] { typeof(string) } },
      { XsdSimpleType.Notation, new Type[] { typeof(string) } },
      { XsdSimpleType.PositiveInteger, new Type[] { typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(string) } },
      { XsdSimpleType.QName, new Type[] { typeof(XmlQualifiedName), typeof(string) } },
      { XsdSimpleType.Short, new Type[] { typeof(short), typeof(ushort), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(long), } },
      { XsdSimpleType.String, new Type[] { typeof(string) } },
      { XsdSimpleType.Time, new Type[] { typeof(DateTime), typeof(DateTimeOffset), typeof(TimeOnly) } },
      { XsdSimpleType.Token, new Type[] { typeof(string) } },
      { XsdSimpleType.UnsignedByte, new Type[] { typeof(byte), typeof(sbyte), typeof(int), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), } },
      { XsdSimpleType.UnsignedInt, new Type[] { typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort), typeof(long), typeof(ulong), } },
      { XsdSimpleType.UnsignedLong, new Type[] { typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort), typeof(long), } },
      { XsdSimpleType.UnsignedShort, new Type[] { typeof(ushort), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(long), } },
    };

    public static readonly Dictionary<Type, TypeConverter> KnownTypeConverters = new Dictionary<Type, TypeConverter>
    {
      { typeof(Array), new ArrayTypeConverter() },
      { typeof(bool), new BooleanTypeConverter() },
      { typeof(byte), new NumericTypeConverter { XsdType = XsdSimpleType.UnsignedByte } },
      { typeof(byte[]), new ArrayTypeConverter{ XsdType = XsdSimpleType.Base64Binary} },
      { typeof(DateOnly), new DateTimeTypeConverter{ XsdType = XsdSimpleType.Date } },
      { typeof(DateTime), new DateTimeTypeConverter{ XsdType = XsdSimpleType.DateTime } },
      { typeof(DateTimeOffset), new DateTimeTypeConverter{ XsdType = XsdSimpleType.DateTime } },
      { typeof(decimal), new NumericTypeConverter{ XsdType = XsdSimpleType.Decimal } },
      { typeof(double), new NumericTypeConverter{ XsdType = XsdSimpleType.Double } },
      { typeof(float), new NumericTypeConverter{ XsdType = XsdSimpleType.Float } },
      { typeof(GDate), new GDateTypeConverter() },
      { typeof(Guid), new GuidConverter() },
      { typeof(int), new NumericTypeConverter{ XsdType = XsdSimpleType.Int } },
      { typeof(long), new NumericTypeConverter{ XsdType = XsdSimpleType.Long } },
      { typeof(sbyte), new NumericTypeConverter{ XsdType = XsdSimpleType.Byte } },
      { typeof(short), new NumericTypeConverter{ XsdType = XsdSimpleType.Short } },
      { typeof(string), new StringTypeConverter() },
      { typeof(string[]), new ArrayTypeConverter() },
      { typeof(TimeOnly), new DateTimeTypeConverter{ XsdType = XsdSimpleType.Time } },
      { typeof(TimeSpan), new TimeSpanTypeConverter() },
      { typeof(uint), new NumericTypeConverter{ XsdType = XsdSimpleType.UnsignedInt } },
      { typeof(ulong), new NumericTypeConverter{ XsdType = XsdSimpleType.UnsignedLong } },
      { typeof(Uri), new UriTypeConverter() },
      { typeof(ushort), new NumericTypeConverter{ XsdType = XsdSimpleType.UnsignedShort } },
      { typeof(XmlQualifiedName), new XmlQualifiedNameTypeConverter() },
    };

    public ValueTypeConverter()
    {
    }

    public ValueTypeConverter(Type? expectedType, XsdSimpleType? xsdType, string? format, CultureInfo? culture = null,
      ConversionOptions? options = null)
    {
      Init(expectedType, xsdType, format, culture, options);
    }

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
            if (expectedType == typeof(bool))
            {
              InternalTypeConverter = CreateBooleanTypeConverter(ExpectedType, XsdType, format, culture, options);
            }
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
        if (KnownTypeConverters.TryGetValue(expectedType, out var converter))
        {
          var converterType = converter.GetType();
          InternalTypeConverter = (TypeConverter?)converterType.GetConstructor(new Type[0])?.Invoke(new object[0]);
          if (converter is ITypeConverter iTypeConverter0 && iTypeConverter0.XsdType != null && xsdType==null)
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
        if (expectedType != null)
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
        Format = format,
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

    private TypeConverter CreateBooleanTypeConverter(Type? expectedType, XsdSimpleType? xsdType, string? format, CultureInfo? culture, ConversionOptions? options = null)
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
}