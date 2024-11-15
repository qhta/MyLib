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
  /// Get <c>KeepLines</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetKeepLines(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.KeepLines != null && paragraphProperties.KeepLines.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>KeepLines</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetKeepLines(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.KeepLines = value ? new KeepLines() : null;
  }

  /// <summary>
  /// Get <c>KeepNext</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetKeepNext(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.KeepNext != null && paragraphProperties.KeepNext.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>KeepNext</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetKeepNext(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.KeepNext = value ? new KeepNext() : null;
  }

  /// <summary>
  /// Get <c>Kinsoku</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetKinsoku(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.Kinsoku != null && paragraphProperties.Kinsoku.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>Kinsoku</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetKinsoku(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.Kinsoku = value ? new Kinsoku() : null;
  }

  /// <summary>
  /// Get <c>MirrorIndents</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetMirrorIndents(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.MirrorIndents != null && paragraphProperties.MirrorIndents.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>MirrorIndents</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetMirrorIndents(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.MirrorIndents = value ? new MirrorIndents() : null;
  }

  /// <summary>
  /// Get <c>AdjustRightIndent</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetAdjustRightIndent(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.AdjustRightIndent != null && paragraphProperties.AdjustRightIndent.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>AdjustRightIndent</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetAdjustRightIndent(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.AdjustRightIndent = value ? new AdjustRightIndent() : null;
  }

  /// <summary>
  /// Get <c>AutoSpaceDE</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetAutoSpaceDE(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.AutoSpaceDE != null && paragraphProperties.AutoSpaceDE.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>AutoSpaceDE</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetAutoSpaceDE(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.AutoSpaceDE = value ? new AutoSpaceDE() : null;
  }

  /// <summary>
  /// Get <c>AutoSpaceDN</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetAutoSpaceDN(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.AutoSpaceDN != null && paragraphProperties.AutoSpaceDN.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>AutoSpaceDN</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetAutoSpaceDN(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.AutoSpaceDN = value ? new AutoSpaceDN() : null;
  }

  /// <summary>
  /// Get <c>ContextualSpacing</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetContextualSpacing(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.ContextualSpacing != null && paragraphProperties.ContextualSpacing.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>ContextualSpacing</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetContextualSpacing(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.ContextualSpacing = value ? new ContextualSpacing() : null;
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
  /// Set <c>FrameProperties</c> element.
  /// If value to set is null, the element will be removed.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value"></param>
  /// <returns></returns>
  public static void SetFrameProperties(this DXW.ParagraphProperties paragraphProperties, DXW.FrameProperties? value)
  {
    var frameProperties = paragraphProperties.FrameProperties;
    if (value == null)
    {
      if (frameProperties != null)
        frameProperties.Remove();
      return;
    }
    paragraphProperties.FrameProperties = value;
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
  /// Set <c>Indentation</c> element.
  /// If value to set is null, the element will be removed.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">Value to set</param>
  /// <returns></returns>
  public static void SetIndentation(this DXW.ParagraphProperties paragraphProperties, DXW.Indentation? value)
  {
    var Indentation = paragraphProperties.Indentation;
    if (value == null)
    {
      if (Indentation != null)
        Indentation.Remove();
      return;
    }
    paragraphProperties.Indentation = value;
  }

  /// <summary>
  /// Get <c>JustificationValues</c> enumeration attribute value.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.JustificationValues? GetJustification(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.Justification?.Val?.Value;
  }

  /// <summary>
  /// Set <c>JustificationValues</c> enumeration attribute value.
  /// If value to set is null, the element will be removed.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">Value to set</param>
  /// <returns></returns>
  public static void GetJustification(this DXW.ParagraphProperties paragraphProperties, DXW.JustificationValues? value)
  {
    var justification = paragraphProperties.Justification;
    if (value == null)
    {
      if (justification != null)
        justification.Remove();
      return;
    }
    if (justification == null)
    {
      justification = new Justification();
      paragraphProperties.Append(justification);
    }
    justification.Val = value;
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
  /// Set <c>NumberingProperties</c> element.
  /// If value to set is null, the element will be removed.
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="numberingInstanceId">Value to set</param>
  /// <returns></returns>
  public static void SetNumbering(this DXW.ParagraphProperties paragraphProperties,int? numberingInstanceId)
  {
    var numberingProperties = paragraphProperties.NumberingProperties;
    if (numberingInstanceId == null)
    {
      if (numberingProperties != null)
        numberingProperties.Remove();
      return;
    }
    if (numberingProperties == null)
    {
      numberingProperties = new NumberingProperties();
      paragraphProperties.Append(numberingProperties);
    }
    int id = (int)numberingInstanceId;
    numberingProperties.NumberingId = new NumberingId { Val = id };
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
  /// Get <c>OverflowPunctuation</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetOverflowPunctuation(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.OverflowPunctuation != null && paragraphProperties.OverflowPunctuation.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>OverflowPunctuation</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetOverflowPunctuation(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.OverflowPunctuation = value ? new OverflowPunctuation() : null;
  }

  /// <summary>
  /// Get <c>PageBreakBefore</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetPageBreakBefore(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.PageBreakBefore != null && paragraphProperties.PageBreakBefore.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>PageBreakBefore</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetPageBreakBefore(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.PageBreakBefore = value ? new PageBreakBefore() : null;
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
  /// Get <c>SuppressAutoHyphens</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetSuppressAutoHyphens(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.SuppressAutoHyphens != null && paragraphProperties.SuppressAutoHyphens.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>SuppressAutoHyphens</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetSuppressAutoHyphens(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.SuppressAutoHyphens = value ? new SuppressAutoHyphens() : null;
  }

  /// <summary>
  /// Get <c>SuppressLineNumbers</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetSuppressLineNumbers(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.SuppressLineNumbers != null && paragraphProperties.SuppressLineNumbers.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>SuppressLineNumbers</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetSuppressLineNumbers(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.SuppressLineNumbers = value ? new SuppressLineNumbers() : null;
  }

  /// <summary>
  /// Get <c>SuppressOverlap</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetSuppressOverlap(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.SuppressOverlap != null && paragraphProperties.SuppressOverlap.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>SuppressOverlap</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetSuppressOverlap(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.SuppressOverlap = value ? new SuppressOverlap() : null;
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
  /// Get <c>TextAlignment</c> enumeration attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.VerticalTextAlignmentValues? GetTextAlignment(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.TextAlignment?.Val?.Value;
  }

  /// <summary>
  /// Set <c>TextAlignment</c> enumeration attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetTextAlignment(this DXW.ParagraphProperties paragraphProperties, DXW.VerticalTextAlignmentValues? value)
  {
    paragraphProperties.TextAlignment =
      value != null ? new TextAlignment { Val = new DX.EnumValue<VerticalTextAlignmentValues>(value) } : null;
  }

  /// <summary>
  /// Get <c>TextBoxTightWrap</c> enumeration attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.TextBoxTightWrapValues? GetTextBoxTightWrap(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.TextBoxTightWrap?.Val?.Value;
  }

  /// <summary>
  /// Set <c>TextBoxTightWrap</c> enumeration attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetTextBoxTightWrap(this DXW.ParagraphProperties paragraphProperties, DXW.TextBoxTightWrapValues? value)
  {
    paragraphProperties.TextBoxTightWrap =
      value != null ? new TextBoxTightWrap { Val = new DX.EnumValue<TextBoxTightWrapValues>(value) } : null;
  }

  /// <summary>
  /// Get <c>TextDirection</c> enumeration attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static DXW.TextDirectionValues? GetTextDirection(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.TextDirection?.Val?.Value;
  }

  /// <summary>
  /// Set <c>TextDirection</c> enumeration attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetTextDirection(this DXW.ParagraphProperties paragraphProperties, DXW.TextDirectionValues? value)
  {
    paragraphProperties.TextDirection = 
      value!=null ? new TextDirection { Val = new DX.EnumValue<DXW.TextDirectionValues>(value) } : null;
  }

  /// <summary>
  /// Get <c>TopLinePunctuation</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetTopLinePunctuation(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.TopLinePunctuation != null && paragraphProperties.TopLinePunctuation.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>TopLinePunctuation</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetTopLinePunctuation(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.TopLinePunctuation = value ? new TopLinePunctuation() : null;
  }

  /// <summary>
  /// Get <c>WidowControl</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetWidowControl(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.WidowControl != null && paragraphProperties.WidowControl.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>WidowControl</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetWidowControl(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.WidowControl = value ? new WidowControl() : null;
  }

  /// <summary>
  /// Get <c>WordWrap</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <returns></returns>
  public static bool GetWordWrap(this DXW.ParagraphProperties paragraphProperties)
  {
    return paragraphProperties.WordWrap != null && paragraphProperties.WordWrap.Val?.Value != false;
  }

  /// <summary>
  /// Set <c>WordWrap</c> boolean attribute value
  /// </summary>
  /// <param name="paragraphProperties">Paragraph properties to process</param>
  /// <param name="value">attribute value</param>
  /// <returns></returns>
  public static void SetWordWrap(this DXW.ParagraphProperties paragraphProperties, bool value)
  {
    paragraphProperties.WordWrap = value ? new WordWrap() : null;
  }

}
