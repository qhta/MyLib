using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Diagnostics;

namespace MyLib.WPFControls
{
  public partial class ExtRichTextBox : RichTextBox
  {
    public ExtRichTextBox()
    {
    }

    public static readonly DependencyProperty TextProperty = DependencyProperty.Register(
      "Text",
      typeof(String),
      typeof(ExtRichTextBox)
    );

    public string Text
    {
      get { return (string)GetValue(TextProperty); }
      set { SetValue(TextProperty, value); }
    }

    public static readonly DependencyProperty AutoSizeProperty = DependencyProperty.Register(
      "AutoSize",
      typeof(Boolean),
      typeof(ExtRichTextBox)
    );

    public Boolean AutoSize
    {
      get { return (Boolean)GetValue(AutoSizeProperty); }
      set { SetValue(AutoSizeProperty, value); }
    }

    private class SizeInfo
    {
      private double fWidth;
      private double fTopOffset;
      private double fTop;
      private double fBottom;
      private double fBottomOffset;
      private double fSuperPos;
      private double fSubPos;
      private double fTopLineOffset;
      private double fLineSpacing;
      private double fLineHeight;
      private double fMaxWidth;
      private double fCurWidth;
      private int fWrappedLines;

      public SizeInfo() { }
      public SizeInfo(double aMaxWidth) { fMaxWidth = aMaxWidth; }
      public SizeInfo(SizeInfo aSize)
      {
        fMaxWidth = aSize.fMaxWidth;
        fCurWidth = aSize.fCurWidth;
      }

      public SizeInfo Clone()
      {
        SizeInfo result = new SizeInfo();
        result.fWidth = this.fWidth;
        result.fTopOffset = this.fTopOffset;
        result.fTop = this.fTop;
        result.fBottom = this.fBottom;
        result.fBottomOffset = this.fBottomOffset;
        result.fSuperPos = this.fSuperPos;
        result.fSubPos = this.fSubPos;
        result.fTopLineOffset = this.fTopLineOffset;
        result.fLineSpacing = this.fLineSpacing;
        result.fLineHeight = this.fLineHeight;
        result.fMaxWidth = this.fMaxWidth;
        result.fCurWidth = this.fCurWidth;
        result.fWrappedLines = this.fWrappedLines;
        return result;
      }

      public double Width { get { return fWidth; } set { fWidth = value; } }
      public double Height { get { return fTopOffset + fTop - fBottom + fBottomOffset; } }
      public double TopLineOffset { get { return fTopLineOffset; } set { fTopLineOffset = value; } }
      public double TopOffset { get { return fTopOffset; } set { fTopOffset = value; } }
      public double BottomOffset { get { return fBottomOffset; } set { fBottomOffset = value; } }
      public double Top { get { return fTop; } set { fTop = value; } }
      public double Bottom { get { return fBottom; } set { fBottom = value; } }
      public double SuperPos { get { return fSuperPos; } set { fSuperPos = value; } }
      public double SubPos { get { return fSubPos; } set { fSubPos = value; } }
      public double LineSpacing { get { return fLineSpacing; } set { fLineSpacing = value; } }
      public double LineHeight { get { return fLineHeight; } set { fLineHeight = value; } }
      public double MaxWidth { get { return fMaxWidth; } }
      public double CurWidth { get { return fCurWidth; } set { fCurWidth = value; } }
      public bool WrapEnabled { get { return fMaxWidth > 0; } }
      public int WrappedLines { get { return fWrappedLines; } set { fWrappedLines = value; } }
    }

    //private List<SizeInfo> fSizeInfos = new List<SizeInfo>();

