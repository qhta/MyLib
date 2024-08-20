using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Methods to help with testing.
/// </summary>
public static class TestTools
{

  /// <summary>
  /// Creates a new property value.
  /// </summary>
  /// <param name="propertyName"></param>
  /// <param name="propertyType"></param>
  /// <returns></returns>
  public static object? CreateNewPropertyValue(string propertyName, Type propertyType)
  {
    if (propertyType.Name.StartsWith("Nullable"))
      propertyType = propertyType.GenericTypeArguments[0];
    if (propertyName == "ApplicationVersion")
      return "1.0";
    if (propertyType == typeof(string))
      return propertyName + "_string";
    if (propertyType == typeof(DateTime))
      return DateTime.Now;
    if (propertyType == typeof(int))
      return 100_000;
    if (propertyType == typeof(bool))
      return true;
    if (propertyType == typeof(decimal))
      return 1.2m;
    if (propertyType == typeof(Twips))
      return new Twips(144000);
    if (propertyType == typeof(DXM.BooleanValues))
      return DXM.BooleanValues.On;
    if (propertyType.GetInterface("IEnumValue") != null)
      return propertyType.GetProperties(BindingFlags.Public | BindingFlags.Static).ToArray()[1].GetValue(null);
    return null;
  }

  /// <summary>
  /// Converts the object to a string.
  /// </summary>
  /// <param name="value">value to convert</param>
  /// <param name="indent">indent size (spaces = indent*2) to put before</param>
  /// <param name="depthLimit">limit of internal elements levels (in composite elements</param>
  /// <returns></returns>
  public static string AsString(this object? value, int indent = 0, int depthLimit = int.MaxValue)
  {
    if (value is Twips twips)
      return twips.Value.ToString();
    if (value is HexInt hexInt)
      return hexInt.Value.ToString("X8");
    if (value is DXW.Rsids rsids)
      return AsString(rsids, indent, depthLimit);
    if (value is DX.IEnumValue enumValue)
      return enumValue.Value;
    if (value is DX.OpenXmlLeafElement leafElement)
      return AsString(leafElement, indent, depthLimit);
    if (value is DX.OpenXmlCompositeElement compositeElement)
      return AsString(compositeElement, indent, depthLimit);
    if (value is string[] strArray)
      return "[" + string.Join(", ", strArray) + "]";
    if (value is object[] objArray)
      return "{" + string.Join(", ", objArray.Select(item => item.AsString())) + "}";
    if (value is null)
      return string.Empty;
    if (value is string str)
      return str;
    if (value is DateTime dateTime)
      return dateTime.ToString("yyyy-MM-dd HH:mm:ss");
    if (value is Boolean vBoolean)
      return vBoolean.ToString().ToLowerInvariant();

    var valueType = value.GetType();
    if (IntegerTypes.Contains(valueType))
      return value.ToString();
    if (DecimalTypes.Contains(valueType))
      return Convert.ToDecimal(value).ToString("F", CultureInfo.InvariantCulture);
    var properties = valueType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
    if (properties.Length > 0)
    {
      var indentStr = new string(' ', indent * 2);
      var propValuesList = new List<string>();
      foreach (var prop in properties)
      {
        if (prop.GetIndexParameters().Length > 0)
          continue;
        if (value==null)
          continue;
        try
        {
          var propValue = prop.GetValue(value);
          if (propValue != null)
          {
            var propValStr = propValue.AsString(indent + 1, depthLimit - 1);
            if (propValStr.Contains("\n"))
              propValuesList.Add($"{indentStr}{prop.Name}:\r\n{propValStr}");
            else
              propValuesList.Add($"{indentStr}{prop.Name}: {propValStr}");
          }
        }
#pragma warning disable CS0168 // Variable is declared but never used
        catch (Exception ex)
#pragma warning restore CS0168 // Variable is declared but never used
        {
          continue;
        }

      }
      var indentStr1 = indent > 1 ? new string(' ', (indent - 1) * 2) : string.Empty;
      return $"{valueType.Name}\r\n{{\r\n{string.Join("\r\n", propValuesList)}\r\n{indentStr1}}}";
    }
    if (valueType.IsEnum && valueType.GetCustomAttributes(typeof(FlagsAttribute), false).Any())
    {
      var enumValues = Enum.GetValues(valueType);
      var selectedValues = new List<string>();
      foreach (var enumValue1 in enumValues)
      {
        if ((Convert.ToInt32(value) & Convert.ToInt32(enumValue1)) != 0)
          selectedValues.Add(enumValue1.ToString());
      }
      return string.Join("+", selectedValues);
    }
    return value?.ToString() ?? string.Empty;
  }

