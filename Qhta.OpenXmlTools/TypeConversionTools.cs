using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for converting OpenXml element types and their values.
/// </summary>
public static class TypeConversionTools
{
  /// <summary>
  /// Converts an OpenXml type to a system type.
  /// If conversion is not possible, the original type is returned.
  /// </summary>
  /// <param name="openXmlType"></param>
  /// <returns></returns>
  public static Type ToSystemType(this Type openXmlType)
  {
    if (openXmlType.Name == "EnumValue`1")
    {
      var type = openXmlType.GenericTypeArguments[0];
      return type;
    }
    if (openXmlType.IsSubclassOf(typeof(DXO10W.OnOffType)))
    {
      var type = typeof(Boolean);
      return type;
    }
    if (OpenXmlTypesToSystemTypes.TryGetValue(openXmlType, out var info))
    {
      return info.targetType;
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
    if (openXmlType == null)
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
    if (openXmlType.IsSubclassOf(typeof(DXO10W.OnOffType)))
    {
      var result = OnOffTypeToBoolean(openXmlValue);
      return result;
    }
    if (OpenXmlTypesToSystemTypes.TryGetValue(openXmlType, out var info))
    {
      var targetValue = info.toSystemValueMethod(openXmlValue);
      return targetValue;
    }
    return openXmlValue;
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
    if (openXmlType == null)
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
    if (openXmlType.IsSubclassOf(typeof(DXO10W.OnOffType)))
    {
      MethodInfo methodInfo = typeof(TypeConversionTools).GetMethod("BooleanToOnOffType", BindingFlags.Static | BindingFlags.NonPublic)!;
      MethodInfo genericMethod = methodInfo.MakeGenericMethod(openXmlType);
      var result = genericMethod.Invoke(null, [systemValue]);
      return result;
    }
    if (OpenXmlTypesToSystemTypes.TryGetValue(openXmlType, out var info))
    {
      var targetValue = info.toOpenXmlValueMethod(systemValue);
      return targetValue;
    }
    return systemValue;
  }

  private static readonly Dictionary<Type, (Type targetType, Func<object?, object?> toSystemValueMethod, Func<object?, object?> toOpenXmlValueMethod)> OpenXmlTypesToSystemTypes = new()
  {
    { typeof(DX.BooleanValue), (typeof(Boolean), BooleanValueToBoolean, BooleanToBooleanValue) },
    { typeof(DX.ByteValue), (typeof(Byte), ByteValueToByte, ByteToByteValue) },
    { typeof(DX.DateTimeValue), (typeof(DateTime), DateTimeValueToDateTime, DateTimeToDateTimeValue) },
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
    { typeof(DXO10W.OnOffValues), (typeof(Boolean?), OnOffValuesToBoolean, BooleanToOnOffValues) },
    { typeof(DX.HexBinaryValue), (typeof(String), HexBinaryValueToString, StringToHexBinaryValue) },
    { typeof(DX.Base64BinaryValue), (typeof(String), Base64BinaryValueToString, StringToBase64BinaryValue) },

  };

  private static object? BooleanValueToBoolean(object? value)
  {
    if (value is null)
      return null;
    if (value is DX.BooleanValue booleanValue)
    {
      return booleanValue.Value;
    }
    throw new ArgumentException("Value is not a BooleanValue", nameof(value));
  }

  private static object? BooleanToBooleanValue(object? value)
  {
    if (value is null)
      return null;
    if (value is Boolean booleanValue)
    {
      return new DX.BooleanValue(booleanValue);
    }
    throw new ArgumentException("Value is not a Boolean", nameof(value));
  }

  private static object? ByteValueToByte(object? value)
  {
    if (value is DX.ByteValue booleanValue)
    {
      return booleanValue.Value;
    }
    throw new ArgumentException("Value is not a ByteValue", nameof(value));
  }

  private static object? ByteToByteValue(object? value)
  {
    if (value is Byte byteValue)
    {
      return new DX.ByteValue(byteValue);
    }
    throw new ArgumentException("Value is not a Byte", nameof(value));
  }

  private static object? DateTimeValueToDateTime(object? value)
  {
    if (value is DX.DateTimeValue dateTimeValue)
    {
      return dateTimeValue.Value;
    }
    throw new ArgumentException("Value is not a DateTimeValue", nameof(value));
  }

  private static object? DateTimeToDateTimeValue(object? value)
  {
    if (value is DateTime dateTimeValue)
    {
      return new DX.DateTimeValue(dateTimeValue);
    }
    throw new ArgumentException("Value is not a DateTime", nameof(value));
  }

  private static object? DecimalValueToDecimal(object? value)
  {
    if (value is DX.DecimalValue decimalValue)
    {
      return decimalValue.Value;
    }
    throw new ArgumentException("Value is not a DecimalValue", nameof(value));
  }

  private static object? DecimalToDecimalValue(object? value)
  {
    if (value is Decimal decimalValue)
    {
      return new DX.DecimalValue(decimalValue);
    }
    throw new ArgumentException("Value is not a Decimal", nameof(value));
  }

  private static object? DoubleValueToDouble(object? value)
  {
    if (value is DX.DoubleValue doubleValue)
    {
      return doubleValue.Value;
    }
    throw new ArgumentException("Value is not a DoubleValue", nameof(value));
  }

  private static object? DoubleToDoubleValue(object? value)
  {
    if (value is Double doubleValue)
    {
      return new DX.DoubleValue(doubleValue);
    }
    throw new ArgumentException("Value is not a Double", nameof(value));
  }

  private static object? Int16ValueToInt16(object? value)
  {
    if (value is DX.Int16Value int16Value)
    {
      return int16Value.Value;
    }
    throw new ArgumentException("Value is not a Int16Value", nameof(value));
  }

  private static object? Int16ToInt16Value(object? value)
  {
    if (value is Int16 int16Value)
    {
      return new DX.Int16Value(int16Value);
    }
    throw new ArgumentException("Value is not a Int16", nameof(value));
  }

  private static object? Int32ValueToInt32(object? value)
  {
    if (value is DX.Int32Value int32Value)
    {
      return int32Value.Value;
    }
    throw new ArgumentException("Value is not a Int32Value", nameof(value));
  }

  private static object? Int32ToInt32Value(object? value)
  {
    if (value is Int32 int32Value)
    {
      return new DX.Int32Value(int32Value);
    }
    throw new ArgumentException("Value is not a Int32", nameof(value));
  }

  private static object? Int64ValueToInt64(object? value)
  {
    if (value is DX.Int64Value int64Value)
    {
      return int64Value.Value;
    }
    throw new ArgumentException("Value is not a Int64Value", nameof(value));
  }

  private static object? Int64ToInt64Value(object? value)
  {
    if (value is Int64 int64Value)
    {
      return new DX.Int64Value(int64Value);
    }
    throw new ArgumentException("Value is not a Int64", nameof(value));
  }

  private static object? StringValueToString(object? value)
  {
    if (value is DX.StringValue stringValue)
    {
      return stringValue.Value!;
    }
    throw new ArgumentException("Value is not a StringValue", nameof(value));
  }

  private static object? StringToStringValue(object? value)
  {
    if (value is String stringValue)
    {
      return new DX.StringValue(stringValue);
    }
    throw new ArgumentException("Value is not a String", nameof(value));
  }

  private static object? UInt16ValueToUInt16(object? value)
  {
    if (value is DX.UInt16Value uint16Value)
    {
      return uint16Value.Value;
    }
    throw new ArgumentException("Value is not a UInt16Value", nameof(value));
  }

  private static object? UInt16ToUInt16Value(object? value)
  {
    if (value is UInt16 uint16Value)
    {
      return new DX.UInt16Value(uint16Value);
    }
    throw new ArgumentException("Value is not a UInt16", nameof(value));
  }

  private static object? UInt32ValueToUInt32(object? value)
  {
    if (value is DX.UInt32Value uint32Value)
    {
      return uint32Value.Value;
    }
    throw new ArgumentException("Value is not a UInt32Value", nameof(value));
  }

  private static object? UInt32ToUInt32Value(object? value)
  {
    if (value is UInt32 uint32Value)
    {
      return new DX.UInt32Value(uint32Value);
    }
    throw new ArgumentException("Value is not a UInt32", nameof(value));
  }

  private static object? UInt64ValueToUInt64(object? value)
  {
    if (value is DX.UInt64Value uint64Value)
    {
      return uint64Value.Value;
    }
    throw new ArgumentException("Value is not a UInt64Value", nameof(value));
  }

  private static object? UInt64ToUInt64Value(object? value)
  {
    if (value is UInt64 uint64Value)
    {
      return new DX.UInt64Value(uint64Value);
    }
    throw new ArgumentException("Value is not a UInt64", nameof(value));
  }

  private static object? OnOffValueToBoolean(object? value)
  {
    if (value is DX.OnOffValue onOffValue)
    {
      return onOffValue.Value;
    }
    throw new ArgumentException("Value is not a OnOffValue", nameof(value));
  }

  private static object? BooleanToOnOffValue(object? value)
  {
    if (value is Boolean booleanValue)
    {
      return new DX.OnOffValue(booleanValue);
    }
    throw new ArgumentException("Value is not a Boolean", nameof(value));
  }

  private static object? OnOffTypeToBoolean(object? value)
  {
    if (value is DXO10W.OnOffType onOffType)
    {
      var val = onOffType.Val?.Value;
      if (val == DXO10W.OnOffValues.One || val == DXO10W.OnOffValues.True)
        return true;
      if (val == DXO10W.OnOffValues.Zero || val == DXO10W.OnOffValues.False)
        return false;
      return null;
    }
    throw new ArgumentException("Value is not a OnOffType", nameof(value));
  }

  private static object? BooleanToOnOffType<T>(object? value) where T : DXO10W.OnOffType, new()
  {
    if (value is Boolean booleanValue)
    {
      return new T { Val = (DXO10W.OnOffValues)BooleanToOnOffValues(booleanValue) };
    }
    throw new ArgumentException("Value is not a Boolean", nameof(value));
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
    throw new ArgumentException("Value is not a OnOffValues", nameof(value));
  }

  private static object BooleanToOnOffValues(object? value)
  {
    if (value is Boolean booleanValue)
    {
      if (booleanValue == true)
        return DXO10W.OnOffValues.One;
      if (booleanValue == false)
        return DXO10W.OnOffValues.Zero;
    }
    throw new ArgumentException("Value is not a Boolean", nameof(value));
  }

  private static object? HexBinaryValueToString(object? value)
  {
    if (value is DX.HexBinaryValue hexBinaryValue)
    {
      return hexBinaryValue.Value;
    }
    throw new ArgumentException("Value is not a HexBinaryValue", nameof(value));
  }

  private static object? StringToHexBinaryValue(object? value)
  {
    if (value is String stringValue)
    {
      return new DX.HexBinaryValue(stringValue);
    }
    throw new ArgumentException("Value is not a String", nameof(value));
  }

  private static object? Base64BinaryValueToString(object? value)
  {
    if (value is DX.Base64BinaryValue base64BinaryValue)
    {
      return base64BinaryValue.Value;
    }
    throw new ArgumentException("Value is not a Base64BinaryValue", nameof(value));
  }

  private static object? StringToBase64BinaryValue(object? value)
  {
    if (value is String stringValue)
    {
      return new DX.Base64BinaryValue(stringValue);
    }
    throw new ArgumentException("Value is not a String", nameof(value));
  }
}