namespace Qhta.Conversion;

/// <summary>
/// This class combines all other converters.
 /// When creating a ValueTypeConverter class converter, the expected .NET data type must be provided, 
 /// and an XSD simple type may be provided.
/// </summary>
public class ValueTypeConverter : BaseTypeConverter
{

  /// <summary>
  /// Type expected in ConvertFrom method.
  /// Overriden as its change must force internal converter reinitialization;
  /// </summary>
  public override Type? ExpectedType
  {
    get => _ExpectedType;
    set
    {
      if (_ExpectedType != value)
      {
        _ExpectedType = value;
        InternalTypeConverter = null;
      }
    }
  }
  private Type? _ExpectedType;

  /// <summary>
  /// Declares XsdSimpleType to Type conversion.
  /// </summary>
  public static readonly Dictionary<SimpleType, Type[]> XsdSimpleTypeAcceptedTypes = new()
  {
    { Xml.SimpleType.AnyUri, new[] { typeof(Uri), typeof(string) } },
    { Xml.SimpleType.Base64Binary, new[] { typeof(byte[]) } },
    { Xml.SimpleType.Boolean, new[] { typeof(bool) } },
    {
      Xml.SimpleType.Byte, new[] { typeof(sbyte), typeof(int), typeof(byte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) }
    },
#if NET6_0_OR_GREATER
    { Xml.SimpleType.Date, new[] { typeof(DateTime), typeof(DateTimeOffset), typeof(DateOnly) } },
#endif
    { Xml.SimpleType.DateTime, new[] { typeof(DateTime), typeof(DateTimeOffset) } },
    {
      Xml.SimpleType.Decimal,
      new[] { typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) }
    },
    { Xml.SimpleType.Double, new[] { typeof(double), typeof(float) } },
    { Xml.SimpleType.Duration, new[] { typeof(TimeSpan), typeof(string) } },
    { Xml.SimpleType.Entities, new[] { typeof(string[]) } },
    { Xml.SimpleType.Entity, new[] { typeof(string) } },
    { Xml.SimpleType.Float, new[] { typeof(float), typeof(double) } },
    { Xml.SimpleType.GDay, new[] { typeof(GDate) } },
    { Xml.SimpleType.GMonth, new[] { typeof(GDate) } },
    { Xml.SimpleType.GMonthDay, new[] { typeof(GDate) } },
    { Xml.SimpleType.GYear, new[] { typeof(GDate) } },
    { Xml.SimpleType.GYearMonth, new[] { typeof(GDate) } },
    { Xml.SimpleType.HexBinary, new[] { typeof(byte[]) } },
    { Xml.SimpleType.Id, new[] { typeof(string) } },
    { Xml.SimpleType.IdRef, new[] { typeof(string) } },
    { Xml.SimpleType.IdRefs, new[] { typeof(string[]) } },
    {
      Xml.SimpleType.Int, new[] { typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) }
    },
    {
      Xml.SimpleType.Integer,
      new[]
      {
        typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong),
        typeof(string)
      }
    },
    { Xml.SimpleType.Language, new[] { typeof(string) } },
    {
      Xml.SimpleType.Long, new[] { typeof(long), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort) }
    },
    { Xml.SimpleType.Name, new[] { typeof(string) } },
    { Xml.SimpleType.NcName, new[] { typeof(string) } },
    {
      Xml.SimpleType.NegativeInteger,
      new[]
      {
        typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong),
        typeof(string)
      }
    },
    { Xml.SimpleType.NmToken, new[] { typeof(string) } },
    { Xml.SimpleType.NmTokens, new[] { typeof(string[]) } },
    {
      Xml.SimpleType.NonNegativeInteger,
      new[]
      {
        typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong),
        typeof(string)
      }
    },
    {
      Xml.SimpleType.NonPositiveInteger,
      new[]
      {
        typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong),
        typeof(string)
      }
    },
    { Xml.SimpleType.NormalizedString, new[] { typeof(string) } },
    { Xml.SimpleType.Notation, new[] { typeof(string) } },
    {
      Xml.SimpleType.PositiveInteger,
      new[]
      {
        typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong),
        typeof(string)
      }
    },
    { Xml.SimpleType.QName, new[] { typeof(XmlQualifiedName), typeof(string) } },
    {
      Xml.SimpleType.Short,
      new[] { typeof(short), typeof(ushort), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(long) }
    },
    { Xml.SimpleType.String, new[] { typeof(string) } },
#if NET6_0_OR_GREATER
    { Xml.SimpleType.Time, new[] { typeof(DateTime), typeof(DateTimeOffset), typeof(TimeOnly) } },
