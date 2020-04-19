using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.IO.Packaging;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;

namespace OpenXMLTools
{
  public class PackageTools
  {
    //public void VerifySecurityEvidenceForIsolatedStorage()
    //{
    //  VerifySecurityEvidenceForIsolatedStorage(this.GetType().Assembly);
    //}

    //public static void VerifySecurityEvidenceForIsolatedStorage(Assembly assembly)
    //{
    //  var isEvidenceFound = true;
    //  var initialAppDomainEvidence = System.Threading.Thread.GetDomain().Evidence;
    //  try
    //  {
    //    // this will fail when the current AppDomain Evidence is instantiated via COM or in PowerShell
    //    using (var usfdAttempt1 = System.IO.IsolatedStorage.IsolatedStorageFile.GetUserStoreForDomain())
    //    {
    //    }
    //  }
    //  catch (System.IO.IsolatedStorage.IsolatedStorageException e)
    //  {
    //    isEvidenceFound = false;
    //  }

    //  if (!isEvidenceFound)
    //  {
    //    initialAppDomainEvidence.AddHostEvidence(new Url(assembly.Location));
    //    initialAppDomainEvidence.AddHostEvidence(new Zone(SecurityZone.MyComputer));

    //    var currentAppDomain = Thread.GetDomain();
    //    var securityIdentityField = currentAppDomain.GetType().GetField("_SecurityIdentity", BindingFlags.Instance | BindingFlags.NonPublic);
    //    securityIdentityField.SetValue(currentAppDomain, initialAppDomainEvidence);

    //    var latestAppDomainEvidence = System.Threading.Thread.GetDomain().Evidence; // setting a breakpoint here will let you inspect the current app domain evidence
    //  }
    //}

    public Package CreatePackageFromWordOpenXML(string wordOpenXML, string filePath)
    {
      XDocument xDoc = XDocument.Parse(wordOpenXML);
      IsolatedStorageFile isolatedStore = IsolatedStorageFile.GetUserStoreForAssembly();
      IsolatedStorageFileStream stream = new IsolatedStorageFileStream(
         "file.txt", FileMode.Create, FileAccess.Write, isolatedStore);
      using (TextWriter textWriter = File.CreateText("Temp.xml"))
        using (XmlWriter xmlWriter = XmlWriter.Create(textWriter,new XmlWriterSettings { Indent=true }))
          xDoc.WriteTo(xmlWriter);

      string packageXmlns = "http://schemas.microsoft.com/office/2006/xmlPackage";

      Package newPkg = System.IO.Packaging.ZipPackage.Open(filePath, FileMode.Create);

      try
      {
        XPathDocument xpDocument = new XPathDocument(new StringReader(wordOpenXML));
        XPathNavigator xpNavigator = xpDocument.CreateNavigator();

        XmlNamespaceManager nsManager = new XmlNamespaceManager(xpNavigator.NameTable);
        nsManager.AddNamespace("pkg", packageXmlns);
        XPathNodeIterator xpIterator = xpNavigator.Select("//pkg:part", nsManager);

        while (xpIterator.MoveNext())
        {
          Uri partUri = new Uri(xpIterator.Current.GetAttribute("name", packageXmlns), UriKind.Relative);

          PackagePart pkgPart = newPkg.CreatePart(partUri, xpIterator.Current.GetAttribute("contentType", packageXmlns));

          // Set this package part's contents to this XML node's inner XML, sans its surrounding xmlData element.
          string strInnerXml = xpIterator.Current.InnerXml
              .Replace("<pkg:xmlData xmlns:pkg=\"" + packageXmlns + "\">", "")
              .Replace("</pkg:xmlData>", "");
          byte[] buffer = Encoding.UTF8.GetBytes(strInnerXml);
          pkgPart.GetStream().Write(buffer, 0, buffer.Length);
        }

        newPkg.Flush();
      }
      finally
      {
        newPkg.Close();
        isolatedStore.Close();
      }
      return newPkg;
    }
  }


}
