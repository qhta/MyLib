using System;

using DocumentFormat.OpenXml;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for OpenXml element types and their values.
/// </summary>
public static class TypeTools
{

  /// <summary>
  /// Gets the properties of the OpenXml type (except for the framework properties).
  /// </summary>
  /// <param name="openXmlType"></param>
  /// <returns></returns>
  public static IEnumerable<PropertyInfo> GetOpenXmlProperties(this Type openXmlType)
  {
    return openXmlType.GetProperties().Where(p => !IsFrameworkProperty(p));
  }

  private static bool IsFrameworkProperty(PropertyInfo property)
  {
    return FrameworkPropNames.Contains(property.Name);
  }

  private static readonly string[] FrameworkPropNames = new[]
  {
    "ChildElements",
    "ExtendedAttributes",
    "Features",
    "FirstChild",
    "HasAttributes",
    "HasChildren",
    "InnerText",
    "InnerXml",
    "LastChild",
    "LocalName",
    "MCAttributes",
    "NamespaceDeclarations",
    "NamespaceUri",
    "OpenXmlElementContext",
    "OuterXml",
    "Parent",
    "Prefix",
    "XmlQualifiedName",
    "XName",
    "Xml",
  };

  /// <summary>
  /// Checks whether the openXmlType is an OpenXml enum.
  /// </summary>
  /// <param name="openXmlType"></param>
  /// <returns></returns>
  public static bool IsOpenXmlEnum(this Type openXmlType)
  {
    if (openXmlType.IsGenericType && openXmlType.GetGenericTypeDefinition() == typeof(DX.EnumValue<>))
      return true;
    if (openXmlType.BaseType == null)
      return false;
    if (openXmlType.BaseType == typeof(DX.OpenXmlLeafElement))
    {
      var properties = openXmlType.GetOpenXmlProperties().ToArray();
      if (properties.Count() == 1 && properties.First().Name == "Val")
        return IsOpenXmlEnum(properties.First().PropertyType);
    }
    return openXmlType.GetInterfaces().Contains(typeof(DX.IEnumValue));
  }

  /// <summary>
  /// Checks whether the openXmlType is an OpenXml enum.
  /// </summary>
  /// <param name="openXmlType"></param>
  /// <returns></returns>
  public static PropertyInfo[] GetOpenXmlEnumValues(this Type openXmlType)
  {
    return openXmlType.GetProperties(BindingFlags.DeclaredOnly | BindingFlags.Static | BindingFlags.Public);
  }

  /// <summary>
  /// Converts an OpenXml type to a system type.
  /// If conversion is not possible, the original type is returned.
  /// </summary>
  /// <param name="openXmlType"></param>
  /// <param name="propertyName">Used when the input type is not sufficient to determine output type</param>
  /// <returns></returns>
  public static Type ToSystemType(this Type openXmlType, string? propertyName)
  {
    if (openXmlType == typeof(object))
      return openXmlType;
    var propName = propertyName ?? "";
    //  Debug.WriteLine($"TypeTools.ToSystemType({openXmlType}, {propertyName})");
    if (openXmlType==typeof(DX.HexBinaryValue) && propName.StartsWith("Rsid"))
      return typeof(HexInt);
    if (openXmlType == typeof(DX.StringValue) && propName.EndsWith("Color.Val"))
      return typeof(HexRGB);
    if (openXmlType == typeof(DX.StringValue) && (propName.EndsWith("ThemeTint") || propName.EndsWith("ThemeShade")))
      return typeof(HexByte);
    if (openXmlType.Name == "EnumValue`1")
    {
      var type = openXmlType.GenericTypeArguments[0];
      return type;
    }
    if (openXmlType.BaseType == typeof(DX.OpenXmlLeafElement))
    {
      var properties = openXmlType.GetOpenXmlProperties().ToArray();
      if (properties.Count() == 1 && properties.First().Name == "Val")
        return ToSystemType(properties.First().PropertyType, propertyName);
    }
    if (OpenXmlTypesToSystemTypes.TryGetValue(openXmlType, out var info))
    {
      return info.targetType;
    }
    if (openXmlType.BaseType == null)
      return openXmlType;
    if (OpenXmlTypesToSystemTypes2.TryGetValue(openXmlType.BaseType, out var info2))
    {
      return info2.targetType;
    }
    return openXmlType;
  }

  /// <summary>
  /// Converts an OpenXml type value to a system type value.
  /// If conversion is not possible, the original value is returned.
  /// </summary>
  /// <param name="openXmlType"></param>
  /// <param name="openXmlValue"></param>
  /// <returns></returns>
  public static object? ToSystemValue(this object? openXmlValue, Type? openXmlType)
  {
    if (openXmlType == null || openXmlType == typeof(object))
      return openXmlValue;
    if (openXmlValue == null)
      return null;
    if (openXmlType.Name == "EnumValue`1")
    {
      var value = openXmlValue.ToString();
      if (value == null)
        return null;
      var props = openXmlType.GenericTypeArguments[0].GetProperties();
      foreach (var prop in props)
      {
        var propValue = prop.GetValue(null);
        if (propValue is DX.IEnumValue enumValue)
        {
          if (enumValue.Value == value)
            return prop.Name;
        }
      }
      return value;
    }