    //private List<SizeInfo> SizeInfos { get { return fSizeInfos; } }
    public override void OnApplyTemplate()
    {
      base.OnApplyTemplate();
      if (Text != null)
      {
        FlowDocumentTranslator aTranslator = new FlowDocumentTranslator();
        aTranslator.DefaultFontSize = this.FontSize;
        aTranslator.DefaultAlignment = this.HorizontalContentAlignment;
        this.Document = aTranslator.CreateRichDocument(Text);
      }
      this.Document.PagePadding = new Thickness(0);
      this.Document.FontFamily = this.FontFamily;
      this.Document.FontSize = this.FontSize;
      this.Document.FontStyle = this.FontStyle;
      this.Document.FontWeight = this.FontWeight;
      this.Document.FontStretch = this.FontStretch;

      this.MinHeight = 0;
    }

    protected override Size MeasureOverride(Size constraint)
    {
      if (AutoSize && (Document != null) && (Document.Blocks.Count > 0))
      {
        Size result = new Size();
        double aWidth;
        if (constraint.Width <= MinWidth)
          aWidth = 0;
        else
          aWidth = Math.Max(0, constraint.Width - (this.Margin.Left + this.Margin.Right) - (this.Padding.Left + this.Padding.Right));

        foreach (Block aBlock in Document.Blocks)
        {
          SizeInfo aSize = new SizeInfo(aWidth);
          Size bSize = new Size();
          if (aBlock is Paragraph)
            bSize = GetParagraphSize((Paragraph)aBlock, aSize);
          else if (aBlock is List)
            bSize = GetListSize((List)aBlock, aSize);
          result.Height += bSize.Height;
          result.Width = Math.Max(result.Width, bSize.Width);
          result.Height += aSize.TopLineOffset;
        }
        this.Document.PagePadding = new Thickness(0, 0, 0, 0);
        result.Width += this.Padding.Left;
        result.Width += this.Padding.Right;
        result.Height += this.Padding.Top;
        result.Height += this.Padding.Bottom;
        result.Width += 2; // for measure errors;
        result.Width += 8; // for editing purposes;
        result.Height += 2; // for measure errors;
        return result;
      }
      else
      {
        Size result = base.MeasureOverride(constraint);
        return result;
      }
    }

    protected override Size ArrangeOverride(Size arrangeBounds)
    {
      Size aSize = base.ArrangeOverride(arrangeBounds);
      if (AutoSize && Document != null)
      {
        Document.PagePadding = new Thickness(0);
        Document.PageWidth = aSize.Width - Safe(this.Padding.Left) - Safe(this.Padding.Right);
        double aHeight = aSize.Height;// - Safe(this.Padding.Top) - Safe(this.Padding.Bottom);
        //Document.PageHeight = aHeight;
        if (VisualChildrenCount > 0)
        {
          for (int i = 0; i < VisualTreeHelper.GetChildrenCount(this); i++)
          {
            Visual childVisual = (Visual)VisualTreeHelper.GetChild(this, i);
            if (childVisual is FrameworkElement)
              (childVisual as FrameworkElement).Height = aHeight;
          }
        }
      }
      return aSize;
    }

