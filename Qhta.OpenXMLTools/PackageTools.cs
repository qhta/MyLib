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
    using (var docx = DXPack.WordprocessingDocument.Open(filePath, true))
    {
      if (docx.MainDocumentPart?.Document is not null) docx.MainDocumentPart.Document.RecreateSubdocumentRelationships(relDirectoryRoot);
    }
  }

  /// <summary>
  /// Uri of the subdocument relationship type.
  /// </summary>
  public const string subdocumentRelType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/subDocument";

  private struct RelPar(DXPack.ExternalRelationship relationship, DXW.Paragraph paragraph)
  {
    public readonly DXPack.ExternalRelationship Relationship = relationship;
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
}