    if (OpenXmlTypesToSystemTypes.TryGetValue(openXmlType, out var info))
    {
      var targetValue = info.toSystemValueMethod(openXmlValue);
      return targetValue;
    }
    if (openXmlType.BaseType == null)
      return openXmlValue;
    if (OpenXmlTypesToSystemTypes2.TryGetValue(openXmlType.BaseType, out var info2))
    {
      var targetValue = info2.toSystemValueMethod(openXmlValue);
      return targetValue;
    }
    return openXmlValue;
  }

  /// <summary>
  /// Updates the OpenXml value with the system value.
  /// If conversion is not possible, the false value is returned.
  /// </summary>
  /// <param name="openXmlElement">OpenXmlElement</param>
  /// <param name="systemValue"></param>
  public static bool UpdateFromSystemValue(this DX.OpenXmlElement openXmlElement, object? systemValue)
  {
    var openXmlType = openXmlElement.GetType();
    if (OpenXmlTypesToSystemTypes2.TryGetValue(openXmlType.BaseType, out var info2))
    {
      info2.updateValueMethod(openXmlElement, systemValue);
      return true;
    }
    return false;
  }

  /// <summary>
  /// Converts a system type value to an OpenXml type value.
  /// If conversion is not possible, the original value is returned.
  /// </summary>
  /// <param name="openXmlType"></param>
  /// <param name="systemValue"></param>
  /// <returns></returns>
  public static object? ToOpenXmlValue(this object? systemValue, Type? openXmlType)
  {
    if (openXmlType == null || openXmlType == typeof(object))
      return systemValue;
    if (systemValue == null)
      return null;
    if (openXmlType.Name == "EnumValue`1")
    {
      var value = systemValue as String;
      if (value == null)
        return null;
      var prop = openXmlType.GenericTypeArguments[0].GetProperty(value);

      var propValue = prop!.GetValue(null);
      var result = Activator.CreateInstance(openXmlType, propValue);
      return result;
    }
    if (openXmlType.BaseType == typeof(DX.OpenXmlLeafElement))
    {
      var properties = openXmlType.GetOpenXmlProperties().ToArray();
      if (properties.Count() == 1 && properties.First().Name == "Val")
      {
        var val = ToOpenXmlValue(systemValue, properties.First().PropertyType);
        var result = Activator.CreateInstance(openXmlType) as DX.OpenXmlLeafElement;
        properties.First().SetValue(result, val);
        return result;
      }
    }
    if (OpenXmlTypesToSystemTypes.TryGetValue(openXmlType, out var info))
    {
      var targetValue = info.toOpenXmlValueMethod(systemValue);
      return targetValue;
    }
    if (OpenXmlTypesToSystemTypes2.TryGetValue(openXmlType.BaseType, out var info2))
    {
      var targetValue = info2.toOpenXmlValueMethod(openXmlType, systemValue);
      return targetValue;
    }
    return systemValue;
  }

  private static readonly Dictionary<Type, (Type targetType, Func<object?, object?> toSystemValueMethod, Func<object?, object?> toOpenXmlValueMethod)> OpenXmlTypesToSystemTypes = new()
  {
    { typeof(DX.BooleanValue), (typeof(Boolean), BooleanValueToBoolean, BooleanToBooleanValue) },
    { typeof(DX.ByteValue), (typeof(Byte), ByteValueToByte, ByteToByteValue) },
    { typeof(DX.DateTimeValue), (typeof(DateTime), DateTimeValueToDateTime, DateTimeToDateTimeValue) },
    { typeof(DX.IntegerValue), (typeof(Int64), IntegerValueToInt64, Int64ToIntegerValue) },
    { typeof(DX.DecimalValue), (typeof(Decimal), DecimalValueToDecimal, DecimalToDecimalValue) },
    { typeof(DX.DoubleValue), (typeof(Double), DoubleValueToDouble, DoubleToDoubleValue) },
    { typeof(DX.Int16Value), (typeof(Int16), Int16ValueToInt16, Int16ToInt16Value) },
    { typeof(DX.Int32Value), (typeof(Int32), Int32ValueToInt32, Int32ToInt32Value) },
    { typeof(DX.Int64Value), (typeof(Int64), Int64ValueToInt64, Int64ToInt64Value) },
    { typeof(DX.StringValue), (typeof(String), StringValueToString, StringToStringValue) },
    { typeof(DX.UInt16Value), (typeof(UInt16), UInt16ValueToUInt16, UInt16ToUInt16Value) },
    { typeof(DX.UInt32Value), (typeof(UInt32), UInt32ValueToUInt32, UInt32ToUInt32Value) },
    { typeof(DX.UInt64Value), (typeof(UInt64), UInt64ValueToUInt64, UInt64ToUInt64Value) },
    { typeof(DX.OnOffValue), (typeof(Boolean), OnOffValueToBoolean, BooleanToOnOffValue) },
    { typeof(DXW.OnOffOnlyValues), (typeof(Boolean), OnOffOnlyValuesToBoolean, BooleanToOnOffOnlyValues) },
    { typeof(DXO10W.OnOffValues), (typeof(Boolean?), OnOffValuesToBoolean, BooleanToOnOffValues) },
    { typeof(DX.HexBinaryValue), (typeof(String), HexBinaryValueToString, StringToHexBinaryValue) },
    { typeof(DX.Base64BinaryValue), (typeof(String), Base64BinaryValueToString, StringToBase64BinaryValue) },

  };

