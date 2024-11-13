using DocumentFormat.OpenXml.Office2021.Excel.RichValueRefreshIntervals;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with table row properties in OpenXml documents.
/// </summary>
public static class TableRowPropertiesTools
{
  /// <summary>
  /// Gets the row height. If the row properties do not contain a <c>TableRowHeight</c> element, a new one is created.
  /// </summary>
  /// <param name="tableRowProperties">Table row properties to examine</param>
  /// <returns></returns>
  public static TableRowHeight GetTableRowHeight(this TableRowProperties tableRowProperties)
  {
    var rowHeight = tableRowProperties.Elements<TableRowHeight>().FirstOrDefault();
    if (rowHeight == null)
    {
      rowHeight = new TableRowHeight();
      tableRowProperties.Append(rowHeight);
    }
    return rowHeight;
  }

  /// <summary>
  /// Sets the row height.
  /// </summary>
  /// <param name="tableRowProperties">Table row properties to set</param>
  /// <param name="value">value to set</param>
  /// <param name="rule">rule to set</param>
  /// <returns></returns>
  public static void SetTableRowHeight(this TableRowProperties tableRowProperties, int? value, DXW.HeightRuleValues rule)
  {
    var rowHeight = tableRowProperties.GetTableRowHeight();
    rowHeight.Val = (uint?)value;
    rowHeight.HeightType = rule;
  }

  /// <summary>
  /// Get <c>CantSplit</c> boolean attribute value
  /// </summary>
  /// <param name="tableRowProperties">Table row properties to process</param>
  /// <returns></returns>
  public static bool GetCantSplit(this DXW.TableRowProperties tableRowProperties)
  {
    var canSplit = tableRowProperties.Elements<DXW.CantSplit>().FirstOrDefault();
    if (canSplit == null)
      return false;
    return true; ;
  }

  /// <summary>
  /// Set <c>CantSplit</c> boolean attribute value
  /// </summary>
  /// <param name="tableRowProperties">Table row properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetCantSplit(this DXW.TableRowProperties tableRowProperties, bool value)
  {
    var cantSplit = tableRowProperties.Elements<DXW.CantSplit>().FirstOrDefault();
    if (cantSplit != null)
    {
      if (value)
        cantSplit.Val = null;
      else
        cantSplit.Remove();
    }
    else
    {
      if (value)
        tableRowProperties.Append(new DXW.CantSplit());
    }
  }
}