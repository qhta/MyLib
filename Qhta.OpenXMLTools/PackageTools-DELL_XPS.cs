using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXMLTools;

/// <summary>
/// Tools for creating packages from Word Open XML.
/// </summary>
public static class PackageTools
{

  /// <summary>
  /// Creates a package from a Word Open XML string.
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

    using (Package newPkg = ZipPackage.Open(filePath, FileMode.Create))
    {
      XPathDocument xpDocument = new XPathDocument(new StringReader(wordOpenXml));
      XPathNavigator xpNavigator = xpDocument.CreateNavigator();

      XmlNamespaceManager nsManager = new XmlNamespaceManager(xpNavigator.NameTable);
      nsManager.AddNamespace("pkg", packageXmlns);

      XPathNodeIterator pkgPartIterator = xpNavigator.Select("//pkg:part", nsManager);
      while (pkgPartIterator.MoveNext())
      {
        if (pkgPartIterator.Current != null)
        {
          var partName = pkgPartIterator.Current.GetAttribute("name", packageXmlns);
          if (partName != null)
          {
            Uri partUri = new Uri(partName, UriKind.Relative);
            var partType = pkgPartIterator.Current.GetAttribute("contentType", packageXmlns);
            if (partType != null)
            {
              var pkgPart = newPkg.CreatePart(partUri, partType);

              string strInnerXml = pkgPartIterator.Current.InnerXml
                .Replace("<pkg:xmlData xmlns:pkg=\"" + packageXmlns + "\">", "")
                .Replace("</pkg:xmlData>", "");
              byte[] buffer = Encoding.UTF8.GetBytes(strInnerXml);
              var pkgStream = pkgPart.GetStream();
              byte[] xmlPreamble = Encoding.UTF8.GetBytes("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>\r\n");
              pkgStream.Write(xmlPreamble, 0, xmlPreamble.Length);
              pkgStream.Write(buffer, 0, buffer.Length);
            }
          }
        }
      }
    }
    using (var docx = WordprocessingDocument.Open(filePath, true))
    {
      if (docx.MainDocumentPart?.Document is not null) docx.MainDocumentPart.Document.RecreateSubdocumentRels(relDirectoryRoot);
    }
  }

  /// <summary>
  /// Constant for the subdocument relationship type.
  /// </summary>
  public const string subdocumentRelType = "http://schemas.openxmlformats.org/officeDocument/2006/relationships/subDocument";

  private struct RelPar(ExternalRelationship rel, Paragraph par)
  {
    public readonly ExternalRelationship Rel = rel;
    public readonly Paragraph Par = par;
  }
  /// <summary>
  /// Recreates the subdocument relationships in the document using the specified path.
  /// </summary>
  /// <param name="doc"></param>
  /// <param name="docxPath"></param>
  public static void RecreateSubdocumentRels(this Document doc, string docxPath)
  {
    var fieldCodes = doc.Descendants<FieldCode>().ToList();
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
            var fieldParagraph = fieldCode.Ancestors<Paragraph>().LastOrDefault();
            if (fieldParagraph != null)
            {
              var targetPath = Path.GetDirectoryName(targetFileName);
              //Debug.WriteLine(docxPath);
              if (targetPath != null && targetPath.StartsWith(docxPath, StringComparison.CurrentCultureIgnoreCase))
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

    foreach (var r in addedRels)
    {
      r.Par.InsertBeforeSelf<SubDocumentReference>(new SubDocumentReference { Id = r.Rel.Id });
      r.Par.Remove();
    }
  }
}