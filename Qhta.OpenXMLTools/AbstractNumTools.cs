namespace Qhta.OpenXmlTools;

/// <summary>
/// A collection of tools for working with AbstractNum elements.
/// </summary>
public static class AbstractNumTools
{
  /// <summary>
  /// Get the abstract number ID of the abstract number.
  /// </summary>
  /// <param name="abstractNum"></param>
  /// <returns></returns>
  public static int? GetAbstractNumId(this DXW.AbstractNum abstractNum)
  {
    var abstractNumId = abstractNum.GetFirstChild<DXW.AbstractNumId>();
    if (abstractNumId == null)
      return null;

    return abstractNumId.Val?.Value;
  }

  /// <summary>
  /// Set the abstract number ID of the abstract number.
  /// If value to set is null, the element will be removed.
  /// </summary>
  /// <param name="abstractNum"></param>
  /// <param name="abstractNumId"></param>
  public static void SetAbstractNumId(this DXW.AbstractNum abstractNum, int? abstractNumId)
  {
    var abstractNumIdElement = abstractNum.GetFirstChild<DXW.AbstractNumId>();
    if (abstractNumId == null)
    {
      if (abstractNumIdElement != null)
        abstractNumIdElement.Remove();
      return;
    }
    if (abstractNumIdElement == null)
    {
      abstractNumIdElement = new DXW.AbstractNumId();
      abstractNum.AppendChild(abstractNumIdElement);
    }
    abstractNumIdElement.Val = abstractNumId;
  }

  /// <summary>
  /// Get the multi-level type of the abstract number.
  /// </summary>
  /// <param name="abstractNum"></param>
  /// <returns></returns>
  public static DXW.MultiLevelValues? GetMultiLevelType(this DXW.AbstractNum abstractNum)
  {
    var multiLevelType = abstractNum.GetFirstChild<DXW.MultiLevelType>();
    if (multiLevelType == null)
      return null;

    return multiLevelType.Val?.Value;
  }

  /// <summary>
  /// Set the multi-level type of the abstract number.
  /// If value to set is null, the element will be removed.
  /// </summary>
  /// <param name="abstractNum"></param>
  /// <param name="multiLevelType"></param>
  public static void SetMultiLevelType(this DXW.AbstractNum abstractNum, DXW.MultiLevelValues? multiLevelType)
  {
    var multiLevelTypeElement = abstractNum.GetFirstChild<DXW.MultiLevelType>();
    if (multiLevelType == null)
    {
      if (multiLevelTypeElement != null)
        multiLevelTypeElement.Remove();
      return;
    }
    if (multiLevelTypeElement == null)
    {

      multiLevelTypeElement = new DXW.MultiLevelType();
      abstractNum.AppendChild(multiLevelTypeElement);
    }
    multiLevelTypeElement.Val = multiLevelType;
  }
}