    private Size GetParagraphSize(Paragraph aParagraph, SizeInfo aSize)
    {

      FontFamily aFontFamily = aParagraph.FontFamily;
      FontStyle aFontStyle = aParagraph.FontStyle;
      FontWeight aFontWeight = aParagraph.FontWeight;
      FontStretch aFontStretch = aParagraph.FontStretch;
      Double aFontSize = aParagraph.FontSize;

      Typeface aTypeface = new Typeface(aFontFamily, aFontStyle, aFontWeight, aFontStretch);

      aSize.Top = Math.Max(aSize.Top, aFontFamily.Baseline * aFontSize);
      aSize.Bottom = Math.Min(aSize.Bottom, -(aFontFamily.LineSpacing - aFontFamily.Baseline) * aFontSize);
      aSize.SuperPos = (aTypeface.XHeight) * aFontSize;
      aSize.SubPos = aTypeface.UnderlinePosition * aFontSize;
      aSize.LineSpacing = Math.Max(aSize.LineSpacing, aFontFamily.LineSpacing * aFontSize);
      if (aSize.TopLineOffset == 0)
        aSize.TopLineOffset = aSize.LineSpacing - aFontSize;  
      aSize.LineHeight = aParagraph.LineHeight;
      //SizeInfos.Clear();
      foreach (Inline aInline in aParagraph.Inlines)
      {
        if (aInline is LineBreak)
        {
          double lineHeight = GetLineHeight(aInline, aSize);
          aSize.TopOffset += lineHeight;
          aSize.CurWidth = 0;
        }
        else
        {
          switch (aInline.BaselineAlignment)
          {
            case BaselineAlignment.Superscript:
              aSize.TopOffset += aSize.SuperPos;
              break;
            case BaselineAlignment.Subscript:
              aSize.BottomOffset -= aSize.SubPos;
              break;
          }
          CalcInlineSize(aInline, aSize);
        }
      }

      Size result = new Size(aSize.Width, aSize.Height);
      if (aParagraph.Margin != null)
      {
        result.Width += Safe(aParagraph.Margin.Left);
        result.Width += Safe(aParagraph.Margin.Right);
      }
      if (aParagraph.Padding != null)
      {
        result.Width += Safe(aParagraph.Padding.Left);
        result.Width += Safe(aParagraph.Padding.Right);
        result.Height += Safe(aParagraph.Padding.Top);
        result.Height += Safe(aParagraph.Padding.Bottom);
      }
      if (aParagraph.PreviousBlock is Paragraph)
        result.Height += GetVerticalDistance((Paragraph)aParagraph.PreviousBlock, aParagraph, aSize);
      return result;
    }

    private Size GetListSize(List aList, SizeInfo aSize)
    {
      Size result = new Size();
      foreach (ListItem aItem in aList.ListItems)
      {
        Size bSize = GetListItemSize(aItem, aSize);
        result.Width = Math.Max(result.Width, bSize.Width);
        result.Height += bSize.Height;
      }

      if (aList.Margin != null)
      {
        result.Width += Safe(aList.Margin.Left);
        result.Width += Safe(aList.Margin.Right);
      }
      if (aList.Padding != null)
      {
        result.Width += Safe(aList.Padding.Left);
        result.Width += Safe(aList.Padding.Right);
        result.Height += Safe(aList.Padding.Top);
        result.Height += Safe(aList.Padding.Bottom);
      }
      if (aList.PreviousBlock != null)
        result.Height += GetVerticalDistance(aList.PreviousBlock, aList, aSize);
      return result;
    }

    private Size GetListItemSize(ListItem aItem, SizeInfo aSize)
    {
      Size result = new Size();
      foreach (Block aBlock in aItem.Blocks)
      {
        Size bSize = new Size();
        if (aBlock is Paragraph)
          bSize = GetParagraphSize((Paragraph)aBlock, aSize);
        else if (aBlock is List)
          bSize = GetListSize((List)aBlock, aSize);
        result.Height += bSize.Height;
        result.Width = Math.Max(result.Width, bSize.Width);
      }
      if (aItem.Margin != null && !Double.IsNaN(aItem.Margin.Left))
        result.Width += aItem.Margin.Left;
      if (aItem.Margin != null && !Double.IsNaN(aItem.Margin.Right))
        result.Width += aItem.Margin.Right;
      if (aItem.Padding != null && !Double.IsNaN(aItem.Margin.Left))
        result.Width += aItem.Padding.Left;
      if (aItem.Padding != null && !Double.IsNaN(aItem.Padding.Right))
        result.Width += aItem.Padding.Right;
      if (aItem.Padding != null && !Double.IsNaN(aItem.Padding.Top))
        result.Height += aItem.Padding.Top;
      if (aItem.Padding != null && !Double.IsNaN(aItem.Padding.Bottom))
        result.Height += aItem.Padding.Bottom;
      if (aItem.PreviousListItem != null)
        result.Height += GetVerticalDistance(aItem.PreviousListItem, aItem, aSize);
      return result;
    }

