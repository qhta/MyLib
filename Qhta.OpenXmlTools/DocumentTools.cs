namespace Qhta.OpenXmlTools;

/// <summary>
/// A collection of tools for working with OpenXml documents.
/// </summary>
public static partial class DocumentTools
{


  /// <summary>
  /// Gets the integer identifier of the document.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static int? GetDocumentId(this DXPP.WordprocessingDocument wordDoc)
  {
    var docSettings = wordDoc.GetSettings();
    return docSettings.GetDocumentId();
  }

  /// <summary>
  /// Gets all the properties of the document to manage them in a uniform way.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static DocumentProperties GetDocumentProperties(this DXPP.WordprocessingDocument wordDoc)
  {
    return new DocumentProperties (wordDoc);
  }

  /// <summary>
  /// Checks if the document has a <c>MainDocumentPart</c>.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static bool HasMainDocumentPart(this DXPP.WordprocessingDocument wordDoc)
  {
    return wordDoc.MainDocumentPart != null;
  }


  /// <summary>
  /// Gets the <c>MainDocumentPart</c> of the document. If the document does not have a <c>MainDocumentPart</c>, it is created with an empty <c>Document</c> element.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static DXPP.MainDocumentPart GetMainDocumentPart(this DXPP.WordprocessingDocument wordDoc)
  {
    var mainDocumentPart = wordDoc.MainDocumentPart;
    if (mainDocumentPart == null)
    {
      mainDocumentPart = wordDoc.AddMainDocumentPart();
      mainDocumentPart.Document = new DXW.Document();
    }
    return mainDocumentPart;
  }

  /// <summary>
  /// Checks if the document has a <c>Body</c> element.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static bool HasBody(this DXPP.WordprocessingDocument wordDoc)
  {
    return wordDoc.MainDocumentPart?.Document?.Body != null;
  }


  /// <summary>
  /// Gets the <c>Body</c> element of the document. If the document does not have a <c>Body</c>, it is created.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static DXW.Body GetBody(this DXPP.WordprocessingDocument wordDoc)
  {
    var mainDocumentPart = wordDoc.MainDocumentPart;
    if (mainDocumentPart == null)
    {
      mainDocumentPart = wordDoc.AddMainDocumentPart();
    }
    if (mainDocumentPart.Document == null)
    {
      mainDocumentPart.Document = new DXW.Document();
    }
    var body = mainDocumentPart.Document.Body;
    if (body == null)
    {
      body = new DXW.Body();
      mainDocumentPart.Document!.Body = body;
    }
    return body;
  }

  /// <summary>
  /// Gets all the <c>Header</c> elements of the document.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static IEnumerable<DXW.Header> GetHeaders(this DXPP.WordprocessingDocument wordDoc)
  {
    var mainDocumentPart = wordDoc.MainDocumentPart;
    if (mainDocumentPart == null)
    {
      mainDocumentPart = wordDoc.AddMainDocumentPart();
    }
    var headers = mainDocumentPart.HeaderParts.Where(part => part.Header is not null).Select(part => part.Header!);
    return headers;
  }


  /// <summary>
  /// Gets all the <c>Footer</c> elements of the document.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static IEnumerable<DXW.Footer> GetFooters(this DXPP.WordprocessingDocument wordDoc)
  {
    var mainDocumentPart = wordDoc.MainDocumentPart;
    if (mainDocumentPart == null)
    {
      mainDocumentPart = wordDoc.AddMainDocumentPart();
      mainDocumentPart.Document = new DXW.Document();
    }
    var footers = mainDocumentPart.FooterParts.Where(part => part.Footer is not null).Select(part => part.Footer!);
    return footers;
  }

  /// <summary>
  /// Gets the <see cref="Range"/> of the <c>Body</c> element of the document. If the document does not have a <c>Body</c>, it is created.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static Range GetRange(this DXPP.WordprocessingDocument wordDoc)
  {
    return wordDoc.GetBody().GetRange();
  }

}