  private static readonly
    Dictionary<Type, (Type targetType, Func<object?, object?> toSystemValueMethod, Func<Type, object?, object?> toOpenXmlValueMethod, Action<DX.OpenXmlElement, object?> updateValueMethod)> OpenXmlTypesToSystemTypes2 = new()
    {
      { typeof(DXW.OnOffType), (typeof(Boolean), OnOffTypeToBoolean, BooleanToOnOffType, UpdateOnOffType) },
      { typeof(DXW.OnOffOnlyType), (typeof(Boolean), OnOffTypeToBoolean, BooleanToOnOffType, UpdateOnOffType) },
      { typeof(DXO10W.OnOffType), (typeof(Boolean), OnOffTypeToBoolean, BooleanToOnOffType, UpdateOnOffType) },
      { typeof(DXO13W.OnOffType), (typeof(Boolean), OnOffTypeToBoolean, BooleanToOnOffType, UpdateOnOffType) },
      { typeof(DXW.LongHexNumberType), (typeof(HexInt), LongHexNumberTypeToHexInt, HexIntToLongHexNumberType, UpdateLongHexNumberType) },
      { typeof(DXW.StringType), (typeof(String), StringTypeToString, StringToStringType, UpdateStringType) },
      { typeof(DXW.String255Type), (typeof(String), StringTypeToString, StringToStringType, UpdateStringType) },
      { typeof(DXW.String253Type), (typeof(String), StringTypeToString, StringToStringType, UpdateStringType) },
      { typeof(DXW.DecimalNumberType), (typeof(int), DecimalNumberTypeToInt, IntToDecimalNumberType, UpdateDecimalType) },
      { typeof(DXW.UnsignedDecimalNumberType), (typeof(int), DecimalNumberTypeToInt, IntToDecimalNumberType, UpdateDecimalType) },
      { typeof(DXW.UnsignedDecimalNumberMax3Type), (typeof(int), DecimalNumberTypeToInt, IntToDecimalNumberType, UpdateDecimalType) },
      { typeof(DXW.UnsignedInt7Type), (typeof(int), DecimalNumberTypeToInt, IntToDecimalNumberType, UpdateDecimalType) },
    };

  private static object? BooleanValueToBoolean(object? value)
  {
    if (value is null)
      return null;
    if (value is DX.BooleanValue booleanValue)
    {
      return booleanValue.Value;
    }
    throw new ArgumentException("Value is not of BooleanValue type", nameof(value));
  }

  private static object? BooleanToBooleanValue(object? value)
  {
    if (value is null)
      return null;
    if (value is Boolean booleanValue)
    {
      return new DX.BooleanValue(booleanValue);
    }
    throw new ArgumentException("Value is not of Boolean type", nameof(value));
  }

  private static object? ByteValueToByte(object? value)
  {
    if (value is DX.ByteValue booleanValue)
    {
      return booleanValue.Value;
    }
    throw new ArgumentException("Value is not of ByteValue type", nameof(value));
  }

  private static object? ByteToByteValue(object? value)
  {
    if (value is Byte byteValue)
    {
      return new DX.ByteValue(byteValue);
    }
    throw new ArgumentException("Value is not of Byte type", nameof(value));
  }

  private static object? DateTimeValueToDateTime(object? value)
  {
    if (value is DX.DateTimeValue dateTimeValue)
    {
      return dateTimeValue.Value;
    }
    throw new ArgumentException("Value is not of DateTimeValue type", nameof(value));
  }

  private static object? DateTimeToDateTimeValue(object? value)
  {
    if (value is DateTime dateTimeValue)
    {
      return new DX.DateTimeValue(dateTimeValue);
    }
    throw new ArgumentException("Value is not of DateTime type", nameof(value));
  }

