using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with grid columns in OpenXml documents.
/// </summary>
public static class GridColumnTools
{

  /// <summary>
  /// Gets the width of the table grid column (in twips).
  /// </summary>
  /// <param name="gridColumn"></param>
  /// <returns></returns>
  public static int? GetWidth(this GridColumn gridColumn) => gridColumn.Width?.Value != null ? int.Parse(gridColumn.Width.Value) : null;

  /// <summary>
  /// Sets the width of the table grid column (in twips).
  /// </summary>
  /// <param name="gridColumn"></param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetWidth(this GridColumn gridColumn, int? value) => gridColumn.Width = value.ToString();
}