    private Double GetVerticalDistance(Block firstBlock, Block secondBlock, SizeInfo aSize)
    {
      double distance = Double.NaN;
      if (firstBlock.Margin != null && !Double.IsNaN(firstBlock.Margin.Bottom))
        distance = firstBlock.Margin.Bottom;
      if (secondBlock.Margin != null && !Double.IsNaN(secondBlock.Margin.Top))
      {
        if (!Double.IsNaN(distance) || secondBlock.Margin.Top > distance)
          distance = secondBlock.Margin.Top;
      }
      if (!Double.IsNaN(distance))
        return distance;
      distance = GetLineHeight(firstBlock, aSize);
      return distance;
    }

    private Double GetVerticalDistance(ListItem firstBlock, ListItem secondBlock, SizeInfo aSize)
    {
      double distance = Double.NaN;
      if (firstBlock.Margin != null && !Double.IsNaN(firstBlock.Margin.Bottom))
        distance = firstBlock.Margin.Bottom;
      if (secondBlock.Margin != null && !Double.IsNaN(secondBlock.Margin.Top))
      {
        if (!Double.IsNaN(distance) || secondBlock.Margin.Top > distance)
          distance = secondBlock.Margin.Top;
      }
      if (!Double.IsNaN(distance))
        return distance;
      distance = GetLineHeight(firstBlock, aSize);
      return distance;
    }

    private void CalcInlineSize(Inline aInline, SizeInfo aSize)
    {
      if (aInline.GetType() == typeof(Run))
        CalcRunSize((Run)aInline, aSize);
      else if (aInline.GetType() == typeof(Bold))
        CalcSpanSize((Span)aInline, aSize);
      else if (aInline.GetType() == typeof(Hyperlink))
        CalcSpanSize((Span)aInline, aSize);
      else if (aInline.GetType() == typeof(Italic))
        CalcSpanSize((Span)aInline, aSize);
      else if (aInline.GetType() == typeof(Span))
        CalcSpanSize((Span)aInline, aSize);
      else if (aInline.GetType() == typeof(Underline))
        CalcSpanSize((Span)aInline, aSize);
    }

    private void CalcSpanSize(Span aSpan, SizeInfo aSize)
    {
      FontFamily aFontFamily = aSpan.FontFamily;
      FontStyle aFontStyle = aSpan.FontStyle;
      FontWeight aFontWeight = aSpan.FontWeight;
      FontStretch aFontStretch = aSpan.FontStretch;
      Double aFontSize = aSpan.FontSize;

      Typeface aTypeface = new Typeface(aFontFamily, aFontStyle, aFontWeight, aFontStretch);

      aSize.Top = Math.Max(aSize.Top, aFontFamily.Baseline * aFontSize);
      aSize.Bottom = Math.Min(aSize.Bottom, -(aFontFamily.LineSpacing - aFontFamily.Baseline) * aFontSize);
      aSize.SuperPos = aTypeface.XHeight * aFontSize;
      aSize.SubPos = aTypeface.UnderlinePosition * aFontSize;

      foreach (Inline aInline in aSpan.Inlines)
      {
        if (aInline is LineBreak)
        {
          double lineHeight = GetLineHeight(aInline, aSize);
          aSize.TopOffset += lineHeight;
          aSize.CurWidth = 0;
        }
        else
        {
          switch (aInline.BaselineAlignment)
          {
            case BaselineAlignment.Superscript:
              aSize.TopOffset += aSize.SuperPos;
              break;
            case BaselineAlignment.Subscript:
              aSize.BottomOffset -= aSize.SubPos;
              break;
          }
          CalcInlineSize(aInline, aSize);
        }
      }
    }