  private static object? IntegerValueToInt64(object? value)
  {
    if (value is DX.IntegerValue integerValue)
    {
      return integerValue.Value;
    }
    throw new ArgumentException("Value is not of IntegerValue type", nameof(value));
  }

  private static object? Int64ToIntegerValue(object? value)
  {
    if (value is Int64 integerValue)
    {
      return new DX.IntegerValue(integerValue);
    }
    throw new ArgumentException("Value is not of Int64 type", nameof(value));
  }

  private static object? DecimalValueToDecimal(object? value)
  {
    if (value is DX.DecimalValue decimalValue)
    {
      return decimalValue.Value;
    }
    throw new ArgumentException("Value is not of DecimalValue type", nameof(value));
  }

  private static object? DecimalToDecimalValue(object? value)
  {
    if (value is Decimal decimalValue)
    {
      return new DX.DecimalValue(decimalValue);
    }
    throw new ArgumentException("Value is not of Decimal type", nameof(value));
  }

  private static object? DoubleValueToDouble(object? value)
  {
    if (value is DX.DoubleValue doubleValue)
    {
      return doubleValue.Value;
    }
    throw new ArgumentException("Value is not of DoubleValue type", nameof(value));
  }

  private static object? DoubleToDoubleValue(object? value)
  {
    if (value is Double doubleValue)
    {
      return new DX.DoubleValue(doubleValue);
    }
    throw new ArgumentException("Value is not of Double type", nameof(value));
  }

  private static object? Int16ValueToInt16(object? value)
  {
    if (value is DX.Int16Value int16Value)
    {
      return int16Value.Value;
    }
    throw new ArgumentException("Value is not of Int16Value type", nameof(value));
  }

  private static object? Int16ToInt16Value(object? value)
  {
    if (value is Int16 int16Value)
    {
      return new DX.Int16Value(int16Value);
    }
    throw new ArgumentException("Value is not of Int16 type", nameof(value));
  }

  private static object? Int32ValueToInt32(object? value)
  {
    if (value is DX.Int32Value int32Value)
    {
      return int32Value.Value;
    }
    throw new ArgumentException("Value is not of Int32Value type", nameof(value));
  }

  private static object? Int32ToInt32Value(object? value)
  {
    if (value is Int32 int32Value)
    {
      return new DX.Int32Value(int32Value);
    }
    throw new ArgumentException("Value is not of Int32 type", nameof(value));
  }

  private static object? Int64ValueToInt64(object? value)
  {
    if (value is DX.Int64Value int64Value)
    {
      return int64Value.Value;
    }
    throw new ArgumentException("Value is not of Int64Value type", nameof(value));
  }

  private static object? Int64ToInt64Value(object? value)
  {
    if (value is Int64 int64Value)
    {
      return new DX.Int64Value(int64Value);
    }
    throw new ArgumentException("Value is not of Int64 type", nameof(value));
  }

  private static object? StringValueToString(object? value)
  {
    if (value is DX.StringValue stringValue)
    {
      return stringValue.Value!;
    }
    throw new ArgumentException("Value is not of StringValue type", nameof(value));
  }

  private static object? StringToStringValue(object? value)
  {
    if (value is String stringValue)
    {
      return new DX.StringValue(stringValue);
    }
    throw new ArgumentException("Value is not of String type", nameof(value));
  }

  private static object? UInt16ValueToUInt16(object? value)
  {
    if (value is DX.UInt16Value uint16Value)
    {
      return uint16Value.Value;
    }
    throw new ArgumentException("Value is not of UInt16Value type", nameof(value));
  }

  private static object? UInt16ToUInt16Value(object? value)
  {
    if (value is UInt16 uint16Value)
    {
      return new DX.UInt16Value(uint16Value);
    }
    throw new ArgumentException("Value is not of UInt16 type", nameof(value));
  }

  private static object? UInt32ValueToUInt32(object? value)
  {
    if (value is DX.UInt32Value uint32Value)
    {
      return uint32Value.Value;
    }
    throw new ArgumentException("Value is not of UInt32Value type", nameof(value));
  }

  private static object? UInt32ToUInt32Value(object? value)
  {
    if (value is UInt32 uint32Value)
    {
      return new DX.UInt32Value(uint32Value);
    }
    throw new ArgumentException("Value is not of UInt32 type", nameof(value));
  }

  private static object? UInt64ValueToUInt64(object? value)
  {
    if (value is DX.UInt64Value uint64Value)
    {
      return uint64Value.Value;
    }
    throw new ArgumentException("Value is not of UInt64Value type", nameof(value));
  }

  private static object? UInt64ToUInt64Value(object? value)
  {
    if (value is UInt64 uint64Value)
    {
      return new DX.UInt64Value(uint64Value);
    }
    throw new ArgumentException("Value is not of UInt64 type", nameof(value));
  }

