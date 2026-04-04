using System;

namespace Qhta.OpenXMLTools;

/// <summary>
/// Legacy implementation of Open XML package properties, designed to be used with DocumentModel and Open XML SDK.
/// </summary>
public class CoreFileProperties: DX.OpenXmlPartRootElement, DXPP.IPackageProperties
{
  /// <summary>
  /// Initializes a new instance of the CoreFileProperties class.
  /// </summary>
  public CoreFileProperties(): base()
  {
  }

  /// <summary>
  /// Initializes a new instance of the CoreFileProperties class using the supplied OpenXmlPart.
  /// </summary>
  /// <param name="openXmlPart">The OpenXmlPart class.</param>
  public CoreFileProperties(DXPP.CoreFilePropertiesPart openXmlPart): base(openXmlPart)
  {
  }

  /// <summary>
  /// Initializes a new instance of the CoreFileProperties class using the supplied outer XML.
  /// </summary>
  /// <param name="outerXml">The outer XML of the element.</param>
  public CoreFileProperties(string outerXml): base(outerXml)
  {
  }
  /// <summary>
  /// Initializes a new instance of the CoreFileProperties class using the supplied list of child elements.
  /// </summary>
  /// <param name="childElements">All child elements.</param>
  public CoreFileProperties(IEnumerable<DX.OpenXmlElement> childElements): base(childElements)
  {
  }
  /// <summary>
  /// Initializes a new instance of the CoreFileProperties class using the supplied array of child elements.
  /// </summary>
  /// <param name="childElements">All child elements</param>
  public CoreFileProperties(params DX.OpenXmlElement[] childElements): base(childElements)
  {
  }

  /// <summary>
  /// Gets or sets the Title element of the core file properties. 
  /// </summary>
  public string? Title
  {
    get
    {
      return ChildElements.FirstOrDefault(element => element.LocalName=="Title")?.InnerText;
    }
    set
    {
      if (ChildElements.FirstOrDefault(e => e.LocalName == "Title") is DXB.Title element)
      {
        element.Text = value ?? string.Empty;
      }
      else if (value != null)
      {
        var newElement = new DXB.Title(value);
        AppendChild(newElement);
      }
    }
  }

  public string? Subject { get; set; }
  public string? Creator { get; set; }
  public string? Keywords { get; set; }
  public string? Description { get; set; }
  public string? LastModifiedBy { get; set; }
  public string? Revision { get; set; }
  public DateTime? LastPrinted { get; set; }
  public DateTime? Created { get; set; }
  public DateTime? Modified { get; set; }
  public string? Category { get; set; }
  public string? Identifier { get; set; }
  public string? ContentType { get; set; }
  public string? Language { get; set; }
  public string? Version { get; set; }
  public string? ContentStatus { get; set; }
}


///// <summary>
///// <para>Title.</para>
///// <para>This class is available in Office 2007 and above.</para>
///// <para>When the object is serialized out as xml, it's qualified name is b:Title.</para>
///// </summary>
//public class Title : OpenXmlLeafTextElement
//{
//  /// <summary>Initializes a new instance of the Title class.</summary>
//  public Title();
//  /// <summary>
//  /// Initializes a new instance of the Title class with the specified text content.
//  /// </summary>
//  /// <param name="text">Specifies the text content of the element.</param>
//  public Title(string text);
//  /// <inheritdoc />
//  public override OpenXmlElement CloneNode(bool deep);