  /// <summary>
  /// Converts the <c>OpenXmlLeafElement</c> to a string.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="indent"></param>
  /// <param name="depthLimit">limit of internal elements levels (in composite elements</param>
  /// <returns></returns>
  public static string AsString(this DX.OpenXmlLeafElement element, int indent = 0, int depthLimit = int.MaxValue)
  {
    var indentStr = new string(' ', indent * 2);
    if (element.HasAttributes)
    {
      var sl = new List<string>();
      foreach (var attr in element.GetAttributes())
        sl.Add($"{attr.LocalName}=\"{attr.Value}\"");
      if (element.InnerText.Trim() != "")
        return $"{indentStr}<{element.Prefix}:{element.LocalName} {string.Join(" ", sl)}>{element.InnerText}</{element.Prefix}:{element.LocalName}>";
      return $"{indentStr}<{element.Prefix}:{element.LocalName} {string.Join(" ", sl)} />";
    }
    else
    {
      if (element.InnerText.Trim() != "")
        return $"{indentStr}<{element.Prefix}:{element.LocalName}>{element.InnerText}</{element.Prefix}:{element.LocalName}>";
      return $"{indentStr}<{element.Prefix}:{element.LocalName}/>";
    }
  }

  /// <summary>
  /// Converts the <c>OpenXmlCompositeElement</c> to a string.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="indent"></param>
  /// <param name="depthLimit">limit of internal elements levels (in composite elements</param>
  /// <returns></returns>
  public static string AsString(this DX.OpenXmlCompositeElement element, int indent = 0, int depthLimit = int.MaxValue)
  {
    if (element is DXVT.VTVector vector && depthLimit == 0)
      return String.Join(", ", vector.ChildElements.Select(item => item.AsString()));
    var indentStr = new string(' ', indent * 2);
    if (element.HasAttributes)
    {
      var sl = new List<string>();
      foreach (var attr in element.GetAttributes())
        sl.Add($"{attr.LocalName}=\"{attr.Value}\"");
      if (depthLimit <= 0)
        return $"{indentStr}<{element.Prefix}:{element.LocalName} {string.Join(" ", sl)} >...";
      if (element.HasChildren)
      {
        var cl = new List<string>();
        foreach (var child in element.Elements())
          cl.Add(child.AsString(indent + 1, depthLimit - 1));
        return $"{indentStr}<{element.Prefix}:{element.LocalName} {string.Join(" ", sl)}>\r\n{string.Join("\r\n", cl)}\r\n{indentStr}</{element.Prefix}:{element.LocalName}>";
      }
      return $"{indentStr}<{element.Prefix}:{element.LocalName} {string.Join(" ", sl)} />";
    }
    if (element.HasChildren)
    {
      if (depthLimit <= 0)
        return $"{indentStr}<{element.Prefix}:{element.LocalName} >...";
      var cl = new List<string>();
      foreach (var child in element.Elements())
        cl.Add(child.AsString(indent + 1, depthLimit - 1));
      return $"{indentStr}<{element.Prefix}:{element.LocalName}>\r\n{string.Join("\r\n", cl)}\r\n{indentStr}</{element.Prefix}:{element.LocalName}>";
    }
    return $"{indentStr}<{element.Prefix}:{element.LocalName} />";
  }

  /// <summary>
  /// Converts the <c>Rsids</c> to a string.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="indent"></param>
  /// <param name="depthLimit">limit of internal elements levels (in composite elements</param>
  /// <returns></returns>
  public static string AsString(this DXW.Rsids element, int indent = 0, int depthLimit = int.MaxValue)
  {
    var items = element.ToArray();
    var str = string.Join(", ", items);
    if (items.Length < element.Count())
      str += ", ...";
    return "{" + str + "}";
  }