  private static object? OnOffValueToBoolean(object? value)
  {
    if (value is DX.OnOffValue onOffValue)
    {
      return onOffValue.Value;
    }
    throw new ArgumentException("Value is not of OnOffValue type", nameof(value));
  }

  private static object? BooleanToOnOffValue(object? value)
  {
    if (value is Boolean booleanValue)
    {
      return new DX.OnOffValue(booleanValue);
    }
    throw new ArgumentException("Value is not of Boolean type", nameof(value));
  }

  private static object? OnOffOnlyValuesToBoolean(object? value)
  {
    if (value is DXW.OnOffOnlyValues offOnlyValue)
    {
      if (offOnlyValue == DXW.OnOffOnlyValues.On)
        return true;
      if (offOnlyValue == DXW.OnOffOnlyValues.Off)
        return false;
      return null;
    }
    throw new ArgumentException("Value is not of OnOffOnlyValues type", nameof(value));
  }

  private static object? BooleanToOnOffOnlyValues(object? value)
  {
    if (value is Boolean booleanValue)
    {
      if (booleanValue)
        return DXW.OnOffOnlyValues.On;
      if (!booleanValue)
        return DXW.OnOffOnlyValues.Off;
    }
    return null;
  }

  private static object? OnOffTypeToBoolean(object? value)
  {
    if (value is DXW.OnOffType onOffTypeValue)
    {
      var val = onOffTypeValue.Val?.Value;
      return val != false;
    }
    if (value is DXO10W.OnOffType onOffTypeValue2)
    {
      var val = onOffTypeValue2.Val?.Value;
      if (val == DXO10W.OnOffValues.One || val == DXO10W.OnOffValues.True)
        return true;
      if (val == DXO10W.OnOffValues.Zero || val == DXO10W.OnOffValues.False)
        return false;
      return null;
    }
    throw new ArgumentException("Value type if invalid to convert to boolean", nameof(value));
  }

  private static object? BooleanToOnOffType(Type targetType, object? value)
  {
    var booleanValue = (bool?)value;
    if (targetType.IsSubclassOf(typeof(DXW.OnOffType)))
    {
      var result = Activator.CreateInstance(targetType) as DXW.OnOffType;
      if (booleanValue != null)
        result!.Val = new DX.OnOffValue(booleanValue);
      return result;
    }

    if (targetType.IsSubclassOf(typeof(DXW.OnOffOnlyType)))
    {
      var result = Activator.CreateInstance(targetType) as DXW.OnOffOnlyType;
      if (booleanValue == true)
        result!.Val = DXW.OnOffOnlyValues.On;
      if (booleanValue == false)
        result!.Val = DXW.OnOffOnlyValues.Off;
      return result;
    }

    if (targetType.IsSubclassOf(typeof(DXO10W.OnOffType)))
    {
      var result = Activator.CreateInstance(targetType) as DXO10W.OnOffType;
      if (booleanValue == true)
        result!.Val = DXO10W.OnOffValues.One;
      if (booleanValue == false)
        result!.Val = DXO10W.OnOffValues.Zero;
      return result;
    }

    if (targetType.IsSubclassOf(typeof(DXO13W.OnOffType)))
    {
      var result = Activator.CreateInstance(targetType) as DXO13W.OnOffType;
      if (booleanValue != null)
        result!.Val = new DX.OnOffValue(booleanValue);
      return result;
    }

    throw new ArgumentException("TargetType is invalid to convert from boolean", nameof(targetType));
  }

  private static void UpdateOnOffType(DX.OpenXmlElement openXmlElement, object? value)
  {
    var booleanValue = (bool?)value;
    if (openXmlElement is DXW.OnOffType onOffType)
    {
      if (booleanValue != null)
        onOffType.Val = new DX.OnOffValue(booleanValue);
      else
        onOffType.Val = null;
    }
    else
    if (openXmlElement is DXW.OnOffOnlyType onOffOnlyType)
    {
      if (booleanValue == true)
        onOffOnlyType.Val = DXW.OnOffOnlyValues.On;
      else
      if (booleanValue == false)
        onOffOnlyType.Val = DXW.OnOffOnlyValues.Off;
      else
        onOffOnlyType.Val = null;
    }
    else
    if (openXmlElement is DXO10W.OnOffType onOffType2)
    {
      if (booleanValue == true)
        onOffType2.Val = DXO10W.OnOffValues.One;
      if (booleanValue == false)
        onOffType2.Val = DXO10W.OnOffValues.Zero;
      else
        onOffType2.Val = null;
    }
    else
    if (openXmlElement is DXO13W.OnOffType onOffType3)
    {
      if (booleanValue != null)
        onOffType3.Val = new DX.OnOffValue(booleanValue);
      else
        onOffType3.Val = null;
    }
    else
      throw new ArgumentException("Object cannot update from boolean", nameof(openXmlElement));
  }

