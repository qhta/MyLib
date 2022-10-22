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

    public ConversionOptions? Options { get; set;}

    public static readonly Dictionary<XsdSimpleType, Type[]> XsdSimpleTypeAcceptedTypes = new()
    {
      { XsdSimpleType.Base64Binary, new Type[] { typeof(byte[]) } },
      { XsdSimpleType.HexBinary, new Type[] { typeof(byte[]) } },
      { XsdSimpleType.Boolean, new Type[] { typeof(bool) } },
      { XsdSimpleType.Integer, new Type[] { typeof(Decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) } },
      { XsdSimpleType.NegativeInteger, new Type[] { typeof(Decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) } },
      { XsdSimpleType.NonNegativeInteger, new Type[] { typeof(Decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) } },
      { XsdSimpleType.NonPositiveInteger, new Type[] { typeof(Decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) } },
      { XsdSimpleType.PositiveInteger, new Type[] { typeof(Decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) } },
      { XsdSimpleType.Int, new Type[] { typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), } },
      { XsdSimpleType.Byte, new Type[] { typeof(sbyte), typeof(int), typeof(byte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), } },
      { XsdSimpleType.UnsignedByte, new Type[] { typeof(byte), typeof(sbyte), typeof(int), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), } },
      { XsdSimpleType.UnsignedInt, new Type[] { typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort), typeof(long), typeof(ulong), } },
      { XsdSimpleType.Short, new Type[] { typeof(short), typeof(ushort), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(long), } },
      { XsdSimpleType.UnsignedShort, new Type[] { typeof(ushort), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(long), } },
      { XsdSimpleType.Long, new Type[] { typeof(long), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort), } },
      { XsdSimpleType.UnsignedLong, new Type[] { typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort), typeof(long), } },

      { XsdSimpleType.Decimal, new Type[] { typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), } },
      { XsdSimpleType.Float, new Type[] { typeof(float), typeof(double) } },
      { XsdSimpleType.Double, new Type[] { typeof(double), typeof(float) } },

      { XsdSimpleType.DateTime, new Type[] { typeof(DateTime) } },
      { XsdSimpleType.Date, new Type[] { typeof(DateTime), typeof(DateOnly) } },
      { XsdSimpleType.Time, new Type[] { typeof(DateTime), typeof(TimeOnly) } },

      { XsdSimpleType.AnyUri, new Type[]{ typeof(String) } },
      { XsdSimpleType.Entity, new Type[] { typeof(String) } },
      { XsdSimpleType.Entities, new Type[] { typeof(String[]) } },
      { XsdSimpleType.GDay, new Type[] { typeof(String) } },
      { XsdSimpleType.GMonth, new Type[] { typeof(String) } },
      { XsdSimpleType.GMonthDay, new Type[] { typeof(String) } },
      { XsdSimpleType.GYear, new Type[] { typeof(String) } },
      { XsdSimpleType.GYearMonth, new Type[] { typeof(String) } },
      { XsdSimpleType.Id, new Type[] { typeof(String) } },
      { XsdSimpleType.IdRef, new Type[] { typeof(String) } },
      { XsdSimpleType.IdRefs, new Type[] { typeof(String[]) } },

      { XsdSimpleType.Language, new Type[] { typeof(String) } },

      { XsdSimpleType.Name, new Type[] { typeof(String) } },
      { XsdSimpleType.NcName, new Type[] { typeof(String) } },
      { XsdSimpleType.NmToken, new Type[] { typeof(String) } },
      { XsdSimpleType.NmTokens, new Type[] { typeof(String[]) } },
      { XsdSimpleType.NormalizedString, new Type[] { typeof(String) } },
      { XsdSimpleType.Notation, new Type[] { typeof(String) } },
      { XsdSimpleType.QName, new Type[] { typeof(XmlQualifiedName) } },
      { XsdSimpleType.Duration, new Type[] { typeof(String) } },
      { XsdSimpleType.String, new Type[] { typeof(String) } },

      { XsdSimpleType.Token, new Type[] { typeof(String) } },
    };

    public static readonly Dictionary<Type, TypeConverter> StandardTypeConverters = new Dictionary<Type, TypeConverter>
    {
      { typeof(string), new StringTypeConverter() },
      { typeof(bool), new BooleanTypeConverter() },
      { typeof(int), new NumericTypeConverter{ ExpectedType = typeof(int)} },
      { typeof(byte), new NumericTypeConverter{ ExpectedType = typeof(byte)} },
      { typeof(uint), new NumericTypeConverter{ ExpectedType = typeof(uint)} },
      { typeof(sbyte), new NumericTypeConverter{ ExpectedType = typeof(sbyte)} },
      { typeof(short), new NumericTypeConverter{ ExpectedType = typeof(short)} },
      { typeof(ushort), new NumericTypeConverter{ ExpectedType = typeof(ushort)} },
      { typeof(long), new NumericTypeConverter{ ExpectedType = typeof(long)} },
      { typeof(ulong), new NumericTypeConverter{ ExpectedType = typeof(ulong)} },
      { typeof(float), new NumericTypeConverter{ ExpectedType = typeof(float)} },
      { typeof(double), new NumericTypeConverter{ ExpectedType = typeof(double)} },
      { typeof(decimal), new NumericTypeConverter{ ExpectedType = typeof(decimal)} },
      { typeof(DateTime), new DateTimeTypeConverter{ Mode=DateTimeConversionMode.Default } },
      { typeof(DateOnly), new DateTimeTypeConverter{ Mode=DateTimeConversionMode.DateOnly } },
      { typeof(TimeOnly), new DateTimeTypeConverter{ Mode=DateTimeConversionMode.TimeOnly } },
    };

    private static readonly Dictionary<XsdSimpleType, TypeConverter> SpecialTypeConverters = new()
    {

      { XsdSimpleType.Base64Binary, new ArrayTypeConverter() },
      { XsdSimpleType.HexBinary, new ArrayTypeConverter() },
      { XsdSimpleType.Date, new DateTimeTypeConverter{ Mode=DateTimeConversionMode.DateOnly } },
      { XsdSimpleType.DateTime, new DateTimeTypeConverter{ Mode=DateTimeConversionMode.DateTime} },
      { XsdSimpleType.Time, new DateTimeTypeConverter{ Mode=DateTimeConversionMode.TimeOnly } },
    };

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
      if (XsdType != null)
      {
        if (!XsdSimpleTypeAcceptedTypes.TryGetValue((XsdSimpleType)XsdType, out var allowedTypes))
          throw new InvalidOperationException($"Unrecognized XmlDataType \"{XsdType}\"");
        if (expectedType != null)
        {
          if (!allowedTypes.Contains(expectedType))
            throw new InvalidOperationException($"XmlDataType {XsdType} can not be applied to {expectedType.Name}");
        }
        else
        {
          expectedType = allowedTypes.First();
        }
      }
      ExpectedType = expectedType;

        switch (XsdType)
        {
          case XsdSimpleType.Date:
            InternalTypeConverter = CreateDateTimeTypeConverter(DateTimeConversionMode.DateOnly, format, culture, options);
            return;
          case XsdSimpleType.DateTime:
            InternalTypeConverter = CreateDateTimeTypeConverter(DateTimeConversionMode.DateTime, format, culture, options);
            return;
          case XsdSimpleType.Time:
            InternalTypeConverter = CreateDateTimeTypeConverter(DateTimeConversionMode.TimeOnly, format, culture, options);
            return;
          case XsdSimpleType.Boolean:
            InternalTypeConverter = CreateBooleanTypeConverter(expectedType, format, culture, options);
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
              InternalTypeConverter = CreateBooleanTypeConverter(expectedType, format, culture, options);
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
              InternalTypeConverter = CreateBooleanTypeConverter(expectedType, format, culture, options);
            ExpectedType = expectedType;
            return;
          case XsdSimpleType.Decimal:
            InternalTypeConverter = new NumericTypeConverter { ExpectedType = expectedType, Format = format };
            if (expectedType == typeof(bool))
              InternalTypeConverter = CreateBooleanTypeConverter(expectedType, format, culture, options);
            return;
          case XsdSimpleType.Float:
          case XsdSimpleType.Double:
            InternalTypeConverter = new NumericTypeConverter { ExpectedType = expectedType, Format = format };
            return;
          default:
            InternalTypeConverter = CreateStringTypeConverter(xsdType, format, culture, options);
            break;

      }
      //}
      //if (XsdType != null)
      //{
      //  if (InternalTypeConverter == null && SpecialTypeConverters.TryGetValue((XsdSimpleType)XsdType, out var specialConverter))
      //    InternalTypeConverter = specialConverter;
      //  if (InternalTypeConverter == null && expectedType == null)
      //    throw new InvalidOperationException($"TypeConverter for {XsdType} not found");
      //}
      //if (expectedType != null)
      //{
      //  if (InternalTypeConverter == null && StandardTypeConverters.TryGetValue(expectedType, out var standardConverter))
      //    InternalTypeConverter = standardConverter;
      if (InternalTypeConverter == null)
        throw new InvalidOperationException($"TypeConverter for {expectedType?.Name} not found");
      //}
    }

    private StringTypeConverter CreateStringTypeConverter(XsdSimpleType? xsdType, string? format, CultureInfo? culture,
      ConversionOptions? options)
    {
      var result = new StringTypeConverter { XsdType = xsdType, Format = format };
      return result;
    }

    private DateTimeTypeConverter CreateDateTimeTypeConverter(DateTimeConversionMode mode, string? format, CultureInfo? culture,
      ConversionOptions? options)
    {
      var result = new DateTimeTypeConverter
      {
        Mode = mode,
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

    private TypeConverter CreateBooleanTypeConverter(Type? objectType, string? format, CultureInfo? culture, ConversionOptions? options = null)
    {
      var result = new BooleanTypeConverter();
      if (options?.BooleanStrings != null)
        result.BooleanStrings = options.BooleanStrings;
      return result;
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
      if (InternalTypeConverter == null)
        Init(ExpectedType, XsdType, Format, culture);
      if (InternalTypeConverter != null)
        return InternalTypeConverter.ConvertTo(context, culture, value, destinationType);
      return base.ConvertTo(context, culture, value, destinationType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
      if (InternalTypeConverter == null)
        Init(ExpectedType, XsdType, Format, culture);
      if (InternalTypeConverter != null)
        return InternalTypeConverter.ConvertFrom(context, culture, value);
      return base.ConvertFrom(context, culture, value);
    }
  }
}