#endif
    { Xml.SimpleType.Token, new[] { typeof(string) } },
    {
      Xml.SimpleType.UnsignedByte,
      new[] { typeof(byte), typeof(sbyte), typeof(int), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) }
    },
    {
      Xml.SimpleType.UnsignedInt,
      new[] { typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort), typeof(long), typeof(ulong) }
    },
    {
      Xml.SimpleType.UnsignedLong,
      new[] { typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort), typeof(long) }
    },
    {
      Xml.SimpleType.UnsignedShort,
      new[] { typeof(ushort), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(long) }
    }
  };

  /// <summary>
  /// Declares known Type converters.
  /// </summary>
  public static readonly Dictionary<Type, TypeConverter> KnownTypeConverters = new()
  {
    //{ typeof(Object), new ObjectTypeConverter() },
    { typeof(Array), new ArrayTypeConverter() },
    { typeof(bool), new BooleanTypeConverter() },
    { typeof(byte), new NumericTypeConverter { SimpleType = Xml.SimpleType.UnsignedByte } },
    { typeof(byte[]), new Base64TypeConverter() },
#if NET6_0_OR_GREATER
    { typeof(DateOnly), new DateTimeTypeConverter { SimpleType = Xml.SimpleType.Date, ExpectedType = typeof(DateOnly)} },
#endif
    { typeof(DateTime), new DateTimeTypeConverter { SimpleType = Xml.SimpleType.DateTime } },
    { typeof(DateTimeOffset), new DateTimeTypeConverter { SimpleType = Xml.SimpleType.DateTime, ExpectedType = typeof(DateTimeOffset) } },
    { typeof(decimal), new NumericTypeConverter { SimpleType = Xml.SimpleType.Decimal } },
    { typeof(double), new NumericTypeConverter { SimpleType = Xml.SimpleType.Double } },
    { typeof(float), new NumericTypeConverter { SimpleType = Xml.SimpleType.Float } },
    { typeof(GDate), new GDateTypeConverter() },
    { typeof(Guid), new GuidConverter() },
    { typeof(int), new NumericTypeConverter { SimpleType = Xml.SimpleType.Int } },
    { typeof(long), new NumericTypeConverter { SimpleType = Xml.SimpleType.Long } },
    { typeof(sbyte), new NumericTypeConverter { SimpleType = Xml.SimpleType.Byte } },
    { typeof(short), new NumericTypeConverter { SimpleType = Xml.SimpleType.Short } },
    { typeof(char), new StringTypeConverter{ ExpectedType = typeof(char) } },
    { typeof(string), new StringTypeConverter() },
    { typeof(string[]), new ArrayTypeConverter() },
#if NET6_0_OR_GREATER
    { typeof(TimeOnly), new DateTimeTypeConverter { SimpleType = Xml.SimpleType.Time, ExpectedType = typeof(DateOnly) } },
#endif
    { typeof(TimeSpan), new TimeSpanTypeConverter() },
    { typeof(uint), new NumericTypeConverter { SimpleType = Xml.SimpleType.UnsignedInt } },
    { typeof(ulong), new NumericTypeConverter { SimpleType = Xml.SimpleType.UnsignedLong } },
    { typeof(Uri), new UriTypeConverter() },
    { typeof(ushort), new NumericTypeConverter { SimpleType = Xml.SimpleType.UnsignedShort } },
    { typeof(XmlQualifiedName), new XmlQualifiedNameTypeConverter() },
    { typeof(DBNull), new DbNullTypeXmlConverter() },
  };

  /// <summary>
  /// Default constructor
  /// </summary>
  public ValueTypeConverter()
  {
  }

  /// <summary>
  /// Constructor with parameters.
  /// </summary>
  /// <param name="expectedType"></param>
  /// <param name="knownTypes"></param>
  /// <param name="knownNamespaces"></param>
  /// <param name="simpleType"></param>
  /// <param name="format"></param>
  /// <param name="culture"></param>
  /// <param name="options"></param>
  public ValueTypeConverter(Type? expectedType, IEnumerable<Type>? knownTypes = null, Dictionary<string, string>? knownNamespaces = null, SimpleType? simpleType = null, string? format = null, CultureInfo? culture = null,
    ConversionOptions? options = null)
  {
    if (knownTypes != null)
      KnownTypes = knownTypes.ToDictionary(type => type.FullName ?? "");
    KnownNamespaces = knownNamespaces;
    Init(expectedType, KnownTypes, KnownNamespaces, simpleType, format, culture, options);
  }

  /// <summary>
  /// InternalTypeConverter that is used in this instance.
  /// </summary>
  public TypeConverter? InternalTypeConverter { get; set; }

  /// <summary>
  /// Conversion options.
  /// </summary>
  public ConversionOptions? Options { get; set; }

  /// <summary>
  /// Initializing method using previously declared properties.
  /// </summary>
  public void Init()
  {
    Init(ExpectedType, KnownTypes, KnownNamespaces, SimpleType, Format, Culture, Options);
  }

    /// <summary>
  /// Initializing method with parameters.
  /// </summary>
  public void Init(Type? expectedType, Dictionary<string, Type>? knownTypes, Dictionary<string, string>? knownNamespaces, SimpleType? simpleType, string? format, CultureInfo? culture = null, ConversionOptions? options = null)
  {
    Options = options;
    if (expectedType?.IsNullable(out var baseType) == true)
      expectedType = baseType;
    ExpectedType = expectedType;

    SimpleType = simpleType;
    Format = format;
    if (SimpleType != null && SimpleType != 0)
    {
      if (!XsdSimpleTypeAcceptedTypes.TryGetValue((SimpleType)SimpleType, out var allowedTypes))
        throw new InvalidOperationException($"Unrecognized XmlDataType \"{SimpleType}\"");
      if (expectedType == null)
        expectedType = allowedTypes.First();
      ExpectedType = expectedType;
      switch (SimpleType)
      {
        case Xml.SimpleType.DateTime:
        case Xml.SimpleType.Date:
        case Xml.SimpleType.Time:
          InternalTypeConverter = CreateDateTimeTypeConverter((SimpleType)SimpleType, format, culture, options);
          return;
        case Xml.SimpleType.Boolean:
          InternalTypeConverter = CreateBooleanTypeConverter(ExpectedType, SimpleType, format, culture, options);
          if (expectedType != typeof(bool))
            InternalTypeConverter = new NumericTypeConverter { ExpectedType = expectedType, Format = format };
          return;
        case Xml.SimpleType.String:
          if (Options != null)
            InternalTypeConverter = new StringTypeConverter
            {
              UseEscapeSequences = Options.UseEscapeSequences,
              UseHtmlEntities = Options.UseHtmlEntities
            };
          else
            InternalTypeConverter = new StringTypeConverter();
          return;
        case Xml.SimpleType.Integer:
        case Xml.SimpleType.NegativeInteger:
        case Xml.SimpleType.NonNegativeInteger:
        case Xml.SimpleType.NonPositiveInteger:
        case Xml.SimpleType.PositiveInteger:
          InternalTypeConverter = new NumericTypeConverter { ExpectedType = expectedType };
          if (expectedType == typeof(bool)) InternalTypeConverter = CreateBooleanTypeConverter(ExpectedType, SimpleType, format, culture, options);
          return;

        case Xml.SimpleType.Int:
        case Xml.SimpleType.Byte:
        case Xml.SimpleType.UnsignedInt:
        case Xml.SimpleType.UnsignedByte:
        case Xml.SimpleType.Short:
        case Xml.SimpleType.UnsignedShort:
        case Xml.SimpleType.Long:
        case Xml.SimpleType.UnsignedLong:
          InternalTypeConverter = new NumericTypeConverter { ExpectedType = expectedType, Format = format };
          if (expectedType == typeof(bool))
            InternalTypeConverter = CreateBooleanTypeConverter(expectedType, SimpleType, format, culture, options);
          ExpectedType = expectedType;
          return;
        case Xml.SimpleType.Decimal:
          InternalTypeConverter = new NumericTypeConverter { ExpectedType = expectedType, Format = format };
          if (expectedType == typeof(bool))
            InternalTypeConverter = CreateBooleanTypeConverter(expectedType, SimpleType, format, culture, options);
          return;
        case Xml.SimpleType.Float:
        case Xml.SimpleType.Double:
          InternalTypeConverter = new NumericTypeConverter { ExpectedType = expectedType, Format = format };
          return;
        case Xml.SimpleType.GYear:
        case Xml.SimpleType.GYearMonth:
        case Xml.SimpleType.GMonth:
        case Xml.SimpleType.GMonthDay:
        case Xml.SimpleType.GDay:
          InternalTypeConverter = new GDateTypeConverter { SimpleType = simpleType };
          return;
      }
    }
    if (expectedType != null)
    {
      if (expectedType == typeof(string) || expectedType == typeof(char))
      {
        if (Options != null)
          InternalTypeConverter = new StringTypeConverter
          {
            UseEscapeSequences = Options.UseEscapeSequences,
            UseHtmlEntities = Options.UseHtmlEntities,
            ExpectedType = expectedType,
          };
        else
          InternalTypeConverter = new StringTypeConverter
          {
            ExpectedType = expectedType,
          };
      }
      else
      if (expectedType.IsEnum)
      {
        InternalTypeConverter = new EnumTypeConverter(expectedType);
      }
      else
      if (KnownTypeConverters.TryGetValue(expectedType, out var converter))
      {
        InternalTypeConverter = converter;

        //var converterType = converter.GetType();
        //InternalTypeConverter = (TypeConverter?)converterType.GetConstructor(new Type[0])?.Invoke(new object[0]);
        //if (converter is ITypeConverter iTypeConverter0 && iTypeConverter0.XsdType != null && simpleType == null)
        //  simpleType = iTypeConverter0.XsdType;

        if (InternalTypeConverter is ITypeConverter iTypeConverter)
        {
          iTypeConverter.ExpectedType = expectedType;
          iTypeConverter.SimpleType = simpleType;
          iTypeConverter.Format = format;
        }
      }
    }

    if (InternalTypeConverter == null)
    {
      if (expectedType != null && expectedType != typeof(object))
      {
        if (TryGetTypeConverter(expectedType, out var converter))
        {
          InternalTypeConverter = converter;
          return;
        }
        if (expectedType.IsConstructedGenericType)
          return;
        if (expectedType == typeof(Type))
        {
          if (knownTypes != null)
            InternalTypeConverter = new TypeNameConverter(knownTypes, knownNamespaces);
          return;
        }
        //throw new InvalidOperationException($"TypeConverter for {expectedType?.Name} type not found");
      }
      if (simpleType != null)
        throw new InvalidOperationException($"TypeConverter for simpleType={simpleType} not found");
      //InternalTypeConverter = new StringTypeConverter();
    }
  }

  private bool TryGetTypeConverter(Type expectedType, out TypeConverter? converter)
  {
    if (registeredTypeConverters.TryGetValue(expectedType, out converter))
      return true;
    var typeConverterAttrib = expectedType.GetCustomAttribute<System.ComponentModel.TypeConverterAttribute>();
    if (typeConverterAttrib != null)
    {
      var converterTypeName = typeConverterAttrib.ConverterTypeName;
      if (converterTypeName != null)
      {
        var converterType = Type.GetType(converterTypeName);
        if (converterType != null)
        {
          converter = (TypeConverter?)converterType.GetConstructor(new Type[0])?.Invoke(null);
          if (converter != null)
          {
            registeredTypeConverters.Add(expectedType, converter);
            return true;
          }
        }
      }
    }
    return false;
  }

  private static Dictionary<Type, TypeConverter> registeredTypeConverters = new()
  {
    { typeof(DBNull), new DbNullTypeXmlConverter() },
  };

  private StringTypeConverter CreateStringTypeConverter(SimpleType? simpleType, string? format, CultureInfo? culture,
    ConversionOptions? options)
  {
    var result = new StringTypeConverter { SimpleType = simpleType, Format = format };
    return result;
  }

  private DateTimeTypeConverter CreateDateTimeTypeConverter(SimpleType simpleType, string? format, CultureInfo? culture,
    ConversionOptions? options)
  {
    var result = new DateTimeTypeConverter
    {
      SimpleType = simpleType,
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

  private TypeConverter CreateBooleanTypeConverter(Type? expectedType, SimpleType? simpleType, string? format, CultureInfo? culture,
    ConversionOptions? options = null)
  {
    var result = new BooleanTypeConverter { ExpectedType = expectedType, SimpleType = simpleType };
    if (options?.BooleanStrings != null)
      result.BooleanStrings = options.BooleanStrings;
    return result;
  }

  /// <inheritdoc/>
  public override bool CanConvertFrom(ITypeDescriptorContext? context, Type sourceType)
  {
    if (ExpectedType == typeof(string))
      Debug.Assert(true);
    if (InternalTypeConverter != null)
      return InternalTypeConverter.CanConvertFrom(context, sourceType);
    return base.CanConvertFrom(context, sourceType);
  }

  /// <inheritdoc/>
  public override bool CanConvertTo(ITypeDescriptorContext? context, Type? destinationType)
  {
    if (InternalTypeConverter != null)
      return InternalTypeConverter.CanConvertTo(context, destinationType);
    return base.CanConvertTo(context, destinationType);
  }

  /// <inheritdoc/>
  public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? Culture, object? value, Type destinationType)
  {

    if (InternalTypeConverter == null)
      Init(ExpectedType, KnownTypes, KnownNamespaces, SimpleType, Format, Culture);
    if (InternalTypeConverter != null)
      return InternalTypeConverter.ConvertTo(context, Culture, value, destinationType);
    return null;
  }

  /// <inheritdoc/>
  public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
  {
    if (InternalTypeConverter == null)
      Init(ExpectedType, KnownTypes, KnownNamespaces, SimpleType, Format, Culture);
    if (InternalTypeConverter != null)
      return InternalTypeConverter.ConvertFrom(context, Culture, value);
    return value;
  }
}