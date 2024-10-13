namespace Qhta.OpenXmlTools;

using DocumentFormat.OpenXml.Wordprocessing;

/// <summary>
/// Tools for changing section properties.
/// </summary>
public static class SectionPropertiesChangeTools
{

  /// <summary>
  /// Get the first <c>PreviousSectionProperties</c> element from the section properties change.
  /// </summary>
  /// <param name="sectionPropertiesChange"></param>
  /// <returns></returns>
  public static PreviousSectionProperties? GetPreviousSectionProperties(this SectionPropertiesChange sectionPropertiesChange)
  {
    return sectionPropertiesChange.Elements<PreviousSectionProperties>().FirstOrDefault();
  }

  /// <summary>
  /// Set <c>PreviousSectionProperties</c> element in the section properties change.
  /// </summary>
  /// <param name="sectionPropertiesChange"></param>
  /// <param name="value"></param>
  public static void SetPreviousSectionProperties(this SectionPropertiesChange sectionPropertiesChange, PreviousSectionProperties? value)
  {
    sectionPropertiesChange.SetFirstElement(value);
  }
}