  private static object? OnOffValuesToBoolean(object? value)
  {
    if (value is DXO10W.OnOffValues onOffValues)
    {
      if (onOffValues == DXO10W.OnOffValues.One || onOffValues == DXO10W.OnOffValues.True)
        return true;
      if (onOffValues == DXO10W.OnOffValues.Zero || onOffValues == DXO10W.OnOffValues.False)
        return false;
    }
    throw new ArgumentException("Value is not of OnOffValues type", nameof(value));
  }

  private static object? BooleanToOnOffValues(object? value)
  {
    if (value is Boolean booleanValue)
    {
      if (booleanValue)
        return DXO10W.OnOffValues.One;
      if (!booleanValue)
        return DXO10W.OnOffValues.Zero;
    }
    throw new ArgumentException("Value is not of Boolean type", nameof(value));
  }

  private static object? HexBinaryValueToString(object? value)
  {
    if (value is DX.HexBinaryValue hexBinaryValue)
    {
      return hexBinaryValue.Value;
    }
    throw new ArgumentException("Value is not of HexBinaryValue type", nameof(value));
  }

  private static object? StringToHexBinaryValue(object? value)
  {
    if (value is String stringValue)
    {
      return new DX.HexBinaryValue(stringValue);
    }
    throw new ArgumentException("Value is not of String type", nameof(value));
  }

  private static object? Base64BinaryValueToString(object? value)
  {
    if (value is DX.Base64BinaryValue base64BinaryValue)
    {
      return base64BinaryValue.Value;
    }
    throw new ArgumentException("Value is not of Base64BinaryValue type", nameof(value));
  }

  private static object? StringToBase64BinaryValue(object? value)
  {
    if (value is String stringValue)
    {
      return new DX.Base64BinaryValue(stringValue);
    }
    throw new ArgumentException("Value is not of String type", nameof(value));
  }

  private static object? LongHexNumberTypeToHexInt(object? value)
  {
    if (value is DXW.LongHexNumberType longHexNumber)
    {
      var val = longHexNumber.Val?.Value;
      if (val != null)
        return new HexInt(val);
      return null;
    }
    throw new ArgumentException("Value is not of LongHexNumberType type", nameof(value));
  }

  private static object? HexIntToLongHexNumberType(Type type, object? value) //where T : DXW.LongHexNumberType, new()
  {
    if (value is HexInt hexIntValue)
    {
      var result = Activator.CreateInstance(type) as DXW.LongHexNumberType;
      result!.Val = new DX.HexBinaryValue(hexIntValue.ToString());
      return result;
    }
    if (value is string stringValue)
    {
      var result = Activator.CreateInstance(type) as DXW.LongHexNumberType;
      result!.Val = new DX.HexBinaryValue(stringValue);
      return result;
    }
    throw new ArgumentException("Value is not an HexInt nor string", nameof(value));
  }

  private static void UpdateLongHexNumberType(DX.OpenXmlElement openXmlElement, object? value)
  {
    if (openXmlElement is DXW.LongHexNumberType longHexNumber)
    {
      if (value is HexInt hexIntValue)
      {
        longHexNumber.Val = new DX.HexBinaryValue(hexIntValue.ToString());
      }
      else
      if (value is string stringValue)
      {
        longHexNumber.Val = new DX.HexBinaryValue(stringValue);
      }
      else
        throw new ArgumentException("Value is not an HexInt nor string", nameof(value));
    }
    else
      throw new ArgumentException("Object is not of LongHexNumberType type", nameof(openXmlElement));
  }

  private static object? StringTypeToString(object? value)
  {
    if (value is DXW.StringType stringValue)
    {
      var val = stringValue.Val?.Value;
      return val;
    }
    if (value is DXW.String255Type string255Value)
    {
      var val = string255Value.Val?.Value;
      return val;
    }
    if (value is DXW.String253Type string253Value)
    {
      var val = string253Value.Val?.Value;
      return val;
    }
    throw new ArgumentException("Value is not of any StringType", nameof(value));
  }

  private static object? StringToStringType(Type targetType, object? value) //where T : DXW.StringType, new()
  {
    if (value is string stringValue)
    {
      if (targetType.BaseType == typeof(DXW.StringType))
      {
        var result = Activator.CreateInstance(targetType) as DXW.StringType;
        result!.Val = new DX.StringValue(stringValue);
        return result;
      }
      if (targetType.BaseType == typeof(DXW.String255Type))
      {
        var result = Activator.CreateInstance(targetType) as DXW.String255Type;
        result!.Val = new DX.StringValue(stringValue);
        return result;
      }
      if (targetType.BaseType == typeof(DXW.String253Type))
      {
        var result = Activator.CreateInstance(targetType) as DXW.String253Type;
        result!.Val = new DX.StringValue(stringValue);
        return result;
      }
      throw new ArgumentException("TargetType is not of any StringType", nameof(targetType));
    }
    throw new ArgumentException("Value is not a string", nameof(value));
  }

