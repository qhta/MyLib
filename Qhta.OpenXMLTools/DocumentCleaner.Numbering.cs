using System;

namespace Qhta.OpenXmlTools;

/// <summary>
/// A composite tool for cleaning a Wordprocessing document.
/// </summary>
public partial class DocumentCleaner
{

  /// <summary>
  /// Detect paragraphs that contain a bullet and enter a new paragraph with bullet numbering.
  /// </summary>
  /// <param name="wordDoc"></param>
  public void RepairBulletContainingParagraph(DXPack.WordprocessingDocument wordDoc)
  {
    if (VerboseLevel > 0)
      Console.WriteLine("\nRepairing paragraphs that contain bullet");
    var body = wordDoc.GetBody();
    var numbering = wordDoc.GetNumberingDefinitions();
    var abstractNumbering = body.FindMostFrequentBulletedAbstractNumbering();
    if (abstractNumbering == null)
      abstractNumbering = numbering.GetDefaultBulletedAbstractNumbering();

    int abstractNumId = abstractNumbering.AbstractNumberId!;
    var numberingStatistic = body.GetNumberingInstanceStatistics(abstractNumId);
    var numberingInstance = numberingStatistic.MostFrequent();
    DXW.ParagraphProperties? defaultParagraphProperties = null;
    if (numberingInstance != null)
      defaultParagraphProperties = numberingInstance.Parent as DXW.ParagraphProperties;
    if (defaultParagraphProperties == null)
    {
      if (numberingInstance == null)
      {
        numberingInstance = numbering.GetNumberingInstance(abstractNumId);
      }
      defaultParagraphProperties = new DXW.ParagraphProperties();
      defaultParagraphProperties.SetNumbering(numberingInstance.NumberID?.Value);
    }
    int count = 0;
    var paragraphs = wordDoc.GetBody().Descendants<DXW.Paragraph>().ToList();
    for (int i = 0; i < paragraphs.Count; i++)
    {
      var paragraph = paragraphs[i];
      //var paraText = paragraph.GetText();
      //if (paraText.Contains("Video (\u00a715.2.17)"))
      //  Debug.Assert(true);
      foreach (var run in paragraph.Elements<DXW.Run>())
      {
        var text = run.GetText();
        if (text.Contains("•"))
        {
          var textItem = run.Descendants<DXW.Text>().FirstOrDefault(t => t.Text.TrimStart().StartsWith("•"));
          if (textItem == null)
            continue;
          textItem.Text = text.Replace("•", "");
          count++;
          DXW.Paragraph? bulletedParagraph = null;
          if (paragraph.IsBulleted())
            bulletedParagraph = paragraph;
          else
          {
            bulletedParagraph = paragraph.FindPreviousNumberedParagraph();
            if (bulletedParagraph == null)
              bulletedParagraph = paragraph.FindNextNumberedParagraph();
          }
          DXW.ParagraphProperties? numberingParagraphProperties =
            bulletedParagraph?.ParagraphProperties;
          if (numberingParagraphProperties == null)
            numberingParagraphProperties = defaultParagraphProperties;
          numberingParagraphProperties = (DXW.ParagraphProperties)numberingParagraphProperties.CloneNode(true);

          var prevSibling = run.PreviousSibling();
          if (prevSibling != null && prevSibling is not DXW.ParagraphProperties && !String.IsNullOrEmpty((prevSibling as DXW.Run)?.GetText()))
          {
            var newParagraph = new DXW.Paragraph();
            newParagraph.ParagraphProperties = numberingParagraphProperties;
            var tailItems = new List<DX.OpenXmlElement>();
            tailItems.Add(run);
            var siblingItem = run.NextSibling();
            while (siblingItem != null)
            {
              tailItems.Add(siblingItem);
              siblingItem = siblingItem.NextSibling();
            }
            foreach (var item in tailItems)
            {
              item.Remove();
              newParagraph.Append(item);
            }
            paragraph.TrimEnd();
            newParagraph.TrimStart();
            newParagraph.TrimEnd();
            if (paragraph.IsEmpty())
            {
              numberingParagraphProperties?.Remove();
              paragraph.ParagraphProperties = numberingParagraphProperties;
              var priorParagraph = paragraph.PreviousSibling<DXW.Paragraph>();
              var after = priorParagraph?.ParagraphProperties?.SpacingBetweenLines?.After;
              if (after != null)
                paragraph.GetParagraphProperties().GetSpacingBetweenLines().After = after;
              foreach (var item in newParagraph.MemberElements())
              {
                item.Remove();
                paragraph.AppendChild(item);
              }
            }
            else
            {
              var priorParagraph = paragraph.PreviousSibling<DXW.Paragraph>();
              if (priorParagraph != null && priorParagraph.ParagraphProperties?.NumberingProperties != null)
                paragraph.ParagraphProperties =
                  (DXW.ParagraphProperties)priorParagraph.ParagraphProperties.CloneNode(true);
              newParagraph.TrimEnd();
              paragraph.InsertAfterSelf(newParagraph);
              paragraphs.Insert(i + 1, newParagraph);
              //if (paragraph.IsEmpty())
              //{
              //  paragraph.Remove();
              //  paragraphs.RemoveAt(i);
              //  i--;
              //}
            }
          }
          else // if it is the first run in the paragraph then do not create a new paragraph.
          {
            paragraph.ParagraphProperties = numberingParagraphProperties;
            paragraph.TrimEnd();
            i--;
          }
        }
      }
    }
    if (VerboseLevel > 0)
      Console.WriteLine($"  {count} bullet containing paragraphs changed to list items");
  }

}
