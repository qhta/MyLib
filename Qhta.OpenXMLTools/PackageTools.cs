using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing Package elements.
/// </summary>
public static class PackageTools
{

  /// <summary>
  /// Create a package from a Word OpenXml string.
  /// </summary>
  /// <param name="wordOpenXml"></param>
  /// <param name="filePath"></param>
  /// <param name="relDirectoryRoot"></param>
  public static void CreatePackageFromWordOpenXML(string wordOpenXml, string filePath, string relDirectoryRoot)
  {
    var xmlFileName = Path.ChangeExtension(filePath, ".xml");
    using (var writer = File.CreateText(xmlFileName))
    {
      writer.Write(wordOpenXml);
    }
    XDocument xDoc = XDocument.Parse(wordOpenXml);

    string packageXmlns = "http://schemas.microsoft.com/office/2006/xmlPackage";

    using (Package newPkg = Package.Open(filePath, FileMode.Create))
    {
      XPathDocument xpDocument = new XPathDocument(new StringReader(wordOpenXml));
      XPathNavigator xpNavigator = xpDocument.CreateNavigator();

      XmlNamespaceManager nsManager = new XmlNamespaceManager(xpNavigator.NameTable);
      nsManager.AddNamespace("pkg", packageXmlns);

      XPathNodeIterator pkgPartIterator = xpNavigator.Select("//pkg:part", nsManager);
      while (pkgPartIterator.MoveNext())
      {
        var partName = pkgPartIterator.Current?.GetAttribute("name", packageXmlns);
        if (partName is null) continue;
        Uri partUri = new Uri(partName, UriKind.Relative);
        var partType = pkgPartIterator.Current?.GetAttribute("contentType", packageXmlns);
        if (partType is null) continue;
        PackagePart pkgPart = newPkg.CreatePart(partUri, partType);

        var strInnerXml = pkgPartIterator.Current?.InnerXml
          .Replace("<pkg:xmlData xmlns:pkg=\"" + packageXmlns + "\">", "")
          .Replace("</pkg:xmlData>", "");
        if (strInnerXml is null) continue;
        byte[] buffer = Encoding.UTF8.GetBytes(strInnerXml);
        var pkgStream = pkgPart.GetStream();
        byte[] xmlPreamble = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n");
        pkgStream.Write(xmlPreamble, 0, xmlPreamble.Length);
        pkgStream.Write(buffer, 0, buffer.Length);
      }
    }
    using (var docx = DXPP.WordprocessingDocument.Open(filePath, true))
    {
      if (docx.MainDocumentPart?.Document is not null) docx.MainDocumentPart.Document.RecreateSubdocumentRelationships(relDirectoryRoot);
    }
  }

  /// <summary>
  /// Uri of the subdocument relationship type.
  /// </summary>
  public const string subdocumentRelType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/subDocument";

  private struct RelPar(DXPP.ExternalRelationship relationship, DXW.Paragraph paragraph)
  {
    public readonly DXPP.ExternalRelationship Relationship = relationship;
    public readonly DXW.Paragraph Paragraph = paragraph;
  }