  private static void UpdateStringType(DX.OpenXmlElement openXmlElement, object? value)
  {
    if (value is string stringValue)
    {
      if (openXmlElement is DXW.StringType stringType)
      {
        stringType.Val = new DX.StringValue(stringValue);
      }
      else
      if (openXmlElement is DXW.String255Type string255Type)
      {
        string255Type.Val = new DX.StringValue(stringValue);
      }
      else
      if (openXmlElement is DXW.String253Type string253Type)
      {
        string253Type.Val = new DX.StringValue(stringValue);
      }
      else
        throw new ArgumentException("Element is not of any StringType", nameof(openXmlElement));
    }
    throw new ArgumentException("Value is not a string", nameof(value));
  }

  private static object? DecimalNumberTypeToInt(object? value)
  {
    if (value is DXW.DecimalNumberType decimalNumberType)
    {
      var val = decimalNumberType.Val?.Value;
      return val;
    }
    if (value is DXW.UnsignedDecimalNumberType unsignedDecimalNumber)
    {
      var val = unsignedDecimalNumber.Val?.Value;
      return val;
    }
    if (value is DXW.UnsignedDecimalNumberMax3Type unsignedDecimalNumberMax3)
    {
      var val = unsignedDecimalNumberMax3.Val?.Value;
      return val;
    }
    if (value is DXW.UnsignedInt7Type unsignedInt7)
    {
      var val = unsignedInt7.Val?.Value;
      return val;
    }
    throw new ArgumentException("Value is not of any DecimalNumber type", nameof(value));
  }

  private static object? IntToDecimalNumberType(Type targetType, object? value) //where T : DXW.DecimalNumberType, new()
  {
    if (value is int intValue)
    {
      if (targetType.BaseType == typeof(DXW.DecimalNumberType))
      {
        var result = Activator.CreateInstance(targetType) as DXW.DecimalNumberType;
        result!.Val = new DX.Int32Value(intValue);
        return result;
      }
      if (targetType.BaseType == typeof(DXW.UnsignedDecimalNumberType))
      {
        var result = Activator.CreateInstance(targetType) as DXW.UnsignedDecimalNumberType;
        result!.Val = new DX.UInt32Value(Convert.ToUInt32(intValue));
        return result;
      }
      if (targetType.BaseType == typeof(DXW.UnsignedDecimalNumberMax3Type))
      {
        var result = Activator.CreateInstance(targetType) as DXW.UnsignedDecimalNumberMax3Type;
        result!.Val = new DX.Int32Value(intValue);
        return result;
      }
      if (targetType.BaseType == typeof(DXW.UnsignedInt7Type))
      {
        var result = Activator.CreateInstance(targetType) as DXW.UnsignedInt7Type;
        result!.Val = new DX.Int32Value(intValue);
        return result;
      }
      throw new ArgumentException("TargetType is not of any DecimalNumber", nameof(targetType));
    }
    if (value is uint uintValue)
    {
      if (targetType.BaseType == typeof(DXW.DecimalNumberType))
      {
        var result = Activator.CreateInstance(targetType) as DXW.DecimalNumberType;
        result!.Val = new DX.Int32Value(Convert.ToInt32(uintValue));
        return result;
      }
      if (targetType.BaseType == typeof(DXW.UnsignedDecimalNumberType))
      {
        var result = Activator.CreateInstance(targetType) as DXW.UnsignedDecimalNumberType;
        result!.Val = new DX.UInt32Value(uintValue);
        return result;
      }
      if (targetType.BaseType == typeof(DXW.UnsignedDecimalNumberMax3Type))
      {
        var result = Activator.CreateInstance(targetType) as DXW.UnsignedDecimalNumberMax3Type;
        result!.Val = new DX.Int32Value(Convert.ToInt32(uintValue));
        return result;
      }
      if (targetType.BaseType == typeof(DXW.UnsignedInt7Type))
      {
        var result = Activator.CreateInstance(targetType) as DXW.UnsignedInt7Type;
        result!.Val = new DX.Int32Value(Convert.ToInt32(uintValue));
        return result;
      }
      throw new ArgumentException("TargetType is not of any DecimalNumber", nameof(targetType));
    }
    throw new ArgumentException("Value is not an int", nameof(value));
  }

