namespace Qhta.OpenXmlTools;

/// <summary>
/// A collection of tools for working with <c>NumberingProperties</c> element.
/// </summary>
public static class NumberingPropertiesTools
{
  /// <summary>
  /// Get the numbering level reference of the numbering properties.
  /// </summary>
  /// <param name="numberingProperties"></param>
  /// <returns></returns>
  public static int? GetNumberingLevelReference(this DXW.NumberingProperties numberingProperties)
  {
    var numberingLevelReference = numberingProperties.GetFirstChild<DXW.NumberingLevelReference>();
    if (numberingLevelReference == null)
      return null;

    return numberingLevelReference.Val?.Value;
  }

  /// <summary>
  /// Set the numbering level reference  of the numbering properties.
  /// If value to set is null, the element will be removed.
  /// </summary>
  /// <param name="numberingProperties"></param>
  /// <param name="level"></param>
  public static void SetNumberingLevelReference(this DXW.NumberingProperties numberingProperties, int? level)
  {
    var numberingLevelReference = numberingProperties.GetFirstChild<DXW.NumberingLevelReference>();
    if (level == null)
    {
      if (numberingLevelReference != null)
        numberingLevelReference.Remove();
      return;
    }
    if (numberingLevelReference == null)
    {
      numberingLevelReference = new DXW.NumberingLevelReference();
      numberingProperties.AppendChild(numberingLevelReference);
    }
    numberingLevelReference.Val = level;
  }
}