using System;
using System.Diagnostics;
using System.IO;
using System.IO.Packaging;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXMLTools
{
  public static class PackageTools
  {

    public static void CreatePackageFromWordOpenXML(string wordOpenXml, string filePath)
    {
      var xmlFileName = Path.ChangeExtension(filePath, ".xml");
      using (var writer = File.CreateText(xmlFileName))
      {
        writer.Write(wordOpenXml);
      }
      XDocument xDoc = XDocument.Parse(wordOpenXml);

      string packageXmlns = "http://schemas.microsoft.com/office/2006/xmlPackage";

      using (Package newPkg = System.IO.Packaging.ZipPackage.Open(filePath, FileMode.Create))
      {
        XPathDocument xpDocument = new XPathDocument(new StringReader(wordOpenXml));
        XPathNavigator xpNavigator = xpDocument.CreateNavigator();

        XmlNamespaceManager nsManager = new XmlNamespaceManager(xpNavigator.NameTable);
        nsManager.AddNamespace("pkg", packageXmlns);

        XPathNodeIterator pkgPartIterator = xpNavigator.Select("//pkg:part", nsManager);
        while (pkgPartIterator.MoveNext())
        {
          var partName = pkgPartIterator.Current.GetAttribute("name", packageXmlns);
          Uri partUri = new Uri(partName, UriKind.Relative);
          var partType = pkgPartIterator.Current.GetAttribute("contentType", packageXmlns);
          PackagePart pkgPart = newPkg.CreatePart(partUri, partType);

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

    public static void RecreateSubdocumentRels(this WordprocessingDocument docx)
    {
      foreach (FieldCode fieldCode in docx.MainDocumentPart.Document.Descendants<FieldCode>())
      {
        var fieldText = fieldCode.InnerText.Trim();
        Debug.WriteLine(fieldText);
        //if (fieldText.StartsWith("HYPERLINK"))
        //{
        //}
      }
    }
  }
}
