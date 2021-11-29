using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Xml.Linq;
using System.Xml;

namespace Qhta.TestHelper
{
  public class XmlFileComparer : FileComparer
  {
    public XmlFileComparer(FileCompareOptions options) : base(options)
    {
    }

    public override bool CompareFiles(string outFilename, string expFilename)
    {
      XDocument outXml;
      XDocument expXml;

      using (TextReader aReader = File.OpenText(outFilename))
        outXml = XDocument.Load(aReader);

      using (TextReader aReader = File.OpenText(expFilename))
        expXml = XDocument.Load(aReader);

      return CompareXml(outXml, expXml);
    }

    public bool CompareXml(XDocument outXml, XDocument expXml)
    {
      var areEqual = CompareXmlElement(outXml.Root, expXml.Root);
      if (areEqual)
      {
        var msg = Options?.EqualityMsg;
        if (msg != null)
          ShowLine(msg);
      }
      else
      {
        var msg = Options?.InequalityMsg;
        if (msg != null)
          ShowLine(msg);
      }
      return areEqual;
    }

    public bool CompareXmlElement(XElement outXml, XElement expXml)
    {
      if (outXml.NodeType != expXml.NodeType)
      {
        ShowUnequalElements(outXml, expXml);
        return false;
      }
      if (!AreEqual(outXml.Name.NamespaceName, expXml.Name.NamespaceName))
      {
        ShowUnequalElements(outXml, expXml);
        return false;
      }
      if (!AreEqual(outXml.Name.LocalName, expXml.Name.LocalName))
      {
        ShowUnequalElements(outXml, expXml);
        return false;
      }
      if (outXml.NodeType != System.Xml.XmlNodeType.Element)
        return true;
      var outAttributes = outXml.Attributes().ToList();
      var expAttributes = expXml.Attributes().ToList();
      if (!AreEqual(outAttributes.Count, outAttributes.Count))
      {
        ShowUnequalElements(outXml, expXml);
        return false;
      }
      for (int i = 0; i < outAttributes.Count; i++)
        if (!CompareXmlAttribute(outAttributes[i], expAttributes[i]))
        {
          ShowUnequalElements(outXml, expXml);
          return false;
        }
      var outNodes = outXml.Elements().ToList();
      var expNodes = expXml.Elements().ToList();
      if (!AreEqual(outNodes.Count, expNodes.Count))
      { 
        ShowUnequalElements(outXml, expXml);
        return false;
      }
      int diffCount = 0;
      bool areEqual = true;
      for (int i = 0; i < outNodes.Count; i++)
        if (!CompareXmlElement(outNodes[i], expNodes[i]))
        {
          areEqual = false;
          diffCount++;
          if (diffCount >= Options.DiffLimit)
            return false;
        }
      return areEqual;
    }

    public bool CompareXmlAttribute(XAttribute outXml, XAttribute expXml)
    {
      if (outXml.NodeType != expXml.NodeType)
        return false;
      if (!AreEqual(outXml.Name.NamespaceName, expXml.Name.NamespaceName))
        return false;
      if (!AreEqual(outXml.Name.LocalName, expXml.Name.LocalName))
        return false;
      if (outXml.NodeType != System.Xml.XmlNodeType.Element)
        return true;
      var outValue = outXml.Value;
      var expValue = outXml.Value;
      if (!AreEqual(outValue, expValue))
        return false;
      return true;
    }

    public void ShowUnequalElements(XElement outXml, XElement expXml)
    {
      ShowLine(Options.StartOfDiffOut);
      ShowXmlElement(outXml, false);
      ShowLine(Options.StartOfDiffExp);
      ShowXmlElement(expXml, true);
      ShowLine(Options.EndOfDiffs);
    }

    protected void ShowXmlElement(XElement element, bool? isExp = null)
    {
      var lines = ToStrings(element);
      ShowLines(lines, isExp);
    }

    protected string[] ToStrings(XElement element)
    {
      using (var stringWriter = new StringWriter())
      {
        var xmlWriter = new XmlTextWriter(stringWriter);
        xmlWriter.Formatting = Formatting.Indented;
        element.WriteTo(xmlWriter);
        var lines = stringWriter.ToString().Split("\r\n");
        if (Options.SyncLimit > 0 && Options.SyncLimit < lines.Count())
        {
          lines = lines.AsSpan(0, Options.SyncLimit).ToArray();
        }
        return lines;
      }
    }
  }
}
