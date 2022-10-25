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
      { XsdSimpleType.Integer, new Type[] { typeof(Decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(string) } },
      { XsdSimpleType.NegativeInteger, new Type[] { typeof(Decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(string) } },
      { XsdSimpleType.NonNegativeInteger, new Type[] { typeof(Decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(string) } },
      { XsdSimpleType.NonPositiveInteger, new Type[] { typeof(Decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(string) } },
      { XsdSimpleType.PositiveInteger, new Type[] { typeof(Decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(string) } },
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

      { XsdSimpleType.AnyUri, new Type[]{ typeof(Uri), typeof(string) } },
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
      { XsdSimpleType.QName, new Type[] { typeof(XmlQualifiedName), typeof(string) } },
      { XsdSimpleType.Duration, new Type[] { typeof(TimeSpan), typeof(String) } },
      { XsdSimpleType.String, new Type[] { typeof(String) } },

      { XsdSimpleType.Token, new Type[] { typeof(String) } },
    };

    public static readonly Dictionary<Type, Type> KnownTypeConvertersTypes = new Dictionary<Type, Type>
    {
      { typeof(string), typeof(StringTypeConverter) },
      { typeof(bool), typeof(BooleanTypeConverter) },
      { typeof(int), typeof(NumericTypeConverter) },
      { typeof(byte), typeof(NumericTypeConverter) },
      { typeof(uint), typeof(NumericTypeConverter) },
      { typeof(sbyte), typeof(NumericTypeConverter) },
      { typeof(short), typeof(NumericTypeConverter) },
      { typeof(ushort), typeof(NumericTypeConverter) },
      { typeof(long), typeof(NumericTypeConverter) },
      { typeof(ulong), typeof(NumericTypeConverter) },
      { typeof(float), typeof(NumericTypeConverter) },
      { typeof(double), typeof(NumericTypeConverter) },
      { typeof(decimal), typeof(NumericTypeConverter) },
      { typeof(DateTime), typeof(DateTimeTypeConverter) },
      { typeof(DateOnly), typeof(DateTimeTypeConverter) },
      { typeof(TimeOnly), typeof(DateTimeTypeConverter) },
      { typeof(TimeSpan), typeof(TimeSpanTypeConverter) },
      { typeof(byte[]), typeof(ArrayTypeConverter) },
      { typeof(string[]), typeof(ArrayTypeConverter) },
      { typeof(XmlQualifiedName), typeof(StringTypeConverter) },
      { typeof(Uri), typeof(StringTypeConverter) },
    };

    //public static readonly Dictionary<Type, ITypeConverter> StandardTypeConverters = new Dictionary<Type, ITypeConverter>
    //{
    //  { typeof(string), new StringTypeConverter() },
    //  { typeof(bool), new BooleanTypeConverter() },
    //  { typeof(int), new NumericTypeConverter{ ExpectedType = typeof(int)} },
    //  { typeof(byte), new NumericTypeConverter{ ExpectedType = typeof(byte)} },
    //  { typeof(uint), new NumericTypeConverter{ ExpectedType = typeof(uint)} },
    //  { typeof(sbyte), new NumericTypeConverter{ ExpectedType = typeof(sbyte)} },
    //  { typeof(short), new NumericTypeConverter{ ExpectedType = typeof(short)} },
    //  { typeof(ushort), new NumericTypeConverter{ ExpectedType = typeof(ushort)} },
    //  { typeof(long), new NumericTypeConverter{ ExpectedType = typeof(long)} },
    //  { typeof(ulong), new NumericTypeConverter{ ExpectedType = typeof(ulong)} },
    //  { typeof(float), new NumericTypeConverter{ ExpectedType = typeof(float)} },
    //  { typeof(double), new NumericTypeConverter{ ExpectedType = typeof(double)} },
    //  { typeof(decimal), new NumericTypeConverter{ ExpectedType = typeof(decimal)} },
    //  { typeof(DateTime), new DateTimeTypeConverter{ Mode=DateTimeConversionMode.Default } },
    //  { typeof(DateOnly), new DateTimeTypeConverter{ Mode=DateTimeConversionMode.DateOnly } },
    //  { typeof(TimeOnly), new DateTimeTypeConverter{ Mode=DateTimeConversionMode.TimeOnly } },
    //};

    private static readonly Dictionary<XsdSimpleType, TypeConverter> SpecialTypeConverters = new()
    {

      { XsdSimpleType.Base64Binary, new ArrayTypeConverter() },
      { XsdSimpleType.HexBinary, new ArrayTypeConverter() },
      { XsdSimpleType.Date, new DateTimeTypeConverter{ XsdType=XsdSimpleType.Date } },
      { XsdSimpleType.Time, new DateTimeTypeConverter{ XsdType=XsdSimpleType.Time } },
      { XsdSimpleType.DateTime, new DateTimeTypeConverter{ XsdType=XsdSimpleType.DateTime} },
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
        }
      }
      if (expectedType != null)
      {
        if (KnownTypeConvertersTypes.TryGetValue(expectedType, out var converterType))
          InternalTypeConverter = (TypeConverter?)converterType?.GetConstructor(new Type[0])?.Invoke(new object[0]);
        if (InternalTypeConverter is ITypeConverter iTypeConverter)
        {
          iTypeConverter.ExpectedType = expectedType;
          iTypeConverter.XsdType = XsdType;
          iTypeConverter.Format = format;
        }
      }
      if (InternalTypeConverter == null)
      {
        if (expectedType!=null)
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
      var result = new BooleanTypeConverter{ExpectedType = expectedType, XsdType = xsdType};
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