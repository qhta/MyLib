namespace Qhta.OpenXmlTools;

/// <summary>
/// A list of OpenXmlElements that represent formatted text objects.
/// </summary>
public class FormattedTextObjects : List<DX.OpenXmlElement>
{
  /// <summary>
  /// Default constructor.
  /// </summary>
  public FormattedTextObjects() { }

  /// <summary>
  /// Constructor that takes a list of OpenXmlElements.
  /// </summary>
  /// <param name="elements"></param>
  public FormattedTextObjects(params DX.OpenXmlElement[] elements)
  {
    AddRange(elements);
  }

  /// <summary>
  /// Gets the OpenXmlElement that represents the specified character.
  /// It searches the list of OpenXmlElements for an element that represents the specified character as defined in TextOptions PlainText.
  /// </summary>
  /// <param name="ch"></param>
  /// <param name="index"></param>
  /// <returns></returns>
  public DX.OpenXmlElement? GetObject(char ch, int index)
  {
    return null;
  }
}