    private void CalcRunSize(Run aElement, SizeInfo aSize)
    {
      FontFamily aFontFamily = aElement.FontFamily;
      FontStyle aFontStyle = aElement.FontStyle;
      FontWeight aFontWeight = aElement.FontWeight;
      FontStretch aFontStretch = aElement.FontStretch;
      Double aFontSize = aElement.FontSize;
      string aText = aElement.Text;
      string[] sText = aText.Split(' ');

      Typeface aTypeface = new Typeface(aFontFamily, aFontStyle, aFontWeight, aFontStretch);

      aSize.Top = Math.Max(aSize.Top, aFontFamily.Baseline * aFontSize);
      aSize.Bottom = Math.Min(aSize.Bottom, -(aFontFamily.LineSpacing - aFontFamily.Baseline) * aFontSize);
      aSize.SuperPos = aTypeface.XHeight * aFontSize;
      aSize.SubPos = aTypeface.UnderlinePosition * aFontSize;

      double aWidth = 0;
      FormattedText fText;

      for (int i=0; i<sText.Length; i++)
      {
        string s = sText[i];
        if (i < sText.Length - 1)
          s += ' ';
        fText = new FormattedText(
          s,/* + (aText.EndsWith(" ") ? "|" : ""),*/
          CultureInfo.GetCultureInfo(aElement.Language.IetfLanguageTag),
          aElement.FlowDirection,
          aTypeface,
          aFontSize,
          Brushes.Black);

        aWidth += fText.WidthIncludingTrailingWhitespace;
        aWidth += fText.OverhangLeading;
        aWidth += fText.OverhangTrailing;

      }

/*
      if (aSize.WrapEnabled)
      {
        int L = sText.Length;
        while (L > 0 && aWidth + aSize.CurWidth > aSize.MaxWidth)
        {
          L--;
          aText = aText.Substring(0, aText.Length - sText[L].Length - (L > 0 ? 1 : 0));
          fText = new FormattedText(
            aText + " |",
            CultureInfo.GetCultureInfo(aElement.Language.IetfLanguageTag),
            aElement.FlowDirection,
            aTypeface,
            aFontSize,
            Brushes.Black);
          aWidth = fText.Width;
        }
        if (L < sText.Length)
        {
          int len = aText.Length;
          aText = aElement.Text;
          aText = aText.Substring(len, aText.Length - len);
          if (L > 0)
            aSize.Width = Math.Max(aSize.Width, aWidth + aSize.CurWidth);
          aSize.TopOffset += GetLineHeight(aElement, aSize);
          aSize.Top = 0;
          aSize.Bottom = 0;
          aSize.CurWidth = 0;
          aSize.WrappedLines += 1;

          Run newRun = new Run(aText);
          CalcRunSize(newRun, aSize);
          return;
        }
      }
*/

      aSize.CurWidth += aWidth;
      aSize.Width = Math.Max(aSize.Width, aSize.CurWidth);
      //SizeInfos.Add(aSize.Clone());
    }

    private static double GetLineHeight(TextElement aElement, SizeInfo aSize)
    {
      //      SizeInfo aSize = new SizeInfo(0);

      FontFamily aFontFamily = aElement.FontFamily;
      FontStyle aFontStyle = aElement.FontStyle;
      FontWeight aFontWeight = aElement.FontWeight;
      FontStretch aFontStretch = aElement.FontStretch;
      Double aFontSize = aElement.FontSize;

      Typeface aTypeface = new Typeface(aFontFamily, aFontStyle, aFontWeight, aFontStretch);
      double aTop = (aFontFamily.Baseline) * aFontSize;
      double aBottom = -(aFontFamily.LineSpacing - aFontFamily.Baseline) * aFontSize;
      double result = aFontFamily.LineSpacing * aFontSize;
      result = Math.Max(result, Safe(aSize.LineHeight));
      result = Math.Max(result, Safe(aSize.LineSpacing));
      //GlyphTypeface aGlyphTypeface;
      //if (aTypeface.TryGetGlyphTypeface(out aGlyphTypeface))
      //{
      //  result += - aGlyphTypeface.UnderlinePosition + aGlyphTypeface.UnderlineThickness;
      //}
      return result;
      //aSize.SuperPos = aTypeface.CapsHeight * aFontSize;
      //aSize.SubPos = aTypeface.UnderlinePosition * aFontSize;
      /*
            double parentLineHeight = Double.NaN;
            if (aElement is ListItem)
            {
              List aList = ParentList((ListItem)aElement);
              if (aList != null)
                parentLineHeight = aList.LineHeight;
            }
            else
            {
              Paragraph aParagraph = ParentParagraph(aElement);
              if (aParagraph != null)
                parentLineHeight = aParagraph.LineHeight;
            }
            if (!Double.IsNaN(parentLineHeight))
              if (parentLineHeight > aSize.Height)
                aSize.Bottom = aSize.Top - parentLineHeight;
      */
    }

