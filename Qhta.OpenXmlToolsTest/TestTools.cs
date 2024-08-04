namespace Qhta.OpenXmlToolsTest;
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
    if (value is HexInt)
      return ((HexInt)value).Value.ToString("X8");
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
      return "\"" + str + "\"";
    var properties = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
    if (properties.Length > 0)
    {
      var indentStr = new string(' ', indent * 2);
      var propValuesList = new List<string>();
      foreach (var prop in properties)
      {
        if (prop.GetIndexParameters().Length > 0)
          continue;
        var propValue = prop.GetValue(value);
        if (propValue != null)
        {
          var propValStr = propValue.AsString(indent + 1, depthLimit-1);
          if (propValStr.Contains("\n"))
            propValuesList.Add($"{indentStr}{prop.Name}:\r\n{propValStr}");
          else
            propValuesList.Add($"{indentStr}{prop.Name}: {propValStr}");
        }
      }
      return $"{value.GetType().Name}\r\n{{\r\n{string.Join("\r\n", propValuesList)}\r\n{new string(' ', (indent-1) * 2)}}}";
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
          cl.Add(child.AsString(indent + 1, depthLimit-1));
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
        cl.Add(child.AsString(indent + 1, depthLimit-1));
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

  ///// <summary>
  ///// Converts the <c>Run</c> to a string.
  ///// </summary>
  ///// <param name="element"></param>
  ///// <param name="indent"></param>
  ///// <param name="depthLimit">limit of internal elements levels (in composite elements</param>
  ///// <returns></returns>
  //public static string AsString(this DXW.Run element, int indent = 0, int depthLimit = int.MaxValue)
  //{
  //  var items = element.ToArray();
  //  var str = string.Join(", ", items);
  //  if (items.Length < element.Count())
  //    str += ", ...";
  //  return "{" + str + "}";
  //}
}
