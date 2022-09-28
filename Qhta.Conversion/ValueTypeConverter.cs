using System.ComponentModel;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml;

using Qhta.TypeUtils;

namespace Qhta.Conversion
{
  public class ValueTypeConverter : TypeConverter
  {
    protected readonly TypeConverter? InternalTypeConverter;
    protected readonly TypeConverter ValueStringConverter;
    /// <summary>
    /// Declared in constructor as parameter
    /// </summary>
    public readonly Type ObjectType;
    /// <summary>
    /// Declared in constructor as parameter
    /// </summary>
    public readonly string? XmlDataType;
    /// <summary>
    /// A type resulting from XmlDataType
    /// </summary>
    public readonly Type ExpDataType;

    private readonly Dictionary<string, Type[]> XmlDataToType = new Dictionary<string, Type[]>
    {
      { "base64Binary", new Type[] { typeof(byte[]) } },
      { "hexBinary", new Type[] { typeof(byte[]) } },
      { "boolean", new Type[] { typeof(bool), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong) } },
      { "integer", new Type[] { typeof(String), typeof(bool), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(bool) } },
      { "negativeInteger", new Type[] { typeof(String), typeof(bool), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(bool) } },
      { "nonNegativeInteger", new Type[] { typeof(string), typeof(bool), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(bool) } },
      { "nonPositiveInteger", new Type[] { typeof(string), typeof(bool), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(bool) } },
      { "positiveInteger", new Type[] { typeof(string), typeof(bool), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(bool) } },
      { "int", new Type[] { typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(bool), } },
      { "byte", new Type[] { typeof(sbyte), typeof(int), typeof(byte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(bool), } },
      { "unsignedByte", new Type[] { typeof(byte), typeof(sbyte), typeof(int), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(bool), } },
      { "unsignedInt", new Type[] { typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(bool), } },
      { "short", new Type[] { typeof(short), typeof(ushort), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(long), typeof(bool), } },
      { "unsignedShort", new Type[] { typeof(ushort), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(long), typeof(bool), } },
      { "long", new Type[] { typeof(long), typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort), typeof(bool), } },
      { "unsignedLong", new Type[] { typeof(ulong), typeof(uint), typeof(byte), typeof(sbyte), typeof(int), typeof(short), typeof(ushort), typeof(long), typeof(bool), } },

      { "decimal", new Type[] { typeof(decimal), typeof(int), typeof(byte), typeof(sbyte), typeof(uint), typeof(short), typeof(ushort), typeof(long), typeof(ulong), typeof(bool), } },
      { "float", new Type[] { typeof(float), typeof(double) } },
      { "double", new Type[] { typeof(double), typeof(float) } },

      { "dateTime", new Type[] { typeof(DateTime) } },
      { "date", new Type[] { typeof(DateTime), typeof(DateOnly) } },
      { "time", new Type[] { typeof(DateTime), typeof(TimeOnly) } },

      { "anyURI", new Type[]{ typeof(String) } },
      { "ENTITY", new Type[] { typeof(String) } },
      { "ENTITIES", new Type[] { typeof(String) } },
      { "gDay", new Type[] { typeof(String) } },
      { "gMonth", new Type[] { typeof(String) } },
      { "gMonthDay", new Type[] { typeof(String) } },
      { "gYear", new Type[] { typeof(String) } },
      { "gYearMonth", new Type[] { typeof(String) } },
      { "ID", new Type[] { typeof(String) } },
      { "IDREF", new Type[] { typeof(String) } },
      { "IDREFS", new Type[] { typeof(String) } },

      { "language", new Type[] { typeof(String) } },

      { "Name", new Type[] { typeof(String) } },
      { "NCName", new Type[] { typeof(String) } },
      { "NMTOKEN", new Type[] { typeof(String) } },
      { "NMTOKENS", new Type[] { typeof(String) } },
      { "normalizedString", new Type[] { typeof(String) } },
      { "NOTATION", new Type[] { typeof(String) } },
      { "QName", new Type[] { typeof(XmlQualifiedName) } },
      { "duration", new Type[] { typeof(String) } },
      { "string", new Type[] { typeof(String) } },