    public static double GetSubscriptOffset(TextElement aElement)
    {
      Typeface aTypeface = new Typeface(aElement.FontFamily, aElement.FontStyle, aElement.FontWeight, aElement.FontStretch);
      double result = aTypeface.CapsHeight;
      return 0;
    }

    private static double Safe(Double value)
    {
      if (!Double.IsNaN(value))
        return value;
      else
        return 0;
    }

    public static Paragraph ParentParagraph(TextElement aElement)
    {
      DependencyObject aObject = aElement;
      do
      {
        if (aObject is Paragraph)
          return aObject as Paragraph;
        else if (aObject is Inline)
          aObject = ((Inline)aObject).Parent;
        else
          return null;
      } while (aObject != null);
      return null;
    }

    public static List ParentList(ListItem aElement)
    {
      DependencyObject aObject = aElement;
      do
      {
        if (aObject is List)
          return aObject as List;
        else if (aObject is ListItem)
          aObject = ((ListItem)aObject).Parent;
        else
          return null;
      } while (aObject != null);
      return null;
    }

    protected override void OnRender(DrawingContext DC)
    {
      Double aLeft = -Margin.Left;
      Double aTop = -Margin.Top;
      Double aWidth = ActualWidth + Margin.Left + Margin.Right;
      Double aHeight = ActualHeight + Margin.Top + Margin.Bottom;
      Brush aBrush = new SolidColorBrush(Color.FromArgb(64, 255, 255, 0));
      //DC.DrawRectangle(aBrush, null, new Rect(aLeft, aTop, aWidth, aHeight));

      aLeft += Margin.Left;
      aTop += Margin.Top;
      aWidth -= (Margin.Left + Margin.Right);
      aHeight -= (Margin.Top + Margin.Bottom);
      if (aWidth > 0 && aHeight > 0)
      {
        DC.DrawRectangle(aBrush, null, new Rect(aLeft, aTop, aWidth, aHeight));
        aLeft += Padding.Left;
        aTop += Padding.Top;
        aWidth -= (Padding.Left + Padding.Right);
        aHeight -= (Padding.Top + Padding.Bottom);
        if (aWidth > 0 && aHeight > 0)
          DC.DrawRectangle(aBrush, null, new Rect(aLeft, aTop, aWidth, aHeight));
      }

      if (Background != null)
      {
        aWidth = ActualWidth;
        aHeight = ActualHeight;
        DC.DrawRectangle(Background, null, new Rect(0, 0, aWidth, aHeight));
      }

      //Debug.WriteLine(String.Format("Padding={0}", Padding.Left));
      //Debug.WriteLine(String.Format("DocumentPadding={0}", Document.PagePadding.Left));
      //Debug.WriteLine(String.Format("ParagraphIndent={0}", (Document.Blocks.FirstBlock as Paragraph).TextIndent));
      base.OnRender(DC);
      /*
      Pen aPen = new Pen(new SolidColorBrush(Colors.Red),1);
      Point P0 = new Point(0, 0);
      Point P1 = new Point(0, 10);
      DC.DrawLine(aPen, P0, P1);
      foreach (SizeInfo aInfo in SizeInfos)
      {
        P0 = new Point(aInfo.Width,0);
        P1 = new Point(aInfo.Width, 10);
        DC.DrawLine(aPen, P0, P1);
      }
      */
    }



  }
}
