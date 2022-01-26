using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Qhta.TestHelper
{
  public class XmlFileComparer : FileComparer
  {
    protected enum CompResult
    {
      AreEqual,
      NameDiff,
      AttrDiff,
      ElementDiff,
      ElementCountDiff,
      ValueDiff,
    }

    private int diffCount;

    [DebuggerStepThrough]
    public XmlFileComparer(FileCompareOptions options, ITraceWriter listener) : base(options, listener)
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
      bool shown = false;
      if (outXml.Root != null && expXml.Root != null)
        return true;
      if (outXml.Root != null || expXml.Root != null)
        return false;
      var result = CompareXmlElement(outXml.Root, expXml.Root, true, ref shown);
      var areEqual = result == CompResult.AreEqual;
      if (!areEqual && !shown)
      {
        throw new InternalException("XmlFileComparer have not shown unequal elements");
      }
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

    protected CompResult CompareXmlElement(XElement outXml, XElement expXml, bool showUnequal, ref bool shown)
    {
      if (outXml.NodeType != expXml.NodeType
        || !AreEqual(outXml.Name.NamespaceName, expXml.Name.NamespaceName)
        || !AreEqual(outXml.Name.LocalName, expXml.Name.LocalName))
      {
        if (showUnequal)
        {
          ShowUnequalElement(outXml, expXml, 1);
          shown = true;
        }
        return CompResult.NameDiff;
      }
      if (!CompareXmlAttributes(outXml, expXml))
      {
        if (showUnequal)
        {
          ShowUnequalElement(outXml, expXml, 1);
          shown = true;
        }
        return CompResult.AttrDiff;
      }
      var outNodes = outXml.Elements().ToArray();
      var expNodes = expXml.Elements().ToArray();
      int outNodesCount = outNodes.Count();
      int expNodesCount = expNodes.Count();
      bool areEqual = outNodesCount == expNodesCount;
      for (int i = 0; i < Math.Min(outNodesCount, expNodesCount); i++)
      {
        var result = CompareXmlElement(outNodes[i], expNodes[i], outNodesCount == expNodesCount, ref shown);
        if (result != CompResult.AreEqual)
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
            if (showUnequal && !shown)
            {
              if (result == CompResult.ElementCountDiff)
              {
                var fromIndex = i;
                if (fromIndex > 0)
                  fromIndex--;
                if (fromIndex > 0)
                  fromIndex--;
                ShowUnequalItemElements(outXml, expXml, fromIndex, 20);
                shown = true;
              }
              else 
              {
                ShowUnequalElements(outNodes, expNodes, i);
                shown = true;
              }
            }
            return CompResult.ElementDiff;
          }
        }
      }
      if (!areEqual)
      {
        if (outNodesCount != expNodesCount)
        {
          ShowUnequalElement(outXml, expXml);
          shown = true;
          return CompResult.ElementCountDiff;
        }
        return CompResult.ElementDiff;
      }
      if (outXml.Value != expXml.Value)
      {
        ShowUnequalElement(outXml, expXml);
        shown = true;
        return CompResult.ValueDiff;
      }
      return CompResult.AreEqual;
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
      if (outXml.NodeType != System.Xml.XmlNodeType.Attribute)
        return true;
      var outValue = outXml.Value;
      var expValue = expXml.Value;
      if (!AreEqual(outValue, expValue))
        return false;
      return true;
    }

    public void ShowUnequalElement(XElement outXml, XElement expXml, int linesLimit = 0)
    {
      if (linesLimit == 0)
        linesLimit = Options.SyncLimit;
      ShowLine(Options.StartOfDiffOut);
      Listener.ForegroundColor = ConsoleColor.Red;
      ShowXmlElement(outXml, false, linesLimit);
      Listener.ResetColor();
      ShowLine(Options.StartOfDiffExp);
      Listener.ForegroundColor = ConsoleColor.Green;
      ShowXmlElement(expXml, true, linesLimit);
      Listener.ResetColor();
      ShowLine(Options.EndOfDiffs);
    }

    public void ShowUnequalElements(XElement[] outNodes, XElement[] expNodes, int fromIndex)
    {
      int linesLimit = Options.SyncLimit;
      ShowLine(Options.StartOfDiffOut);
      Listener.ForegroundColor = ConsoleColor.Red;
      for (int j = 0; j < linesLimit; j++)
      {
        var ndx = fromIndex + j;
        if (ndx >= outNodes.Count())
          break;
        var linesShown = ShowXmlElement(outNodes[ndx], false, linesLimit);
        linesLimit -= linesShown;
        if (linesLimit <= 0)
        {
          if (linesLimit < -1)// -1 means that "..." line was shown
            ShowLine("...");
          break;
        }
      }
      linesLimit = Options.SyncLimit;
      Listener.ResetColor();
      ShowLine(Options.StartOfDiffExp);
      Listener.ForegroundColor = ConsoleColor.Green;
      for (int j = 0; j < linesLimit; j++)
      {
        var ndx = fromIndex + j;
        if (ndx >= expNodes.Count())
          break;
        var linesShown = ShowXmlElement(expNodes[ndx], true, linesLimit);
        linesLimit -= linesShown;
        if (linesLimit <= 0)
        {
          if (linesLimit < -1)// -1 means that "..." line was shown
            ShowLine("...");
          break;
        }
      }
      Listener.ResetColor();
      ShowLine(Options.EndOfDiffs);

    }

    public void ShowUnequalItemElements(XElement outXml, XElement expXml, int fromIndex, int linesLimit)
    {
      if (linesLimit == 0)
        linesLimit = Options.SyncLimit;
      ShowLine(Options.StartOfDiffOut);
      Listener.ForegroundColor = ConsoleColor.Red;
      var outElements = outXml.Elements().ToArray();
      int outLimit = linesLimit;
      for (int i = fromIndex; i < outElements.Count(); i++)
      {
        if (i - fromIndex > outLimit)
          break;
        var linesShown = ShowXmlElement(outElements[i], false, outLimit);
        outLimit -= linesShown;
      }
      Listener.ResetColor();
      ShowLine(Options.StartOfDiffExp);
      Listener.ForegroundColor = ConsoleColor.Green;
      var expElements = expXml.Elements().ToArray();
      int expLimit = linesLimit;
      for (int i = fromIndex; i < expElements.Count(); i++)
      {
        if (i - fromIndex > expLimit)
          break;
        var linesShown = ShowXmlElement(expElements[i], true, expLimit);
        expLimit -= linesShown;
      }
      Listener.ResetColor();
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
      if (element.NodeType == XmlNodeType.Element && element.Name=="t")
        Debug.Assert(true);
      using (var stringWriter = new StringWriter())
      {
        var xmlWriter = new XmlTextWriter(stringWriter);
        xmlWriter.Formatting = Formatting.Indented;
        (element as IXmlSerializable).WriteXml(xmlWriter);
        var lines = stringWriter.ToString().Split("\r\n");
        return lines;
      }
    }
  }
}
