using DocumentFormat.OpenXml.Wordprocessing;

namespace Qhta.OpenXmlTools;

/// <summary>
/// Tools for working with OpenXml Wordprocessing ParagraphProperties element.
/// </summary>
public static class ParagraphPropertiesTools
{
  /// <summary>
  /// Convert a <see cref="ParagraphProperties"/> element to a <see cref="StyleParagraphProperties"/> element.
  /// </summary>
  /// <param name="paragraphProperties">source properties</param>
  /// <returns>new style paragraph properties with cloned source elements</returns>
  public static StyleParagraphProperties ToStyleParagraphProperties(this ParagraphProperties paragraphProperties)
  {
    var styleParagraphProperties = new StyleParagraphProperties();
    if (paragraphProperties.AdjustRightIndent != null)
      styleParagraphProperties.AdjustRightIndent = (AdjustRightIndent)paragraphProperties.AdjustRightIndent.CloneNode(true);
    if (paragraphProperties.AutoSpaceDE != null)
      styleParagraphProperties.AutoSpaceDE = (AutoSpaceDE)paragraphProperties.AutoSpaceDE.CloneNode(true);
    if (paragraphProperties.AutoSpaceDN != null)
      styleParagraphProperties.AutoSpaceDN = (AutoSpaceDN)paragraphProperties.AutoSpaceDN.CloneNode(true);
    if (paragraphProperties.ContextualSpacing != null)
      styleParagraphProperties.ContextualSpacing = (ContextualSpacing)paragraphProperties.ContextualSpacing.CloneNode(true);
    if (paragraphProperties.FrameProperties != null)
      styleParagraphProperties.FrameProperties = (FrameProperties)paragraphProperties.FrameProperties.CloneNode(true);
    if (paragraphProperties.Indentation != null)
      styleParagraphProperties.Indentation = (Indentation)paragraphProperties.Indentation.CloneNode(true);
    if (paragraphProperties.Justification != null)
      styleParagraphProperties.Justification = (Justification)paragraphProperties.Justification.CloneNode(true);
    if (paragraphProperties.KeepLines != null)
      styleParagraphProperties.KeepLines = (KeepLines)paragraphProperties.KeepLines.CloneNode(true);
    if (paragraphProperties.KeepNext != null)
      styleParagraphProperties.KeepNext = (KeepNext)paragraphProperties.KeepNext.CloneNode(true);
    if (paragraphProperties.Kinsoku != null)
      styleParagraphProperties.Kinsoku = (Kinsoku)paragraphProperties.Kinsoku.CloneNode(true);
    if (paragraphProperties.MirrorIndents != null)
      styleParagraphProperties.MirrorIndents = (MirrorIndents)paragraphProperties.MirrorIndents.CloneNode(true);
    if (paragraphProperties.NumberingProperties != null)
      styleParagraphProperties.NumberingProperties = (NumberingProperties)paragraphProperties.NumberingProperties.CloneNode(true);
    if (paragraphProperties.OutlineLevel != null)
      styleParagraphProperties.OutlineLevel = (OutlineLevel)paragraphProperties.OutlineLevel.CloneNode(true);
    if (paragraphProperties.OverflowPunctuation != null)
      styleParagraphProperties.OverflowPunctuation = (OverflowPunctuation)paragraphProperties.OverflowPunctuation.CloneNode(true);
    if (paragraphProperties.PageBreakBefore != null)
      styleParagraphProperties.PageBreakBefore = (PageBreakBefore)paragraphProperties.PageBreakBefore.CloneNode(true);
    if (paragraphProperties.ParagraphBorders != null)
      styleParagraphProperties.ParagraphBorders = (ParagraphBorders)paragraphProperties.ParagraphBorders.CloneNode(true);
    if (paragraphProperties.Shading != null)
      styleParagraphProperties.Shading = (Shading)paragraphProperties.Shading.CloneNode(true);
    if (paragraphProperties.SpacingBetweenLines != null)
      styleParagraphProperties.SpacingBetweenLines = (SpacingBetweenLines)paragraphProperties.SpacingBetweenLines.CloneNode(true);
    if (paragraphProperties.SuppressAutoHyphens != null)
      styleParagraphProperties.SuppressAutoHyphens = (SuppressAutoHyphens)paragraphProperties.SuppressAutoHyphens.CloneNode(true);
    if (paragraphProperties.SuppressLineNumbers != null)
      styleParagraphProperties.SuppressLineNumbers = (SuppressLineNumbers)paragraphProperties.SuppressLineNumbers.CloneNode(true);
    if (paragraphProperties.SuppressOverlap != null)
      styleParagraphProperties.SuppressOverlap = (SuppressOverlap)paragraphProperties.SuppressOverlap.CloneNode(true);
    if (paragraphProperties.Tabs != null)
      styleParagraphProperties.Tabs = (Tabs)paragraphProperties.Tabs.CloneNode(true);
    if (paragraphProperties.TextAlignment != null)
      styleParagraphProperties.TextAlignment = (TextAlignment)paragraphProperties.TextAlignment.CloneNode(true);
    if (paragraphProperties.TextBoxTightWrap != null)
      styleParagraphProperties.TextBoxTightWrap = (TextBoxTightWrap)paragraphProperties.TextBoxTightWrap.CloneNode(true);
    if (paragraphProperties.TextDirection != null)
      styleParagraphProperties.TextDirection = (TextDirection)paragraphProperties.TextDirection.CloneNode(true);
    if (paragraphProperties.TopLinePunctuation != null)
      styleParagraphProperties.TopLinePunctuation = (TopLinePunctuation)paragraphProperties.TopLinePunctuation.CloneNode(true);
    if (paragraphProperties.WidowControl != null)
      styleParagraphProperties.WidowControl = (WidowControl)paragraphProperties.WidowControl.CloneNode(true);
    if (paragraphProperties.WordWrap != null)
      styleParagraphProperties.WordWrap = (WordWrap)paragraphProperties.WordWrap.CloneNode(true);

    return styleParagraphProperties;
  }
}
