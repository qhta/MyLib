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

    public bool CompareXmlElement(XElement outXml, XElement expXml, bool showUnequal = true)
    {
      if (outXml.NodeType != expXml.NodeType
        || !AreEqual(outXml.Name.NamespaceName, expXml.Name.NamespaceName)
        || !AreEqual(outXml.Name.LocalName, expXml.Name.LocalName))
      {
        if (showUnequal)
          ShowUnequalElements(outXml, expXml);
        return false;
      }
      if (!CompareXmlAttributes(outXml, expXml))
      {
        if (showUnequal)
          ShowUnequalElements(outXml, expXml);
        return false;
      }

      var outNodes = outXml.Elements().ToList();
      var expNodes = expXml.Elements().ToList();
      int diffCount = 0;
      bool areEqual = true;
      int outNodesCount = outNodes.Count;
      int expNodesCount = expNodes.Count;
      for (int i = 0; i < Math.Min(outNodesCount, expNodesCount); i++)
      { 
        if (!CompareXmlElement(outNodes[i], expNodes[i], outNodesCount==expNodesCount))
        {
          areEqual = false;
          if (outNodesCount == expNodesCount)
          {
            diffCount++;
            if (diffCount >= Options.DiffLimit)
              return false;
          }
          else
          {
            if (showUnequal)
            {
              ShowLine(Options.StartOfDiffOut);
              for (int j = i; j < outNodesCount; j++)
                ShowXmlElement(outNodes[j], false, Options.SyncLimit);
              ShowLine(Options.StartOfDiffExp);
              for (int j = i; j < expNodesCount; j++)
                ShowXmlElement(expNodes[j], true, Options.SyncLimit);
              ShowLine(Options.EndOfDiffs);
              return false;
            }
          }
        }
      }
      return areEqual;
    }

    public bool CompareXmlAttributes(XElement outXml, XElement expXml)
    {
      var outAttributes = outXml.Attributes().ToList();
      var expAttributes = expXml.Attributes().ToList();
      if (!AreEqual(outAttributes.Count, expAttributes.Count))
      {
        return false;
      }
      for (int i = 0; i < outAttributes.Count; i++)
        if (!CompareXmlAttribute(outAttributes[i], expAttributes[i]))
        {
          return false;
        }
      return true;
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
      ShowXmlElement(outXml, false, Options.SyncLimit);
      ShowLine(Options.StartOfDiffExp);
      ShowXmlElement(expXml, true, Options.SyncLimit);
      ShowLine(Options.EndOfDiffs);
    }

    protected void ShowXmlElement(XElement element, bool? isExp = null, int linesLimit=0)
    {
      var lines = ToStrings(element);
      if (linesLimit > 0 && linesLimit < lines.Count())
      {
        lines = lines.AsSpan(0, linesLimit).ToArray().Append("...").ToArray();
      }
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
        return lines;
      }
    }
  }
}
