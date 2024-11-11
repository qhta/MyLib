using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with table width types in OpenXml documents.
/// </summary>
public static class TableWidthDxaNilTypeTools
{
  /// <summary>
  /// Gets the value of the table width.
  /// </summary>
  /// <param name="tableWidth"></param>
  /// <returns></returns>
  public static int? GetValue(this TableWidthDxaNilType tableWidth)
  {
    return tableWidth.Width?.Value;
  }

  /// <summary>
  /// Sets the value of the table width.
  /// </summary>
  /// <param name="tableWidth"></param>
  /// <param name="value"></param>
  public static void SetValue(this TableWidthDxaNilType tableWidth, int? value)
  {
    if (value.HasValue)
      tableWidth.Width = new DX.Int16Value((short)value);
    else
      tableWidth.Width = null;
  }
}