      { "token", new Type[] { typeof(String) } },
    };

    private readonly Dictionary<Type, TypeConverter> StandardTypeConverters = new Dictionary<Type, TypeConverter>
    {
      { typeof(string), new StringTypeConverter() },
      { typeof(bool), new BooleanTypeConverter() },
      { typeof(int), new NumericTypeConverter{ ValueType = typeof(int)} },
      { typeof(byte), new NumericTypeConverter{ ValueType = typeof(byte)} },
      { typeof(uint), new NumericTypeConverter{ ValueType = typeof(uint)} },
      { typeof(sbyte), new NumericTypeConverter{ ValueType = typeof(sbyte)} },
      { typeof(short), new NumericTypeConverter{ ValueType = typeof(short)} },
      { typeof(ushort), new NumericTypeConverter{ ValueType = typeof(ushort)} },
      { typeof(long), new NumericTypeConverter{ ValueType = typeof(long)} },
      { typeof(ulong), new NumericTypeConverter{ ValueType = typeof(ulong)} },
      { typeof(float), new NumericTypeConverter{ ValueType = typeof(float)} },
      { typeof(double), new NumericTypeConverter{ ValueType = typeof(double)} },
      { typeof(decimal), new NumericTypeConverter{ ValueType = typeof(decimal)} },
      { typeof(DateTime), new DateTimeTypeConverter{ Mode=DateTimeFormatMode.Default } },
      { typeof(DateOnly), new DateTimeTypeConverter{ Mode=DateTimeFormatMode.DateOnly } },
      { typeof(TimeOnly), new DateTimeTypeConverter{ Mode=DateTimeFormatMode.TimeOnly } },
    };

    private readonly Dictionary<string, TypeConverter> SpecialTypeConverters = new Dictionary<string, TypeConverter>
    {

      { "base64Binary", new Base64BinaryTypeConverter() },
      { "hexBinary", new HexBinaryTypeConverter() },
      { "date", new DateTimeTypeConverter{ Mode=DateTimeFormatMode.DateOnly } },
      { "dateTime", new DateTimeTypeConverter{ Mode=DateTimeFormatMode.DateTime} },
      { "time", new DateTimeTypeConverter{ Mode=DateTimeFormatMode.TimeOnly } },
      { "defaultDateTime", new DateTimeTypeConverter{ Mode=DateTimeFormatMode.Default } },
    };