  private static void UpdateDecimalType(DX.OpenXmlElement openXmlElement, object? value)
  {
    if (value is int intValue)
    {
      if (openXmlElement is DXW.DecimalNumberType decimalNumberType)
      {
        decimalNumberType.Val = new DX.Int32Value(intValue);
      }
      else
      if (openXmlElement is DXW.UnsignedDecimalNumberType unsignedDecimalNumberType)
      {
        unsignedDecimalNumberType.Val = new DX.UInt32Value(Convert.ToUInt32(intValue));
      }
      else
      if (openXmlElement is DXW.UnsignedDecimalNumberMax3Type unsignedDecimalNumberMax3Type)
      {
        unsignedDecimalNumberMax3Type.Val = new DX.Int32Value(intValue);
      }
      else
      if (openXmlElement is DXW.UnsignedInt7Type unsignedInt7Type)
      {
        unsignedInt7Type.Val = new DX.Int32Value(intValue);
      }
      else
        throw new ArgumentException("Element is not of any DecimalNumber", nameof(openXmlElement));
    }
    else
    if (value is uint uintValue)
    {
      if (openXmlElement is DXW.DecimalNumberType decimalNumberType)
      {
        decimalNumberType.Val = new DX.Int32Value(Convert.ToInt32(uintValue));
      }
      else
      if (openXmlElement is DXW.UnsignedDecimalNumberType unsignedDecimalNumberType)
      {
        unsignedDecimalNumberType.Val = new DX.UInt32Value(uintValue);
      }
      else
      if (openXmlElement is DXW.UnsignedDecimalNumberMax3Type unsignedDecimalNumberMax3Type)
      {
        unsignedDecimalNumberMax3Type.Val = new DX.Int32Value(Convert.ToInt32(uintValue));
      }
      else
      if (openXmlElement is DXW.UnsignedInt7Type unsignedInt7Type)
      {
        unsignedInt7Type.Val = new DX.Int32Value(Convert.ToInt32(uintValue));
      }
      else
        throw new ArgumentException("Element is not of any DecimalNumber", nameof(openXmlElement));
    }
    throw new ArgumentException("Value is not an int", nameof(value));
  }


  /// <summary>
  /// Check whether the OpenXml type can contain members.
  /// </summary>
  /// <param name="openXmlType"></param>
  /// <returns></returns>
  public static bool IsContainer(this Type openXmlType)
  {
    //return MemberTypes.ContainsKey(openXmlType);
    return typeof(OpenXmlCompositeElement).IsAssignableFrom(openXmlType) && GetMemberTypes(openXmlType).Any();
  }

  /// <summary>
  /// Gets the acceptable member types of the OpenXml type.
  /// </summary>
  /// <param name="openXmlType"></param>
  /// <returns></returns>
  public static Type[] GetMemberTypes(this Type openXmlType)
  //{
  //  if (MemberTypes.TryGetValue(openXmlType, out var types))
  //  {
  //    return types;
  //  }
  //  return [];
  //}

  //public static List<Type> GetAllowedMemberClasses(Type openXmlType)
  {
    //if (!typeof(OpenXmlCompositeElement).IsAssignableFrom(openXmlType))
    //{
    //  throw new ArgumentException("Type must be a subclass of OpenXmlCompositeElement", nameof(openXmlType));
    //}

    if (openXmlType == typeof(DXW.Rsids))
      return [typeof(DXW.Rsid)];

    var openXmlPropertyClasses = new List<Type>();
    var openXmlMemberClasses = new List<Type>();

    // Get properties that are OpenXmlElement or derived types
    var properties = openXmlType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
      .Where(p => typeof(OpenXmlElement).IsAssignableFrom(p.PropertyType));

    foreach (var property in properties)
    {
      openXmlPropertyClasses.Add(property.PropertyType);
    }

    // Get child element types from the ChildElements property
    var childElementInfo = openXmlType.GetProperty("ChildElements", BindingFlags.Public | BindingFlags.Instance);
    if (childElementInfo != null)
    {
      var childElementTypes = childElementInfo.PropertyType.GenericTypeArguments;
      foreach (var childElementType in childElementTypes)
      {
        if (!openXmlPropertyClasses.Contains(childElementType))
        {
          openXmlMemberClasses.Add(childElementType);
        }
      }
    }
    return openXmlMemberClasses.Distinct().ToArray();
  }

  private static readonly Dictionary<Type, Type[]> MemberTypes = new()
  {
    { typeof(DXW.Body), [typeof(DXW.Paragraph), typeof(DXW.Table)] },
    { typeof(DXW.Paragraph), [typeof(DXW.Run), typeof(DXW.Break), typeof(DXW.TabChar), typeof(DXW.Text)] },
    { typeof(DXW.Table), [typeof(DXW.TableRow)] },
    { typeof(DXW.TableRow), [typeof(DXW.TableCell)] },
    { typeof(DXW.TableCell), [typeof(DXW.Paragraph)] },
    { typeof(DXW.Run), [typeof(DXW.Text)] },
    { typeof(DXW.Rsids), [typeof(DXW.Rsid)] }
  };

}