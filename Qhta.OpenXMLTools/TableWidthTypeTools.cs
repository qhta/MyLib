using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with table width types in OpenXml documents.
/// </summary>
public static class TableWidthTypeTools
{
  /// <summary>
  /// Gets the value of the table width.
  /// </summary>
  /// <param name="tableWidth"></param>
  /// <returns></returns>
  public static int? GetValue(this TableWidthType tableWidth)
  {
    if (int.TryParse(tableWidth.Width?.Value, out var val))
      return val;
    return null;
  }

  /// <summary>
  /// Sets the value of the table width.
  /// </summary>
  /// <param name="tableWidth"></param>
  /// <param name="value"></param>
  public static void SetValue(this TableWidthType tableWidth, int? value)
  {
    if (value.HasValue)
      tableWidth.Width = value.Value.ToString();
    else
      tableWidth.Width = null;
  }
}