    public ValueTypeConverter(Type objectType, string? xmlDataType, string? format, CultureInfo? culture, ConversionOptions? options = null)
    {
      if (objectType.IsNullable(out var baseType))
        objectType = baseType;
      ObjectType = objectType;
      if (string.IsNullOrEmpty(xmlDataType))
      {
        if (!StandardTypeConverters.TryGetValue(objectType, out var standardTypeConverter))
          throw new InvalidOperationException($"XmlDataType for {objectType.Name} not found");
        ValueStringConverter = standardTypeConverter;
        //XmlDataType = xmlDataType;
        ExpDataType = objectType;
        return;
      }
      else
      {
        if (!XmlDataToType.TryGetValue(xmlDataType, out var allowedTypes))
          throw new InvalidOperationException($"Unrecognized XmlDataType \"{xmlDataType}\"");
        if (!allowedTypes.Contains(objectType))
          throw new InvalidOperationException($"XmlDataType {xmlDataType} can not be applied to {objectType.Name}");
        XmlDataType = xmlDataType;
        ExpDataType = allowedTypes[0];
      }
      if (/*xmlDataType!=null ||*/ format != null || culture != null || options != null)
      {
        switch (xmlDataType)
        {
          case "date":
            ValueStringConverter = CreateDateTimeTypeConverter(DateTimeFormatMode.DateOnly, format, culture, options);
            return;
          case "dateTime":
            ValueStringConverter = CreateDateTimeTypeConverter(DateTimeFormatMode.DateTime, format, culture, options);
            return;
          case "time":
            ValueStringConverter = CreateDateTimeTypeConverter(DateTimeFormatMode.TimeOnly, format, culture, options);
            return;
          case "defaultDateTime":
            ValueStringConverter = CreateDateTimeTypeConverter(DateTimeFormatMode.Default, format, culture, options);
            return;
          case "boolean":
            ValueStringConverter = CreateBooleanTypeConverter(objectType, format, culture, options);
            if (objectType != typeof(bool))
              InternalTypeConverter = new NumericTypeConverter { ValueType = objectType, Format = format };
            ExpDataType = typeof(bool);
            return;
          case "integer":
          case "negativeInteger":
          case "nonNegativeInteger":
          case "nonPositiveInteger":
          case "positiveInteger":
            ValueStringConverter = new NumericTypeConverter { ValueType = objectType };
            if (objectType == typeof(bool))
              InternalTypeConverter = CreateBooleanTypeConverter(objectType, format, culture, options);
            ExpDataType = objectType;
            return;

          case "int":
          case "byte":
          case "uint":
          case "sbyte":
          case "short":
          case "ushort":
          case "long":
          case "ulong":
            ValueStringConverter = new NumericTypeConverter { ValueType = objectType, Format = format };
            if (objectType == typeof(bool))
              InternalTypeConverter = CreateBooleanTypeConverter(objectType, format, culture, options);
            ExpDataType = objectType;
            return;
          case "decimal":
            ValueStringConverter = new NumericTypeConverter { ValueType = objectType, Format = format };
            if (objectType == typeof(bool))
              InternalTypeConverter = CreateBooleanTypeConverter(objectType, format, culture, options);
            ExpDataType = objectType;
            return;
          case "float":
          case "double":
            ValueStringConverter = new NumericTypeConverter { ValueType = objectType, Format = format };
            ExpDataType = objectType;
            return;
        }
      }
      if (ValueStringConverter == null && SpecialTypeConverters.TryGetValue(xmlDataType, out var specialConverter))
      {
        ValueStringConverter = specialConverter;
      }
      if (ValueStringConverter == null && StandardTypeConverters.TryGetValue(objectType, out var standardConverter))
      {
        ValueStringConverter = standardConverter;
      }
      if (ValueStringConverter == null)
        throw new InvalidOperationException($"TypeConverter for {xmlDataType} not found");
    }

    private DateTimeTypeConverter CreateDateTimeTypeConverter(DateTimeFormatMode mode, string? format, CultureInfo? culture,
      ConversionOptions? options)
    {
      var result = new DateTimeTypeConverter
      {
        Mode = mode,
        Format = format,
        Culture = culture,
      };
      if (options != null)
      {
        result.DateTimeSeparator = options.DateTimeSeparator;
        result.ShowSecondsFractionalPart = options.ShowSecondsFractionalPart;
        result.ShowTimeZone = options.ShowTimeZone;
      }
      return result;
    }

    private TypeConverter CreateBooleanTypeConverter(Type objectType, string? format, CultureInfo? culture, ConversionOptions? options = null)
    {
      var result = new BooleanTypeConverter();
      if (options?.BooleanStrings != null)
        result.BooleanStrings = options.BooleanStrings;
      return result;
    }

    public override object? ConvertTo(ITypeDescriptorContext? context, CultureInfo? culture, object? value, Type destinationType)
    {
      if (culture == null)
        culture = CultureInfo.InvariantCulture;
      if (InternalTypeConverter != null)
        value = InternalTypeConverter.ConvertTo(context, culture, value, ExpDataType);
      return ValueStringConverter.ConvertTo(context, culture, value, destinationType);
    }

    public override object? ConvertFrom(ITypeDescriptorContext? context, CultureInfo? culture, object value)
    {
      if (culture == null)
        culture = CultureInfo.InvariantCulture;
      if (ExpDataType != ObjectType)
      {
        var val = ValueStringConverter.ConvertFrom(context, culture, value);
        if (val != null && val.GetType() == ObjectType)
          return val;
        return ValueStringConverter.ConvertTo(context, culture, val, ObjectType);
      }
      return ValueStringConverter.ConvertFrom(context, culture, value);
    }
  }
}