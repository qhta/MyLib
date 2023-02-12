using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Qhta.TestHelper;

/// <summary>
/// Concrete class to compare Xml files. 
/// </summary>
public class XmlFileComparer : AbstractFileComparer
{
  /// <summary>
  /// Compare result for internal methods
  /// </summary>
  protected enum CompResult
  {
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    AreEqual,
    NameDiff,
    AttrDiff,
    ElementDiff,
    ElementCountDiff,
    ValueDiff,
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
  }

  private int diffCount;

  /// <summary>
  /// Constructor invoking base class constructor
  /// </summary>
  /// <param name="options">Options to compare</param>
  /// <param name="writer">Writer used to show different elements</param>
  [DebuggerStepThrough]
  public XmlFileComparer(FileCompareOptions options, ITraceTextWriter? writer) : base(options, writer)
  {
  }

  /// <summary>
  /// Implemented method of file comparison.
  /// Simply reads out whole xml documents and invokes <see cref="CompareXmlDocuments"/>.
  /// </summary>
  /// <param name="outFilename">Received output filename</param>
  /// <param name="expFilename">Expected content filename</param>
  /// <returns>true is both files are equal</returns>
  public override bool CompareFiles(string outFilename, string expFilename)
  {
    XDocument outXml;
    XDocument expXml;
    diffCount = 0;
    using (TextReader aReader = File.OpenText(outFilename))
      outXml = XDocument.Load(aReader);

    using (TextReader aReader = File.OpenText(expFilename))
      expXml = XDocument.Load(aReader);

    return CompareXmlDocuments(outXml, expXml);
  }

  /// <summary>
  /// Main method to compare Xml documents. 
  /// Root elements are compared through <see cref="CompareXmlElements"/>.
  /// </summary>
  /// <param name="outDocument">Received output Xml document</param>
  /// <param name="expDocument">Expected content Xml document</param>
  /// <returns>true if both documents are equal</returns>
  protected virtual bool CompareXmlDocuments(XDocument outDocument, XDocument expDocument)
  {
    bool shown = false;

    if (outDocument?.Root == null && expDocument?.Root == null)
      return true;
    if (outDocument?.Root == null || expDocument?.Root == null)
      return false;

    var result = CompareXmlElements(outDocument.Root, expDocument.Root, true, ref shown);
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

  /// <summary>
  /// Main method to compare XmlElements. If elements are different, they may be show.
  /// Parameters <paramref name="showUnequal"/> and <paramref name="shown"/> are important
  /// when the method is called recursive.
  /// </summary>
  /// <param name="outElement">Received output Xml element</param>
  /// <param name="expElement">Expected content Xml element</param>
  /// <param name="showUnequal">If unequal element should be shown</param>
  /// <param name="shown">If unequal element were shown</param>
  /// <returns>true if both elements are equal</returns>
  protected virtual CompResult CompareXmlElements(XElement outElement, XElement expElement, bool showUnequal, ref bool shown)
  {
    if (outElement.NodeType != expElement.NodeType
        || !AreEqual(outElement.Name.NamespaceName, expElement.Name.NamespaceName)
        || !AreEqual(outElement.Name.LocalName, expElement.Name.LocalName))
    {
      if (showUnequal)
      {
        ShowUnequalElement(outElement, expElement, 1);
        shown = true;
      }
      return CompResult.NameDiff;
    }
    if (!CompareXmlAttributes(outElement, expElement))
    {
      if (showUnequal)
      {
        ShowUnequalElement(outElement, expElement, 1);
        shown = true;
      }
      return CompResult.AttrDiff;
    }
    var outNodes = outElement.Elements().ToArray();
    var expNodes = expElement.Elements().ToArray();
    int outNodesCount = outNodes.Count();
    int expNodesCount = expNodes.Count();
    bool areEqual = outNodesCount == expNodesCount;
    for (int i = 0; i < Math.Min(outNodesCount, expNodesCount); i++)
    {
      var result = CompareXmlElements(outNodes[i], expNodes[i], outNodesCount == expNodesCount, ref shown);
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
              ShowUnequalItemElements(outElement, expElement, fromIndex, 20);
              shown = true;
            }
            else 
            {
              ShowUnequalElements(outNodes[i..^0], expNodes[i..^0]);
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
        ShowUnequalElement(outElement, expElement);
        shown = true;
        return CompResult.ElementCountDiff;
      }
      return CompResult.ElementDiff;
    }
    if (outElement.Value != expElement.Value)
    {
      ShowUnequalElement(outElement, expElement);
      shown = true;
      return CompResult.ValueDiff;
    }
    return CompResult.AreEqual;
  }