  /// <summary>
  /// Recreate subdocument relationships in the document.
  /// </summary>
  /// <param name="doc">document which contains relationships</param>
  /// <param name="docxPath">new path for subdocuments</param>
  public static void RecreateSubdocumentRelationships(this DXW.Document doc, string docxPath)
  {
    var fieldCodes = doc.Descendants<DXW.FieldCode>().ToList();
    var addedRels = new List<RelPar>();
    foreach (var fieldCode in fieldCodes)
    {
      var fieldText = fieldCode.InnerText.Trim();
      //Debug.WriteLine(fieldText);
      if (fieldText.StartsWith("HYPERLINK"))
      {
        int k = fieldText.IndexOf('"')+1;
        int l = fieldText.LastIndexOf('"')-k;
        if (l>0)
        {
          var targetFileName = fieldText.Substring(k,l);
          //if (targetFileName.Length>2 && targetFileName.First()=='"' && targetFileName.Last()=='"')
          //{
          //  targetFileName = targetFileName.Substring(1, targetFileName.Length - 2);
          //}
          targetFileName = targetFileName.Replace(@"\\", @"\");
          //Debug.WriteLine(targetFileName);
          if (targetFileName.EndsWith(".docx", StringComparison.InvariantCultureIgnoreCase))
          {
            var fieldParagraph = fieldCode.Ancestors<DXW.Paragraph>().LastOrDefault();
            if (fieldParagraph != null)
            {
              var targetPath = Path.GetDirectoryName(targetFileName) ?? "";
              //Debug.WriteLine(docxPath);
              if (targetPath.StartsWith(docxPath, StringComparison.CurrentCultureIgnoreCase))
              {
                targetFileName = targetFileName.Substring(targetPath.Length);
                if (targetFileName.StartsWith(@"\"))
                  targetFileName = targetFileName.Substring(1);
              }
              //Debug.WriteLine(targetFileName);

              //var hostElement = fieldParagraph.Parent;
              var targetUri = new Uri(targetFileName, UriKind.RelativeOrAbsolute);
              var targetRel = doc.MainDocumentPart?.AddExternalRelationship(subdocumentRelType, targetUri);
              if (targetRel != null) 
                addedRels.Add(new RelPar(targetRel, fieldParagraph));
              //var subDoc = fieldParagraph.InsertBeforeSelf<SubDocumentReference>(new SubDocumentReference { Id = targetRel.Id });
              //fieldParagraph.Remove();
            }
          }
        }
      }
    }

    foreach (var pair in addedRels)
    {
      pair.Paragraph.InsertBeforeSelf(new DXW.SubDocumentReference { Id = pair.Relationship.Id });
      pair.Paragraph.Remove();
    }
  }

  /// <summary>
  /// Create a Flat OPC XML string from the package.
  /// </summary>
  /// <param name="package">The package to convert to Flat OPC XML.</param>
  /// <returns>A string containing the Flat OPC XML representation of the package.</returns>
  public static string CreateFlatOpcXml(DXPP.IPackage package)
  {
    System.Xml.Linq.XNamespace pkg = "http://schemas.microsoft.com/office/2006/xmlPackage";

    var flatOpc = new System.Xml.Linq.XDocument(
      new System.Xml.Linq.XDeclaration("1.0", "utf-8", "yes"),
      new System.Xml.Linq.XElement(pkg + "package",
        package.GetParts()
          .OrderBy(part => part.Uri.ToString(), StringComparer.Ordinal)
          .Select(part => CreateFlatOpcPart(part, pkg))));

    return flatOpc.ToString(System.Xml.Linq.SaveOptions.DisableFormatting);
  }

  /// <summary>
  /// Creates an Open Packaging Conventions (OPC) part element in Flat OPC XML format from the specified package part.
  /// </summary>
  /// <remarks>If the part's content type is XML, the method attempts to parse and embed the XML content
  /// directly. If parsing fails or the content type is not XML, the part's data is embedded as base64-encoded binary
  /// data.</remarks>
  /// <param name="part">The package part to convert to a Flat OPC XML element. Must provide access to the part's stream and content type.</param>
  /// <param name="pkg">The XML namespace to use for Flat OPC elements.</param>
  /// <returns>An <see cref="System.Xml.Linq.XElement"/> representing the Flat OPC XML element for the specified package part.
  /// The element contains either the XML data or the base64-encoded binary data of the part.</returns>
  public static System.Xml.Linq.XElement CreateFlatOpcPart(DXPP.IPackagePart part, System.Xml.Linq.XNamespace pkg)
  {
    var partElement = new System.Xml.Linq.XElement(pkg + "part",
      new System.Xml.Linq.XAttribute(pkg + "name", part.Uri.ToString()),
      new System.Xml.Linq.XAttribute(pkg + "contentType", part.ContentType));

    using var partStream = part.GetStream(FileMode.Open, FileAccess.Read);
    using var memoryStream = new MemoryStream();
    partStream.CopyTo(memoryStream);
    var partBytes = memoryStream.ToArray();

    if (IsXmlContentType(part.ContentType))
    {
      try
      {
        var xmlText = GetXmlText(partBytes);
        var xmlRoot = System.Xml.Linq.XElement.Parse(xmlText, System.Xml.Linq.LoadOptions.PreserveWhitespace);
        partElement.Add(new System.Xml.Linq.XElement(pkg + "xmlData", xmlRoot));
        return partElement;
      }
      catch
      {
      }
    }

    partElement.Add(new System.Xml.Linq.XElement(pkg + "binaryData", Convert.ToBase64String(partBytes)));
    return partElement;
  }

  /// <summary>
  /// Helper method to read XML text from a byte array, handling potential Byte Order Marks (BOM)
  /// and ensuring correct encoding.
  /// </summary>
  /// <param name="bytes">The byte array containing the XML data.</param>
  /// <returns>A string containing the XML text.</returns>
  private static string GetXmlText(byte[] bytes)
  {
    using var stream = new MemoryStream(bytes);
    using var reader = new StreamReader(stream, System.Text.Encoding.UTF8, detectEncodingFromByteOrderMarks: true);
    return reader.ReadToEnd();
  }

  /// <summary>
  /// Helper method to determine if a content type is XML-based by checking if it ends with "/xml" or "+xml".
  /// </summary>
  /// <param name="contentType">The content type to check.</param>
  /// <returns>True if the content type is XML-based; otherwise, false.</returns>
  private static bool IsXmlContentType(string contentType)
  {
    return contentType.EndsWith("/xml", StringComparison.OrdinalIgnoreCase)
      || contentType.EndsWith("+xml", StringComparison.OrdinalIgnoreCase);
  }
}