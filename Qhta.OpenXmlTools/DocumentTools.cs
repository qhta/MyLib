namespace Qhta.OpenXmlTools;

/// <summary>
/// A collection of tools for working with OpenXml documents.
/// </summary>
public static class DocumentTools
{

  /// <summary>
  /// Gets all the properties of the document to manage them in a uniform way.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static DocumentProperties GetDocumentProperties(this DXPack.WordprocessingDocument wordDoc)
  {
    return new DocumentProperties (wordDoc);
  }

  /// <summary>
  /// Checks if the document has a <c>MainDocumentPart</c>.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static bool HasMainDocumentPart(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.MainDocumentPart != null;
  }


  /// <summary>
  /// Gets the <c>MainDocumentPart</c> of the document. If the document does not have a <c>MainDocumentPart</c>, it is created with an empty <c>Document</c> element.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static DXPack.MainDocumentPart GetMainDocumentPart(this DXPack.WordprocessingDocument wordDoc)
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
  public static bool HasBody(this DXPack.WordprocessingDocument wordDoc)
  {
    return wordDoc.MainDocumentPart?.Document?.Body != null;
  }


  /// <summary>
  /// Gets the <c>Body</c> element of the document. If the document does not have a <c>Body</c>, it is created.
  /// </summary>
  /// <param name="wordDoc"></param>
  /// <returns></returns>
  public static DXW.Body GetBody(this DXPack.WordprocessingDocument wordDoc)
  {
    var mainDocumentPart = wordDoc.MainDocumentPart;
    if (mainDocumentPart == null)
    {
      mainDocumentPart = wordDoc.AddMainDocumentPart();
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
}
