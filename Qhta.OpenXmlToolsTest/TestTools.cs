namespace Qhta.OpenXmlToolsTest;
public static class TestTools
{

  public static string AsString(this object? value)
  {
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
}
