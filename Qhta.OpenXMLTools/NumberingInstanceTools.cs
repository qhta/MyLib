namespace Qhta.OpenXmlTools;

/// <summary>
/// A collection of tools for working with NumberingInstance elements.
/// </summary>
public static class NumberingInstanceTools
{
  /// <summary>
  /// Get the numbering ID of the numbering instance.
  /// </summary>
  /// <param name="numberingInstance"></param>
  /// <returns></returns>
  public static int? GetNumberingId(this DXW.NumberingInstance numberingInstance)
  {
    var numberingId = numberingInstance.GetFirstChild<DXW.NumberingId>();
    if (numberingId == null)
      return null;

    return numberingId.Val?.Value;
  }

  /// <summary>
  /// Set the numbering ID of the numbering instance.
  /// If value to set is null, the element will be removed.
  /// </summary>
  /// <param name="numberingInstance"></param>
  /// <param name="numberingId"></param>
  public static void SetNumberingId(this DXW.NumberingInstance numberingInstance, int? numberingId)
  {
    var numberingIdElement = numberingInstance.GetFirstChild<DXW.NumberingId>();
    if (numberingId == null)
    {
      if (numberingIdElement != null)
        numberingIdElement.Remove();
      return;
    }
    if (numberingIdElement == null)
    {
      numberingIdElement = new DXW.NumberingId();
      numberingInstance.AppendChild(numberingIdElement);
    }
    numberingIdElement.Val = numberingId;
  }

  /// <summary>
  /// Get the abstract number ID of the numbering instance.
  /// </summary>
  /// <param name="numberingInstance"></param>
  /// <returns></returns>
  public static int? GetAbstractNumId(this DXW.NumberingInstance numberingInstance)
  {
    var abstractNumId = numberingInstance.GetFirstChild<DXW.AbstractNumId>();
    if (abstractNumId == null)
      return null;

    return abstractNumId.Val?.Value;
  }

  /// <summary>
  /// Set the abstract number ID of the numbering instance.
  /// If value to set is null, the element will be removed.
  /// </summary>
  /// <param name="numberingInstance"></param>
  /// <param name="abstractNumId"></param>
  public static void SetAbstractNumId(this DXW.NumberingInstance numberingInstance, int? abstractNumId)
  {
    var abstractNumIdElement = numberingInstance.GetFirstChild<DXW.AbstractNumId>();
    if (abstractNumId == null)
    {
      if (abstractNumIdElement != null)
        abstractNumIdElement.Remove();
      return;
    }
    if (abstractNumIdElement == null)
    {
      abstractNumIdElement = new DXW.AbstractNumId();
      numberingInstance.AppendChild(abstractNumIdElement);
    }
    abstractNumIdElement.Val = abstractNumId;
  }


}