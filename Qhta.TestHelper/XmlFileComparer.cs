using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Qhta.TestHelper
{
  public class XmlFileComparer : FileComparer
  {
    protected enum CompResult
    {
      areEqual,
      nameDiff,
      attrDiff,
      elementDiff,
      elementCountDiff,
      valueDiff,
    }

    private int diffCount;

    public XmlFileComparer(FileCompareOptions options, TextWriterTraceListener listener) : base(options, listener)
    {
    }

    public override bool CompareFiles(string outFilename, string expFilename)
    {
      XDocument outXml;
      XDocument expXml;
      diffCount = 0;
      using (TextReader aReader = File.OpenText(outFilename))
        outXml = XDocument.Load(aReader);

      using (TextReader aReader = File.OpenText(expFilename))
        expXml = XDocument.Load(aReader);

      return CompareXml(outXml, expXml);
    }

    public bool CompareXml(XDocument outXml, XDocument expXml)
    {
      var areEqual = CompareXmlElement(outXml.Root, expXml.Root) == CompResult.areEqual;
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

    protected CompResult CompareXmlElement(XElement outXml, XElement expXml, bool showUnequal = true)
    {
      //if (outXml.Name=="LinkText")
      //  Debug.Assert(true);
      if (outXml.NodeType != expXml.NodeType
        || !AreEqual(outXml.Name.NamespaceName, expXml.Name.NamespaceName)
        || !AreEqual(outXml.Name.LocalName, expXml.Name.LocalName))
      {
        if (showUnequal)
          ShowUnequalElements(outXml, expXml, 1);
        return CompResult.nameDiff;
      }
      if (!CompareXmlAttributes(outXml, expXml))
      {
        if (showUnequal)
          ShowUnequalElements(outXml, expXml, 1);
        return CompResult.attrDiff;
      }
      var outNodes = outXml.Elements().ToList();
      var expNodes = expXml.Elements().ToList();
      int outNodesCount = outNodes.Count;
      int expNodesCount = expNodes.Count;
      bool areEqual = outNodesCount == expNodesCount;
      for (int i = 0; i < Math.Min(outNodesCount, expNodesCount); i++)
      {
        var result = CompareXmlElement(outNodes[i], expNodes[i], outNodesCount == expNodesCount);
        if (result != CompResult.areEqual)
        {
          areEqual = false;
          if (outNodesCount == expNodesCount)
          {
            diffCount++;
            if (diffCount >= Options.DiffLimit)
              break;
          }
          else
          {
            if (showUnequal && result != CompResult.elementCountDiff && result != CompResult.elementDiff)
            {
              int linesLimit = Options.SyncLimit;
              ShowLine(Options.StartOfDiffOut);
              for (int j = 0; j < linesLimit; j++)
              {
                var ndx = i + j;
                if (ndx >= outNodesCount)
                  break;
                var linesShown = ShowXmlElement(outNodes[ndx], false, linesLimit);
                linesLimit -= linesShown;
                if (linesLimit <= 0)
                {
                  ShowLine("...");
                  break;
                }
              }
              linesLimit = Options.SyncLimit;
              ShowLine(Options.StartOfDiffExp);
              for (int j = 0; j < linesLimit; j++)
              {
                var ndx = i + j;
                if (ndx >= expNodesCount)
                  break;
                var linesShown = ShowXmlElement(expNodes[ndx], true, linesLimit);
                linesLimit -= linesShown;
                if (linesLimit <= 0)
                {
                  ShowLine("...");
                  break;
                }
              }
              ShowLine(Options.EndOfDiffs);
            }
            return CompResult.elementDiff;
          }
        }
      }
      if (!areEqual)
      {
        if (outNodesCount != expNodesCount)
        {
          if (showUnequal)
            ShowUnequalElements(outXml, expXml);
          return CompResult.elementCountDiff;
        }
        return CompResult.elementDiff;
      }
      if (outXml.Value != expXml.Value)
      {
        if (showUnequal)
          ShowUnequalElements(outXml, expXml);
        return CompResult.valueDiff;
      }
      return CompResult.areEqual;
    }

    public bool CompareXmlAttributes(XElement outXml, XElement expXml)
    {
      var outAttributes = outXml.Attributes().ToList();
      var expAttributes = expXml.Attributes().ToList();
      if (!AreEqual(outAttributes.Count, expAttributes.Count))
      {
        return false;
      }
      if (Options.IgnoreAttributesOrder)
      {
        outAttributes.Sort(CompareAttrName);
        expAttributes.Sort(CompareAttrName);
      }
      for (int i = 0; i < outAttributes.Count; i++)
        if (!CompareXmlAttribute(outAttributes[i], expAttributes[i]))
        {
          return false;
        }
      return true;
    }

    public int CompareAttrName(XAttribute attr1, XAttribute attr2)
    {
      var name1 = attr1.Name;
      var name2 = attr2.Name;
      var result = name1.Namespace.ToString().CompareTo(name2.Namespace.ToString());
      if (result != 0)
        return result;
      result = name1.LocalName.ToString().CompareTo(name2.LocalName.ToString());
      return result;
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

    public void ShowUnequalElements(XElement outXml, XElement expXml, int linesLimit = 0)
    {
      if (linesLimit == 0)
        linesLimit = Options.SyncLimit;
      ShowLine(Options.StartOfDiffOut);
      ShowXmlElement(outXml, false, linesLimit);
      ShowLine(Options.StartOfDiffExp);
      ShowXmlElement(expXml, true, linesLimit);
      ShowLine(Options.EndOfDiffs);
    }

    /// <returns>number of lines shown</returns>
    protected int ShowXmlElement(XElement element, bool? isExpected = null, int linesLimit = 0)
    {
      var lines = ToStrings(element);
      if (linesLimit > 0 && linesLimit < lines.Count())
      {
        if (linesLimit == 1)
        {
          lines = lines.AsSpan(0, 1).ToArray();
          lines[0] += "...";
        }
        else
          lines = lines.AsSpan(0, linesLimit).ToArray().Append("...").ToArray();
      }
      ShowLines(lines, isExpected);
      return lines.Count();
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