  /// <summary>
  /// Method to compare Xml attributes of two Xml elements.
  /// </summary>
  /// <param name="outElement">Received output Xml element</param>
  /// <param name="expElement">Expected content Xml element</param>
  /// <returns>true if attributes of both elements are equal</returns>
  protected virtual bool CompareXmlAttributes(XElement outElement, XElement expElement)
  {
    var outAttributes = outElement.Attributes().ToList();
    var expAttributes = expElement.Attributes().ToList();
    if (outAttributes.Count != expAttributes.Count)
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

  /// <summary>
  /// Helper method to compare attribute names. Used to sort attribute lists.
  /// Namespaces are compared first, local names next.
  /// </summary>
  /// <param name="attr1">First attribute to compare</param>
  /// <param name="attr2">Second attribute to compare</param>
  /// <returns>1 if first name is greater then second, -1 it is less, 0 if are qual</returns>
  protected virtual int CompareAttrName(XAttribute attr1, XAttribute attr2)
  {
    var name1 = attr1.Name;
    var name2 = attr2.Name;
    var result = String.Compare((name1.Namespace??"").ToString(), (name2.Namespace??"").ToString(), StringComparison.Ordinal);
    if (result != 0)
      return result;
    result = String.Compare(name1.LocalName.ToString(), name2.LocalName.ToString(), StringComparison.Ordinal);
    return result;
  }

  /// <summary>
  /// Helper method to compare two attributes. Attribute namespaces, localnames and values are compared.
  /// </summary>
  /// <param name="outAttribute">First attribute to compare</param>
  /// <param name="expAttribute">Second attribute to compare</param>
  /// <returns>true if both attributes are euqal</returns>
  protected virtual bool CompareXmlAttribute(XAttribute outAttribute, XAttribute expAttribute)
  {
    if (!AreEqual(outAttribute.Name.NamespaceName, expAttribute.Name.NamespaceName))
      return false;
    if (!AreEqual(outAttribute.Name.LocalName, expAttribute.Name.LocalName))
      return false;
    var outValue = outAttribute.Value ?? "";
    var expValue = expAttribute.Value ?? "";
    if (!AreEqual(outValue, expValue))
      return false;
    return true;
  }

  /// <summary>
  /// Helper method to show unequal elements.
  /// </summary>
  /// <param name="outElement">Received output Xml element</param>
  /// <param name="expElement">Expected content Xml element</param>
  /// <param name="linesLimit">Limit of content lines to show 
  /// (default 0 changed to <see cref="FileCompareOptions.SyncLimit"/>)</param>
  protected virtual void ShowUnequalElement(XElement outElement, XElement expElement, int linesLimit=0)
  {
    if (linesLimit == 0)
      linesLimit = Options.SyncLimit;
    ShowLine(Options.StartOfDiffOut);
    if (Options.OutLinesColor != null && Writer != null)
      Writer.ForegroundColor = (ConsoleColor)Options.OutLinesColor;
    ShowXmlElement(outElement, false, linesLimit);
    if (Options.OutLinesColor != null && Writer != null)
      Writer.ResetColors();
    ShowLine(Options.StartOfDiffExp);
    if (Options.ExpLinesColor != null && Writer != null)
      Writer.ForegroundColor = (ConsoleColor)Options.ExpLinesColor;
    ShowXmlElement(expElement, true, linesLimit);
    if (Options.ExpLinesColor != null && Writer != null)
      Writer.ResetColors();
    ShowLine(Options.EndOfDiffs);
  }

  /// <summary>
  /// Helper method to show unequal elements. Two collections of elements are taken.
  /// Limits from <see cref="FileCompareOptions.SyncLimit"/> is applied.
  /// </summary>
  /// <param name="outElements">Collection of received output Xml elements</param>
  /// <param name="expElements">Collection of expected content Xml elements</param>
  /// <param name="linesLimit">Limit of content lines to show 
  /// (default 0 changed to <see cref="FileCompareOptions.SyncLimit"/>)</param>
  protected virtual void ShowUnequalElements(XElement[] outElements, XElement[] expElements, int linesLimit=0)
  {
    if (linesLimit == 0)
      linesLimit = Options.SyncLimit;
    if (linesLimit == 0)
      linesLimit = int.MaxValue;
    var outLinesLimit = linesLimit;
    ShowLine(Options.StartOfDiffOut);
    if (Options.OutLinesColor != null && Writer != null)
      Writer.ForegroundColor = (ConsoleColor)Options.OutLinesColor;
    for (int i = 0; i < outLinesLimit; i++)
    {
      if (i >= outElements.Count())
        break;
      var linesShown = ShowXmlElement(outElements[i], false, outLinesLimit);
      outLinesLimit -= linesShown;
      if (outLinesLimit <= 0)
      {
        if (outLinesLimit < -1)// -1 means that "..." line was shown
          ShowLine("...");
        break;
      }
    }
    if (Options.OutLinesColor != null && Writer != null)
      Writer.ResetColors();

    ShowLine(Options.StartOfDiffExp);
    if (Options.ExpLinesColor != null && Writer != null)
      Writer.ForegroundColor = (ConsoleColor)Options.ExpLinesColor;
    var expLinesLimit = linesLimit;
    for (int i = 0; i < outLinesLimit; i++)
    {
      if (i >= expElements.Count())
        break;
      var linesShown = ShowXmlElement(expElements[i], true, expLinesLimit);
      expLinesLimit -= linesShown;
      if (expLinesLimit <= 0)
      {
        if (expLinesLimit < -1)// -1 means that "..." line was shown
          ShowLine("...");
        break;
      }
    }
    if (Options.ExpLinesColor != null && Writer != null)
      Writer.ResetColors();

    ShowLine(Options.EndOfDiffs);
  }

  /// <summary>
  /// Helper method of unequal items of elements. Similar to previous but with several differeces.
  /// </summary>
  /// <param name="outElement">Collection of received output Xml elements</param>
  /// <param name="expElement">Collection of expected content Xml elements</param>
  /// <param name="fromIndex">Index of the first item</param>
  /// <param name="linesLimit">Limit of content lines to show 
  /// (default 0 changed to <see cref="FileCompareOptions.SyncLimit"/>)</param>
  protected virtual void ShowUnequalItemElements(XElement outElement, XElement expElement, int fromIndex, int linesLimit=0)
  {
    if (linesLimit == 0)
      linesLimit = Options.SyncLimit;
    ShowLine(Options.StartOfDiffOut);
    if (Options.OutLinesColor != null && Writer != null)
      Writer.ForegroundColor = (ConsoleColor)Options.OutLinesColor;
    var outElements = outElement.Elements().ToArray();
    int outLimit = linesLimit;
    for (int i = fromIndex; i < outElements.Count(); i++)
    {
      if (i - fromIndex > outLimit)
        break;
      var linesShown = ShowXmlElement(outElements[i], false, outLimit);
      outLimit -= linesShown;
    }
    if (Options.OutLinesColor != null && Writer != null)
      Writer.ResetColors();

    ShowLine(Options.StartOfDiffExp);
    if (Options.ExpLinesColor != null && Writer != null)
      Writer.ForegroundColor = (ConsoleColor)Options.ExpLinesColor;
    var expElements = expElement.Elements().ToArray();
    int expLimit = linesLimit;
    for (int i = fromIndex; i < expElements.Count(); i++)
    {
      if (i - fromIndex > expLimit)
        break;
      var linesShown = ShowXmlElement(expElements[i], true, expLimit);
      expLimit -= linesShown;
    }
    if (Options.ExpLinesColor != null && Writer != null)
      Writer.ResetColors();

    ShowLine(Options.EndOfDiffs);
  }


  /// <summary>
  /// Helper method to show Xml element
  /// </summary>
  /// <param name="xmlElement">Element to show</param>
  /// <param name="isExpected">
  ///   true if lines belong to "expected" file, false if belong to "output" files, or null if none or both
  /// </param>
  /// <param name="linesLimit">The actual limit of lines to shown</param>
  /// <returns>the number of lines shown</returns>
  protected virtual int ShowXmlElement(XElement xmlElement, bool? isExpected = null, int linesLimit = 0)
  {
    var lines = ToStrings(xmlElement);
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

  /// <summary>
  /// Helper method to convert xml element to a collection of lines
  /// </summary>
  /// <param name="xmlElement">Element to convert</param>
  /// <returns>Collection of lines</returns>
  protected virtual string[] ToStrings(XElement xmlElement)
  {
    using (var stringWriter = new StringWriter())
    {
      var xmlWriter = new XmlTextWriter(stringWriter);
      xmlWriter.Formatting = Formatting.Indented;
      (xmlElement as IXmlSerializable).WriteXml(xmlWriter);
      var str = stringWriter.ToString();
      str = str.Replace("\r\n", "\n");;
      var lines = str.Split("\n");
      return lines;
    }
  }
}