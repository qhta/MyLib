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

  /// <summary>
  /// Get <c>AdjustRightIndent</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.AdjustRightIndent GetAdjustRightIndent(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.AdjustRightIndent ??= new AdjustRightIndent();
  }

  /// <summary>
  /// Get <c>AutoSpaceDE</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.AutoSpaceDE GetAutoSpaceDE(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.AutoSpaceDE ??= new AutoSpaceDE();
  }

  /// <summary>
  /// Get <c>AutoSpaceDN</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.AutoSpaceDN GetAutoSpaceDN(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.AutoSpaceDN ??= new AutoSpaceDN();
  }

  /// <summary>
  /// Get <c>ContextualSpacing</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.ContextualSpacing GetContextualSpacing(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.ContextualSpacing ??= new ContextualSpacing();
  }

  /// <summary>
  /// Get <c>FrameProperties</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.FrameProperties GetFrameProperties(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.FrameProperties ??= new FrameProperties();
  }

  /// <summary>
  /// Get <c>Indentation</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.Indentation GetIndentation(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.Indentation ??= new Indentation();
  }

  /// <summary>
  /// Get <c>Justification</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.Justification GetJustification(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.Justification ??= new Justification();
  }


  /// <summary>
  /// Get <c>KeepLines</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.KeepLines GetKeepLines(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.KeepLines ??= new KeepLines();
  }

  /// <summary>
  /// Get <c>KeepNext</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.KeepNext GetKeepNext(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.KeepNext ??= new KeepNext();
  }

  /// <summary>
  /// Get <c>Kinsoku</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.Kinsoku GetKinsoku(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.Kinsoku ??= new Kinsoku();
  }

  /// <summary>
  /// Get <c>MirrorIndents</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.MirrorIndents GetMirrorIndents(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.MirrorIndents ??= new MirrorIndents();
  }

  /// <summary>
  /// Get <c>NumberingProperties</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.NumberingProperties GetNumberingProperties(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.NumberingProperties ??= new NumberingProperties();
  }

  /// <summary>
  /// Get <c>OutlineLevel</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.OutlineLevel GetOutlineLevel(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.OutlineLevel ??= new OutlineLevel();
  }

  /// <summary>
  /// Get <c>OverflowPunctuation</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.OverflowPunctuation GetOverflowPunctuation(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.OverflowPunctuation ??= new OverflowPunctuation();
  }

  /// <summary>
  /// Get <c>PageBreakBefore</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.PageBreakBefore GetPageBreakBefore(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.PageBreakBefore ??= new PageBreakBefore();
  }

  /// <summary>
  /// Get <c>ParagraphBorders</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.ParagraphBorders GetParagraphBorders(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.ParagraphBorders ??= new ParagraphBorders();
  }

  /// <summary>
  /// Get <c>Shading</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.Shading GetShading(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.Shading ??= new Shading();
  }

  /// <summary>
  /// Get <c>SpacingBetweenLines</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.SpacingBetweenLines GetSpacingBetweenLines(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.SpacingBetweenLines ??= new SpacingBetweenLines();
  }
  
  /// <summary>
  /// Get <c>SuppressAutoHyphens</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.SuppressAutoHyphens GetSuppressAutoHyphens(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.SuppressAutoHyphens ??= new SuppressAutoHyphens();
  }

  /// <summary>
  /// Get <c>SuppressLineNumbers</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.SuppressLineNumbers GetSuppressLineNumbers(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.SuppressLineNumbers ??= new SuppressLineNumbers();
  }

  /// <summary>
  /// Get <c>SuppressOverlap</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.SuppressOverlap GetSuppressOverlap(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.SuppressOverlap ??= new SuppressOverlap();
  }

  /// <summary>
  /// Get <c>Tabs</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.Tabs GetTabs(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.Tabs ??= new Tabs();
  }

  /// <summary>
  /// Get <c>TextAlignment</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.TextAlignment GetTextAlignment(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.TextAlignment ??= new TextAlignment();
  }

  /// <summary>
  /// Get <c>TextBoxTightWrap</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.TextBoxTightWrap GetTextBoxTightWrap(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.TextBoxTightWrap ??= new TextBoxTightWrap();
  }

  /// <summary>
  /// Get <c>TextDirection</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.TextDirection GetTextDirection(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.TextDirection ??= new TextDirection();
  }

  /// <summary>
  /// Get <c>TopLinePunctuation</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.TopLinePunctuation GetTopLinePunctuation(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.TopLinePunctuation ??= new TopLinePunctuation();
  }

  /// <summary>
  /// Get <c>WidowControl</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.WidowControl GetWidowControl(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.WidowControl ??= new WidowControl();
  }

  /// <summary>
  /// Get <c>WordWrap</c> element or create a new one.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.WordWrap GetWordWrap(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.WordWrap ??= new WordWrap();
  }

}
