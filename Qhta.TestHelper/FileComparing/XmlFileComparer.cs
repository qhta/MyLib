using System.Diagnostics;
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
  public XmlFileComparer(FileCompareOptions options, ITraceTextWriter? writer = null) : base(options, writer)
  {
  }

  /// <summary>
  /// Implemented method of file comparison.
  /// Simply reads out whole xml documents and invokes <see cref="CompareXmlDocuments"/>.
  /// </summary>
  /// <param name="recFilename">Received content filename</param>
  /// <param name="expFilename">Expected content filename</param>
  /// <returns>true is both files are equal</returns>
  public override bool CompareFiles(string recFilename, string expFilename)
  {
    XDocument outXml;
    XDocument expXml;
    diffCount = 0;
    using (TextReader aReader = File.OpenText(recFilename))
      outXml = XDocument.Load(aReader);

    using (TextReader aReader = File.OpenText(expFilename))
      expXml = XDocument.Load(aReader);

    return CompareXmlDocuments(outXml, expXml);
  }

  /// <summary>
  /// Main method to compare Xml documents. 
  /// Root elements are compared through CompareXmlElements.
  /// </summary>
  /// <param name="recDocument">Received Xml document</param>
  /// <param name="expDocument">Expected Xml document</param>
  /// <returns>true if both documents are equal</returns>
  protected virtual bool CompareXmlDocuments(XDocument recDocument, XDocument expDocument)
  {
    bool shown = false;

    if (recDocument?.Root == null && expDocument?.Root == null)
      return true;
    if (recDocument?.Root == null || expDocument?.Root == null)
      return false;

    var result = CompareXmlElements(recDocument.Root, expDocument.Root, true, ref shown);
    var areEqual = result == CompResult.AreEqual;
    if (!areEqual && !shown)
    {
      throw new InvalidOperationException("XmlFileComparer have not shown unequal elements");
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
  /// <param name="recElement">Received Xml element</param>
  /// <param name="expElement">Expected Xml element</param>
  /// <param name="showUnequal">If unequal element should be shown</param>
  /// <param name="shown">If unequal element were shown</param>
  /// <returns>true if both elements are equal</returns>
  protected virtual CompResult CompareXmlElements(XElement recElement, XElement expElement, bool showUnequal, ref bool shown)
  {
    if (recElement.NodeType != expElement.NodeType
        || !AreEqual(recElement.Name.NamespaceName, expElement.Name.Namespace.NamespaceName)
        || !AreEqual(recElement.Name.LocalName, expElement.Name.LocalName))
    {
      if (showUnequal)
      {
        ShowUnequalElements(recElement, expElement, 1);
        shown = true;
      }
      return CompResult.NameDiff;
    }
    if (!CompareXmlAttributes(recElement, expElement))
    {
      if (showUnequal)
      {
        ShowUnequalElements(recElement, expElement, 1);
        shown = true;
      }
      return CompResult.AttrDiff;
    }
    var recElementsList = recElement.Elements().ToList();
    var expElementsList = expElement.Elements().ToList();
    Dictionary<string, XElement> recElementsDir = null!;
    Dictionary<string, XElement> expElementsDir = null!;
    // Dictionaries of compared child element can be used only if the element names are unique.
    var itemsAreUnique = TryCreateDictionary(recElementsList, out recElementsDir) && TryCreateDictionary(expElementsList, out expElementsDir);
    if (itemsAreUnique)
      return CompareXmlElements(recElement, expElement,
        recElementsDir, expElementsDir, showUnequal, ref shown);
    else
      return CompareXmlElements(recElement, expElement,
        recElementsList, expElementsList, showUnequal, ref shown);
  }

  /// <summary>
  /// Creates dictionary of elements using element names as keys.
  /// Returs true if, and only if, all elements have unique names.
  /// </summary>
  /// <param name="items">Input enumerations of XElements</param>
  /// <param name="itemsDictionary">Output dictionary of XElements</param>
  /// <returns></returns>
  private static bool TryCreateDictionary(IEnumerable<XElement> items, out Dictionary<string, XElement> itemsDictionary)
  {
    bool result = true;
    itemsDictionary = new Dictionary<string, XElement>();
    foreach (XElement item in items)
    {
      var itemName = item.Name.NamespaceName + ":" + item.Name.LocalName;
      if (!itemsDictionary.ContainsKey(itemName))
        itemsDictionary.Add(itemName, item);
      else
        result = false;
    }
    return result;
  }

  /// <summary>
  /// Compares local names of elements.
  /// </summary>
  /// <param name="recElement"></param>
  /// <param name="expElement"></param>
  /// <returns></returns>
  private static int CmpXElements(XElement recElement, XElement expElement)
  {
    return recElement.Name.LocalName.CompareTo(expElement.Name.LocalName);
  }

  /// <summary>
  /// Compares dictionaries of items of recElement and expElement
  /// </summary>
  /// <param name="recElement">Received Xml element</param>
  /// <param name="expElement">Expected Xml element</param>
  /// <param name="recItemsDict">List of child elements of out element</param>
  /// <param name="expItemsDict">List of child elements of exp element</param>
  /// <param name="showUnequal">If unequal element should be shown</param>
  /// <param name="shown">If unequal element were shown</param>
  /// <returns>true if both elements are equal</returns>
  protected virtual CompResult CompareXmlElements(XElement recElement, XElement expElement,
    Dictionary<string, XElement> recItemsDict, Dictionary<string, XElement> expItemsDict, bool showUnequal, ref bool shown)
  {
    var result = CompResult.AreEqual;
    int recItemsCount = recItemsDict.Count();
    int expItemsCount = expItemsDict.Count();
    bool areEqual = recItemsCount == expItemsCount;
    List<XElement> missingElements = new();
    List<XElement> checkedElements = new();
    foreach (var item in expItemsDict)
    {
      var expItem = item.Value;
      if (recItemsDict.TryGetValue(item.Key, out var outItem))
      {
        result = CompareXmlElements(outItem, expItem, true, ref shown);
        checkedElements.Add(outItem);
        return result;
      }
      else
        missingElements.Add(expItem);
    }
    
    bool showEndOfDiffs = false;
    if (missingElements.Count > 0)
    {
      if (showUnequal)
      {
        ShowMissingElements(missingElements);
        shown = true;
        showEndOfDiffs |= true;
      }
      result = CompResult.ElementCountDiff;
    }
    if (recItemsCount > checkedElements.Count)
    {
      if (showUnequal)
      {
        List<XElement> excessiveElements = recItemsDict.Values.ToList();
        foreach (var item in checkedElements)
          excessiveElements.Remove(item);
        ShowExcessiveElements(excessiveElements);
        shown = true;
        showEndOfDiffs |= true;
      }
      result = CompResult.ElementCountDiff;
    }
    if (showEndOfDiffs)
      ShowLine(Options.EndOfDiffs);
    return result;
  }

  /// <summary>
  /// Compares items lists of recElement and expElement.
  /// </summary>
  /// <param name="recElement">Received Xml element</param>
  /// <param name="expElement">Expected Xml element</param>
  /// <param name="outItemsList">List of child elements of out element</param>
  /// <param name="expItemsList">List of child elements of exp element</param>
  /// <param name="showUnequal">If unequal element should be shown</param>
  /// <param name="shown">If unequal element were shown</param>
  /// <returns>true if both elements are equal</returns>
  protected virtual CompResult CompareXmlElements(XElement recElement, XElement expElement,
    List<XElement> outItemsList, List<XElement> expItemsList, bool showUnequal, ref bool shown)
  {
    int outItemsCount = outItemsList.Count();
    int expItemsCount = expItemsList.Count();
    bool areEqual = outItemsCount == expItemsCount;
    for (int i = 0; i < Math.Min(outItemsCount, expItemsCount); i++)
    {
      var result = CompareXmlElements(outItemsList[i], expItemsList[i], outItemsCount == expItemsCount, ref shown);
      if (result != CompResult.AreEqual)
      {
        areEqual = false;
        if (outItemsCount == expItemsCount)
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
              ShowUnequalItemElements(recElement, expElement, fromIndex, 20);
              shown = true;
            }
            else
            {
              ShowUnequalElements(outItemsList.ToArray()[i..^0], expItemsList.ToArray()[i..^0]);
              shown = true;
            }
          }
          return CompResult.ElementDiff;
        }
      }
    }
    if (!areEqual)
    {
      if (outItemsCount != expItemsCount)
      {
        ShowUnequalElements(recElement, expElement);
        shown = true;
        return CompResult.ElementCountDiff;
      }
      return CompResult.ElementDiff;
    }
    if (recElement.Value != expElement.Value)
    {
      ShowUnequalElements(recElement, expElement);
      shown = true;
      return CompResult.ValueDiff;
    }
    return CompResult.AreEqual;
  }

  /// <summary>
  /// Method to compare Xml attributes of two Xml elements.
  /// </summary>
  /// <param name="recElement">Received Xml element</param>
  /// <param name="expElement">Expected Xml element</param>
  /// <returns>true if attributes of both elements are equal</returns>
  protected virtual bool CompareXmlAttributes(XElement recElement, XElement expElement)
  {
    var recAttributes = recElement.Attributes().Where(item => !IgnoreAttribute(item)).ToList();
    var expAttributes = expElement.Attributes().Where(item => !IgnoreAttribute(item)).ToList();
    if (recAttributes.Count != expAttributes.Count)
    {
      return false;
    }
    if (Options.IgnoreAttributesOrder)
    {
      recAttributes.Sort(CompareAttrName);
      expAttributes.Sort(CompareAttrName);
    }
    for (int i = 0; i < recAttributes.Count; i++)
      if (!CompareXmlAttribute(recAttributes[i], expAttributes[i]))
      {
        return false;
      }
    return true;
  }

  /// <summary>
  /// Helper function to filter out NamespaceDeclarations and Ignorable attributes.
  /// </summary>
  /// <param name="attribute"></param>
  /// <returns></returns>
  private bool IgnoreAttribute(XAttribute attribute)
  {
    if (attribute.IsNamespaceDeclaration)
      return true;
    if (Options.IgnoreIgnorableAttribute && attribute.Name.LocalName=="Ignorable")
      return true;
    return false;
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
    var result = String.Compare((name1.Namespace ?? "").ToString(), (name2.Namespace ?? "").ToString(), StringComparison.Ordinal);
    if (result != 0)
      return result;
    result = String.Compare(name1.LocalName.ToString(), name2.LocalName.ToString(), StringComparison.Ordinal);
    return result;
  }

  /// <summary>
  /// Helper method to compare two attributes. Attribute namespaces, localnames and values are compared.
  /// </summary>
  /// <param name="recAttribute">First attribute to compare</param>
  /// <param name="expAttribute">Second attribute to compare</param>
  /// <returns>true if both attributes are euqal</returns>
  protected virtual bool CompareXmlAttribute(XAttribute recAttribute, XAttribute expAttribute)
  {
    if (!AreEqual(recAttribute.Name.NamespaceName, expAttribute.Name.NamespaceName))
      return false;
    if (!AreEqual(recAttribute.Name.LocalName, expAttribute.Name.LocalName))
      return false;
    var outValue = recAttribute.Value ?? "";
    var expValue = expAttribute.Value ?? "";
    if (!AreEqual(outValue, expValue))
      return false;
    return true;
  }

  /// <summary>
  /// Helper method to show collection of missing elements.
  /// </summary>
  /// <param name="expElements">Collection of missing Xml elements</param>
  protected virtual void ShowMissingElements(IEnumerable<XElement> expElements)
  {
    var linesLimit = Options.SyncLimit;
    ShowLine(Options.StartOfDiffMis);
    if (Options.MisLinesColor != null && Writer != null)
      Writer.ForegroundColor = (ConsoleColor)Options.MisLinesColor;
    var expLimit = expElements.Count();
    foreach (var expElement in expElements)
    {
      var linesShown = ShowXmlElement(expElement, Options.MisLinesColor, linesLimit);
      expLimit -= linesShown;
      if (expLimit < 0)
      {
        if (Writer != null)
          Writer.WriteLine("...");
        break;
      }
    }
    if (Options.ExpLinesColor != null && Writer != null)
      Writer.ResetColors();
  }

  /// <summary>
  /// Helper method to show collection of excessive elements.
  /// </summary>
  /// <param name="recElements">Collection of excessive Xml elements</param>
  protected virtual void ShowExcessiveElements(IEnumerable<XElement> recElements)
  {
    var linesLimit = Options.SyncLimit;
    ShowLine(Options.StartOfDiffExc);
    if (Options.ExcLinesColor != null && Writer != null)
      Writer.ForegroundColor = (ConsoleColor)Options.ExcLinesColor;
    var expLimit = recElements.Count();
    foreach (var recElement in recElements)
    {
      var linesShown = ShowXmlElement(recElement, Options.ExcLinesColor, linesLimit);
      expLimit -= linesShown;
      if (expLimit < 0)
      {
        if (Writer != null)
          Writer.WriteLine("...");
        break;
      }
    }
    if (Options.RecLinesColor != null && Writer != null)
      Writer.ResetColors();
  }

  /// <summary>
  /// Helper method to show unequal elements.
  /// </summary>
  /// <param name="recElement">Received Xml element</param>
  /// <param name="expElement">Expected Xml element</param>
  /// <param name="linesLimit">Limit of content lines to show 
  /// (default 0 changed to <see cref="FileCompareOptions.SyncLimit"/>)</param>
  protected virtual void ShowUnequalElements(XElement recElement, XElement expElement, int linesLimit = 0)
  {
    if (linesLimit == 0)
      linesLimit = Options.SyncLimit;
    ShowLine(Options.StartOfDiffRec);
    if (Options.RecLinesColor != null && Writer != null)
      Writer.ForegroundColor = (ConsoleColor)Options.RecLinesColor;
    ShowXmlElement(recElement, Options.RecLinesColor, linesLimit);
    if (Options.RecLinesColor != null && Writer != null)
      Writer.ResetColors();
    ShowLine(Options.StartOfDiffExp);
    if (Options.ExpLinesColor != null && Writer != null)
      Writer.ForegroundColor = (ConsoleColor)Options.ExpLinesColor;
    ShowXmlElement(expElement, Options.ExpLinesColor, linesLimit);
    if (Options.ExpLinesColor != null && Writer != null)
      Writer.ResetColors();
    ShowLine(Options.EndOfDiffs);
  }

  /// <summary>
  /// Helper method to show unequal elements. Two collections of elements are taken.
  /// Limits from <see cref="FileCompareOptions.SyncLimit"/> is applied.
  /// </summary>
  /// <param name="recElements">Collection of Received Xml elements</param>
  /// <param name="expElements">Collection of expected Xml elements</param>
  /// <param name="linesLimit">Limit of content lines to show 
  /// (default 0 changed to <see cref="FileCompareOptions.SyncLimit"/>)</param>
  protected virtual void ShowUnequalElements(XElement[] recElements, XElement[] expElements, int linesLimit = 0)
  {
    if (linesLimit == 0)
      linesLimit = Options.SyncLimit;
    if (linesLimit == 0)
      linesLimit = int.MaxValue;
    var recLinesLimit = linesLimit;
    ShowLine(Options.StartOfDiffRec);
    if (Options.RecLinesColor != null && Writer != null)
      Writer.ForegroundColor = (ConsoleColor)Options.RecLinesColor;
    for (int i = 0; i < recLinesLimit; i++)
    {
      if (i >= recElements.Count())
        break;
      var linesShown = ShowXmlElement(recElements[i], Options.RecLinesColor, recLinesLimit);
      recLinesLimit -= linesShown;
      if (recLinesLimit <= 0)
      {
        if (recLinesLimit < -1)// -1 means that "..." line was shown
          ShowLine("...");
        break;
      }
    }
    if (Options.RecLinesColor != null && Writer != null)
      Writer.ResetColors();

    ShowLine(Options.StartOfDiffExp);
    if (Options.ExpLinesColor != null && Writer != null)
      Writer.ForegroundColor = (ConsoleColor)Options.ExpLinesColor;
    var expLinesLimit = linesLimit;
    for (int i = 0; i < recLinesLimit; i++)
    {
      if (i >= expElements.Count())
        break;
      var linesShown = ShowXmlElement(expElements[i], Options.ExpLinesColor, expLinesLimit);
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
  /// Helper method to show unequal items of elements. Similar to previous but with several differeces.
  /// </summary>
  /// <param name="recElement">Collection of Received Xml elements</param>
  /// <param name="expElement">Collection of expected Xml elements</param>
  /// <param name="fromIndex">Index of the first item</param>
  /// <param name="linesLimit">Limit of content lines to show 
  /// (default 0 changed to <see cref="FileCompareOptions.SyncLimit"/>)</param>
  protected virtual void ShowUnequalItemElements(XElement recElement, XElement expElement, int fromIndex, int linesLimit = 0)
  {
    if (linesLimit == 0)
      linesLimit = Options.SyncLimit;
    ShowLine(Options.StartOfDiffRec);
    if (Options.RecLinesColor != null && Writer != null)
      Writer.ForegroundColor = (ConsoleColor)Options.RecLinesColor;
    var recElements = recElement.Elements().ToArray();
    int outLimit = linesLimit;
    for (int i = fromIndex; i < recElements.Count(); i++)
    {
      if (i - fromIndex > outLimit)
        break;
      var linesShown = ShowXmlElement(recElements[i], Options.RecLinesColor, outLimit);
      outLimit -= linesShown;
    }
    if (Options.RecLinesColor != null && Writer != null)
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
      var linesShown = ShowXmlElement(expElements[i], Options.ExpLinesColor, expLimit);
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
  /// <param name="color">Color of the text (if null then lef unchanged)</param>  /// <param name="linesLimit">The actual limit of lines to shown</param>
  /// <returns>the number of lines shown</returns>
  protected virtual int ShowXmlElement(XElement xmlElement, ConsoleColor? color = null, int linesLimit = 0)
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
    ShowLines(lines, color);
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
      str = str.Replace("\r\n", "\n"); ;
      var lines = str.Split("\n");
      return lines;
    }
  }
}