  /// <summary>
  /// Converts the object form a string.
  /// </summary>
  /// <param name="value">value to convert</param>
  /// <param name="targetType"></param>
  /// <returns></returns>
  public static object? FromString(this string? value, Type targetType)
  {
    if (value == null)
      return null;
    value = value.Trim();
    if (targetType == typeof(Twips))
      return new Twips(value);
    if (targetType == typeof(HexInt))
      return new HexInt(value);


    if (targetType == typeof(DXW.Rsids))
    {
      var rsids = new DXW.Rsids();
      value = value.Trim('{', '}').Trim();
      var items = value.Split([','], StringSplitOptions.RemoveEmptyEntries);
      for (int i = 0; i < items.Length; i++)
      {
        string item = items[i].Trim();
        if (i == 0)
          rsids.RsidRoot = new DXW.RsidRoot { Val = new DX.HexBinaryValue(item) };
        else
          rsids.Append(new DXW.Rsid { Val = new DX.HexBinaryValue(item) });
      };
    }
    if (targetType.ImplementsInterfaces(typeof(DX.IEnumValue)))
    {
      Type genericTypeDefinition = typeof(DX.IEnumValueFactory<>);

      Type[] typeArguments = { targetType };
      Type constructedType = genericTypeDefinition.MakeGenericType(typeArguments);
      var createMethod = constructedType.GetMethod("Create");
      var enumValue = createMethod!.Invoke(null, [value]);
      return enumValue;
    }
    if (targetType.ImplementsInterfaces(typeof(DX.OpenXmlLeafElement)))
    {
      var leafElement = (DX.OpenXmlLeafElement)Activator.CreateInstance(targetType);
      var valProperty = targetType.GetProperty("Val");
      if (valProperty != null)
        valProperty.SetValue(leafElement, value.FromString(valProperty.PropertyType));
    }
    if (targetType.ImplementsInterfaces(typeof(DX.OpenXmlCompositeElement)))
      throw new NotSupportedException($"{targetType} not supported in FromString");
    if (targetType == typeof(string[]))
    {
      value = value.Trim('{', '}').Trim();
      var items = value.Split([','], StringSplitOptions.RemoveEmptyEntries);
      var strArray = new string[items.Length];
      for (int i = 0; i < items.Length; i++)
      {
        string item = items[i].Trim();
        strArray[i] = item;
      }
      return strArray;
    }
    if (targetType == typeof(object[]))
    {
      value = value.Trim('{', '}').Trim();
      var items = value.Split([','], StringSplitOptions.RemoveEmptyEntries);
      var objArray = new object[items.Length];
      for (int i = 0; i < items.Length; i++)
      {
        string item = items[i].Trim();
        objArray[i] = item;
      }
      return objArray;
    }
    if (targetType == typeof(string))
      return value;
    if (targetType == typeof(DateTime))
      return DateTime.Parse(value);
    if (targetType == typeof(Boolean))
      switch (value.ToLowerInvariant())
      {
        case "true":
        case "on":
        case "yes":
        case "1":
          return true;
        case "false":
        case "off":
        case "no":
        case "0":
          return false;

        default:
          throw new FormatException($"Cannot convert {value} to Boolean");
      }
    if (IntegerTypes.Contains(targetType))
      return Convert.ChangeType(value, targetType);
    if (DecimalTypes.Contains(targetType))
    {
      var decimalValue = Decimal.Parse(value, CultureInfo.InvariantCulture);
      return Convert.ChangeType(decimalValue, targetType);
    }
    return value;

  }

  private static readonly List<Type> IntegerTypes =
  [
    typeof(byte), typeof(sbyte), typeof(short), typeof(ushort), typeof(int), typeof(uint), typeof(long), typeof(ulong)
  ];

  private static readonly List<Type> DecimalTypes =
  [
    typeof(float), typeof(double), typeof(decimal)
  ];

  private static bool ImplementsInterfaces(this Type type, Type interfaceType) => Array.Exists(type.GetInterfaces(), t => t == interfaceType);
}
