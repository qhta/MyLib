namespace Qhta.OpenXmlToolsTest;
public static class TestTools
{
  /// <summary>
  /// Converts the object to a string.
  /// </summary>
  /// <param name="value"></param>
  /// <param name="indent"></param>
  /// <returns></returns>
  public static string AsString(this object? value, int indent =0)
  {
    if (value is DXW.Rsids rsids)
      return AsString(rsids, indent);
    if (value is DX.OpenXmlLeafElement leafElement)
      return AsString(leafElement, indent);
    if (value is DX.OpenXmlCompositeElement compositeElement)
      return AsString(compositeElement, indent);
    if (value is string[] strArray)
      return "[" + string.Join(", ", strArray) +"]";
    if (value is object[] objArray)
      return "{" + string.Join(", ", objArray.Select(item=>item.AsString())) + "}";
    if (value is null)
      return string.Empty;
    if (value is string str)
      return "\""+str+"\"";
    return value?.ToString() ?? string.Empty;
  }

  /// <summary>
  /// Converts the <c>OpenXmlLeafElement</c> to a string.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="indent"></param> 
  /// <returns></returns>
  public static string AsString(this DX.OpenXmlLeafElement element, int indent = 0)
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
  /// <returns></returns>
  public static string AsString(this DX.OpenXmlCompositeElement element, int indent = 0)
  {
    var indentStr = new string(' ', indent * 2);
    if (element.HasAttributes)
    {
      var sl = new List<string>();
      foreach (var attr in element.GetAttributes())
        sl.Add($"{attr.LocalName}=\"{attr.Value}\"");

      if (element.HasChildren)
      {
        var cl = new List<string>();
        foreach (var child in element.Elements())
          cl.Add(child.AsString(indent+1));
        return $"{indentStr}<{element.Prefix}:{element.LocalName} {string.Join(" ", sl)}>\r\n{string.Join("\r\n", cl)}\r\n{indentStr}</{element.Prefix}:{element.LocalName}>";
      }
      else
      {
        return $"{indentStr}<{element.Prefix}:{element.LocalName} {string.Join(" ", sl)} />";
      }
    }
    else
    {
      if (element.HasChildren)
      {
        var cl = new List<string>();
        foreach (var child in element.Elements())
          cl.Add(child.AsString(indent + 1));
        return $"{indentStr}<{element.Prefix}:{element.LocalName}>\r\n{string.Join("\r\n", cl)}\r\n{indentStr}</{element.Prefix}:{element.LocalName}>";
      }
      else
      {
        return $"{indentStr}<{element.Prefix}:{element.LocalName} />";
      }
    }
  }

  /// <summary>
  /// Converts the <c>Rsids</c> to a string.
  /// </summary>
  /// <param name="element"></param>
  /// <param name="indent"></param> 
  /// <returns></returns>
  public static string AsString(this DXW.Rsids element, int indent = 0)
  {
    return "{"+string.Join(", ", element.ToArray())+"}";
  }
}
