namespace Qhta.OpenXmlTools;

/// <summary>
/// Contains tools for working with TableCellMarginDefault elements.
/// </summary>
public static class TableCellMarginDefaultTools
{
  /// <summary>
  /// Sets the default margins for the table cell.
  /// If the value is null, the margin is not set.
  /// </summary>
  /// <param name="margin"></param>
  /// <param name="top"></param>
  /// <param name="left"></param>
  /// <param name="bottom"></param>
  /// <param name="right"></param>
  public static void SetMargins(this DXW.TableCellMarginDefault margin, int? left, int? top, int? right, int? bottom)
  {
    if (left == null && top == null && right == null && bottom == null)
    {
      margin.Remove();
      return;
    }
    margin.TableCellLeftMargin = left == null
      ? null
      : new DXW.TableCellLeftMargin
      {
        Width = new DX.Int16Value((short)left), Type = new DX.EnumValue<DXW.TableWidthValues>(DXW.TableWidthValues.Dxa)
      };
    margin.TopMargin = left == null
      ? null
      : new DXW.TopMargin
      {
        Width = top.ToString(),
        Type = new DX.EnumValue<DXW.TableWidthUnitValues>(DXW.TableWidthUnitValues.Dxa)
      };
    margin.TableCellRightMargin = right == null
      ? null
      : new DXW.TableCellRightMargin
      {
        Width = new DX.Int16Value((short)right),
        Type = new DX.EnumValue<DXW.TableWidthValues>(DXW.TableWidthValues.Dxa)
      };
    margin.BottomMargin = left == null
      ? null
      : new DXW.BottomMargin
      {
        Width = bottom.ToString(),
        Type = new DX.EnumValue<DXW.TableWidthUnitValues>(DXW.TableWidthUnitValues.Dxa)
      };
  }
}