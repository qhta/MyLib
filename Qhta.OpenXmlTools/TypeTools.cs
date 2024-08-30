using System;

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
    if (OpenXmlTypesToSystemTypes.TryGetValue(openXmlType, out var info))
    {
      return info.targetType;
    }
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
    if (openXmlType == null || openXmlType==typeof(object))
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
      { typeof(DXW.LongHexNumberType), (typeof(HexInt), LongHexNumberTypeToHexInt, HexIntToLongHexNumberType, UpdateLongHexNumberType) }
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
      return val;
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
      if (booleanValue!=null)
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
      if (booleanValue!=null)
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

  /// <summary>
  /// Check whether the OpenXml type can contain members.
  /// </summary>
  /// <param name="openXmlType"></param>
  /// <returns></returns>
  public static bool IsContainer(this Type openXmlType)
  {
    return MemberTypes.ContainsKey(openXmlType);
  }

  /// <summary>
  /// Gets the acceptable member types of the OpenXml type.
  /// </summary>
  /// <param name="openXmlType"></param>
  /// <returns></returns>
  public static Type[] GetMemberTypes(this Type openXmlType)
  {
    if (MemberTypes.TryGetValue(openXmlType, out var types))
    {
      return types;
    }
    return [];
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