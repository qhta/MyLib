using System;
using System.Windows;
using System.Windows.Documents;
using System.Xml;

namespace MyLib.WPFControls
{
  public class FlowDocumentTranslator
  {
    private Double fFontSize = 10;
    private HorizontalAlignment hAlignment = HorizontalAlignment.Left;

    public Double DefaultFontSize { get { return fFontSize; } set { fFontSize = value; } }
    public HorizontalAlignment DefaultAlignment { get { return hAlignment; } set { hAlignment = value; } }

    public FlowDocument CreateDocument(string aText)
    {
      if (aText.StartsWith("<") && aText.EndsWith(">"))
        return CreateRichDocument(aText);
      else
        return CreateSimpleDocument(aText);
    }

    public FlowDocument CreateSimpleDocument(string aText)
    {
      Run aRun = new Run(aText);
      Paragraph newParagraph = new Paragraph(aRun);
      AdjustParagraph(newParagraph);
      return new FlowDocument(newParagraph);
    }

    private void AdjustParagraph(Paragraph newParagraph)
    {
      switch (hAlignment)
      {
        case HorizontalAlignment.Left:
          newParagraph.TextAlignment = TextAlignment.Left;
          break;
        case HorizontalAlignment.Center:
          newParagraph.TextAlignment = TextAlignment.Center;
          break;
        case HorizontalAlignment.Right:
          newParagraph.TextAlignment = TextAlignment.Right;
          break;
        case HorizontalAlignment.Stretch:
          newParagraph.TextAlignment = TextAlignment.Justify;
          break;
      }
      newParagraph.Padding = new Thickness(0);
      newParagraph.Margin = new Thickness(0);
    }

    public FlowDocument CreateRichDocument(string aText)
    {
      if (!(aText.StartsWith("<p>") && aText.EndsWith("</p>")))
        aText = "<p>" + aText + "</p>";
      aText = "<Text>" + aText + "</Text>";
      XmlParserContext aContext = new XmlParserContext(null, null, null, XmlSpace.Preserve);
      XmlReader aReader = new XmlTextReader(aText, XmlNodeType.Element, aContext);
      XmlDocument aDocument = new XmlDocument();
      aDocument.Load(aReader);
      FlowDocument result = new FlowDocument();
      foreach (XmlNode aElement in aDocument.DocumentElement.ChildNodes)
      {
        Block aBlock = NewFlowBlock(aElement);
        if (aBlock!=null)
          result.Blocks.Add(aBlock);
      }
      return result;
    }

    private Block NewFlowBlock(XmlNode aNode)
    {
      if (aNode.NodeType == XmlNodeType.Element)
      {
        XmlElement aElement = aNode as XmlElement;
        switch (aElement.Name)
        {
          case "p":
            Paragraph newParagraph = new Paragraph();
            foreach (XmlNode childNode in aElement.ChildNodes)
            {
              Inline aInline = NewFlowInline(childNode);
              if (aInline!=null)
                newParagraph.Inlines.Add(aInline);
            }
            AdjustParagraph(newParagraph);
            return newParagraph;
          default:
            return null;
        }
      }
      else
        return null;
    }

    private Inline NewFlowInline(XmlNode aNode)
    {
      if (aNode.NodeType == XmlNodeType.Element)
      {
        XmlElement aElement = aNode as XmlElement;
        switch (aElement.Name)
        {
          case "b":
            return NewSpan(aElement, new Bold());
          case "i":
            return NewSpan(aElement, new Italic());
          case "u":
            return NewSpan(aElement, new Underline());
          case "sup":
            return NewSpan(aElement, BaselineAlignment.Superscript);
          case "sub":
            return NewSpan(aElement, BaselineAlignment.Subscript);
          default:
            return null;
        }
      }
      else if (aNode.NodeType == XmlNodeType.Text)
      {
        return new Run(aNode.Value);
      }
      else if (aNode.NodeType == XmlNodeType.SignificantWhitespace)
      {
        return new Run(aNode.Value);
      }
      else
        return null;
    }

    private Span NewSpan(XmlElement aElement, Span newSpan)
    {
      foreach (XmlNode childNode in aElement.ChildNodes)
      {
        Inline aInline = NewFlowInline(childNode);
        newSpan.Inlines.Add(aInline);
      }
      return newSpan;
    }

    private Span NewSpan(XmlElement aElement, BaselineAlignment bAlignment)
    {
      Span newSpan = new Span();
      newSpan.BaselineAlignment = bAlignment;
      newSpan.FontSize = fFontSize*0.8;
      return NewSpan(aElement, newSpan);
    }

  }
}
