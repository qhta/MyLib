namespace Qhta.OpenXmlTools;

public partial class DocumentCleaner
{
  /// <summary>
  /// Converts all Anchors to inline graphics
  /// </summary>
  /// <param name="body"></param>
  /// <returns></returns>
  public int ConvertAnchorsToInline(DX.OpenXmlCompositeElement body)
  {
    var count = 0;
    var anchors = body.Descendants<DXDW.Anchor>().ToList();
    foreach (var anchor in anchors)
    {
      var drawing = anchor.Parent as DXW.Drawing;
      if (drawing is null)
        continue;
      var drawingRun = drawing.Parent as DXW.Run;
      if (drawingRun is null)
        continue;
      var paragraph = drawingRun.Parent as DXW.Paragraph;
      if (paragraph is null)
        continue;
      var inline = anchor.ConvertAnchorToInline();
      drawing.Anchor = null;
      drawing.Inline = inline;
      var nextParagraph = paragraph.NextSibling() as DXW.Paragraph;
      var targetParagraph = nextParagraph;
      if (nextParagraph is null)
      {
        targetParagraph = paragraph;
      }
      if (targetParagraph != null)
      {
        var textOptions = TextOptions.ParaText;
        var targetParaText = targetParagraph.GetText(textOptions);
        Debug.WriteLine($"Target Para is \"{targetParaText}\"");
        DXW.TabChar? tabChar = null;
        var targetParaMembers = targetParagraph.GetFlattenedMemberList();
        if (targetParaMembers.Any())
        {
          var firstMember = targetParaMembers.FirstOrDefault();
          var firstTabCharIndex = -1;
          if (firstMember is DXW.TabChar tabChar1)
          {
            tabChar = tabChar1;
            firstTabCharIndex = 1;
          }
          if (firstMember is DXW.Drawing drawing1 && targetParaMembers.Count>=2 && targetParaMembers[1] is DXW.TabChar tabChar2)
          {
            tabChar = tabChar2;
            firstTabCharIndex = 2;
          }
          for (int i = firstTabCharIndex + 1; i < targetParaMembers.Count; i++)
          {
            if (targetParaMembers[i] is DXW.TabChar tabChar3)
            {
              tabChar = tabChar3;
              break;
            }
          }
        }
        if (tabChar != null)
        {
          var newParagraph = targetParagraph.SplitAfter(tabChar);
          if (newParagraph != null)
            targetParagraph.InsertAfterSelf(newParagraph);

        }
        var lastRun = targetParagraph.Elements<DXW.Run>().LastOrDefault();
        if (lastRun != null)
        {
          var lastRunMember = lastRun.GetMembers().LastOrDefault();
          if (lastRunMember is not DXW.TabChar)
          {
            lastRun.Append(new DXW.TabChar());
          }
        }
        drawingRun.Remove();
        targetParagraph!.Append(drawingRun);
        paragraph.SetSpacingAfter(0, null, null);
        count++;
      }
    }
    return count;
  }
}